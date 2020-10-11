using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using xxw.Logs;
using xxw.TraceDataFormat;
using xxw.utilities;

namespace OQC_IN
{
    public class Line : BaseModel
    {
        public string Name { get; } 
        public string StateText { get; set; } = "空闲";
        public string NgText { get; set; } = "";
        public Visibility NgShow => NgText == "" ? Visibility.Collapsed : Visibility.Visible;
        public string StateColor { get; set; } = "#FFEDEDED";
        public string StateTextColor { get; set; } = "#000000";
        public int Total { get; set; }
        public int OK { get; set; }
        public int NG { get; set; }
        public bool IsReading { get; private set; }
        private string Version = $"V{System.Reflection.Assembly.GetExecutingAssembly().GetName().Version}";
        readonly int LineNo;
        readonly CountHelper Count;
        readonly LineOut LineOut;
        readonly StopHelper Stop;
        readonly ConfigModel Config = App.Config;
        readonly TraceDataHelper traceDataHelper;
        readonly IOCard iOCard;
        public Line(int lineNo, CountHelper count, LineOut lineOut, IOCard ioCard, StopHelper stopHelper)
        {
            Name = Config.Trigger[lineNo-1].Name;
            OnPropertyChanged(nameof(Name));
            LineNo = lineNo;
            Count = count;
            LineOut = lineOut;
            iOCard = ioCard;
            Stop = stopHelper;
            traceDataHelper = new TraceDataHelper();
            traceDataHelper.Init();
        }
        public void Read(List<string> data)
        {
            Count.AddTotal();
            Total++;
            OnPropertyChanged(nameof(Total));
            var success = Convert.ToBoolean(data[3]);
            if (!success)
            {
                NG++;
                OnPropertyChanged(nameof(NG));
                LogData.Log.Info($"{LineNo} NG");
                SetNg("读码失败");
                
                return;
            }
            OK++;

            OnPropertyChanged(nameof(OK));
            int fgIndex = Config.DataMapping.FirstOrDefault(p => p.CAMNO.Contains(LineNo))?.Mapping.FirstOrDefault(p => p.Name == "fgcode")?.Index ?? -1;
            if (fgIndex > data.Count)
            {
                LogError.Log.Fatal("定义的FG码映射位置超过最大位置");
                LogData.Log.Info($"{LineNo} NG");
                SetNg("定义的FG码映射位置超过最大位置");
                return;
            }
            string fg = data[fgIndex];
            SetPosting();
            //FG码转SN码
            string sn = GetBand(fg, out string msg);
            if (sn == "")
            {
                SetNg(msg);
                LogData.Log.Error($"{LineNo} NG NULL {fg} 获取SN失败");
                return;
            }
            //抛线尾
            if (Config.CheckOut)
            {
                var b = LineOut.Send(LineNo, new InDatas { FG = fg, SN = sn });
                if (!b)
                {
                    SetNg("发送数据到线尾失败");
                    LogData.Log.Error($"{LineNo} NG {sn} {fg} 连接线尾失败");
                    return;
                }
            }

            //抛Trace
            PostToTrace(fg, sn);
        }
        private string GetBand(string fgcode, out string msg)
        {
            if (Config.POSTData.GetBandType.ToLower() == "trace")
            {
                var tsr = TraceSerial.GetSerial(Config.POSTData.GetBandUrl, "fg", fgcode, Config.POSTData.ProcessName, true);
                if (tsr.Response.Code == HttpStatusCode.OK)
                {
                    if (string.IsNullOrEmpty(tsr.TraceSerial.Serials.Band))
                    {
                        msg = $"获取Band码失败:接口未返回Band码\r\n {fgcode}";
                        return "";
                    }
                    msg = "";
                    return tsr.TraceSerial.Serials.Band;
                }
                else
                {
                    msg = $"获取Band码失败:{tsr.Response.Error}\r\n {fgcode}";
                    return "";
                }
            }
            else if (Config.POSTData.GetBandType.ToLower() == "jgp")
            {
                LogRead.Log.Info($"获取Band码: {fgcode}");
                var res = Http.Get(Config.POSTData.GetBandUrl.Replace("{fg}", fgcode.Replace("+", "%20")));
                if (res.Code != HttpStatusCode.OK)
                {
                    msg = $"获取Band码失败: {fgcode}";
                    LogRead.Log.Error(msg);
                    return "";
                }
                LogRead.Log.Info($"获取Band码响应结果: {res.Contact}");
                try
                {
                    CheckResult checkResult = res.Contact.ToEntity<CheckResult>();
                    if (!checkResult.status)
                    {
                        msg = $"获取Band码失败:接口返回false\r\n{fgcode}";
                        LogRead.Log.Error(msg);
                        return "";
                    }
                    if (checkResult.msg.IndexOf("null") > -1)
                    {
                        msg = $"获取Band码失败:接口返回null\r\n{fgcode}";
                        LogRead.Log.Error(msg);
                        return "";
                    }
                    if (string.IsNullOrEmpty(checkResult.msg))
                    {
                        msg = $"获取Band码失败:接口返回空\r\n{fgcode}";
                        LogRead.Log.Error(msg);
                        return "";
                    }
                    msg = "";
                    return checkResult.msg;
                }
                catch (Exception ex)
                {
                    msg = $"获取Band码失败:{ex.Message}\r\n{fgcode}";
                    LogRead.Log.Error(msg);
                    return "";
                }
            }
            else
            {
                msg = ($"获取Band码失败:不支持的Band码获取类型\r\n{fgcode}");
                LogRead.Log.Error(msg);
                return "";
            }
        }
        private void PostToTrace(string fg, string sn)
        {
            if (!Config.POSTData.ToTrace)
            {
                Count.AddOK();
                return;
            }
            string dataJson = "";
            try
            {
                TraceData traceData = new TraceData
                {
                    ListData = new Dictionary<string, List<string>>(),
                    FuncData = new Dictionary<string, Func<JToken, string>>
                    {
                        { "error_1",(d)=>{
                            return Stop.Stop.StopType==null? ""
                            :$"{Stop.Stop.Codes[Stop.Stop.StopType??0].Code}_{Stop.Stop.LastDateTime:yyyy-MM-dd HH:mm:ss}";
                        } },
                        { "error_2",(d)=>{
                            return Stop.Stop.StopType==null? ""
                            :$"00000000_{DateTime.Now:yyyy-MM-dd HH:mm:ss}";
                        } },
                        { "unit_serial_number", (d)=>{ var b = Config.POSTData.SnType.ToLower() == "band"? sn:fg;  return b.Length > 17 ? b.Substring(0,17) : b; } },
                    },
                    DicData = new Dictionary<string, string>
                    {
                        { "fg", fg},
                        { "fg_sn", Config.POSTData.SnType.ToLower() == "band"? sn:fg },
                        { "uut_start", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") },
                        { "uut_stop", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") },
                        { "software_version" , Version}
                    }
                };
                dataJson = traceDataHelper.GetJson(traceData);
            }
            catch (Exception ex)
            {
                LogRead.Log.Error("解码数据失败：" + ex.Message);
                SetNg("解码数据失败\r\n" + ex.Message);
                LogData.Log.Info($"{LineNo} NG {sn} {fg} 解码数据失败");
                Count.AddNG();
                return;
            }
            Count.AddOK();
            Count.AddTraceTotal();
            //Process Control 
            Result result = Config.POSTData.ProcessControl
                ? TracePost.ProcessControl(sn, dataJson)
                : TracePost.PostTrace(dataJson);
            if (result.Success)
            {
                if(Stop.Stop.StopType != null)
                {
                    Stop.Stop.StopType = null;
                    Stop.StopMsgStr = "";
                    Stop.OnPropertyChanged("StopMsgStr");
                }
                //重置空闲计时
                Stop.SetStopTimer();
                LogRead.Log.Info("上抛Trace 成功");
                LogData.Log.Info($"{LineNo} PASS {sn} {fg}");
                SetPass();
                Count.AddTraceOk();
            }
            else
            {
                LogRead.Log.Info("上抛Trace 失败");
                LogData.Log.Info($"{LineNo} NG {sn} {fg} 上抛Trace失败\r\n{string.Join("\r\n", result.Message)}");
                SetNg($"上抛Trace 失败:{string.Join("\r\n", result.Message)}");
                Count.AddTraceNG();
            }
        }
        public void ClearCount()
        {
            OK = 0;
            NG = 0;
            Total = 0;
            OnPropertyChanged(nameof(OK));
            OnPropertyChanged(nameof(NG));
            OnPropertyChanged(nameof(Total));
        }
        #region 设置状态
        public void SetDefault()
        {
            StateText = "空闲";
            StateColor = "#FFEDEDED";
            StateTextColor = "#000000";
            NgText = "";
            UpdateUI();
        }
        private void SetPosting()
        {
            StateText = "上抛中...";
            StateColor = "#FFDADADA";
            StateTextColor = "#000000";
            NgText = "";
            UpdateUI();
        }
        public void SetReading()
        {
            IsReading = true;
            StateText = "读码中...";
            StateColor = "#FFDADADA";
            StateTextColor = "#000000";
            NgText = "";
            UpdateUI();
        }
        private void SetNg(string msg)
        {
            IsReading = false;
            StateText = "NG";
            StateColor = "#FFE20404";
            StateTextColor = "#FFFFFF";
            NgText = msg;
            UpdateUI();
            Count.AddNG();
            //报警
            SetMachineWaring();
        }
        private void SetPass()
        {
            IsReading = false;
            StateText = "PASS";
            StateColor = "#FF11BB00";
            StateTextColor = "#FFFFFF";
            NgText = "";
            UpdateUI();
        }
        void UpdateUI()
        {
            App.Current.Dispatcher.Invoke(new Action(() =>
            {
                OnPropertyChanged(nameof(StateText));
                OnPropertyChanged(nameof(StateColor));
                OnPropertyChanged(nameof(StateTextColor));
                OnPropertyChanged(nameof(NgText));
                OnPropertyChanged(nameof(NgShow));
            }));
        }
        #endregion
        #region 机台报警
        void SetMachineWaring()
        {
            Task.Run(() =>
            {
                if (Config.IOCard.Line[LineNo - 1].MachineWaringDelay > 0)
                    Task.Delay(Config.IOCard.Line[LineNo - 1].MachineWaringDelay).Wait();
                iOCard.Waring(LineNo - 1, true);
                if (Config.IOCard.Line[LineNo - 1].MachineWaringTime > 0)
                    Task.Delay(Config.IOCard.Line[LineNo - 1].MachineWaringTime).Wait();
                iOCard.Waring(LineNo - 1, false);
            });
        }
        #endregion
    }


    public class CheckResult
    {
        public bool status { get; set; }
        public string msg { get; set; }
    }
}

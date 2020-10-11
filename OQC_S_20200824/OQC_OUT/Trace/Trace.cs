using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using xxw.Logs;
using xxw.TraceDataFormat;
using xxw.utilities;

namespace OQC_OUT
{
    public class Trace
    {
        private readonly ConfigModel Config = App.Config;
        private readonly SettingsModel Settings = App.Settings;
        private readonly string Version = $"V{System.Reflection.Assembly.GetExecutingAssembly().GetName().Version}";
        readonly TraceDataHelper traceDataHelper;
        public Trace()
        {
            traceDataHelper = new TraceDataHelper();
            traceDataHelper.Init();
        }
        public void PostData(Machine machine, Product product)
        {

            string id = Guid.NewGuid().ToString();
            string msg = "";
            Datas datas = product.Get(id, App.Settings, ref msg);
            if (datas == null)
            {
                product.AddNgMsg(msg);
                machine.AddCount(product);
            }
            //检查国别、颜色
            else if (!CheckData(datas.FGCode, product))
            {
                product.IsCheck = false;
                product.IsPosting = false;
                product.OnPropertyChanged("IsCheck");
                machine.AddCount(product);
                return;
            }
            else
            {
                product.IsPosting = true;
                //获取band码
                GetBand(datas, product);
                if (string.IsNullOrEmpty(datas.SN))
                {
                    product.GetBandSuccess = false;
                    product.IsPosting = false;
                    machine.AddCount(product);
                    return;
                }

                var jgpTask = Task.Run(() =>
                {
                    if (Config.POSTData.ToJGP) PostToJPG(machine, product, datas);
                });
                var traceTask = Task.Run(() =>
                {
                    var successOktoStart = PostToOktoStart(machine, product, datas);
                    if (!successOktoStart) return;
                    var success = PostToIFactory(machine, product, datas);
                    if (!success) return;
                    PostToTrace(machine, product, datas);
                });
                System.Threading.Tasks.Task.WaitAll(jgpTask, traceTask);
                product.IsPosting = false;
                machine.AddCount(product);
            }
        }
        private bool CheckData(string fgcode, Product product)
        {
            if (!Config.POSTData.CheckFG) return true;
            //if(fgcode)
            var code = fgcode.Substring(11, 4);
            if (code != Settings.EngineeringCode)
            {
                if (Settings.EngineeringCode.Length < 4)
                {
                    product.AddNgMsg("工程代码配置错误");
                    LogRead.Log.Info($"工程代码配置错误 {fgcode}");
                }
                else
                {
                    if (code.Substring(0, 3) != Settings.EngineeringCode.Substring(0, 3))
                    {
                        product.AddNgMsg("国别信息不匹配");
                        LogRead.Log.Info($"国别信息不匹配 {fgcode}");
                    }
                    if (code.Substring(3) != Settings.EngineeringCode.Substring(3))
                    {
                        product.AddNgMsg("颜色信息不匹配");
                        LogRead.Log.Info($"颜色信息不匹配 {fgcode}");
                    }
                }
                return false;
            }
            LogRead.Log.Info($"国别颜色验证通过 {fgcode}");
            return true;
        }
        private void GetBand(Datas data, Product product)
        {
            var db = new DbContext();
            var db_datas = db.DatasDb;
            string fgcode = data.FGCode;
            if (Config.CheckIn)
            {
                var db_indatas = db.InDatasDb;
                var inData = db_indatas.GetSingle(p => p.FG == data.FGCode);
                if (inData == null)
                {
                    LogRead.Log.Error($"线头数据验证失败，未找到对应线头数据！FGCode: {data.FGCode}");
                    product.AddNgMsg("线头未过站！");
                    //product.GetBandSuccess = false;
                    return;
                }
                data.SN = inData.SN;
                //更新数据库
                db_datas.Update(data);
            }
            else
            {
                if (Config.POSTData.GetBandType.ToLower() == "jgp")
                {
                    LogRead.Log.Info($"获取Band码 FGCode: {fgcode}");
                    var res = Http.Get(Config.POSTData.GetBandUrl.Replace("{fg}", fgcode.Replace("+", "%20")));
                    SaveRequestTime("GetBand", res.Elapsed);
                    if (res.Code != HttpStatusCode.OK)
                    {
                        product.AddNgMsg("获取Band码失败");
                        LogRead.Log.Error($"获取Band码失败 FGCode: {fgcode}");
                        product.GetBandSuccess = false;
                        return;
                    }
                    LogRead.Log.Info($"获取Band码响应结果: {res.Contact}");
                    try
                    {
                        CheckResult checkResult = res.Contact.ToEntity<CheckResult>();
                        if (!checkResult.status)
                        {
                            product.AddNgMsg("获取Band码失败:接口返回false");
                            LogRead.Log.Error($"获取Band码失败:接口返回false  {fgcode}");
                            product.GetBandSuccess = false;
                            return;
                        }
                        if (checkResult.msg.IndexOf("null") > -1)
                        {
                            product.AddNgMsg("获取Band码失败:接口返回空");
                            LogRead.Log.Error($"获取Band码失败:接口返回空  {fgcode}");
                            product.GetBandSuccess = false;
                            return;
                        }
                        if (string.IsNullOrEmpty(checkResult.msg))
                        {
                            product.AddNgMsg("获取Band码失败:接口返回空");
                            LogRead.Log.Error($"获取Band码失败:接口返回空  {fgcode}");
                            product.GetBandSuccess = false;
                            return;
                        }
                        data.SN = checkResult.msg;
                        LogRead.Log.Info($"获取Band码成功:{checkResult.msg}  {fgcode}");
                        //更新数据库
                        db_datas.Update(data);
                    }
                    catch (Exception ex)
                    {
                        product.AddNgMsg("获取Band码失败");
                        LogRead.Log.Error($"获取Band码失败:{ex.Message} {fgcode}");
                        product.GetBandSuccess = false;
                        return;
                    }
                }
                else if (Config.POSTData.GetBandType.ToLower() == "trace")
                {
                    var tsr = TraceSerial.GetSerial(Config.POSTData.GetBandUrl, "fg", fgcode, Config.POSTData.ProcessName, true);
                    SaveRequestTime("GetBand", tsr.Response.Elapsed);
                    if (tsr.Response.Code == HttpStatusCode.OK && tsr.TraceSerial != null)
                    {
                        if (string.IsNullOrEmpty(tsr.TraceSerial.Serials.Band))
                        {
                            product.AddNgMsg("获取Band码失败，接口未返回Band码");
                            LogRead.Log.Error($"获取Band码失败:接口未返回Band码 {fgcode}");
                            product.GetBandSuccess = false;
                        }
                        else
                        {
                            data.SN = tsr.TraceSerial.Serials.Band;
                            LogRead.Log.Info($"获取Band码成功:{tsr.TraceSerial.Serials.Band}  {fgcode}");
                            //更新数据库
                            db_datas.Update(data);
                        }
                    }
                    else
                    {
                        product.AddNgMsg("获取Band码失败，接口请求失败");
                        LogRead.Log.Error($"获取Band码失败:{tsr.Response.Error} {fgcode}");
                        product.GetBandSuccess = false;
                        return;
                    }
                }
                else
                {
                    product.AddNgMsg("获取Band码失败");
                    LogRead.Log.Error($"获取Band码失败:不支持的Band码获取类型 {fgcode}");
                    product.GetBandSuccess = false;
                    return;
                }
            }
        }
        List<Dictionary<string, string>> GetTraceResults(Datas datas)
        {
            List<string> userIds = new List<string>{
                                    datas.Ins1Code,
                                    datas.Ins2Code,
                                    datas.Ins3Code,
                                    datas.Ins4Code,
                                    datas.Ins5Code,
                                    datas.Ins6Code,
                                    datas.Ins7Code,
                                    datas.Ins8Code,
                                    datas.Ins9Code,
                                    datas.Ins10Code
                                }.Where(t => t != null).ToList();
            string classes = App.Settings.ShiftCode;
            var users = new DbContext().UsersDb.GetList(p => userIds.Contains(p.UserCode) && (p.UserClasses == classes || p.UserClasses == null));
            return users.Select(oneuser => new Dictionary<string, string> {
                    { "usernumber",oneuser.UserNumber }, //员工号
                    { "usernamepy",oneuser.UserNamePY }, //员工姓名，必须为拼音
                    { "seatingcode",oneuser.SeatingCode }, //员工线体座位号
                    { "grade",oneuser.Grade }, //员工等级(A, B, C)
                    { "onboarddate",oneuser.OnboardDate?.ToString("yyyyMMdd") }, //员工入职日期，格式 yyyyMMdd
                    { "qualificationdate",oneuser.QualificationDate?.ToString("yyyyMMdd") } //员工上岗日期，格式 yyyyMMdd
                }).ToList();
        }
        private void PostToTrace(Machine machine, Product product, Datas datas)
        {
            if (!Config.POSTData.ToTrace)
                return;
            machine.AddPostTotal();
            string dataJson = "";
            try
            {
                TraceData traceData = new TraceData
                {
                    ListData = new Dictionary<string, List<string>>(),
                    FuncData = new Dictionary<string, Func<JToken, string>>
                    {
                        { "unit_serial_number", (d)=>{ var b = Config.POSTData.SnType?.ToLower() == "fg"?datas.FGCode: datas.SN;  return b.Length > 17 ? b.Substring(0,17) : b; } }
                    },
                    DicData = new Dictionary<string, string>
                    {
                        { "fg",datas.FGCode},
                        { "fg_sn",Config.POSTData.SnType?.ToLower() == "fg"?datas.FGCode: datas.SN},
                        { "uut_start",DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")},
                        { "uut_stop",DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")},
                        { "software_version" , Version}
                    },
                    ResultsData = GetTraceResults(datas)
                };
                dataJson = traceDataHelper.GetJson(traceData);
            }
            catch (Exception ex)
            {
                LogRead.Log.Error($"解码数据失败：{datas.FGCode}\r\n" + ex.Message);
                LogError.Log.Fatal("Trace 解码数据失败：" + ex.StackTrace);
                product.AddNgMsg("解码数据失败!");
                product.PostTraceSuccess = false;
                return;
            }

            //Process Control 
            Result result = Config.POSTData.ProcessControl
                ? TracePost.ProcessControl(Config.POSTData.ProcessControlUrl, datas.SN, dataJson)
                : TracePost.PostTrace(Config.POSTData.TeaceUrl, dataJson);
            foreach (var one in result.Elapseds)
            {
                SaveRequestTime(one.Key, one.Value);
            }
            if (result.Success)
            {
                LogRead.Log.Info($"上抛Trace 成功 {datas.FGCode}");
                datas.TracePost = true;
                datas.TracePostInformation = "";
                product.PostTraceSuccess = true;
                machine.AddCount(0, 1, 0);
            }
            else
            {
                LogRead.Log.Info($"上抛Trace 失败 {datas.FGCode}");
                product.AddNgMsg(result.Message);
                datas.TracePost = false;
                datas.TracePostInformation = string.Join("\r\n", result.Message);
                product.PostTraceSuccess = false;
            }
            //更新数据库
            new DbContext().DatasDb.Update(datas);
        }
        string SetNullIns(string val) => !string.IsNullOrEmpty(val) ? Uri.EscapeDataString(val) : "F";
        private void PostToJPG(Machine machine, Product product, Datas datas)
        {
            if (!Config.POSTData.ToJGP)
                return;
            string msg;
            try
            {
                LogPOSTJGP.Log.Info($"上抛JGP 开始");
                List<string> list = new List<string> {
                    $"sn={datas.SN}",
                    $"stationid={Uri.EscapeDataString(datas.StationId)}",
                    $"ins1code={SetNullIns(datas.Ins1Code)}",
                    $"ins2code={SetNullIns(datas.Ins2Code)}",
                    $"ins3code={SetNullIns(datas.Ins3Code)}",
                    $"ins4code={SetNullIns(datas.Ins4Code)}",
                    $"ins5code={SetNullIns(datas.Ins5Code)}",
                    $"ins6code={SetNullIns(datas.Ins6Code)}",
                    $"ins7code={SetNullIns(datas.Ins7Code)}",
                    $"ins8code={SetNullIns(datas.Ins8Code)}",
                    $"ins9code={SetNullIns(datas.Ins9Code)}",
                    $"ins10code={SetNullIns(datas.Ins10Code)}",
                    $"ins1name={SetNullIns(datas.Ins1Name)}",
                    $"ins2name={SetNullIns(datas.Ins2Name)}",
                    $"ins3name={SetNullIns(datas.Ins3Name)}",
                    $"ins4name={SetNullIns(datas.Ins4Name)}",
                    $"ins5name={SetNullIns(datas.Ins5Name)}",
                    $"ins6name={SetNullIns(datas.Ins6Name)}",
                    $"ins7name={SetNullIns(datas.Ins7Name)}",
                    $"ins8name={SetNullIns(datas.Ins8Name)}",
                    $"ins9name={SetNullIns(datas.Ins9Name)}",
                    $"ins10name={SetNullIns(datas.Ins10Name)}",
                    $"color={Uri.EscapeDataString(datas.Color)}",
                    $"region={Uri.EscapeDataString(datas.Region)}",
                    $"project={Uri.EscapeDataString(datas.Project)}",
                    $"location={Uri.EscapeDataString(datas.Location)}",
                    $"pahse={Uri.EscapeDataString(datas.Pahse)}"
                };
                LogPOSTJGP.Log.Info($"上抛JGP数据：{string.Join("&", list)}");
                var r = Http.GetXml($"{Config.POSTData.JGPUrl}?{string.Join("&", list)}");
                SaveRequestTime("PostJGP", r.Elapsed);
                LogPOSTJGP.Log.Info($"收到JGP数据：{r.Contact}");
                if (r.Code != HttpStatusCode.OK)
                {
                    msg = $"上抛JGP失败 {r.Code}";
                    LogPOSTJGP.Log.Error(msg);
                    product.AddNgMsg(msg);
                    datas.JgpPost = false;
                    datas.JgpPostInformation = r.Code.ToString();
                    product.PostJGPSuccess = false;
                }
                else
                {
                    var res = r.Contact;
                    if (res.IndexOf("[PASS]") > -1)
                    {
                        msg = $"上抛JGP 成功：{res}";
                        LogPOSTJGP.Log.Info(msg);
                        datas.JgpPost = true;
                        datas.JgpPostInformation = res;
                        product.PostJGPSuccess = true;
                        machine.AddCount(1, 0, 0);
                    }
                    else
                    {
                        msg = $"上抛JGP：{res}";
                        LogPOSTJGP.Log.Error(msg);
                        product.AddNgMsg(msg);
                        datas.JgpPost = false;
                        datas.JgpPostInformation = res;
                        product.PostJGPSuccess = false;
                    }
                    //更新数据库
                    new DbContext().DatasDb.Update(datas);
                }
            }
            catch (Exception ex)
            {
                msg = $"上抛JGP 失败：{ex.Message}";
                LogPOSTJGP.Log.Error(msg);
                product.AddNgMsg(msg);
                product.PostJGPSuccess = false;
            }
        }
        private List<string> PostToIFactoryAsChainese(List<string> msgs)
        {
            List<string> msgs_ch = new List<string>();
            foreach (var msg in msgs)
            {
                if (msg.Contains("WIP cannot start at any route step"))
                    msgs_ch.Add("物料站点不对，请查询物料的站点信息");
                else if (msg.Contains("WIP is packed in container"))
                {
                    var matchs = new Regex("'(.*?)'").Matches(msg);
                    var box = "";
                    if (matchs.Count == 2)
                    {
                        box = matchs[1].Groups[1].Value;
                    }
                    msgs_ch.Add($"物料已经打包进箱子{box}");
                }
                else if (msg.Contains("Material Checkpoint validation failed"))
                {
                    msgs_ch.Add("该物料没有Link客户码");
                }
                else if (msg.Contains("Required WIP Attribute' validation failed"))
                {
                    msgs_ch.Add("物料缺少必须的属性");
                }
                else if (msg.Contains("WIP doesn’t fulfill the minimum time interval"))
                {
                    var matchs = new Regex("(([0-1]?[0-9])|([2][0-3])):([0-5]?[0-9])(:([0-5]?[0-9]))?").Matches(msg);
                    var time = "";
                    if (matchs.Count == 1)
                    {
                        time = matchs[0].Value;
                    }
                    msgs_ch.Add($"请在{time}后再在本站点进行扫码");
                }
                else if (msg.Contains("The WIP's operation history does not match with WIP Checkpoint"))
                {
                    var matchs = new Regex("'(.*?)'").Matches(msg.Replace("'s", ""));
                    var staion = "";
                    if (matchs.Count == 1)
                    {
                        staion = matchs[0].Groups[1].Value;
                    }
                    msgs_ch.Add($"物料没有经过{staion}站点，不能扫入本站点");
                }
                else if (msg.Contains("WIP is not allowed to start WIP Checkpoint Pre-Start Rule"))
                {
                    var matchs = new Regex("'(.*?)'").Matches(msg.Replace("'s", ""));
                    var staion = "";
                    if (matchs.Count == 1)
                    {
                        staion = matchs[0].Groups[1].Value;
                    }
                    msgs_ch.Add($"物料经过了{staion}站点不能扫入本站点");
                }
                else if (msg.Contains("Validate Mask Rule"))
                {
                    msgs_ch.Add("物料不符合编码规则");
                }
                else if (msg.Contains("is on hold by hold reason code"))
                {
                    var matchs = new Regex("\"(.*?)\"").Matches(msg);
                    var res = "";
                    if (matchs.Count == 2)
                    {
                        res = matchs[1].Groups[1].Value;
                    }
                    msgs_ch.Add($"该物料已将被HOLD,HOLD原因{res}");
                }
                else if (msg.Contains("该SN未经过以下Trace站点"))
                {
                    var matchs = new Regex("\"(.*?)\"").Matches(msg);
                    List<string> res = new List<string>();
                    if (matchs.Count > 1)
                    {
                        for (var _i = 1; _i < matchs.Count; _i++)
                        {
                            res.Add(matchs[_i].Groups[1].Value);
                        }
                    }
                    msgs_ch.Add($"该SN未经过以下Trace站点：{string.Join(",", res)}");
                }
                else
                {
                    msgs_ch.Add(msg);
                }
            }
            return msgs_ch;
        }
        private bool PostToIFactory(Machine machine, Product product, Datas datas)
        {
            if (!Config.POSTData.ToIFactory)
                return true;
            string msg;
            bool success;
            try
            {
                LogPOSTIFactory.Log.Info($"上抛IFactory 开始");
                LogPOSTIFactory.Log.Info($"上抛IFactory 数据：{Config.POSTData.IFactoryParam.Replace("{sn}", datas.SN)}");
                var res = Http.PostJson(Config.POSTData.IFactoryUrl + "?" + Config.POSTData.IFactoryParam.Replace("{sn}", datas.SN), "");
                SaveRequestTime("PostIFactory", res.Elapsed);
                LogPOSTIFactory.Log.Info($"收到IFactory 数据：{res.Contact}");
                if (res.Code == HttpStatusCode.OK)
                {
                    IfactoryResult result = res.Contact.ToEntity<IfactoryResult>();
                    if (result.result == "pass")
                    {
                        msg = $"上抛IFactory 成功";
                        LogPOSTIFactory.Log.Error(msg);
                        datas.IFactoryPost = true;
                        datas.IFactoryPostInformation = res.Contact;
                        product.PostIfactorySuccess = true;
                        machine.AddCount(0, 0, 1);
                        success = true;
                    }
                    else
                    {
                        var msgs_ch = PostToIFactoryAsChainese(result.errors);
                        msg = $"上抛IFactory 失败：{string.Join("\r\n", result.errors)}";
                        LogPOSTIFactory.Log.Error(msg);
                        product.AddNgMsg($"上抛IFactory 失败\r\n{string.Join("\r\n", msgs_ch)}");
                        datas.IFactoryPost = false;
                        datas.IFactoryPostInformation = res.Contact;
                        product.PostIfactorySuccess = false;
                        success = false;
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(res.Contact) && res.Contact != "http error")
                    {
                        IfactoryResult result = res.Contact.ToEntity<IfactoryResult>();
                        msg = $"上抛IFactory 失败：{string.Join("\r\n", result.errors)}";
                        var msgs_ch = PostToIFactoryAsChainese(result.errors);
                        product.AddNgMsg($"上抛IFactory 失败\r\n{string.Join("\r\n", msgs_ch)}");
                    }
                    else
                    {
                        msg = $"上抛IFactory 失败：{res.Code}";
                        product.AddNgMsg(msg);
                    }
                    LogPOSTIFactory.Log.Error(msg);
                    datas.IFactoryPost = false;
                    datas.IFactoryPostInformation = res.Contact;
                    product.PostIfactorySuccess = false;
                    success = false;
                }
                //更新数据库
                new DbContext().DatasDb.Update(datas);
            }
            catch (Exception ex)
            {
                msg = $"上抛IFactory 失败：{ex.Message}";
                LogPOSTIFactory.Log.Error(msg);
                product.AddNgMsg(msg);
                product.PostIfactorySuccess = false;
                success = false;
            }
            return success;
        }
        private bool PostToOktoStart(Machine machine, Product product, Datas datas)
        {
            if (!Config.POSTData.ToOktoStart)
                return true;
            string msg;
            bool success;
            try
            {
                LogPOSTOktoStart.Log.Info($"OktoStart 开始");
                LogPOSTOktoStart.Log.Info($"OktoStart 数据：{Config.POSTData.OktoStartParam.Replace("{sn}", datas.SN)}");
                var res = Http.PostJson(Config.POSTData.OkToStartUrl + "?" + Config.POSTData.OktoStartParam.Replace("{sn}", datas.SN), "");
                SaveRequestTime("PostOktoStart", res.Elapsed);
                LogPOSTOktoStart.Log.Info($"收到OktoStart 数据：{res.Contact}");
                if (res.Code == HttpStatusCode.OK)
                {
                    OktoStartResult result = res.Contact.ToEntity<OktoStartResult>();
                    if (result.result == "pass")
                    {
                        msg = $"OktoStart 成功";
                        LogPOSTOktoStart.Log.Error(msg);
                        datas.OktoStartPost = true;
                        datas.OktoStartPostInformation = res.Contact;
                        product.PostOktoStartSuccess = true;
                        //machine.AddCount(0, 0, 1);
                        success = true;
                    }
                    else
                    {
                        msg = $"OktoStart 失败：{string.Join("\r\n", result.errors)}";
                        LogPOSTOktoStart.Log.Error(msg);
                        product.AddNgMsg($"OktoStart 失败");
                        datas.OktoStartPost = false;
                        datas.OktoStartPostInformation = res.Contact;
                        product.PostOktoStartSuccess = false;
                        success = false;
                    }
                }
                else
                {
                    msg = $"请求 OktoStart 失败：{res.Code}";
                    product.AddNgMsg(msg);
                    LogPOSTOktoStart.Log.Error(msg);
                    datas.OktoStartPost = false;
                    datas.OktoStartPostInformation = res.Contact;
                    product.PostOktoStartSuccess = false;
                    success = false;
                }
                //更新数据库
                new DbContext().DatasDb.Update(datas);
            }
            catch (Exception ex)
            {
                msg = $"上抛OktoStart 失败：{ex.Message}";
                LogPOSTOktoStart.Log.Error(msg);
                product.AddNgMsg(msg);
                product.PostOktoStartSuccess = false;
                success = false;
            }
            return success;
        }
        void SaveRequestTime(string type, double times)
        {
            new DbContext().InterfaceTimeDb.Insert(new InterfaceTime
            {
                CreateTime = DateTime.Now,
                InterfaceType = type,
                RequestTime = times
            });
        }
    }

    public class CheckResult
    {
        public bool status { get; set; }
        public string msg { get; set; }
    }

    public class IfactoryResult
    {
        public string result { get; set; }
        public List<string> errors { get; set; }
    }
    public class OktoStartResult
    {
        public string result { get; set; }
        public List<string> errors { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using xxw.utilities;

namespace OQC_OUT
{
    public class Product : BaseModel
    { 
        readonly ConfigModel Config = App.Config;
        #region MVVM
        public string Text
        {
            get
            {
                if (!Complate) return "读取中...";
                if (IsPosting) return "上抛中...";
                if (!Have) return "无产品";
                if (IsRepeat) return "重复检测";
                if (!IsCheck) return "信息不匹配";
                //有产品
                if (State != 1)//摆放不正常
                {
                    switch (State)
                    {
                        case 2:
                            return "产品上下颠倒";
                        case 3:
                            return "产品翻盖";
                        case 4:
                            return "工站条码缺失";
                        case -1:
                            return "检测信号异常";
                        case -2:
                            return "读码返回数据异常";
                    }
                }
                else//摆放正常
                {
                    if (!PostJGPSuccess) return "POST JGP NG";
                    if (!PostTraceSuccess) return "POST Trace NG";
                    if (!GetBandSuccess) return "GET Band Code NG";
                    if (IsNg)
                    {
                        string snNg = Datas.Any(p => new List<int> { 1, 3 }.Contains(p.Key) && p.Value.Success == false) ? "SN码检测失败" : "";
                        string codeNg = Datas.Any(p => new List<int> { 2, 4 }.Contains(p.Key) && p.Value.Success == false) ? "工站码检测失败" : "";
                        if (snNg != "" && codeNg != "")
                            snNg += "\r\n";
                        string msg = snNg + codeNg;
                        return msg == "" ? "NG" : msg;
                    }
                    if (Success) return "检测通过";
                }
                return State.ToString();
            }
        }
        public string Color
        {
            get
            {
                if (IsRepeat) return "#FFFC6969";
                if (!Complate) return "#FFA0A0A0";
                if (!Have) return "White";
                if ((!PostJGPSuccess || !PostTraceSuccess || !PostIfactorySuccess || !GetBandSuccess) && Have) return "#FFFFBA25";
                if (IsNg && Have) return "#FFE20404";
                if (Success && Have) return "#FF46B402";
                return "";
            }
        }
        public string FColor
        {
            get
            {
                if (!Have) return "Black";
                else if (!Complate || !PostJGPSuccess || !PostTraceSuccess || IsNg || Success || IsRepeat)
                    return "White";
                else
                    return "Black";
            }
        }
        #endregion
        #region 属性
        public string ProductId { get; private set; }
        /// <summary>
        /// 读码数据
        /// </summary>
        public Dictionary<int, CamData> Datas { get; set; }
        private int ReadCount;
        private int CamNumber;
        /// <summary>
        /// 产品状态（0、无产品，1、有产品，2、产品颠倒，3、产品翻盖，-1、信号异常，-2、读码返回数据异常）
        /// </summary>
        public double State => Datas.Max(p => p.Value.State);
        /// <summary>
        /// 读码完成
        /// </summary>
        public bool Complate => Datas.Count == CamNumber && Datas.All(p => p.Value.ReadTimes == ReadCount);
        /// <summary>
        /// 读码成功
        /// </summary>
        public bool Success => Complate && Datas.All(p => p.Value.Success);
        /// <summary>
        /// 有无产品
        /// </summary>
        public bool Have => Complate && State != 0;
        /// <summary>
        /// 重复检测
        /// </summary>
        public bool IsRepeat { get; set; }
        /// <summary>
        /// 工艺检测
        /// </summary>
        public bool IsCheck { get; set; } = true;
        /// <summary>
        /// 读码Ng
        /// </summary>
        public bool IsNg => (!Success && Have) || !PostJGPSuccess || !PostTraceSuccess || !PostIfactorySuccess || NgMessage.Count > 0;
        /// <summary>
        /// Ng消息
        /// </summary>
        public ObservableCollection<string> NgMessage { get; set; } = new ObservableCollection<string>();
        public Visibility ShowNgMessage => NgMessage.Count == 0 || State == 2 || State == 3 ? Visibility.Collapsed : Visibility.Visible;
        /// <summary>
        /// 获取Band码结果
        /// </summary>
        public bool GetBandSuccess { get; set; } = true;
        /// <summary>
        /// 上抛JGP结果
        /// </summary>
        public bool PostJGPSuccess { get; set; } = true;
        /// <summary>
        /// 上抛Trace结果
        /// </summary>
        public bool PostTraceSuccess { get; set; } = true;
        /// <summary>
        /// 上抛Ifactory结果
        /// </summary>
        public bool PostIfactorySuccess { get; set; } = true;
        /// <summary>
        /// 上抛OktoStart结果
        /// </summary>
        public bool PostOktoStartSuccess { get; set; } = true;
        private bool _IsPosting;
        /// <summary>
        /// 上抛状态
        /// </summary>
        public bool IsPosting
        {
            get { return _IsPosting; }
            set
            {
                _IsPosting = value;
                OnPropertyChanged(
                    nameof(Color),
                    nameof(Text),
                    nameof(FColor),
                    nameof(PostJGPSuccess),
                    nameof(PostTraceSuccess),
                    nameof(IsCheck),
                    nameof(IsPosting));
            }
        }
        #endregion
        /// <summary>
        /// 产品构造
        /// </summary>
        /// <param name="readCount">读码次数</param>
        /// <param name="camNumber">相机数量</param>
        public Product(int readCount, int camNumber)
        {
            ProductId = Guid.NewGuid().ToString();
            ReadCount = readCount;
            CamNumber = camNumber;
            Datas = new Dictionary<int, CamData>();
        }
        /// <summary>
        /// 添加产品读码数据
        /// </summary>
        /// <param name="data"></param>
        public void Add(CamData data)
        {

            if (!Datas.ContainsKey(data.CamNo))
                Datas.Add(data.CamNo, data);
            else if (!Datas[data.CamNo].Success)
            {
                data.ReadTimes = Datas[data.CamNo].ReadTimes;
                Datas[data.CamNo] = data;
            }
            Datas[data.CamNo].ReadTimes++;
            UpdateUI();
        }
        public void AddNgMsg(List<string> NgMsgs)
        {
            App.Current.Dispatcher.Invoke(new Action(() =>
            {
                foreach (var msg in NgMsgs)
                    NgMessage.Add(msg);
            }));
            OnPropertyChanged(nameof(ShowNgMessage));
        }
        public void AddNgMsg(string NgMsg)
        {
            App.Current.Dispatcher.Invoke(new Action(() =>
            {
                NgMessage.Add(NgMsg);
            }));
            OnPropertyChanged(nameof(ShowNgMessage));
        }
        public void UpdateUI() 
            => OnPropertyChanged(
                nameof(Color),
                nameof(Text),
                nameof(FColor),
                nameof(PostJGPSuccess),
                nameof(PostTraceSuccess),
                nameof(IsPosting),
                nameof(ShowNgMessage));
        public Datas Get(string Id, SettingsModel settingsModel, ref string msg)
        {
            if (State == -1) return null;
            var db = new DbContext();
            Datas datas = db.DatasDb.GetById(Id);
            if (datas != null) return datas;
            datas = new Datas
            {
                Id = Id,
                StationId = settingsModel.StationId,
                Color = settingsModel.Color,
                Region = settingsModel.Region,
                Project = settingsModel.Project,
                Location = settingsModel.Location,
                Pahse = settingsModel.Pahse,
                CreateDate = DateTime.Now
            };
            var ps = typeof(Datas).GetProperties();
            foreach (var one in Datas)
            {
                var d = one.Value;
                if (d.Data == null) continue;
                var map = Config.DataMapping.FirstOrDefault(p => p.CAMNO.Contains(one.Key));
                if (map != null)
                {
                    foreach(var mapping in map.Mapping)
                    {
                        if (mapping.Name.ToLower().IndexOf("ins") > -1) continue;
                        var pinfo = ps.Where(p => p.Name.ToLower() == mapping.Name.ToLower()).FirstOrDefault();
                        if (pinfo == null) continue;
                        var v = d.Data[mapping.Index];
                        pinfo.SetValue(datas, v, null);
                    }
                }
                for (int i = 0; i < Config.Station.Count; i++) {
                    var pinfo = ps.Where(p => p.Name.ToLower() == $"ins{i + 1}code").FirstOrDefault();
                    var npinfo = ps.Where(p => p.Name.ToLower() == $"ins{i + 1}name").FirstOrDefault();
                    if (pinfo == null) continue;
                    var users = GetTypeData(i + 1, d.Data);
                    if (users != null)
                    {
                        pinfo.SetValue(datas, users.UserCode, null);
                        npinfo.SetValue(datas, users.UserNumber, null);
                    }

                }
            }
            if (string.IsNullOrEmpty(msg))
                db.DatasDb.Insert(datas);
            else
                return null;
            return datas;
        }
        private Users GetTypeData(int type, List<string> data)
        {
            var code_data = data.Where(p => data.LastIndexOf(p) >= 7);
            var users = App.Users.Where(p => p.UserClasses == App.Settings.ShiftCode || string.IsNullOrEmpty(p.UserClasses)).ToList();
            var uList = users.Where(p => p.UserType == type.ToString() && code_data.Contains(p.UserCode)).ToList();
            if (uList.Count == 1)
                return uList.First();
            return null;
        }
    }
}

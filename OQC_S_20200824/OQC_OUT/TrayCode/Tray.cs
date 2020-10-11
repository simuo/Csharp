using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using xxw.utilities;

namespace OQC_OUT
{
    public class Tray : BaseModel
    {
        public string TrayId { get; }
        public ObservableCollection<TrayColumn> Columns { get; } = new ObservableCollection<TrayColumn>();
        private int Index = 1;
        private readonly ConfigModel Config = App.Config;
        private readonly int RowCount;
        private readonly int ColumnCount;
        private Product GetProduct(int camNo, int stageId)
        {
            TrayColumn column = Columns[stageId - 1];
            if (column.Products?.Count == 0) return null;
            int row = Config.Group.FirstOrDefault(p => p.CAMNO.Contains(camNo))?.Row ?? 1;
            return column.Products[row - 1];
        }
        public Tray(string trayId) : this(2, 5, trayId) { }
        public Tray(int rowCount, int columnCount, string trayId)
        {
            TrayId = trayId;
            RowCount = rowCount;
            ColumnCount = columnCount;
            Index = (Config.Direction == "right" || Config.Direction == "bottom") ? columnCount : 1;
            for (int c = 0; c < ColumnCount; c++)
                Columns.Add(new TrayColumn());
        }
        /// <summary>
        /// Tray盘是否Ng
        /// </summary>
        public bool IsNg => Columns.Any(p => p.Products == null || p.Products.Any(c => c.IsNg));
        /// <summary>
        /// Tray盘上抛中
        /// </summary>
        public bool IsPosting => Columns.Any(p => p.Products != null && p.Products.Any(c => c.IsPosting));
        /// <summary>
        /// 全部执行读码
        /// </summary>
        public bool IsComplate => Columns.All(p => p.Products != null);
        public Product Add(int camNo, int stageId, List<string> data)
        {
            Product product = GetProduct(camNo, stageId);

            var config = App.Config.DataMapping.FirstOrDefault(p => p.CAMNO.Contains(camNo));
            bool isSn = config?.Mapping.Any(p => p.Name.IndexOf("fgcode", StringComparison.Ordinal) > -1) ?? false;
            double codeCount = data.Count - (isSn ? 8 : 7); //读码个数
            codeCount = codeCount < 0 ? 0 : codeCount;
            int needCount = isSn ? 1 : Config.Station.Count;//期望读码个数

            double state = data.Count > 5 ? Convert.ToDouble(data[5]) : -2;
            CamData camOneData = new CamData
            {
                CamNo = camNo,
                Data = data,
                Success = Convert.ToBoolean(data[3]) && codeCount == needCount,
                State = state
            };
            product.Add(camOneData);
            if (state != 1) return product;

            if (!isSn)
            {
                List<string> msg = new List<string>();
                List<Users> users = App.Users.Where(p => p.UserClasses == App.Settings.ShiftCode || string.IsNullOrEmpty(p.UserClasses)).ToList();
                //检测工站码数量
                if (codeCount < needCount)
                    msg.Add($"工站条码缺失！期望{needCount}个，读到{codeCount}个");
                if (codeCount > needCount)
                    msg.Add($"工站条码多贴！期望{needCount}个，读到{codeCount}个");

                var code_data = data.Skip(7);
                //检测工站码重复
                int stationIndex = 0;
                foreach (var station in Config.Station)
                {
                    stationIndex++;
                    var uList = users.Where(p => p.UserType == stationIndex.ToString() && code_data.Contains(p.UserCode)).ToList();
                    if (uList.Count > 1)
                        msg.Add($"{station} 工站码重复[{string.Join(",", uList.Select(p => p.UserCode))}]");
                }
                //检测工站码配置
                for (int i = 0; i < code_data.Count(); i++)
                {
                    string code = code_data.ElementAt(i);
#if CD
                    //替换工站码班别
                    if (code.IndexOf("A") > -1 || code.IndexOf("B") > -1)
                        code = App.Settings.ShiftCode + code.Replace("A", "").Replace("B", "");
#endif
                    var user = users.Where(p => p.UserCode == code).ToList();
                    if (user.Count == 0)
                        msg.Add($"工站码[{code}]未配置人员");
                }

                //检测工站缺失
                List<string> ts = users.Where(p => code_data.Contains(p.UserCode)).Select(p => p.UserType).ToList();
                List<string> ls = Config.Station.Where(p => !ts.Contains((Config.Station.IndexOf(p) + 1).ToString())).Select(p => p).ToList();
                if (ls.Count > 0)
                    msg.Add($"[{string.Join(",", ls)}] 工站码缺失");
                if (msg.Count > 0)
                    camOneData.Success = false;
                product.AddNgMsg(msg);
                product.UpdateUI();
            }
            else
            {
                if (codeCount < needCount)
                    product.AddNgMsg($"SN条码缺失！");
                if (codeCount > needCount)
                    product.AddNgMsg($"SN条码多贴！");
                if (camOneData.Success && Config.POSTData.CheckRepeat)
                {
                    string fg = data[config.Mapping[0].Index];
                    //检测重复
                    product.IsRepeat = new DbContext().Db.Queryable<Datas>().Any(p => p.FGCode == fg && p.TracePost == true);
                }
            }
            return product;
        }
        public int? NewColumn()
        {
            if (Index > Columns.Count || Index < 0) return null;
            int nowIndex = Index;
            TrayColumn column = Columns[nowIndex - 1];
            column.Create(RowCount);
            OnPropertyChanged(nameof(Columns));
            if (Config.Direction == "right" || Config.Direction == "bottom")
                Index--;
            else
                Index++;
            return nowIndex;
        }
        /// <summary>
        /// 添加Ng信息
        /// </summary>
        public void AddNgMessage(int camNo, int stageId, string msg)
        {
            Product product = GetProduct(camNo, stageId);
            product?.AddNgMsg(msg);
        }
        /// <summary>
        /// 设置上抛状态
        /// </summary>
        public void SetPosting(int camNo, int stageId)
        {
            Product product = GetProduct(camNo, stageId);
            if (product == null) return;
            product.IsPosting = true;
        }
        /// <summary>
        /// 设置上抛结果
        /// </summary>
        public void SetPosted(int camNo, int stageId, bool v)
        {
            Product product = GetProduct(camNo, stageId);
            if (product == null) return;
            product.PostJGPSuccess = v;
            product.IsPosting = false;
        }
    }
}

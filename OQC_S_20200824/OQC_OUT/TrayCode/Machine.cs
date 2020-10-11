using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using xxw.utilities;

namespace OQC_OUT
{
    public class Machine : BaseModel
    {
        public Machine()
        {
            var c = new DbContext().CountsDb.GetById(1);
            TotalCount = c.TotalCount;
            NgCount = c.NgCount;
            ReCount = c.ReCount;
            OkCount = c.OkCount;
            NoneCount = c.NullCount;
            DiandaoCount = c.DianDaoCount;
            FangaiCount = c.FanGaiCount;
            PostTotalCount = c.PostTotalCount;
            JgpPostCount = c.JGPCount;
            PostJGPNgCount = c.PostTotalCount - c.JGPCount;
            TracePostCount = c.TraceCount;
            PostTraceNgCount = c.PostTotalCount - c.TraceCount;
            IfactoryPostCount = c.IFactoryCount;
            PostIfactoryNgCount = c.PostTotalCount - c.IFactoryCount;
            LoadStatistical();
        }
        #region 计数
        public int NgCount { get; set; }
        public int ReCount { get; set; }
        public int OkCount { get; set; }
        public int TotalCount { get; set; }
        public int NoneCount { get; set; }
        public int DiandaoCount { get; set; }
        public int FangaiCount { get; set; }
        public int PostTotalCount { get; set; }
        public int PostJGPNgCount { get; set; }
        public int PostTraceNgCount { get; set; }
        public int PostIfactoryNgCount { get; set; }
        public int JgpPostCount { get; set; }
        public int TracePostCount { get; set; }
        public int IfactoryPostCount { get; set; }
        public void AddCount(Product product)
        {
            if (!product.Complate) return;
            TotalCount += 1;
            if (product.Success)
                OkCount++;
            else
            {
                if (!product.Have)
                    NoneCount++;
                else if (product.State == 2)
                    DiandaoCount++;
                else if (product.State == 3)
                    FangaiCount++;
                else if (product.IsRepeat)
                    ReCount++;
                else
                    NgCount++;
                if (!product.PostJGPSuccess)
                    PostJGPNgCount++;
                if (!product.PostTraceSuccess)
                    PostTraceNgCount++;
                if (!product.PostIfactorySuccess)
                    PostIfactoryNgCount++;
            }
            //NgCount += product.Success ? 0 : 1;
            //OkCount += product.Success ? 1 : 0;
            //NoneCount += product.Have ? 0 : 1;
            //DiandaoCount += product.State == 2 ? 1 : 0;
            //FangaiCount += product.State == 3 ? 1 : 0;

            OnPropertyChanged(
                nameof(TotalCount),
                nameof(NgCount),
                nameof(ReCount),
                nameof(OkCount),
                nameof(NoneCount),
                nameof(DiandaoCount),
                nameof(FangaiCount),
                nameof(PostJGPNgCount),
                nameof(PostTraceNgCount),
                nameof(PostIfactoryNgCount));
        }
        public void AddPostTotal()
        {
            PostTotalCount++;
            OnPropertyChanged(nameof(PostTotalCount));
        }
        public void AddCount(int JgpPost, int TracePost, int IfactoryPost)
        {
            JgpPostCount += JgpPost;
            TracePostCount += TracePost;
            IfactoryPostCount += IfactoryPost;
            OnPropertyChanged(
                nameof(JgpPostCount),
                nameof(TracePostCount),
                nameof(IfactoryPostCount));
        }
        public void ClearCount()
        {
            TotalCount = 0;
            NgCount = 0;
            OkCount = 0;
            NoneCount = 0;
            DiandaoCount = 0;
            FangaiCount = 0;

            JgpPostCount = 0;
            TracePostCount = 0;
            IfactoryPostCount = 0;

            PostJGPNgCount = 0;
            PostTraceNgCount = 0;
            PostIfactoryNgCount = 0;

            PostTotalCount = 0;
            OnPropertyChanged(nameof(TotalCount),
                nameof(NgCount),
                nameof(OkCount),
                nameof(NoneCount),
                nameof(DiandaoCount),
                nameof(FangaiCount),
                nameof(JgpPostCount),
                nameof(TracePostCount),
                nameof(IfactoryPostCount),
                nameof(PostJGPNgCount),
                nameof(PostTraceNgCount),
                nameof(PostIfactoryNgCount),
                nameof(PostTotalCount));
            new DbContext().Execute(db =>
                db.CountsDb.Update(new Counts
                {
                    Id = 1,
                    TotalCount = 0,
                    OkCount = 0,
                    NgCount = 0,
                    NullCount = 0,
                    DianDaoCount = 0,
                    FanGaiCount = 0,
                    PostTotalCount = 0,
                    JGPCount = 0,
                    TraceCount = 0,
                    IFactoryCount = 0,
                    ReCount = 0,
                    LastDate = DateTime.Now
                })
            );
        }
        #endregion
        /// <summary>
        /// 读码NG
        /// </summary>
        public event Action<Tray> OnNg;
        /// <summary>
        /// 信号异常
        /// </summary>
        public event Action OnSignalError;
        public event Action OnComplate;
        public string NowTrayId { get; set; }
        public List<Tray> Trays { get; set; } = new List<Tray>();
        public Tray NowTray => Trays.FirstOrDefault(p => p.TrayId == NowTrayId);
        public void TrayIn()
        {
            if (NowTrayId != null)
                TrayOut();
            NowTrayId = Guid.NewGuid().ToString();
            var tray = new Tray(NowTrayId);
            Trays.Add(tray);
        }
        public void TrayOut()
        {
            Task.Run(() => {
                //保存计数
                SaveCount();
                //统计数据
                LoadStatistical();
            });
            if (NowTrayId == null) return;
            var nowTray = NowTray;
            if (nowTray == null) return;
            if (!nowTray.IsComplate || (!nowTray.IsNg && !nowTray.IsPosting))
            {
                Remove();
                if (!nowTray.IsComplate)
                    OnSignalError?.Invoke();
            }
            else
            {
                Task.Run(() =>
                {
                    Tray NgTray = Trays.FirstOrDefault();
                    if (NgTray == null) { Remove(); return; }
                    //计算停机延时
                    double speed = double.Parse(App.Settings.Speed);
                    double delay = App.Config.Tray.StopInterval / speed;
                    Task.Delay((int)delay).Wait();
                    if (nowTray.IsNg || nowTray.IsPosting)
                        OnNg?.Invoke(NgTray);
                    Remove();
                    
                });
            }
            NowTrayId = null;
        }
        public void Remove()
        {
            Trays.RemoveAt(0);
            OnComplate?.Invoke();
        }

        private void SaveCount() => new DbContext().Execute(db =>
                                      db.CountsDb.Update(new Counts
                                      {
                                          Id = 1,
                                          TotalCount = TotalCount,
                                          OkCount = OkCount,
                                          NgCount = NgCount,
                                          NullCount = NoneCount,
                                          DianDaoCount = DiandaoCount,
                                          FanGaiCount = FangaiCount,
                                          PostTotalCount = PostTotalCount,
                                          JGPCount = JgpPostCount,
                                          TraceCount = TracePostCount,
                                          IFactoryCount = IfactoryPostCount,
                                          ReCount = ReCount,
                                          LastDate = DateTime.Now
                                      })
           );

        public List<StatisticalData> BaiBanData { get; set; } = new List<StatisticalData>();
        public List<StatisticalData> YeBanData { get; set; } = new List<StatisticalData>();
        private void LoadStatistical()
        {
            var isAcrossDay = false;
            var date = DateTime.Now;
            //判断是否在夜班跨天
            if (date.Hour > 0 && date.Hour < 6)
                isAcrossDay = true;
            //白班
            //var db = new DbContext().Db;
            string b_day = (isAcrossDay ? date.AddDays(-1) : date).ToString("yyyy-MM-dd");
            string y_day = (isAcrossDay ? date : date.AddDays(1)).ToString("yyyy-MM-dd");
            string b_sql = SqlStr(b_day + " 06:00:00", b_day + " 18:00:00");
            string y_sql = SqlStr(b_day + " 18:00:00", y_day + " 6:00:00");
            var dbcontext = new DbContext();
            DataTable b_dt = dbcontext.Read(db => db.Db.SqlQueryable<StatisticalData>(b_sql).ToDataTable());
            DataTable y_dt = dbcontext.Read(db => db.Db.SqlQueryable<StatisticalData>(y_sql).ToDataTable());
            BaiBanData = ToStatisticalData(b_dt, -1);
            YeBanData = ToStatisticalData(y_dt, -2);
            OnPropertyChanged(nameof(BaiBanData), nameof(YeBanData));
        }

        string SqlStr(string begin_day, string end_day)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(" SELECT ");
            sb.AppendFormat("	Hour, ");
            sb.AppendFormat("	Count( 1 ) AS Total, ");
            sb.AppendFormat("	IFNULL(SUM( JgpPost ),0) AS JgpPost, ");
            sb.AppendFormat("	IFNULL(SUM( TracePost ),0) AS TracePost, ");
            sb.AppendFormat("	IFNULL(SUM( IFactoryPost ),0) AS IFactoryPost ");
            sb.AppendFormat(" FROM ");
            sb.AppendFormat("	(");
            sb.AppendFormat("	SELECT ");
            sb.AppendFormat("		*, ");
            sb.AppendFormat("	CASE ");
            sb.AppendFormat("			STRFTIME(\'%H\', CreateDate ) % 2 ");
            sb.AppendFormat("			WHEN 0 THEN ");
            sb.AppendFormat("			STRFTIME( \'%H\', CreateDate ) ELSE STRFTIME( \'%H\', CreateDate, \'-1 hours\' ) ");
            sb.AppendFormat("		END AS Hour ");
            sb.AppendFormat("	FROM ");
            sb.AppendFormat("		Datas ");
            sb.AppendFormat("	WHERE ");
            sb.AppendFormat("		CreateDate BETWEEN \'" + begin_day + "\' ");
            sb.AppendFormat("		AND \'" + end_day + "\' ");
            sb.AppendFormat("	) ");
            sb.AppendFormat(" GROUP BY ");
            sb.AppendFormat("	Hour ");
            return sb.ToString();
        }
        List<StatisticalData> ToStatisticalData(DataTable dt,int h)
        {
            List<StatisticalData> list = new List<StatisticalData>();
            StatisticalData b_heji = new StatisticalData { Hour = h };
            foreach (DataRow row in dt.Rows)
            {
                var one = new StatisticalData
                {
                    Hour = row[0].ObjToInt(),
                    Total = row[1].ObjToInt(),
                    JgpPost = row[2].ObjToInt(),
                    TracePost = row[3].ObjToInt(),
                    IFactoryPost = row[4].ObjToInt()
                };
                list.Add(one);
                b_heji.Total += one.Total;
                b_heji.JgpPost += one.JgpPost;
                b_heji.TracePost += one.TracePost;
                b_heji.IFactoryPost += one.IFactoryPost;
            }
            list.Add(b_heji);
            return list;
        }
    }

    public class StatisticalData
    {
        public int Hour { get; set; }
        [SugarColumn(IsIgnore = true)]
        public string HourStr => Hour < 0 ? $"{(Hour==-1?"白班":"夜班")}合计" : $"{Hour}:00-{(Hour + 2)}:00";
        public int Total { get; set; }
        public int JgpPost { get; set; }
        public int TracePost { get; set; }
        public int IFactoryPost { get; set; }
        [SugarColumn(IsIgnore = true)]
        public string JgpPostP => Total == 0 ? "" : Math.Round(((double)JgpPost / Total * (double)100), 2).ToString() + "%";
        [SugarColumn(IsIgnore = true)]
        public string TracePostP => Total == 0 ? "" : Math.Round(((double)TracePost / Total * (double)100), 2).ToString() + "%";
        [SugarColumn(IsIgnore = true)]
        public string IFactoryPostP => Total == 0 ? "" : Math.Round(((double)IFactoryPost / Total * (double)100), 2).ToString() + "%";

        
    }
}

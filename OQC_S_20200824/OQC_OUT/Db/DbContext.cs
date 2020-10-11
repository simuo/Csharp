using SqlSugar;
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using xxw.Logs;

namespace OQC_OUT
{
    public class DbContext
    {
        public SqlSugarClient Db;//用来处理事务多表查询和复杂的操作
        public SimpleClient<Admin> AdminDb { get { return new SimpleClient<Admin>(Db); } }
        public SimpleClient<InDatas> InDatasDb { get { return new SimpleClient<InDatas>(Db); } }
        public SimpleClient<Datas> DatasDb { get { return new SimpleClient<Datas>(Db); } }
        public SimpleClient<Settings> SettingsDb { get { return new SimpleClient<Settings>(Db); } }
        public SimpleClient<Users> UsersDb { get { return new SimpleClient<Users>(Db); } }
        public SimpleClient<UsersBase> UsersBaseDb { get { return new SimpleClient<UsersBase>(Db); } }
        public SimpleClient<Counts> CountsDb { get { return new SimpleClient<Counts>(Db); } }
        public SimpleClient<Logs> LogsDb { get { return new SimpleClient<Logs>(Db); } }
        public SimpleClient<Station> StationDb { get { return new SimpleClient<Station>(Db); } }
        public SimpleClient<Versions> VersionsDb { get { return new SimpleClient<Versions>(Db); } }
        public SimpleClient<InterfaceTime> InterfaceTimeDb { get { return new SimpleClient<InterfaceTime>(Db); } }
        public DbContext()
        {
            Db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = "Data Source=xxw.data.cache.dll;",
                DbType = DbType.Sqlite,
                InitKeyType = InitKeyType.Attribute,//从特性读取主键和自增列信息
                IsAutoCloseConnection = true
            });
#if DEBUG
            //调式代码 用来打印SQL 
            Db.Aop.OnLogExecuting = (sql, pars) =>
            {
                for (var i = pars.Length; i > 0; i--)
                {
                    var one = pars[i - 1];
                    if (one.Value?.ToString() == "True")
                        sql = sql.Replace($"@{one.ParameterName}", $"1").Replace(one.ParameterName, $"1");
                    else if (one.Value?.ToString() == "False")
                        sql = sql.Replace($"@{one.ParameterName}", $"0").Replace(one.ParameterName, $"0");
                    else if (one.Value == null)
                        sql = sql.Replace($"@{one.ParameterName}", $"null").Replace(one.ParameterName, $"null");
                    else
                        sql = sql.Replace($"@{one.ParameterName}", $"'{one.Value}'").Replace(one.ParameterName, $"'{one.Value}'");
                }
                LogDb.Log.Info(sql);
                //Console.WriteLine("====================================SQL BEGIN====================================");
                //Console.WriteLine(sql);
                //Console.WriteLine("=====================================SQL END====================================");
            };
#endif
        }
        /// <summary>
        /// 同步数据库
        /// </summary>
        public void DdAsyn()
        {
            var v = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            try
            {

                Db.Ado.Open();
                Db.Ado.Close();
                //清理数据
                DatasDb.Delete(p => p.CreateDate > DateTime.Now.AddMonths(-1));
                InDatasDb.Delete(p => p.CreateDate > DateTime.Now.AddMonths(-1));
                InterfaceTimeDb.Delete(p => p.CreateTime > DateTime.Now.AddMonths(-1));
                //判断版本
                if (!Db.DbMaintenance.GetTableInfoList().Any(p => p.Name == "Versions"))
                    InitTable(v);
                else
                {
                    if (!VersionsDb.IsAny(p => p.Ver == v))
                        InitTable(v);
                }
            }
            catch
            {
                InitTable(v);
            }
            finally
            {
                stopwatch.Stop();
#if DEBUG
                LogDb.Log.Info($"DB初始化执行耗时：{stopwatch.Elapsed.TotalMilliseconds}ms");
#endif
            }
        }

        void InitTable(string ver) {
            try
            {
                Db.CodeFirst.InitTables(
                    typeof(Datas),
                    typeof(InDatas),
                    typeof(Settings),
                    typeof(Users),
                    typeof(UsersBase),
                    typeof(Counts),
                    typeof(Admin),
                    typeof(Logs),
                    typeof(Station),
                    typeof(Versions),
                    typeof(InterfaceTime)
                    );
                if (!Db.Queryable<Counts>().Any())
                    CountsDb.Insert(new Counts());
                if (!Db.Queryable<Admin>().Any())
                    AdminDb.Insert(new Admin
                    {
                        UserName = "Admin",
                        UserPassword = "Admin".ToPwd(),
                        UserType = "管理员"
                    });
                if (Db.Queryable<Versions>().Any())
                    Db.Deleteable<Versions>().ExecuteCommand();
                VersionsDb.Insert(new Versions { Ver = ver });
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                Environment.Exit(0);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Windows;
using xxw.Config;
using xxw.Licence;
using xxw.Logs;
using xxw.TraceDataFormat;

namespace OQC_OUT
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        public static ConfigModel Config { get; private set; }
        public static SettingsModel Settings { get; private set; }
        private static List<Users> _users;
        public static void ClearUsers() { _users = null; }
        public static List<Users> Users
        {
            get
            {
                if (_users == null)
                {
                    _users = new DbContext().Read(db => db.UsersDb.GetList());//.UsersDb.GetList();
                }
                return _users;
            }
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            try
            {
                #region 全局异常捕获
                //注册全局异常处理
                DispatcherUnhandledException += App_DispatcherUnhandledException;
                #endregion
                LicenceHelper.SoftName = "捷普流水线读码系统";
                LicenceHelper.SoftCode = "JPLSXDMXT_OUT";
                if (!LicenceHelper.IsReg)
                    return;
                LogHelper.Init(LogInfo.Log, LogError.Log, LogTrace.Log, LogRead.Log, LogPOSTJGP.Log, LogPOSTIFactory.Log);
                ConfigHelper.Init();
                Config = ConfigHelper.GetConfig<ConfigModel>();
                Settings = new SettingsModel();
                #region 初始化数据库
                new DbContext().DdAsyn();
                Settings.Load();
                #endregion
                base.OnStartup(e);
            }
            catch (Exception ex)
            {
                LogError.Log.Error("未捕获异常", ex);
                MessageBox.Show(ex.Message, "启动失败", MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(0);
            }
        }
        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;

            LogInfo.Log.Error("未捕获异常", e.Exception);
            MessageBox.Show(e.Exception.Message, "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

    }
}

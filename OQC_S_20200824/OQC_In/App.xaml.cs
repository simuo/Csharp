using System;
using System.Windows;
using xxw.Config;
using xxw.Licence;
using xxw.Logs;

namespace OQC_IN
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        public static ConfigModel Config { get; private set; }
        protected override void OnStartup(StartupEventArgs e)
        {
            try
            {
                #region 全局异常捕获
                //注册全局异常处理
                DispatcherUnhandledException += App_DispatcherUnhandledException;
                #endregion
                LicenceHelper.SoftName = "捷普流水线读码系统";
                LicenceHelper.SoftCode = "JPLSXDMXT_IN";
                if (!LicenceHelper.IsReg)
                    return;
                //LogHelper.Init(LogInfo.Log, LogError.Log);
                ConfigHelper.Init();
                Config = ConfigHelper.GetConfig<ConfigModel>();
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

using System;
using System.Windows;
using System.Windows.Input;
using xxw.utilities;

namespace OQC_OUT
{
    public class MainCommands
    {
        public event Action OnLoginChange;
        public event Action OnSettingsChange;
        public ICommand LoginCommand => new Command(() =>
        {
            if ((bool)new Login { Owner = Application.Current.MainWindow }.ShowDialog())
                OnLoginChange?.Invoke();
        });
        public ICommand LoginOutCommand => new Command(() =>
        {
            if (MessageBox.Show("您确定要注销登录？", "注销提示", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                Admin.LoginAdmin = null;
                OnLoginChange?.Invoke();
            }
        });
        public ICommand ChangePwdCommand => new Command(() =>
        {
            if ((bool)new AdminChangePwd { Owner = Application.Current.MainWindow }.ShowDialog())
                MessageBox.Show("密码修改成功，请您牢记新密码！", "密码修改", MessageBoxButton.OK, MessageBoxImage.Information);
        });
        public ICommand SettingCommand => new Command(() =>
        {
            var r = new SettingsSet { Owner = Application.Current.MainWindow }.ShowDialog();
            if ((bool)r)
                OnSettingsChange?.Invoke();
        });
        public ICommand StationCommand => new Command(() => { 
            new StationList { Owner = Application.Current.MainWindow }.ShowDialog();
        });
        public ICommand UsersCommand => new Command(() =>
        {
#if CD
            new UsersList { Owner = Application.Current.MainWindow }.ShowDialog();
#endif
#if WX
            new UsersList2 { Owner = Application.Current.MainWindow }.ShowDialog();
#endif
        });
        public ICommand AdminCommand => new Command(() => new AdminList { Owner = Application.Current.MainWindow }.ShowDialog());
        public ICommand DatasExport => new Command(() => { new DatasExport { Owner = Application.Current.MainWindow }.Show(); });
        public ICommand LogsCommand => new Command(() =>
        {
            string v_OpenFolderPath = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), @"Log");
            System.Diagnostics.Process.Start("explorer.exe", v_OpenFolderPath);
        });
        public ICommand SettingLogsCommand => new Command(() =>
        {
            LogsList logsList = new LogsList { Owner = Application.Current.MainWindow };
            logsList.Show();
        });
        public ICommand ClearCountCommand => new Command(() =>
        {
            if (MessageBox.Show("您确定要重置计数？", "重置计数提示", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                return;
            ((MainWindow)Application.Current.MainWindow).Machine.ClearCount();
            //traceDataHelper.ClearCount();
            //datasHelper.ClearCount();
        });
        public ICommand ClearLogCommand => new Command(() =>
        {
            ((MainWindow)Application.Current.MainWindow).LogsData.Clear();
        });
        //public ICommand StateCommand => new Command(() =>
        //{
        //    if (detection.IsReading) return;
        //    if (detection.IsRuning)
        //    {
        //        detection.Stop(true);
        //        StopM();
        //        detection.IsRuning = false;
        //    }
        //    else
        //    {
        //        detection.Start();
        //        StartM();
        //        detection.IsRuning = true;
        //    }
        //    OnPropertyChanged(nameof(StateText));
        //    OnPropertyChanged(nameof(StopVisibility));
        //});
    }
}

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using xxw.Config;
using xxw.Licence;
using xxw.Logs;
using xxw.TraceDataFormat;
using xxw.utilities;

namespace OQC_OUT
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public MainWindow()
        {
            InitializeComponent();
            Title = LicenceHelper.SoftName + " - OUT";
            DataContext = this;
            Closing += MainWindow_Closing;
            Loaded += MainWindow_Loaded;
            InitCommand();
            InitMachine();
            InitSocket();
            InitDetection();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {

            if (!Config.POSTData.CheckParam)
            {
                Init();
            }
            else
            {
                Loading.IsBusy = true;
                Loading.Text = "机台配置验证中...";
                Task.Run(() =>
                {
                    var c = ConfigHelper.GetConfig<JObject>();
                    var station = c.SelectToken("TraceData.data.insight.test_station_attributes");
                    string url = $"{Config.POSTData.CheckParamUrl}&Line_id={station.Value<string>("line_id")}&Station_id={station.Value<string>("station_id")}&SoftwareName={station.Value<string>("software_name")}";
#if DEBUG
                    Task.Delay(500).Wait();
                    var r = new ResponseModel { Code = System.Net.HttpStatusCode.OK, Contact = "Pass" };
#else
                    var r = Http.GetXml(url);
#endif
                    if (r.Code != System.Net.HttpStatusCode.OK)
                    {
                        MessageBox.Show("验证机台配置失败，请求接口失败", "验证失败", MessageBoxButton.OK, MessageBoxImage.Warning);
                        Environment.Exit(0);
                        return;
                    }
                    if (r.Contact != "Pass")
                    {
                        MessageBox.Show($"验证机台配置失败，{r.Contact}", "验证失败", MessageBoxButton.OK, MessageBoxImage.Warning);
                        Environment.Exit(0);
                        return;
                    }
                    App.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        Loading.IsBusy = false;
                    }));
                    Init();
                });
            }
        }

        public void Init()
        {
            InitTrigger();
            InitServerSocket();
            InitInterfaceTimeChart();
        }
        #region 接口耗时统计
        public InterfaceTimeChart TimeChart { get; set; }
        void InitInterfaceTimeChart()
        {
            TimeChart = new InterfaceTimeChart();
            TimeChart.Init();
        }
        #endregion
        #region 属性
        private readonly ConfigModel Config = App.Config;
        #region 基础
        public MainCommands Commands { get; set; }
        public string SoftName => LicenceHelper.SoftName;
        public bool IsLogin => Admin.LoginAdmin != null;
        public string LoginUserText => IsLogin ? $"已登录：{Admin.LoginAdmin.UserName}[{Admin.LoginAdmin.UserType}](_L)" : "未登录";
        //public string StateText => (!detection.IsRuning && !detection.IsReading) ? "启动" : "停止";
        #endregion
        #region 控件状态
        public Visibility LoginVisibility => IsLogin ? Visibility.Visible : Visibility.Collapsed;
        public Visibility UnLoginVisibility => !IsLogin ? Visibility.Visible : Visibility.Collapsed;
        public Visibility AdminVisibility => IsLogin && Admin.LoginAdmin.UserType == "管理员" ? Visibility.Visible : Visibility.Collapsed;
        //public Visibility NgVisibility => datasHelper.TrayData.IsNg ? Visibility.Visible : Visibility.Collapsed;
        public Visibility StopVisibility => !(IOCard?.IsRuning ?? false) ? Visibility.Visible : Visibility.Collapsed;// (!detection.IsRuning && !detection.IsReading) ? Visibility.Visible : Visibility.Collapsed;
        public Visibility MoniVisibility { get; set; } = Visibility.Collapsed;
        public Visibility TrayShow => Config.Direction == "right" || Config.Direction == "left" ? Visibility.Visible : Visibility.Collapsed;
        public Visibility TrayVerticalShow => Config.Direction == "top" || Config.Direction == "bottom" ? Visibility.Visible : Visibility.Collapsed;
        public Visibility LineInVisibility => Config.CheckIn ? Visibility.Visible : Visibility.Collapsed;

        #endregion
        #region 参数
        public SettingsModel SettingsModel { get; set; } = App.Settings;
        #endregion
        #region 日志
        public ObservableCollection<string> LogsData { get; set; } = new ObservableCollection<string>();
        #endregion
        #region Data
        public string TraySpeed => (double.Parse(SettingsModel.Speed ?? "0") * 1000).ToString("0.00") + "mm/s";
        public Tray NowTray => Machine?.NowTray;
        #endregion
        #endregion
        #region Command
        void InitCommand()
        {
            Commands = new MainCommands();
            Commands.OnLoginChange += CheckLogin;
            Commands.OnSettingsChange += () => { OnPropertyChanged(nameof(SettingsModel)); };
        }
        #endregion
        #region Socket
        public SocketHelper SocketHelper { get; set; }
        void InitSocket()
        {
            SocketHelper = new SocketHelper();
            SocketHelper.OnLog += ShowLog;
        }
        #endregion
        #region 机台
        public Machine Machine { get; set; }
        void InitMachine()
        {
            Machine = new Machine();
            Machine.OnNg += (d) =>
            {
                IOCard.MachineStop();
                App.Current.Dispatcher.Invoke(new Action(() =>
                {
                    NgShow ngShow = new NgShow
                    {
                        TrayData = d,
                        Owner = this
                    };
                    ngShow.ShowDialog();
                    IOCard.MachineStart();
                }));
            };
            Machine.OnSignalError += () =>
            {
                if (IOCard.IsRuning)
                {
                    IOCard.MachineStop();
                    MessageBox.Show("检测信号异常", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    IOCard.MachineStart();
                }
            };
            Machine.OnComplate += () =>
            {
                TimeChart.Refresh(TrayInDate, DateTime.Now);
            };
        }
        #endregion
        #region 监测
        private DateTime TrayInDate;
        public IOCard IOCard { get; set; }
        void InitDetection()
        {
            //IO监测
            if (Config.IOCard.Enable)
            {
                IOCard = new IOCard();
                IOCard.OnTrayIn += () =>
                {
                    TrayInDate = DateTime.Now;
                    Machine.TrayIn();
                    UpdateUi();
                };
                IOCard.OnTrayOut += () =>
                {
                    Machine.TrayOut();
                    UpdateUi();
                };
                IOCard.OnTrigger += () =>
                {
                    int? index = Machine.NowTray?.NewColumn();
                    if (index == null) return;
                    Trigger.TriggerOn((int)index);
                    UpdateUi();
                };
                IOCard.OnMachineStart += () => { OnPropertyChanged(nameof(StopVisibility)); };
                IOCard.OnMachineStop += () => { OnPropertyChanged(nameof(StopVisibility)); };
                if (IOCard is IOCard ioCard)
                    ioCard.OnMoni += () =>
                    {
                        MoniVisibility = Visibility.Visible;
                        OnPropertyChanged(nameof(MoniVisibility));
                    };
            }
            //相机监测
            else
            {

            }
        }
        void UpdateUi()
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                OnPropertyChanged(nameof(NowTray));
                OnPropertyChanged(nameof(TraySpeed));
            }));
        }
        #endregion
        #region 读码
        public Trigger Trigger { get; set; }

        readonly object TriggerDataObject = new object();
        void InitTrigger()
        {
            Trigger = new Trigger(SocketHelper.visionClient);
            Trigger.OnLog += ShowLog;
            Trigger.OnRead += (cam, stageId, data) =>
            {

                if (cam == 1 && data.Count > 7)
                {
                    double.TryParse(data[7], out double px);
                    IOCard.AddTriggerDelay(px);
                }
                if (Machine.NowTray == null)
                {
                    LogError.Log.Debug($"Not Find Tray! \r\nTrayID:{Machine.NowTrayId} {cam} {stageId} {data.ToJson()}");
                    return;
                }

                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                Product product;// = Machine.NowTray.Add(cam, stageId, data);
                lock (TriggerDataObject)
                {
                    product = Machine.NowTray.Add(cam, stageId, data);
                    if (product == null)
                    {
                        return;
                    }
                    if (!product.Success || product.IsRepeat)
                    {
                        Machine.AddCount(product);
                        return;
                    }

                }

                stopwatch.Stop();
                //LogRead.Log.Debug($"{stageId} {cam} 锁时长：{stopwatch.Elapsed.TotalMilliseconds}");
                Task.Run(() =>
                {
                    //LogRead.Log.Debug($" {stageId} {cam} 读码完成");
                    new Trace().PostData(Machine, product);
                });

            };
        }
        #endregion
        #region 线尾服务
        public ServerHelper Server { get; set; }
        void InitServerSocket()
        {
            Server = new ServerHelper();
            Server.Init();
        }
        #endregion
        #region 方法
        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            var r = MessageBox.Show("您确定要退出系统吗？\r\n这将影响机台正常工作！！", "退出提示", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (r != MessageBoxResult.Yes)
                e.Cancel = true;
        }
        void CheckLogin()
        {
            OnPropertyChanged(nameof(IsLogin));
            OnPropertyChanged(nameof(LoginUserText));
            OnPropertyChanged(nameof(UnLoginVisibility));
            OnPropertyChanged(nameof(LoginVisibility));
            OnPropertyChanged(nameof(AdminVisibility));
        }

        void ShowLog(string msg)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                LogsData.Insert(0, $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} {msg}");
                if (LogsData.Count > 150)
                    LogsData.RemoveAt(LogsData.Count - 1);
            }));
            LogInfo.Log.Info(msg);
        }
        #endregion
        #region MVVM
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        #endregion

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            IOCard.MoniTray(2);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            IOCard.MoniMachine(IOCard.IsRuning);
        }
    }
}

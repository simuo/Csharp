using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using xxw.Licence;
using xxw.Logs;
using xxw.utilities;

namespace OQC_IN
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public MainWindow()
        {
            InitializeComponent();
            Title = LicenceHelper.SoftName + " - IN";
            Stop = new StopHelper();
            DataContext = this;
            Closing += MainWindow_Closing;
            Loaded += MainWindow_Loaded;
            IOInit();
            InitTrigger();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            InitStop();
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            var r = MessageBox.Show("您确定要退出系统吗？\r\n这将影响机台正常工作！！", "退出提示", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (r != MessageBoxResult.Yes)
                e.Cancel = true;
        }
        #region 日志
        public ObservableCollection<string> LogsData { get; set; } = new ObservableCollection<string>();
        #endregion
        #region IO
        public Visibility MoniVisibility { get; set; } = Visibility.Collapsed;
        public IOCard IOCard { get; set; }
        void IOInit()
        {
            IOCard = new IOCard();
            IOCard.OnTrigger += (index) =>
            {
                if (Stop.IsShowStop) return;
                Trigger.TriggerOn(index);
                Lines[index].SetReading();
            };
            IOCard.OnComplate += (index) =>
            {
                Lines[index].SetDefault();
                Count.UpdateToFile();
            };
            IOCard.OnMoni += () =>
            {
                MoniVisibility = Visibility.Visible;
                OnPropertyChanged(nameof(MoniVisibility));
            };
            OnPropertyChanged(nameof(IOCard));
        }
        #endregion
        #region 读码

        public Trigger Trigger { get; set; }
        public SocketHelper SocketHelper { get; set; }
        public CountHelper Count { get; set; }
        public LineOut LineOut { get; set; }
        public List<Line> Lines { get; set; }
        //public Line Line1 { get; set; }
        //public Line Line2 { get; set; }
        void InitSocket()
        {
            SocketHelper = new SocketHelper();
            SocketHelper.OnLog += ShowLog;
        }
        void InitLineOut()
        {
            LineOut = new LineOut();
            LineOut.Init();
        }
        void InitTrigger()
        {
            Count = new CountHelper();
            InitSocket();
            InitLineOut();
            int LineNum = 0;
            Lines = new List<Line>();
            foreach(var one in App.Config.IOCard.Line)
            {
                LineNum++;
                Lines.Add(new Line(LineNum, Count, LineOut, IOCard, Stop));
            }

            Trigger = new Trigger(SocketHelper.visionClient);
            Trigger.OnLog += ShowLog;
            Trigger.OnRead += (cam, stageId, data) =>
            {
                Task.Run(() =>
                {
                    var line = Lines[stageId - 1];
                    if (line.IsReading)
                    {
                        line.Read(data);
                    }
                });
            };
        }
        #endregion
        #region Command
        public ICommand ClearLogCommand => new Command(() =>
        {
            if (MessageBox.Show("您确定要重置计数？", "重置计数提示", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                return;
            Count.Clear();
        });
        public ICommand ClearLogCommand1 => new Command(() =>
        {
            if (MessageBox.Show("您确定要重置线1计数？", "重置计数提示", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                return;
            Lines[0].ClearCount();
        });
        public ICommand ClearLogCommand2 => new Command(() =>
        {
            if (MessageBox.Show("您确定要重置线2计数？", "重置计数提示", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                return;
            Lines[1].ClearCount();
        });
        #endregion
        #region 空闲停机
        public StopHelper Stop { get; set; }
        private void InitStop()
        {
            Stop.Init();
            OnPropertyChanged(nameof(Stop));

        }
        #endregion
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
        #region MVVM
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        #endregion

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn)
            {
                Moni(btn, 0);
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn)
            {
                Moni(btn, 1);
            }
        }
        private void Moni(Button btn,int line)
        {
            btn.IsEnabled = false;
            IOCard.Moni(line, () =>
            {
                App.Current.Dispatcher.Invoke(new Action(() =>
                {
                    btn.IsEnabled = true;
                }));
            });
        }
        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Stop.ShowStop();
        }
    }
}

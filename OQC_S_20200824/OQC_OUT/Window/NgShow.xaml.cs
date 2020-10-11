using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using xxw.utilities;

namespace OQC_OUT
{
    /// <summary>
    /// NgShow.xaml 的交互逻辑
    /// </summary>
    public partial class NgShow : Window, INotifyPropertyChanged
    {
        private readonly ConfigModel Config = App.Config;
        public Visibility TrayShow { get; set; } = Visibility.Collapsed;
        public Visibility TrayVerticalShow { get; set; } = Visibility.Collapsed;

        public Tray TrayData { get; set; }
        public NgShow()
        {
            TrayShow = Config.Direction == "right" || Config.Direction == "left" ? Visibility.Visible : Visibility.Collapsed;
            TrayVerticalShow = Config.Direction == "top" || Config.Direction == "bottom" ? Visibility.Visible : Visibility.Collapsed;
            InitializeComponent();
            DataContext = this;
            Closing += NgShow_Closing;
        }

        private void NgShow_Closing(object sender, CancelEventArgs e)
        {
            if (TrayData.IsPosting)
            {
                e.Cancel = true;
                MessageBox.Show("产品数据正在上抛中，请等待！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                if (MessageBox.Show("您确定已完成产品清理？", "提示", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                    e.Cancel = true;
            }
        }

        public void UpdateInfo(Tray trayData)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                TrayData = trayData;
                OnPropertyChanged(nameof(TrayData));
            }));
        }

        public ICommand OkBtnCommand => new Command(() =>
        {
            Close();
        });

        #region
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        #endregion
    }
}

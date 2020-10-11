using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using xxw.utilities;

namespace OQC_OUT
{
    /// <summary>
    /// LogsList.xaml 的交互逻辑
    /// </summary>
    public partial class LogsList : Window, INotifyPropertyChanged
    {
        public LogsList()
        {
            InitializeComponent();
            DataContext = this;
            LoadLogsData();
        }

        public List<Logs> LogsData { get; set; }
        public DateTime StartDate { get; set; } = DateTime.Now.AddDays(-30);
        public DateTime EndDate { get; set; } = DateTime.Now;
        public ICommand SearchCommand => new Command(() => { LoadLogsData(); });
        public ICommand ClearCommand => new Command(() =>
        {
            if (MessageBox.Show("您确定要清除所有操作记录？", "清除提示", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                new DbContext().Db.Deleteable<Logs>().ExecuteCommand();
                LogsHelper.LogWrite("清除操作记录");
                LoadLogsData();
            }
        });
        void LoadLogsData()
        {
            var sd = DateTime.Parse(StartDate.ToString("yyyy-MM-dd 00:00:00"));
            var ed = DateTime.Parse(EndDate.ToString("yyyy-MM-dd 23:59:59"));
            LogsData = new DbContext().Db.Queryable<Logs>().Where(p => SqlSugar.SqlFunc.Between(p.CreateDate, sd, ed)).OrderBy(p => p.CreateDate, SqlSugar.OrderByType.Desc).ToList();
            OnPropertyChanged(nameof(LogsData));
        }
        #region
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        #endregion
    }
}

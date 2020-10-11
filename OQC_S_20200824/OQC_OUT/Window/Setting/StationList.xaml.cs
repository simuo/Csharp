using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using xxw.utilities;

namespace OQC_OUT
{
    /// <summary>
    /// StationList.xaml 的交互逻辑
    /// </summary>
    public partial class StationList : Window, INotifyPropertyChanged
    {
        readonly DbContext db = new DbContext();
        public StationList()
        {
            InitializeComponent();
            DataContext = this;
            GetStation();
        }
        public void GetStation()
        {
            StationData = db.StationDb.GetList().OrderBy(p => p.StationNum).ToList();
            var StationList = App.Config.Station;
            foreach (var one in StationData)
            {
                if (int.TryParse(one.StationNum, out int iStationNum) && iStationNum <= StationList.Count)
                    one.StationName = StationList[iStationNum - 1];
            }
            OnPropertyChanged(nameof(StationData));
        }
        public int SelectedIndex { get; set; }
        public List<Station> StationData { get; set; }
        public ICommand AddCommand => new Command(() => {
            StationEdit edit = new StationEdit
            {
                Owner = this,
                NowData = new Station { StationCode = "", StationNum = "" }
            };
            if ((bool)edit.ShowDialog())
                GetStation();
        });
        public ICommand EditCommand => new Command(() => {
            if (StationData.Count < SelectedIndex) return;
            StationEdit edit = new StationEdit
            {
                Owner = this,
                NowData = StationData[SelectedIndex]
            };
            if ((bool)edit.ShowDialog())
                GetStation();
        });
        public ICommand DeleteCommand => new Command(() => {
            if (StationData.Count < SelectedIndex) return;
            Station selectData = StationData[SelectedIndex];

            if (MessageBox.Show($"您确定要删除工站码【{selectData.StationCode}】？", "删除提示", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                db.StationDb.DeleteById(selectData.StationCode);
                GetStation();
                LogsHelper.LogWrite($"删除工站码：{selectData.StationCode}");
            }
        });

        #region MVVM
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        #endregion
    }
}

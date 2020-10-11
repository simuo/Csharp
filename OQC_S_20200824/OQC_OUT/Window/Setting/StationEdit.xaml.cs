using NPOI.SS.Formula.Functions;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using xxw.utilities;

namespace OQC_OUT
{
    /// <summary>
    /// StationEdit.xaml 的交互逻辑
    /// </summary>
    public partial class StationEdit : Window
    {
        readonly DbContext db = new DbContext();
        private Station nowData;
        private int stationSelectIndex;

        public Station NowData
        {
            get => nowData;
            set
            {
                nowData = value;
                IsAdd = string.IsNullOrEmpty(NowData.StationCode);
                if (IsAdd)
                {
                    Title = "添加工站码";
                    nowData.StationNum = "1";
                    nowData.StationName = StationList[0];
                }
                else
                {
                    Title = "编辑工站码";
                    SCode.IsReadOnly = true;
                    stationSelectIndex = int.Parse(NowData.StationNum) - 1;
                }
            }
        }
        public int StationSelectIndex
        {
            get => stationSelectIndex; 
            set
            {
                stationSelectIndex = value;
                NowData.StationNum = (value + 1).ToString();
                NowData.StationName = StationList[value];
            }
        }
        public List<string> StationList => App.Config.Station;
        private bool IsAdd { get; set; }
        public StationEdit()
        {
            InitializeComponent();
            DataContext = this;
            
        }

        public ICommand AddCommand => new Command(() =>
        {
            if (string.IsNullOrEmpty(NowData.StationCode))
            {
                MessageBox.Show("工站码不能为空", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (string.IsNullOrEmpty(NowData.StationNum))
            {
                MessageBox.Show("工站号不能为空", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (IsAdd)
            {
                if (db.StationDb.IsAny(p => p.StationCode == NowData.StationCode))
                {
                    MessageBox.Show("工站码已存在，无需重复添加", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                DialogResult = db.StationDb.Insert(NowData);
            }
            else
                DialogResult = db.StationDb.Update(NowData);
            if ((bool)DialogResult)
                LogsHelper.LogWrite($"{(IsAdd ? "添加" : "编辑")}工站码：{NowData.StationCode}");
            Close();
        });
    }
}

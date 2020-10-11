using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using xxw.utilities;

namespace OQC_OUT
{
    /// <summary>
    /// SettingsSet.xaml 的交互逻辑
    /// </summary>
    public partial class SettingsSet : Window, INotifyPropertyChanged
    {
        DbContext db = new DbContext();
        public SettingsSet()
        {
            LoadSetting();
            InitializeComponent();
            DataContext = this;
        }
        #region 属性
        public SettingsModel SettingsModel { get; set; } = new SettingsModel();
        public List<Settings> Settings { get; set; }
        public Visibility AdminVisibility => Admin.LoginAdmin?.UserType == "管理员" ? Visibility.Visible : Visibility.Collapsed;

        #endregion
        #region Command
        public ICommand SaveCommand => new Command(() =>
        {
            List<string> logs = new List<string>();
            foreach (var one in PropertyInfos)
            {
                var val = one.GetValue(SettingsModel, null)?.ToString();
                if (string.IsNullOrEmpty(val)) continue;
                Settings selectedEntity = Settings?.FirstOrDefault(p => p.Type == one.Name && p.IsSelected == true);
                if (selectedEntity != null)
                {
                    selectedEntity.IsSelected = false;
                    db.Db.Updateable(selectedEntity).AddQueue();
                }

                if (Settings.Any(p => p.Type == one.Name && p.Value == val))
                {
                    Settings entity = Settings.First(p => p.Type == one.Name && p.Value == val);
                    entity.IsSelected = true;
                    db.Db.Updateable(entity).AddQueue();
                    logs.Add($"修改参数[{one.Name}]：{val}");
                }
                else
                {
                    Settings entity = new Settings { IsSelected = true, Type = one.Name, Value = val };
                    db.Db.Insertable(entity).AddQueue();
                    logs.Add($"新增参数[{one.Name}]：{val}");
                }
            }
            db.Db.SaveQueues();
            LoadSetting();
            MessageBox.Show("参数保存成功", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            DialogResult = true;
            App.Settings.Load();
            LogsHelper.LogWrite(string.Join("\r\n", logs));
        });
        public ICommand DeleteCommand => new Command(() => {
            if (MessageBox.Show("是否要清除所有参数设置？", "提示", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                return;
            db.Db.Deleteable<Settings>().ExecuteCommand();
            LoadSetting();
            MessageBox.Show("所有参数清除成功", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            LogsHelper.LogWrite($"清除所有参数成功");
        });
        #endregion
        #region 方法
        IEnumerable<PropertyInfo> PropertyInfos
            => SettingsModel.GetType().GetProperties().Where(p =>
            p.CanWrite == true
            && p.PropertyType == typeof(string));
        void LoadSetting()
        {
            Settings = db.SettingsDb.GetList();
            SettingsModel.Load();
            OnPropertyChanged(nameof(Settings));
            OnPropertyChanged(nameof(SettingsModel));
        }
        #endregion

        #region
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        #endregion
    }

    public class SettingsListConver : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            List<string> data = new List<string>();
            if (value is List<Settings> list)
            {
                foreach (var one in list.Where(p => p.Type == parameter.ToString()).OrderBy(p=>p.Value))
                    data.Add(one.Value);
            }
            return data;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}

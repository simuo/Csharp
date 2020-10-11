using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using xxw.utilities;

namespace OQC_OUT
{
    /// <summary>
    /// AdminAdd.xaml 的交互逻辑
    /// </summary>
    public partial class AdminAdd : Window, INotifyPropertyChanged
    {
        public AdminAdd()
        {
            InitializeComponent();
            DataContext = this;
        }
        public string UserName { get; set; }
        public int? UserType { get; set; }
        public ICommand AddCommand => new Command(() =>
        {
            if (UserType == null)
            {
                MessageBox.Show("请选择用户角色", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (string.IsNullOrEmpty(UserName))
            {
                MessageBox.Show("用户名不能为空", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (string.IsNullOrEmpty(PasswordText.Password))
            {
                MessageBox.Show("密码不能为空", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            DbContext db = new DbContext();
            if (db.AdminDb.IsAny(p => p.UserName == UserName))
            {
                MessageBox.Show("添加的用户名已存在", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            DialogResult = db.AdminDb
            .Insert(new Admin
            {
                UserName = UserName,
                UserType = UserType == 0 ? "员工" : "管理员",
                UserPassword = PasswordText.Password.ToPwd()
            });
            if ((bool)DialogResult)
                LogsHelper.LogWrite($"添加{(UserType == 0 ? "员工" : "管理员")}：{UserName}"); 
            Close();
        });
        #region
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        #endregion
    }
}

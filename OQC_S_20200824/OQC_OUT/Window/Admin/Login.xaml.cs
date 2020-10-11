using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using xxw.utilities;
using System.Linq;

namespace OQC_OUT
{
    /// <summary>
    /// Login.xaml 的交互逻辑
    /// </summary>
    public partial class Login : Window, INotifyPropertyChanged
    {
        public Login()
        {
            InitializeComponent();
            DataContext = this;
        }
        public string UserName { get; set; }
        public List<string> UserList
        {
            get
            {
                return (from one in new DbContext().AdminDb.GetList()
                        select one.UserName).ToList();
            }
        }

        public ICommand LoginCommand => new Command(() =>
        {
            if (string.IsNullOrEmpty(UserName))
            {
                MessageBox.Show("请选择要登录的用户", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (string.IsNullOrEmpty(UserName))
            {
                MessageBox.Show("请输入登录密码", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            Admin admin = new DbContext().AdminDb.GetSingle(p => p.UserName == UserName);
            if (admin == null)
            {
                MessageBox.Show("用户不存在！", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                LogsHelper.LogWrite($"用户[{UserName}]登录失败：用户不存在");
                return;
            }
            if (admin.UserPassword != PasswordText.Password.ToPwd())
            {
                MessageBox.Show("用户密码验证失败！", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                LogsHelper.LogWrite($"用户[{UserName}]登录失败：用户密码验证失败");
                return;
            }
            DialogResult = true;
            Admin.LoginAdmin = admin;
            LogsHelper.LogWrite($"用户[{UserName}]登录成功");
            Close();
        });
        #region
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        #endregion
    }
}

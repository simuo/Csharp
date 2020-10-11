using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using xxw.utilities;

namespace OQC_OUT
{
    /// <summary>
    /// AdminChangePwd.xaml 的交互逻辑
    /// </summary>
    public partial class AdminChangePwd : Window, INotifyPropertyChanged
    {
        public AdminChangePwd()
        {
            InitializeComponent();
            DataContext = this;
        }
        public string UserName => Admin.LoginAdmin.UserName;
        public ICommand SetPwdCommand => new Command(() =>
        {
            if (string.IsNullOrEmpty(PasswordOldText.Password))
            {
                MessageBox.Show("原密码不能为空！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (string.IsNullOrEmpty(PasswordText.Password))
            {
                MessageBox.Show("设置的密码不能为空！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (PasswordText.Password != PasswordReText.Password)
            {
                MessageBox.Show("两次密码输入不一致！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (PasswordOldText.Password.ToPwd() != Admin.LoginAdmin.UserPassword)
            {
                MessageBox.Show("原密码验证失败！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            Admin.LoginAdmin.UserPassword = PasswordText.Password.ToPwd();
            DialogResult = new DbContext().AdminDb.Update(Admin.LoginAdmin);
            if ((bool)DialogResult)
                LogsHelper.LogWrite($"修改用户[{Admin.LoginAdmin.UserName}]的密码");
            Close();
        });

        #region
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        #endregion
    }
}

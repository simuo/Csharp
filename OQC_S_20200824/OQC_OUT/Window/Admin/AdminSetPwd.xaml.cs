using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using xxw.utilities;

namespace OQC_OUT
{
    /// <summary>
    /// AdminSetPwd.xaml 的交互逻辑
    /// </summary>
    public partial class AdminSetPwd : Window, INotifyPropertyChanged
    {
        public AdminSetPwd(Admin admin)
        {
            NowAdmin = admin;
            InitializeComponent();
            DataContext = this;
        }
        public Admin NowAdmin { get; }
        public string UserName => NowAdmin.UserName;
        public ICommand SetPwdCommand => new Command(() =>
        {
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
            NowAdmin.UserPassword = PasswordText.Password.ToPwd();
            DialogResult = new DbContext().AdminDb.Update(NowAdmin);
            if ((bool)DialogResult)
                LogsHelper.LogWrite($"修改用户[{UserName}]的密码");
            Close();
        });
        #region
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        #endregion
    }
}

using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using xxw.utilities;

namespace OQC_OUT
{
    /// <summary>
    /// AdminList.xaml 的交互逻辑
    /// </summary>
    public partial class AdminList : Window, INotifyPropertyChanged
    {
        public AdminList()
        {
            InitializeComponent();
            DataContext = this;
            GetAdmins();
        }
        public int SelectedIndex { get; set; }
        public List<Admin> Admins { get; set; }
        void GetAdmins()
        {
            Admins = new DbContext().AdminDb.GetList(p => p.Id != Admin.LoginAdmin.Id);
            OnPropertyChanged(nameof(Admins));
        }
        public ICommand AddCommand => new Command(() =>
        {
            AdminAdd adminAdd = new AdminAdd
            {
                Owner = this
            };
            if ((bool)adminAdd.ShowDialog())
                GetAdmins();
        });
        public ICommand SetPwdCommand => new Command(() =>
        {
            if (SelectedIndex < 0)
            {
                return;
            }
            AdminSetPwd adminSetPwd = new AdminSetPwd(Admins[SelectedIndex])
            {
                Owner = this
            };
            adminSetPwd.ShowDialog();
        });
        public ICommand DeleteCommand => new Command(() =>
        {
            if (SelectedIndex < 0)
            {
                return;
            }
            var db = new DbContext();
            Admin selectAdmin = Admins[SelectedIndex];
            if (!db.AdminDb.IsAny(p => p.Id != selectAdmin.Id && p.UserType == "管理员"))
            {
                MessageBox.Show("最后一个管理员不能删除！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (MessageBox.Show("您确定要删除选中用户？", "删除提示", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                db.AdminDb.DeleteById(selectAdmin.Id);
                GetAdmins();
                LogsHelper.LogWrite($"删除用户：（{selectAdmin.UserType}）{selectAdmin.UserName}");

            }
        });

        #region
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        #endregion
    }
}

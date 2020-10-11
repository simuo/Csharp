using NPOI.SS.Formula.Functions;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using xxw.utilities;

namespace OQC_OUT
{
    /// <summary>
    /// UserAdd.xaml 的交互逻辑
    /// </summary>
    public partial class UserEdit : Window, INotifyPropertyChanged
    {
        public Users EditUser
        {
            get => editUser;
            set
            {
                editUser = value;
                var old = db.UsersDb.GetById(EditUser.UserNumber);
                if (old != null)
                {
                    isEdit = true;
                    EditUser.UserCode = old.UserCode;
                    EditUser.UserType = old.UserType;
                }
                Title = $"{(isEdit ? "编辑" : "添加")}检验员信息";
            }
        }

        readonly DbContext db = new DbContext();
        private Users editUser;

        private bool isEdit { get; set; }
        public UserEdit()
        {
            InitializeComponent();
            DataContext = this;

            ContentRendered += UserEdit_ContentRendered;
            
        }

        private void UserEdit_ContentRendered(object sender, System.EventArgs e)
        {
            AddInput.SelectionBrush = Brushes.Blue;
            AddInput.Focus();
            AddInput.SelectAll();
        }

        public ICommand AddCommand => new Command(() =>
        {
            if (string.IsNullOrEmpty(EditUser.UserCode))
            {
                MessageBox.Show("检测员工站码不能为空", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (string.IsNullOrEmpty(EditUser.UserName))
            {
                MessageBox.Show("检测员名称不能为空", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (string.IsNullOrEmpty(EditUser.UserType))
            {
                MessageBox.Show($"工站码【{EditUser.UserCode}】未绑定工站！\r\n请先绑定工站码", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            EditUser.SeatingCode = $"{EditUser.UserCode}_{App.Config.Station[Convert.ToInt32(EditUser.UserType) - 1]}";
            //删除旧信息
            db.UsersDb.Delete(p => p.UserCode == EditUser.UserCode || p.UserNumber == EditUser.UserNumber);
            DialogResult = db.UsersDb.Insert(EditUser);
            if ((bool)DialogResult)
            {
                App.ClearUsers();
                LogsHelper.LogWrite($"保存检测员：（{EditUser.UserNumber}）{EditUser.UserName} -工站码：{EditUser.UserCode} 工站号：{EditUser.UserType}");
            }
            Close();
        });
        #region
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        #endregion

        private void AddInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            var d = db.StationDb.GetById(EditUser.UserCode);
            if (d != null)
                EditUser.UserType = d.StationNum;
            else
                EditUser.UserType = "";
            OnPropertyChanged(nameof(EditUser));
        }

        private void AddInput_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                textBox.SelectionBrush = Brushes.Blue;
                textBox.Focus();
                textBox.SelectAll();
            }
        }
    }
}

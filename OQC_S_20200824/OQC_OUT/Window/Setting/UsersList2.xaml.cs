using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
    /// UsersList2.xaml 的交互逻辑
    /// </summary>
    public partial class UsersList2 : Window, INotifyPropertyChanged
    {
        public UsersList2()
        {
            InitializeComponent();
            DataContext = this;
            Load();
        }
        const int UserCount = 10;
        private ConfigModel Config = App.Config;
        public string NowClasses => App.Settings.ShiftCode;
        public ObservableCollection<StationModel> Stations { get; set; }
        public int ColumnsNum => (Config.Station.Count / 2) + (Config.Station.Count % 2 > 0 ? 1 : 0);
        void Load()
        {
            var db = new DbContext();
            var users = db.UsersDb.GetList(p => p.UserClasses == NowClasses);
            Stations = new ObservableCollection<StationModel>();
            var stations = Config.Station.ToJson().ToEntity<List<string>>();
            stations.Add("Retrial");
            for (var i = 1; i <= Config.Station.Count; i++)
            {
                var stationCode = Config.Station[i - 1];
                var station = new StationModel
                {
                    Index = i - 1,
                    StationName = stationCode == "Retrial" ? "复判工站" : $"工站{i}",
                    StationCheck = stationCode
                };
                var stations_users = users.Where(p => p.UserType == i.ToString());
                int row_num = 1;
                foreach (var one in stations_users)
                {
                    one.SeatingCode = $"{station.StationCheck}-{row_num.ToString().PadLeft(2, '0')}";
                    station.Users.Add(one);
                    row_num++;
                }
                for (int ii = 0; ii < UserCount - stations_users.Count(); ii++)
                {
                    station.Users.Add(new Users
                    {
                        UserType = i.ToString(),
                        SeatingCode = $"{station.StationCheck}-{row_num.ToString().PadLeft(2, '0')}"
                    });
                    row_num++;
                }
                Stations.Add(station);
            }
            //int fpIndex = Config.Station.Count + 1;
            //Stations.Add(new StationModel {
            //    Index = fpIndex - 1,
            //    StationName = $"复判工站",
            //    StationCheck = "Retrial"
            //});
            OnPropertyChanged(nameof(Stations));
        }

        private void DataGrid_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is DataGrid grid)
            {
                grid.BeginEdit(e);
            }
        }
        private string LastValue;
        private void DataGrid_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            if (e.Column.GetCellContent(e.Row) is TextBlock block)
                LastValue = block?.Text ?? "";
        }

        private void DataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (sender is DataGrid grid)
                if (e.Key == Key.Enter)
                {
                    e.Handled = true;
                    int row = grid.Items.IndexOf(grid.CurrentItem);
                    int Col = grid.Columns.IndexOf(grid.CurrentColumn);
                    var currentCell = GetCell(grid, row, Col);
                    if (currentCell.Content is TextBox block)
                    {
                        var newValue = block.Text;
                        if (newValue != LastValue)
                        {
                            var stations_index = Convert.ToInt32(grid.Tag);
                            #region 处理员工工号
                            if (Col == 0)
                            {
                                var old_user = Stations[stations_index].Users[row];
                                if (string.IsNullOrEmpty(newValue))
                                {
                                    old_user.UserName = "";
                                    old_user.UserCode = "";
                                    old_user.OnPropertyChanged("UserName");
                                    old_user.OnPropertyChanged("UserCode");
                                    LastValue = "";
                                    return;
                                }
                                //查询数据
                                var user = new DbContext().UsersBaseDb.GetById(newValue);
                                if (user == null)
                                {
                                    MessageBox.Show($"未查询到相关员工信息，员工ID：{newValue}", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                                    block.Text = LastValue;
                                    return;
                                }
                                //检查重复
                                if(Stations.Any(p=>p.Users.Any(u=>u.UserNumber == newValue)))
                                {
                                    var ss = Stations.FirstOrDefault(p => p.Users.Any(u => u.UserNumber == newValue));
                                    var uu = ss.Users.FirstOrDefault(u => u.UserNumber == newValue);
                                    MessageBox.Show($"员工ID：{newValue} \r\n已关联到 工站：{ss.StationName} 座位：{uu.SeatingCode}", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                                    block.Text = LastValue;
                                    return;
                                }
                                
                                old_user.UserClasses = NowClasses;
                                old_user.UserName = user.UserName;
                                old_user.Grade = user.Grade;
                                old_user.OnboardDate = user.OnboardDate;
                                old_user.QualificationDate = user.QualificationDate;
                                old_user.UserNamePY = user.UserNamePY;
                                old_user.UserPhoto = $"{user.UserNumber}.jpg";
                                old_user.OnPropertyChanged("UserName");
                                SetNextCell(grid);
                            }
                            #endregion
                            #region 处理员工码
                            if (Col == 2) {
                                //检查重复
                                if (Stations.Any(p => p.Users.Any(u => u.UserCode == newValue)))
                                {
                                    var ss = Stations.FirstOrDefault(p => p.Users.Any(u => u.UserCode == newValue));
                                    var uu = ss.Users.FirstOrDefault(u => u.UserCode == newValue);
                                    MessageBox.Show($"员工代码：{newValue} \r\n已关联到 工站：{ss.StationName} 座位：{uu.SeatingCode}", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                                    block.Text = LastValue;
                                    return;
                                }
                                var old_user = Stations[stations_index].Users[row];
                                old_user.UserCode = newValue;
                                old_user.OnPropertyChanged("UserCode");
                                SetNextCell(grid);
                            }
                            #endregion
                        }
                        else
                            SetNextCell(grid);
                    }
                    else
                    {
                        SetNextCell(grid);
                    }
                }
        }
        private void SetNextCell(DataGrid grid)
        {
            int row = grid.Items.IndexOf(grid.CurrentItem);
            int Col = grid.Columns.IndexOf(grid.CurrentColumn);
            var currentCell = GetCell(grid, row, Col);
            if (Col + 1 == grid.Columns.Count)
            {
                row = row + 1;
                Col = 0;
            }
            else
            {
                Col = Col + 1;
            }
            var cell = GetCell(grid, row, Col);
            
            if (cell != null)
            {
                currentCell.IsSelected = false;
                cell.IsSelected = true;
                cell.Focus();
                if (cell.IsReadOnly)
                    SetNextCell(grid);
                else
                {
                    grid.BeginEdit();
                }
            }
        }
        #region 事件
        public ICommand SaveCommand => new Command(() => {
            try
            {
                var db = new DbContext().Db;
                db.Deleteable<Users>(p => p.UserClasses == NowClasses || p.UserClasses == null).AddQueue();
                foreach (var station in Stations)
                {
                    foreach (var user in station.Users.Where(p => !string.IsNullOrEmpty(p.UserNumber) && !string.IsNullOrEmpty(p.UserCode)))
                    {
                        db.Insertable(user).AddQueue();
                    }
                }
                db.SaveQueues();
                MessageBox.Show($"保存检验员信息成功", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                App.ClearUsers();
            }
            catch(Exception ex)
            {
                MessageBox.Show($"保存检验员信息失败\r\n{ex.Message}", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        });
        public ICommand ImportTempCommand => new Command(() =>
        {
            try
            {
                System.Windows.Forms.SaveFileDialog sfd = new System.Windows.Forms.SaveFileDialog
                {
                    Filter = "excel文件(*xlsx)|*xlsx",
                    FileName = "用户数据导入模板.xlsx"
                };
                if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    System.Reflection.Assembly Asmb = System.Reflection.Assembly.GetExecutingAssembly();
                    string strName = Asmb.GetName().Name + ".Window.Temp.xlsx";
                    Stream excelStream = Asmb.GetManifestResourceStream(strName);

                    byte[] excelBuf = new Byte[excelStream.Length];
                    excelStream.Read(excelBuf, 0, excelBuf.Length);
                    excelStream.Seek(0, SeekOrigin.Begin);
                    string path = sfd.FileName;
                    using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write))
                    {
                        fs.Write(excelBuf, 0, excelBuf.Length);
                        fs.Close();
                    }
                    MessageBox.Show($"导出成功!\r\n{path}", "导出成功", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"导出失败!\r\n{ex.Message}", "导出失败", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        });
        public ICommand ImportCommand => new Command(() =>
        {
            System.Windows.Forms.OpenFileDialog dialog = new System.Windows.Forms.OpenFileDialog
            {
                Title = "请选择要导入用户信息表",
                Filter = "excel文件(*xlsx)|*xlsx"
            };
            if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
            {
                return;
            }
            Task.Run(() =>
            {
                try
                {
                    string file = dialog.FileName;
                    using (FileStream fs = File.OpenRead(file))
                    {
                        IWorkbook workbook = new XSSFWorkbook(fs);
                        if (workbook == null)
                        {
                            MessageBox.Show($"导入失败!\r\n导入表中未找到用户数据", "导入失败", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                        ISheet sheet = workbook.GetSheet("Users");
                        if (sheet == null)
                        {
                            MessageBox.Show($"导入失败!\r\n导入表中未找到名为Users的Sheet", "导入失败", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                        int rowCount = sheet.LastRowNum;//总行数
                        if (rowCount <= 1)
                        {
                            MessageBox.Show($"导入失败!\r\n导入表中用户数量为0" +
                                $"", "导入失败", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                        App.Current.Dispatcher.Invoke(new Action(() =>
                        {
                            //加载提示
                            Loading.IsBusy = true;
                            Loading.Text = "员工信息导入中...";
                        }));
                        //循环读取数据
                        var db = new DbContext();
                        db.Db.Deleteable<UsersBase>().ExecuteCommand();
                        IRow firstRow = sheet.GetRow(0);//第一行
                        int cellCount = firstRow.LastCellNum;//列数
                        List<string> errors = new List<string>();
                        for (int i = 1; i <= rowCount; i++)
                        {
                            App.Current.Dispatcher.Invoke(new Action(() =>
                            {
                                //加载提示
                                Loading.IsBusy = true;
                                Loading.Text = $"员工信息导入中({i}/{rowCount})...";
                            }));
                            try
                            {
                                var row = sheet.GetRow(i);
                                string userNumber = row.GetCell(0)?.StringCellValue ?? "";
                                if (!string.IsNullOrEmpty(userNumber))
                                {
                                    UsersBase users = new UsersBase
                                    {
                                        UserNumber = row.GetCell(0).StringCellValue,
                                        UserName = row.GetCell(1).StringCellValue,
                                        Grade = row.GetCell(2).StringCellValue,
                                    };
                                    users.OnboardDate = DateTime.Parse(row.GetCell(3).StringCellValue);
                                    users.QualificationDate = DateTime.Parse(row.GetCell(4).StringCellValue);
                                    var namepy = ToolGood.Words.WordsHelper.GetPinyinForName(users.UserName, " ");
                                    users.UserNamePY = namepy;
                                    db.Db.Insertable(users).ExecuteCommand();
                                }
                            }
                            catch (Exception ex)
                            {
                                errors.Add($"第{i + 1}行导入失败：{ex.Message}");
                            }
                        }
                        App.Current.Dispatcher.Invoke(new Action(() =>
                        {
                            Loading.IsBusy = false;
                        }));
                        if (errors.Count > 0)
                            MessageBox.Show($"导入存在错误!\r\n{string.Join("\r\n", errors)}", "导入提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                        else
                            MessageBox.Show($"全部导入成功", "导入提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch (Exception ex)
                {
                    App.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        Loading.IsBusy = false;
                    }));
                    MessageBox.Show($"导入存在错误!\r\n{ex.Message}", "导入提示", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            });
        });
        #endregion
        #region 操作DataGrid
        /// <summary>
        /// 根据行、列索引取的对应单元格对象
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public DataGridCell GetCell(DataGrid grid, int row, int column)
        {
            DataGridRow rowContainer = GetRow(grid, row);
            if (rowContainer != null)
            {
                DataGridCellsPresenter presenter = GetVisualChild<DataGridCellsPresenter>(rowContainer);
                if (presenter == null)
                {
                    grid.ScrollIntoView(rowContainer, grid.Columns[column]);
                    presenter = GetVisualChild<DataGridCellsPresenter>(rowContainer);
                }
                DataGridCell cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(column);
                return cell;
            }
            return null;
        }

        /// <summary>
        /// 根据行索引取的行对象
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public DataGridRow GetRow(DataGrid grid, int index)
        {
            DataGridRow row = (DataGridRow)grid.ItemContainerGenerator.ContainerFromIndex(index);
            if (row == null)
            {
                grid.ScrollIntoView(grid.Items[index]);
                row = (DataGridRow)grid.ItemContainerGenerator.ContainerFromIndex(index);
            }
            return row;
        }

        /// <summary>
        /// 获取指定类型的子元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parent"></param>
        /// <returns></returns>
        private T GetVisualChild<T>(Visual parent) where T : Visual
        {
            T child = default(T);
            int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < numVisuals; i++)
            {
                Visual visual = (Visual)VisualTreeHelper.GetChild(parent, i);
                child = visual as T;
                if (child == null)
                {
                    child = GetVisualChild<T>(visual);
                }
                if (child != null)
                {
                    break;
                }
            }
            return child;
        }
        #endregion
        #region MVVM
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        #endregion
    }

    public class StationModel : BaseModel
    {
        public int Index { get; set; }
        public string StationName { get; set; }
        public string StationCheck { get; set; }
        public ObservableCollection<Users> Users { get; set; } = new ObservableCollection<Users>();
    }
}

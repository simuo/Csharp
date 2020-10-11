using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Input;
using xxw.TraceDataFormat;
using xxw.utilities;

namespace OQC_OUT
{
    /// <summary>
    /// UsersList.xaml 的交互逻辑
    /// </summary>
    public partial class UsersList : Window, INotifyPropertyChanged
    {
        readonly DbContext db = new DbContext();
        readonly ConfigModel Config = App.Config;
        public UsersList()
        {
            InitializeComponent();
            DataContext = this;
            GetUserList();
        }
        void GetUserList()
        {
            Users = db.UsersDb.GetList(p => p.UserClasses == ShiftCode);
            OnPropertyChanged(nameof(Users));
        }
        public string ShiftCode => App.Settings.ShiftCode;
        public bool IsJGPWX => string.IsNullOrEmpty(Config.POSTData.GetUserUrl);
        public Visibility JGPWXShow => IsJGPWX ? Visibility.Visible : Visibility.Collapsed;
        public string NewUserNumber { get; set; }
        public int SelectedIndex { get; set; }
        public List<Users> Users { get; set; }
        public ICommand AddCommand => new Command(() =>
        {
            if (string.IsNullOrEmpty(NewUserNumber))
            {
                MessageBox.Show($"工号不能为空", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            //加载提示
            Loading.IsBusy = true;
            Loading.Text = "员工信息加载中...";
            Task.Run(() =>
            {
                try
                {
#if DEBUG
                    var res = "[PASS]-[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],[杨杰],[2019-10-15],[2019-11-11],[1],[A]";
#else
                    //oqcsi.oqcsi oqcsi = new oqcsi.oqcsi();
                    //var res = oqcsi.GetInspetorInfo(NewUserNumber);
                    var r = Http.GetXml(Config.POSTData.GetUserUrl.Replace("{user}", NewUserNumber));
                    if(r.Code != HttpStatusCode.OK)
                    {
                        Application.Current.Dispatcher.Invoke(new Action(() =>
                        {
                            Loading.IsBusy = false;
                            MessageBox.Show($"获取员工信息失败", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }));
                        return;
                    }
                    string res = r.Contact;
#endif
                    //加载数据
                    var resArr = res.Split(new string[] { "]-[" }, StringSplitOptions.None);
                    var isSuccess = resArr[0].Replace("[", "").Replace("]", "") == "PASS";
                    var data = resArr[1];
                    if (!isSuccess)
                    {
                        Application.Current.Dispatcher.Invoke(new Action(() =>
                        {
                            Loading.IsBusy = false;
                            MessageBox.Show($"员工信息获取失败！\r\n{data}", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }));
                    }
                    else
                    {
                        var resUserArr = data.Split(',');
                        var ptoto = resUserArr[0].Replace("[", "").Replace("]", "");//头像
                        var name = resUserArr[1].Replace("[", "").Replace("]", "");//姓名
                        var OnboardDate = resUserArr[2].Replace("[", "").Replace("]", "");//入职时间
                        var QualificationDate = resUserArr[3].Replace("[", "").Replace("]", "");//上岗时间
                        //var name = resUserArr[4].Replace("[", "").Replace("]", "");//在职状态
                        var Grade = resUserArr[5].Replace("[", "").Replace("]", "");//员工检验水平
                        if(Grade.ToUpper()!="A" && Grade.ToUpper() != "B" && Grade.ToUpper() != "C")
                        {
                            Application.Current.Dispatcher.Invoke(new Action(() =>
                            {
                                Loading.IsBusy = false;
                                MessageBox.Show($"员工信息验证失败！\r\n员工等级期望A、B、C 获取等级为{Grade}", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                            }));
                            return;
                        }
                        var namepy = ToolGood.Words.WordsHelper.GetPinyinForName(name, " ");
                        string fileName = "";
                        if (!string.IsNullOrEmpty(ptoto))
                        {
                            byte[] imgBytesIn = Convert.FromBase64String(ptoto);
                            MemoryStream ms = new MemoryStream(imgBytesIn);
                            Image bmp = new Bitmap(ms);
                            string photoBasePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Photo");
                            if (!Directory.Exists(photoBasePath))
                                Directory.CreateDirectory(photoBasePath);
                            fileName = $"{NewUserNumber}.jpg";
                            string photoPath = Path.Combine(photoBasePath, fileName);
                            if (File.Exists(photoPath))
                                File.Delete(photoPath);
                            bmp.Save(photoPath);
                            bmp.Dispose();
                        }
                        Application.Current.Dispatcher.Invoke(new Action(() =>
                        {
                            Loading.IsBusy = false;
                            UserEdit add = new UserEdit
                            {
                                Owner = this,
                                EditUser = new Users
                                {
                                    UserName = name,
                                    UserNumber = NewUserNumber,
                                    UserCode = "",
                                    UserPhoto = fileName,
                                    UserType = "",
                                    Grade = Grade,
                                    OnboardDate = DateTime.Parse(OnboardDate),
                                    QualificationDate = DateTime.Parse(QualificationDate),
                                    UserNamePY = namepy,
                                    UserClasses = ShiftCode
                                }
                            };
                            if ((bool)add.ShowDialog())
                                GetUserList();
                        }));
                    }


                    NewUserNumber = "";
                    OnPropertyChanged(nameof(NewUserNumber));
                }
                catch (Exception ex)
                {
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        Loading.IsBusy = false;
                        MessageBox.Show($"员工信息获取失败！\r\n{ex.Message}", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }));
                }
            });
        });
        public ICommand EditCommand => new Command(() =>
        {
            if (Users.Count < SelectedIndex) return;
            UserEdit edit = new UserEdit
            {
                Owner = this,
                EditUser = Users[SelectedIndex]
            };
            if ((bool)edit.ShowDialog())
                GetUserList();
        });
        public ICommand DeleteCommand => new Command(() =>
        {
            if (SelectedIndex < 0) return;
            if (Users.Count < SelectedIndex) return;
            Users selectUsers = Users[SelectedIndex];

            if (MessageBox.Show($"您确定要删除检测员【{selectUsers.UserName}】？", "删除提示", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                db.UsersDb.DeleteById(selectUsers.UserNumber);
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Photo", selectUsers.UserPhoto);
                if (File.Exists(path))
                    File.Delete(path);
                GetUserList();
                LogsHelper.LogWrite($"删除检测员：（{selectUsers.UserNumber}）{selectUsers.UserName} -工站码：{selectUsers.UserCode} 工站号：{selectUsers.UserType}");
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
                                UsersBase users = new UsersBase
                                {
                                    UserNumber = row.GetCell(0).StringCellValue,
                                    UserName = row.GetCell(1).StringCellValue,
                                    Grade = row.GetCell(2).StringCellValue,
                                };
                                users.OnboardDate = DateTime.Parse(row.GetCell(3).StringCellValue);
                                users.QualificationDate = DateTime.Parse(row.GetCell(4).StringCellValue);
                                db.Db.Insertable(users).ExecuteCommand();
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
                        GetUserList();
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
        public ICommand RefreshCommand => new Command(() => {
            //加载提示
            Loading.IsBusy = true;
            Task.Run(() =>
            {
                var db = new DbContext().UsersDb;
                int total = Users.Count;
                List<string> errors = new List<string>();
                for (var i = 0; i < total; i++)
                {
                    try
                    {
                        var user = Users[i];
                        App.Current.Dispatcher.Invoke(new Action(() =>
                        {
                            Loading.Text = $"员工信息刷新中({i + 1}/{total})...";
                        }));
                        #region 请求用户数据
                        var r = Http.GetXml(Config.POSTData.GetUserUrl.Replace("{user}", user.UserNumber));
                        if (r.Code != HttpStatusCode.OK)
                        {
                            errors.Add($"[{user.UserNumber}]{user.UserName}：请求接口失败");
                            continue;
                        }
                        string res = r.Contact;
                        var resArr = res.Split(new string[] { "]-[" }, StringSplitOptions.None);
                        if (resArr.Length < 2)
                        {
                            errors.Add($"[{user.UserNumber}]{user.UserName}：接口返回数据不正确");
                            continue;
                        }
                        var isSuccess = resArr[0].Replace("[", "").Replace("]", "") == "PASS";
                        var data = resArr[1];
                        if (!isSuccess)
                        {
                            errors.Add($"[{user.UserNumber}]{user.UserName}：{data}");
                            continue;
                        }
                        var resUserArr = data.Split(',');
                        if (resUserArr.Length < 6)
                        {
                            errors.Add($"[{user.UserNumber}]{user.UserName}：接口返回数据不正确");
                            continue;
                        }
                        var ptoto = resUserArr[0].Replace("[", "").Replace("]", "");//头像
                        var name = resUserArr[1].Replace("[", "").Replace("]", "");//姓名
                        var OnboardDate = resUserArr[2].Replace("[", "").Replace("]", "");//入职时间
                        var QualificationDate = resUserArr[3].Replace("[", "").Replace("]", "");//上岗时间
                                                                                                //var name = resUserArr[4].Replace("[", "").Replace("]", "");//在职状态
                        var Grade = resUserArr[5].Replace("[", "").Replace("]", "");//员工检验水平
                        if (Grade.ToUpper() != "A" && Grade.ToUpper() != "B" && Grade.ToUpper() != "C")
                        {
                            errors.Add($"[{user.UserNumber}]{user.UserName}：员工等级期望A、B、C 获取等级为{Grade}");
                            continue;
                        }
                        if(Convert.ToInt32(user.UserType)> Config.Station.Count)
                        {
                            errors.Add($"[{user.UserNumber}]{user.UserName}：绑定工站号不在配置工站范围");
                            continue;
                        }
                        var namepy = ToolGood.Words.WordsHelper.GetPinyinForName(name, " ");
                        string fileName = "";
                        if (!string.IsNullOrEmpty(ptoto))
                        {
                            byte[] imgBytesIn = Convert.FromBase64String(ptoto);
                            MemoryStream ms = new MemoryStream(imgBytesIn);
                            Image bmp = new Bitmap(ms);
                            string photoBasePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Photo");
                            if (!Directory.Exists(photoBasePath))
                                Directory.CreateDirectory(photoBasePath);
                            fileName = $"{user.UserNumber}.jpg";
                            string photoPath = Path.Combine(photoBasePath, fileName);
                            if (File.Exists(photoPath))
                                File.Delete(photoPath);
                            bmp.Save(photoPath);
                            bmp.Dispose();
                        }
                        user.UserName = name;
                        user.Grade = Grade;
                        user.OnboardDate = DateTime.Parse(OnboardDate);
                        user.QualificationDate = DateTime.Parse(QualificationDate);
                        user.UserNamePY = namepy;
                        user.SeatingCode = $"{user.UserCode}_{Config.Station[Convert.ToInt32(user.UserType) - 1]}";
                        db.Update(user);
                        #endregion
                    }
                    catch (Exception ex)
                    {
                        errors.Add($"第{i + 1}个信息刷新失败：{ex.Message}");
                        continue;
                    }
                }
                App.Current.Dispatcher.Invoke(new Action(() =>
                {
                    Loading.IsBusy = false;
                }));
                //刷新缓存
                GetUserList();
                App.ClearUsers();
                if (errors.Count == 0)
                    MessageBox.Show($"成功刷新所有用户基础信息", "刷新成功", MessageBoxButton.OK, MessageBoxImage.Information);
                else
                    MessageBox.Show($"刷新员工信息出现错误：\r\n{string.Join("\r\n", errors)}", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
            });
        });
        #region MVVM
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        #endregion
    }
}

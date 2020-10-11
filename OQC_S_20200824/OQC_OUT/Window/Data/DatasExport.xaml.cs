using NPOI.HSSF.UserModel;
using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using xxw.utilities;

namespace OQC_OUT
{
    /// <summary>
    /// DatasExport.xaml 的交互逻辑
    /// </summary>
    public partial class DatasExport : Window, INotifyPropertyChanged
    {
        public DatasExport()
        {
            InitializeComponent();
            DataContext = this;
        }

        public DateTime? StartDate { get; set; } = DateTime.Now.AddDays(-1);
        public DateTime? EndDate { get; set; } = DateTime.Now;
        public bool BtnEnable { get; set; } = true;
        public string BtnText { get { return BtnEnable ? "导出Excel" : "导出中..."; } }
        public ICommand ExportCommand => new Command(() =>
        {
            if (StartDate == null || EndDate == null || StartDate?.Ticks > EndDate?.Ticks)
            {
                MessageBox.Show(this, "请选择正确的开始和结束时间", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();
            dialog.Description = "请选择导出Excel保存文件夹";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (string.IsNullOrEmpty(dialog.SelectedPath))
                {
                    MessageBox.Show(this, "文件夹路径不能为空", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                ExportExcel(dialog.SelectedPath);
            }
        });
        void ExportExcel(string path)
        {
            BtnEnable = false;
            OnPropertyChanged(nameof(BtnEnable));
            OnPropertyChanged(nameof(BtnText));
            Task.Run(() =>
            {
                //查询数据
                DateTime sd = DateTime.Parse(((DateTime)StartDate).ToString("yyyy-MM-dd 00:00:00"));
                DateTime ed = DateTime.Parse(((DateTime)EndDate).ToString("yyyy-MM-dd 23:59:59"));
                var data = new DbContext().DatasDb.GetList(p => SqlSugar.SqlFunc.Between(p.CreateDate, sd, ed));
                HSSFWorkbook workbook = new HSSFWorkbook();
                var sheet = workbook.CreateSheet($"{sd.ToString("yyyy-MM-dd")}至{ed.ToString("yyyy-MM-dd")}");

                int rowNo = 0;
                //填充表头
                var dataRow = sheet.CreateRow(rowNo);
                dataRow.CreateCell(0).SetCellValue("条码(SN)");
                dataRow.CreateCell(1).SetCellValue("线体号");
                dataRow.CreateCell(2).SetCellValue("检验员1编码");
                dataRow.CreateCell(3).SetCellValue("检验员2编码");
                dataRow.CreateCell(4).SetCellValue("检验员3编码");
                dataRow.CreateCell(5).SetCellValue("检验员4编码");
                dataRow.CreateCell(6).SetCellValue("检验员5编码");
                dataRow.CreateCell(7).SetCellValue("检验员6编码");
                dataRow.CreateCell(8).SetCellValue("检验员1姓名");
                dataRow.CreateCell(9).SetCellValue("检验员2姓名");
                dataRow.CreateCell(10).SetCellValue("检验员3姓名");
                dataRow.CreateCell(11).SetCellValue("检验员4姓名");
                dataRow.CreateCell(12).SetCellValue("检验员5姓名");
                dataRow.CreateCell(13).SetCellValue("检验员6姓名");
                dataRow.CreateCell(14).SetCellValue("产品颜色");
                dataRow.CreateCell(15).SetCellValue("国别");
                dataRow.CreateCell(16).SetCellValue("专案");
                dataRow.CreateCell(17).SetCellValue("楼栋");
                dataRow.CreateCell(18).SetCellValue("生产阶段");
                dataRow.CreateCell(19).SetCellValue("上抛JGP");
                dataRow.CreateCell(20).SetCellValue("上抛JGP信息");
                dataRow.CreateCell(21).SetCellValue("上抛Trace");
                dataRow.CreateCell(22).SetCellValue("上抛Trace信息");

                //填充数据
                foreach (var one in data)
                {
                    rowNo++;
                    dataRow = sheet.CreateRow(rowNo);
                    dataRow.CreateCell(0).SetCellValue(one.SN);
                    dataRow.CreateCell(1).SetCellValue(one.StationId);
                    dataRow.CreateCell(2).SetCellValue(one.Ins1Code);
                    dataRow.CreateCell(3).SetCellValue(one.Ins2Code);
                    dataRow.CreateCell(4).SetCellValue(one.Ins3Code);
                    dataRow.CreateCell(5).SetCellValue(one.Ins4Code);
                    dataRow.CreateCell(6).SetCellValue(one.Ins5Code);
                    dataRow.CreateCell(7).SetCellValue(one.Ins6Code);
                    dataRow.CreateCell(8).SetCellValue(one.Ins1Name);
                    dataRow.CreateCell(9).SetCellValue(one.Ins2Name);
                    dataRow.CreateCell(10).SetCellValue(one.Ins3Name);
                    dataRow.CreateCell(11).SetCellValue(one.Ins4Name);
                    dataRow.CreateCell(12).SetCellValue(one.Ins5Name);
                    dataRow.CreateCell(13).SetCellValue(one.Ins6Name);
                    dataRow.CreateCell(14).SetCellValue(one.Color);
                    dataRow.CreateCell(15).SetCellValue(one.Region);
                    dataRow.CreateCell(16).SetCellValue(one.Project);
                    dataRow.CreateCell(17).SetCellValue(one.Location);
                    dataRow.CreateCell(18).SetCellValue(one.Pahse);
                    dataRow.CreateCell(19).SetCellValue(one.JgpPost?.ToString());
                    dataRow.CreateCell(20).SetCellValue(one.JgpPostInformation);
                    dataRow.CreateCell(21).SetCellValue(one.TracePost?.ToString());
                    dataRow.CreateCell(22).SetCellValue(one.TracePostInformation);
                }
                //保存
                string strFileName = System.IO.Path.Combine(path, $"{StartDate?.ToString("yyyyMMdd")}-{EndDate?.ToString("yyyyMMdd")}.xls");
                using (MemoryStream ms = new MemoryStream())
                {
                    using (FileStream fs = new FileStream(strFileName, FileMode.Create, FileAccess.Write))
                    {
                        workbook.Write(fs);
                    }
                }
                BtnEnable = true;
                OnPropertyChanged(nameof(BtnEnable));
                OnPropertyChanged(nameof(BtnText));
                LogsHelper.LogWrite($"导出{sd.ToString("yyyy-MM-dd")}至{ed.ToString("yyyy-MM-dd")}的数据");
                MessageBox.Show("Excel导出成功", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            });
        }
        #region
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        #endregion
    }
}

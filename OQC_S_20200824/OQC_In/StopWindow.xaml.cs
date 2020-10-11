using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using xxw.utilities;

namespace OQC_IN
{
    /// <summary>
    /// StopWindow.xaml 的交互逻辑
    /// </summary>
    public partial class StopWindow : Window, INotifyPropertyChanged
    {
        private const int GWL_STYLE = -16;
        private const int WS_SYSMENU = 0x80000;
        

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        public StopHelper StopHelper { get; set; }
        public StopWindow()
        {
            InitializeComponent();
            Loaded += StopWindow_Loaded;
            DataContext = this;
        }
        private int stopMsgIndex = -1;
        public int StopMsgIndex
        {
            get => stopMsgIndex;
            set { 
                stopMsgIndex = value;
                BtnShow = value > -1;
                OnPropertyChanged(nameof(BtnShow));
            }
        }
        public List<string> StopMsgs => StopHelper.Stop.Codes.Select(p => p.Text).ToList();
        public bool BtnShow { get; set; }

        private void StopWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var hwnd = new WindowInteropHelper(this).Handle;
            SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);
        }
        public ICommand SetCommand => new Command(() => {
            if (StopMsgIndex < 0) return;
            StopHelper.Stop.StopType = StopMsgIndex;
            StopHelper.SaveStop();
            Close();
        });
        #region MVVM
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        #endregion
    }
}

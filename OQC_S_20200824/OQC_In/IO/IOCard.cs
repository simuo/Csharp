using System;
using System.Collections.Generic;
using System.Windows;
using xxw.Logs;
using xxw.utilities;

namespace OQC_IN
{
    public class IOCard : BaseModel
    {
        private readonly ConfigModel Config = App.Config;
        private readonly short CardNo;
        private System.Threading.Tasks.Task Listener;
        public event Action<int> OnTrigger;
        public event Action<int> OnComplate;
        public event Action OnMoni;
        public string TriggerStateColor1 { get; set; }
        public string TriggerStateColor2 { get; set; }
        public IOCard()
        {
            for (int line_index = 0; line_index < Config.IOCard.Line.Count; line_index++)
            {
                LastState.Add(line_index, false);
                NowState.Add(line_index, false);
                LastStateDate.Add(line_index, null);
            }
            CardNo = USBDASK.UD_Register_Card(USBDASK.USB_7230, (ushort)Config.IOCard.CardNo);
            if (CardNo < 0)
            {
                LogInfo.Log.Fatal("IO 控制板连接失败！");
                Task.Run(() =>
                {
                    Task.Delay(1000).Wait();
                    OnMoni?.Invoke();
                });
            }
            else
            {
                Start();
            }
        }
        public void Start()
        {
            if (Listener == null)
            {
                Listener = Task.Run(() =>
                {
                    while (true)
                    {
                        int cam_index = 0;
                        foreach (var one in Config.IOCard.Line)
                        {
                            bool b = ReadDI((ushort)one.TriggerNo);
                            Trigger(b, cam_index);
                            cam_index++;
                        }
                        ChangeUI();
                        Task.Delay(10).Wait();
                    }
                });
                //初始化DO口
                uint CtrlByte = 0;
                foreach (var line in Config.IOCard.Line)
                    CtrlByte |= (uint)0 << line.MachineWaringNo;
                USBDASK.UD_DO_WritePort((ushort)CardNo, 0, CtrlByte);
            }
        }
        public void Stop()
        {
            if (Listener != null)
            {
                Listener.Dispose();
                Listener = null;
            }
        }
        bool ReadDI(ushort port)
        {
            short err = USBDASK.UD_DI_ReadPort((ushort)CardNo, 0, out uint StatusByte);
            if (err == 0)
            {
                return Convert.ToBoolean((StatusByte >> port) & 1);
            }
            return false;
        }
        bool ReadDO(ushort port)
        {
            short err = USBDASK.UD_DO_ReadPort((ushort)CardNo, 0, out uint StatusByte);
            if (err == 0)
            {
                return Convert.ToBoolean((StatusByte >> port) & 1);
            }
            return false;
        }
        void ChangeUI()
        {
            TriggerStateColor1 = LastState[0] ? "#FF11BB00" : "#FFF4F4F5";
            TriggerStateColor2 = LastState[1] ? "#FF11BB00" : "#FFF4F4F5";
            OnPropertyChanged(nameof(TriggerStateColor1));
            OnPropertyChanged(nameof(TriggerStateColor2));
        }
        #region Trigger信号
        readonly Dictionary<int, bool> LastState = new Dictionary<int, bool>();
        readonly Dictionary<int, bool> NowState = new Dictionary<int, bool>();
        readonly Dictionary<int, DateTime?> LastStateDate = new Dictionary<int, DateTime?>();
        void Trigger(bool now, int line)
        {
            if (now != NowState[line])
            {
                NowState[line] = now;
                LastStateDate[line] = DateTime.Now;
            }
            if (NowState[line] == LastState[line]) return;
            #region 滤波
            var delay = (DateTime.Now - LastStateDate[line]).Value.TotalMilliseconds;
            if ((NowState[line] && delay > Config.IOCard.Line[line].OnDelay)
                || (!NowState[line] && delay > Config.IOCard.Line[line].OffDelay))
            {
                LastState[line] = NowState[line];
                if (now)
                    Task.Run(() => { OnTrigger?.Invoke(line); });
                else
                    Task.Run(() => { OnComplate?.Invoke(line); });
            }
            #endregion
        }
        #endregion
        #region 模拟
        public void Moni(int line, Action complate)
        {
            Task.Run(() =>
            {
                int delay = 0;
                while (delay < 2000)
                {
                    Trigger(true, line);
                    ChangeUI();
                    Task.Delay(10).Wait();
                    delay += 10;
                }

                delay = 0;
                while (delay < 200)
                {
                    Trigger(false, line);
                    ChangeUI();
                    Task.Delay(10).Wait();
                    delay += 10;
                }
                complate.Invoke();
            });
        }
        #endregion
        public void Waring(int line, bool isWaring)
        {
            Console.WriteLine($"{line} {isWaring}");
            USBDASK.UD_DO_WriteLine((ushort)CardNo, 0, (ushort)Config.IOCard.Line[line].MachineWaringNo, (ushort)(!isWaring ? 0 : 1));
            if (isWaring)
                MachineStop(line);
        }
        public void MachineStop(int line)
        {
            if (!Config.IOCard.MachineStopEnable) return;
            Task.Run(() =>
            {
                Task.Delay(Config.IOCard.MachineStopDelay).Wait();
                USBDASK.UD_DO_WriteLine((ushort)CardNo, 0, (ushort)Config.IOCard.MachineStopNo, 1);
                App.Current.Dispatcher.Invoke(new Action(() =>
                {
                    MessageBox.Show($"线 {line + 1} NG，请捡NG料后点击确定后继续", "停机中...", MessageBoxButton.OK, MessageBoxImage.Warning);
                    USBDASK.UD_DO_WriteLine((ushort)CardNo, 0, (ushort)Config.IOCard.MachineStopNo, 0);
                }));
            });
        }
        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            Stop();
            USBDASK.UD_Release_Card((ushort)CardNo);
        }
    }
}

using System;
using System.Web.Services.Description;
using System.Windows;
using xxw.Logs;
using xxw.utilities;

namespace OQC_OUT
{
    /// <summary>
    /// 
    /// </summary>
    public class IOCard : BaseModel, IIOCard
    {
        private readonly ConfigModel Config = App.Config;
        private readonly short CardNo;
        private System.Threading.Tasks.Task Listener;
        public event Action OnTrayIn;
        public event Action OnTrayOut;
        public event Action OnMachineStart;
        public event Action OnMachineStop;
        public event Action OnTrigger;
        public event Action OnMoni;
        public string TrayStateColor { get; set; }
        public string MachineStateColor { get; set; }
        public string MachineWaringColor { get; set; }
        public string MachineControlColor { get; set; }

        public IOCard()
        {
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

        private bool IsMoni = false;
        private int TotlaMs = 0;
        int moniTimes = 0;

        private void StopMoni()
        {
            IsMoni = false;
            TotlaMs = 0;
        }
        public void MoniTray(int times)
        {
            if (IsMoni) return;
            moniTimes = 0;
            Task.Run(() =>
            {
                moniTimes++;
                MoniTray();

                Task.Delay(180).Wait();
                if (moniTimes < times)
                    MoniTray();
            });
        }
        public void MoniTray()
        {
            if (IsMoni) return;
            IsMoni = true;
            int t = 0;
            while (t < Config.IOCard.TrayOnDelay)
            {
                t += 10;
                SetTrayChange(true);
                Task.Delay(10).Wait();
            }
            int wt = 0;
            while (wt < 6000)
            {
                Task.Delay(10).Wait();
                if (!isSuspend)
                    wt += 10;
            }

            t = 0;
            while (t < Config.IOCard.TrayOffDelay)
            {
                t += 10;
                SetTrayChange(false);
                Task.Delay(10).Wait();
            }
            IsMoni = false;
        }
        bool isMMoni = false;
        bool isSuspend = false;
        int totlaMMs = 0;
        public void SuspendMMoni()
        {
            isSuspend = !isSuspend;
        }
        private void StopMMoni()
        {
            isMMoni = false;
            isSuspend = false;
            totlaMMs = 0;

        }
        public void MoniMachine(bool v)
        {
            if (isMMoni) {
                return;
            };
            SuspendMMoni();
            isMMoni = true;
            totlaMMs = 0;
            Task.Run(() =>
            {
                while (isMMoni)
                {
                    if (totlaMMs < 1000)
                        SetMachineChange(v);
                    else
                        StopMMoni();
                    Task.Delay(10).Wait();
                    totlaMMs += 10;
                }
            });
        }
        public void Start()
        {
            if (Listener == null)
            {
                Listener = Task.Run(() =>
                {
                    while (true)
                    {
                        bool trayState = ReadDI((ushort)Config.IOCard.TrayNo);
                        SetTrayChange(trayState);
                        bool machineState = ReadDI((ushort)Config.IOCard.MachineStateNo);
                        SetMachineChange(machineState);
                        bool machineWaring = ReadDO((ushort)Config.IOCard.MachineWaringNo);
                        bool machineControl = ReadDO((ushort)Config.IOCard.MachineControlNo);
                        ChangeUI(trayState, machineState, machineWaring, machineControl);
                        Task.Delay(10).Wait();
                    }
                });
                //初始化DO口
                uint CtrlByte = 0;
                CtrlByte |= (uint)0 << Config.IOCard.MachineControlNo;
                CtrlByte |= (uint)0 << Config.IOCard.MachineWaringNo;
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
        void ChangeUI(bool trayState, bool machineState, bool machineWaring, bool machineControl)
        {
            TrayStateColor = trayState ? "#FF11BB00" : "#FFF4F4F5";
            MachineStateColor = machineState ? "#FF11BB00" : "#FFF4F4F5";
            MachineWaringColor = machineWaring ? "#FF11BB00" : "#FFF4F4F5";
            MachineControlColor = machineControl ? "#FF11BB00" : "#FFF4F4F5";
            OnPropertyChanged(nameof(TrayStateColor),
                nameof(MachineStateColor),
                nameof(MachineWaringColor),
                nameof(MachineControlColor));
        }

        #region Tray 信号
        bool TrayNowValue = false, TrayLastValue = false;
        DateTime TrayLastChangeDate, TrayInDate;
        void SetTrayChange(bool on)
        {
            if (TrayNowValue != on)
            {
                TrayNowValue = on;
                TrayLastChangeDate = DateTime.Now;
            }
            if (TrayNowValue == TrayLastValue) return;
            var t = (DateTime.Now - TrayLastChangeDate).TotalMilliseconds;
            if ((TrayNowValue && t > Config.IOCard.TrayOnDelay)
                || (!TrayNowValue && t > Config.IOCard.TrayOffDelay))
            {
                TrayLastValue = TrayNowValue;
                if (TrayLastValue)
                {
                    MachineStoped = false;
                    TrayInDate = DateTime.Now;
                    LogRead.Log.Info($"料盘进:IO 触发");
                    OnTrayIn?.Invoke();
                    Trigger();
                }
                else
                {
                    DateTime TrayOutDate = DateTime.Now;
                    if (!MachineStoped)
                    {
                        double time = (TrayOutDate - TrayInDate).TotalMilliseconds - Config.IOCard.TrayOffDelay + Config.IOCard.TrayOnDelay;
                        double speed = Config.Tray.Length / time;
                        SettingsModel.Set(nameof(SettingsModel.Speed), speed);
                    }
                    LogRead.Log.Info($"料盘出:IO 触发 (耗时：{(TrayOutDate - TrayInDate).TotalSeconds}秒）");
                    OnTrayOut?.Invoke();
                }
            }
        }
        #endregion
        #region Machine 信号
        /// <summary>
        /// 运行中
        /// </summary>
        public bool IsRuning => MachineControl == 0;
        bool MachineNowValue = false, MachineLastValue = false, MachineStoped = false;
        DateTime MachineLastChangeDate, MachineStopDate;
        void SetMachineChange(bool on)
        {
            if (MachineNowValue != on)
            {
                MachineNowValue = on;
                MachineLastChangeDate = DateTime.Now;
            }
            if (MachineNowValue == MachineLastValue) return;

            if ((DateTime.Now - MachineLastChangeDate).TotalMilliseconds > Config.IOCard.MachineStateDelay)
            {
                MachineLastValue = MachineNowValue;
                SetMachineChangeDo(MachineLastValue);
            }
        }
        void SetMachineChangeDo(bool state)
        {
            MachineControl = (uint)(state ? 1 : 0);
            if (state)
            {
                MachineStoped = true;
                OnMachineStop?.Invoke();
                MachineStopDate = DateTime.Now;
                triggerTimer?.Stop();
                LogRead.Log.Info($"停机:{!triggerTimer?.Enabled}");
            }
            else
            {
                OnMachineStart?.Invoke();
                //计算停机时长
                nextDelay += (DateTime.Now - MachineStopDate).TotalMilliseconds;
                //停机补偿
                nextDelay += Config.IOCard.MachineStopCompensate;
                triggerTimer?.Start();
                LogRead.Log.Info($"启动:{triggerTimer?.Enabled}");
            }
        }
        #endregion
        //double speed => double.Parse(App.Settings.Speed ?? "0");
        double nextDelay = 0;
        int triggerTimes = 0;
        DateTime beginDate;
        System.Timers.Timer triggerTimer;
        void Trigger()
        {
            if (triggerTimer != null) {
                triggerTimer.Stop();
                triggerTimer.Dispose();
                triggerTimer.Elapsed -= TriggerTimer_Elapsed;
                triggerTimer = null;
            }
            triggerTimes = 0;
            beginDate = DateTime.Now;
            double speed = double.Parse(App.Settings.Speed ?? "0");
            //第一次执行的延时
            nextDelay = Config.Tray.FirstInterval / speed;
            triggerTimer = new System.Timers.Timer(10);
            triggerTimer.Elapsed += TriggerTimer_Elapsed;
            if (IsRuning)
                triggerTimer.Start();
        }

        private void TriggerTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (!IsRuning)
            {
                triggerTimer?.Stop();
                return;
            }
            if ((DateTime.Now - beginDate).TotalMilliseconds >= nextDelay)
            {
                Task.Run(() => { OnTrigger?.Invoke(); });
                beginDate = DateTime.Now;
                double speed = double.Parse(App.Settings.Speed ?? "0");
                nextDelay = Config.Tray.OtherInterval / speed;
                triggerTimes++;
            }
            if (triggerTimes >= 5)
            {
                if (triggerTimer == null) return;
                triggerTimer.Stop();
                triggerTimer.Dispose();
                triggerTimer.Elapsed -= TriggerTimer_Elapsed;
                triggerTimer = null;
            }
        }

        public void AddTriggerDelay(double px)
        {
            double speed = double.Parse(App.Settings.Speed ?? "0");
            double delay = (double)Config.Vision.View / Config.Vision.Pixel * px / speed;
            nextDelay += delay;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            Stop();
            USBDASK.UD_Release_Card((ushort)CardNo);
        }
        uint MachineControl = 0;
        uint MachineWaring = 0;
        void SetMachine()
        {
            try
            {
                uint CtrlByte = 0;
                CtrlByte |= MachineControl << Config.IOCard.MachineControlNo;
                CtrlByte |= MachineWaring << Config.IOCard.MachineWaringNo;
                var res = USBDASK.UD_DO_WritePort((ushort)CardNo, 0, CtrlByte);
                if (res != 0)
                {
                    LogError.Log.Error($"写入IO卡 DO 信号失败：Code {res}");
                }
            }
            catch (Exception ex)
            {
                LogError.Log.Error($"写入IO卡 DO 信号失败：{ex.Message}");
            }
        }
        public void MachineStart()
        {
            isSuspend = false;
            MachineControl = 0;
            MachineWaring = 0;
            SetMachine();
            SetMachineChangeDo(false);
        }

        public void MachineStop()
        {
            isSuspend = true;
            MachineControl = 1;
            MachineWaring = 1;
            SetMachine();
            SetMachineChangeDo(true);
            Task.Run(() =>
            {
                Task.Delay(Config.IOCard.MachineWaringTime).Wait();
                MachineWaring = 0;
                SetMachine();
            });
        }
    }
}

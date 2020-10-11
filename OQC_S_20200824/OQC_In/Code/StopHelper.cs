using System;
using System.IO;
using System.Text;
using System.Windows;
using xxw.utilities;

namespace OQC_IN
{
    public class StopHelper : BaseModel
    {
        readonly string StopPath = Directory.GetCurrentDirectory() + "\\stop.json";
        public DateTime? LastDate { get; private set; }
        public StopModel Stop { get; set; }
        public string LastDateStr { get; private set; }
        public string StopTimeStr { get; set; }
        public string StopMsgStr { get; set; }
        public bool IsShowStop { get; set; }
        public Visibility Enable => App.Config.StopDelay == 0 ? Visibility.Collapsed : Visibility.Visible;
        public string StopDelayStr { get; set; }
        public void Init()
        {
            if (!File.Exists(StopPath))
                File.CreateText(StopPath);
            Stop = File.ReadAllText(StopPath, Encoding.Default).ToEntity<StopModel>();
            if (Stop.LastDateTime == null)
                SetStopTimer();
            else
            {
                LastDate = Stop.LastDateTime;
                LastDateStr = LastDate?.ToString("yyyy-MM-dd HH:mm:ss");
                OnPropertyChanged(nameof(LastDate));
            }
            BeginTimer();

            if (Stop.StopType != null)
            {
                StopMsgStr = Stop.Codes[Stop.StopType ?? 0].Text;
                OnPropertyChanged(nameof(StopMsgStr));
            }
            else
            {
                if (Stop.LastDateTime != null && (DateTime.Now - Stop.LastDateTime).Value.TotalSeconds > App.Config.StopDelay)
                {
                    ShowStop();
                }
            }

        }
        private double SetStopTime()
        {
            var seconds = (DateTime.Now - LastDate).Value.TotalSeconds;
            StopTimeStr = formatDateTime((long)seconds);
            var needseconds = App.Config.StopDelay - (int)seconds;
            StopDelayStr = needseconds <= 0 ? "空闲警告中" : formatDateTime2(needseconds);
            OnPropertyChanged(nameof(StopTimeStr));
            OnPropertyChanged(nameof(StopDelayStr));
            return seconds;
        }
        System.Timers.Timer timer;
        private void BeginTimer()
        {
            SetStopTime();
            timer = new System.Timers.Timer(1000);
            timer.Elapsed += (s, e) =>
            {
                if (LastDate == null) return;
                if (SetStopTime() > App.Config.StopDelay && !IsShowStop && Stop.StopType == null)
                {
                    ShowStop();
                }
            };
            timer.Start();
        }
        private object StopObject = new object();
        public void SetStopTimer()
        {
            LastDate = DateTime.Now;
            LastDateStr = LastDate?.ToString("yyyy-MM-dd HH:mm:ss");
            Stop.LastDateTime = LastDate;
            StopMsgStr = "";
            OnPropertyChanged(nameof(StopMsgStr));
            OnPropertyChanged(nameof(LastDate));
            SaveStop();
        }
        public void SaveStop()
        {
            lock (StopObject)
            {
                File.WriteAllText(StopPath, Stop.ToJson(), Encoding.Default);
            }
        }
        /// <summary>
        /// 弹出停机提示
        /// </summary>
        public void ShowStop()
        {
            if (App.Config.StopDelay == 0)
            {
                return;
            }
            App.Current.Dispatcher.Invoke(new Action(() =>
            {
                StopWindow stop = new StopWindow()
                {
                    Owner = App.Current.MainWindow,
                    StopHelper = this
                };
                IsShowStop = true;
                stop.ShowDialog();
                IsShowStop = false;
                StopMsgStr = Stop.Codes[Stop.StopType ?? 0].Text;
                OnPropertyChanged(nameof(StopMsgStr));
            }));
        }

        public string formatDateTime(long mss)
        {
            long days = mss / (60 * 60 * 24);
            long hours = (mss % (60 * 60 * 24)) / (60 * 60);
            long minutes = (mss % (60 * 60)) / 60;
            long seconds = mss % 60;
            if (days > 0)
            {
                return $"{days}:{hours:00}:{minutes:00}:{seconds:00}";
            }
            else if (hours > 0)
            {
                return $"{hours:00}:{minutes:00}:{seconds:00}";
            }
            else if (minutes > 0)
            {
                return $"{minutes:00}:{seconds:00}";
            }
            else
            {
                return $"00:{seconds:00}";
            }
        }
        public string formatDateTime2(long mss)
        {
            long days = mss / (60 * 60 * 24);
            long hours = (mss % (60 * 60 * 24)) / (60 * 60);
            long minutes = (mss % (60 * 60)) / 60;
            long seconds = mss % 60;
            if (days > 0)
            {
                return $"{days}天{hours:00}小时{minutes:00}分{seconds:00}秒";
            }
            else if (hours > 0)
            {
                return $"{hours}小时{minutes:00}分{seconds:00}秒";
            }
            else if (minutes > 0)
            {
                return $"{minutes}分{seconds:00}秒";
            }
            else
            {
                return $"{seconds:00}秒";
            }
        }
    }
}

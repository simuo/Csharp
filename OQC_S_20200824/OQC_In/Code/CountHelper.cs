using System.ComponentModel;
using System.IO;
using System.Text;
using xxw.utilities;

namespace OQC_IN
{
    public class CountHelper : CountModel, INotifyPropertyChanged
    {
        readonly string CountPath = Directory.GetCurrentDirectory() + "\\count.json";
        private object CountObject = new object();
        public CountModel Count { get; set; }
        public CountHelper()
        {
            if (!File.Exists(CountPath))
                File.CreateText(CountPath);
            Count = File.ReadAllText(CountPath).ToEntity<CountModel>();
            OnPropertyChanged(nameof(Count));
        }
        public void AddTotal()
        {
            Count.Total++;
            OnPropertyChanged(nameof(Count));
        }
        public void AddOK()
        {
            Count.OK++;
            OnPropertyChanged(nameof(Count));
        }
        public void AddNG()
        {
            Count.NG++;
            OnPropertyChanged(nameof(Count));
        }
        public void AddTraceTotal()
        {
            Count.TraceTotal++;
            OnPropertyChanged(nameof(TraceTotal));
        }
        public void AddTraceNG()
        {
            Count.TraceNG++;
            OnPropertyChanged(nameof(Count));
        }
        public void AddTraceOk()
        {
            Count.TraceOk++;
            OnPropertyChanged(nameof(Count));
        }
        public void Clear()
        {
            Count.Total = 0;
            Count.OK = 0;
            Count.NG = 0;
            Count.TraceTotal = 0;
            Count.TraceOk = 0;
            Count.TraceNG = 0;
            OnPropertyChanged(nameof(Count));
        }
        public void UpdateToFile()
        {
            lock (CountObject)
            {
                File.WriteAllText(CountPath, Count.ToJson(), Encoding.Default);
            }
        }
    }

    public class CountModel : BaseModel
    {
        public int Total { get; set; }
        public int OK { get; set; }
        public int NG { get; set; }
        public int TraceTotal { get; set; }
        public int TraceOk { get; set; }
        public int TraceNG { get; set; }
    }
}

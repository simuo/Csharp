using xxw.Logs;

namespace OQC_IN
{
    public class LogRead : ILogType
    {
        public string LogName => "Read";
        public static ILogType Log { get; private set; }
        static LogRead()
        {
            if (Log == null)
                Log = new LogRead();
        }
        public LogRead()
        {
            LogHelper.Init(this);
        }
    }
    public class LogData : ILogType
    {
        public string LogName => "Data";
        public static ILogType Log { get; private set; }
        static LogData()
        {
            if (Log == null)
                Log = new LogData();
        }
        public LogData()
        {
            LogHelper.Init(this);
        }
    }
}

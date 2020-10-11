using xxw.Logs;

namespace OQC_OUT
{
#if DEBUG
    public class LogDb : ILogType
    {
        public string LogName => "Db";
        public static ILogType Log { get; private set; }
        static LogDb()
        {
            if (Log == null)
                Log = new LogDb();
        }
        public LogDb()
        {
            LogHelper.Init(this);
        }
    }
#endif
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

    public class LogPOSTJGP : ILogType
    {
        public string LogName => "POST_JGP";
        public static ILogType Log { get; private set; }
        static LogPOSTJGP()
        {
            if (Log == null)
                Log = new LogPOSTJGP();
        }
        public LogPOSTJGP()
        {
            LogHelper.Init(this);
        }
    }

    public class LogPOSTIFactory : ILogType
    {
        public string LogName => "POST_IFactory";
        public static ILogType Log { get; private set; }
        static LogPOSTIFactory()
        {
            if (Log == null)
                Log = new LogPOSTIFactory();
        }
        public LogPOSTIFactory()
        {
            LogHelper.Init(this);
        }
    }

    public class LogPOSTOktoStart : ILogType
    {
        public string LogName => "POST_OktoStart";
        public static ILogType Log { get; private set; }
        static LogPOSTOktoStart()
        {
            if (Log == null)
                Log = new LogPOSTOktoStart();
        }
        public LogPOSTOktoStart()
        {
            LogHelper.Init(this);
        }
    }
}

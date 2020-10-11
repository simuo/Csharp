using System;
using System.Diagnostics;
using xxw.Logs;

namespace OQC_OUT
{
    public static class DbHelper
    {
        public static void Execute(this DbContext db, Action<DbContext> action)
        {
#if DEBUG
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            action.Invoke(db);
            stopwatch.Stop();
            LogDb.Log.Info($"==SQL执行耗时：{stopwatch.Elapsed.TotalMilliseconds}ms==");
#else
            action.Invoke(db);
#endif
        }

        public static T Read<T>(this DbContext db, Func<DbContext, T> action)
        {
#if DEBUG
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            var r = action.Invoke(db);
            stopwatch.Stop();
            LogDb.Log.Info($"==SQL执行耗时：{stopwatch.Elapsed.TotalMilliseconds}ms==");
            return r;
#else
            return action.Invoke(db);
#endif
        }
    }
}

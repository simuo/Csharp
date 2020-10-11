using System;
using System.Threading;
using System.Threading.Tasks;
using xxw.Logs;

namespace OQC_OUT
{
    internal class Task
    {
        public static System.Threading.Tasks.Task Run(Action action)
        {
            var tcs = new TaskCompletionSource<object>();
            new Thread(() =>
            {
                try
                {
                    action();
                    tcs.SetResult(null);
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                    LogError.Log.Error($"Task Error:{ex.Message}\r\n{ex.StackTrace}");
                }
            })
            { IsBackground = true }.Start();
            return tcs.Task;
        }
        public static Task<TResult> Run<TResult>(Func<TResult> function)
        {
            var tcs = new TaskCompletionSource<TResult>();
            new Thread(() =>
            {
                try
                {
                    tcs.SetResult(function());
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                    LogError.Log.Error($"Task Error:{ex.Message}\r\n{ex.StackTrace}");
                }
            })
            { IsBackground = true }.Start();
            return tcs.Task;
        }
        public static System.Threading.Tasks.Task Delay(int milliseconds)
        {
            var tcs = new TaskCompletionSource<object>();
            var timer = new System.Timers.Timer(milliseconds) { AutoReset = false };
            timer.Elapsed += delegate { timer.Dispose(); tcs.SetResult(null); };
            timer.Start();
            return tcs.Task;
        }

    }
}

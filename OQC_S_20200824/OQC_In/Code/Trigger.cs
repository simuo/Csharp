using System;
using System.Collections.Generic;
using System.Threading;
using xxw.Logs;
using xxw.Sockets;

namespace OQC_IN
{
    public class Trigger
    {
        private readonly ConfigModel Config = App.Config;
        private readonly ClientTcp visionClient;
        public event Action<string> OnLog;
        public event Action<int, int, List<string>> OnRead;
        private readonly ManualResetEvent TimeoutObject = new ManualResetEvent(false);
        public Trigger(ClientTcp client)
        {
            visionClient = client;
            visionClient.OnReceiveEvent += (ip, b, s) =>
            {
                CamHelper.ParsingCamData(s, (cam, sid, str, data) =>
                {
                    Task.Run(() =>
                    {
                        TimeoutObject.Set();
                        LogRead.Log.Info($"读码结果：{str}");
                        OnLog?.Invoke(str);
                        OnRead?.Invoke(cam, sid, data);
                    });
                });
            };
        }
        /// <summary>
        /// 发送读码命令
        /// </summary>
        public void TriggerOn(int index)
        {
            string Command = Config.Trigger[index].Command;
            if (string.IsNullOrEmpty(Command)) return;
            visionClient.SendAsync(Command);
            OnLog?.Invoke($"SEND:{Command}");
            LogRead.Log.Info($"发送读码命令：{Command}");
        }
    }
}

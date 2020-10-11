using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using xxw.Logs;
using xxw.Sockets;

namespace OQC_OUT
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
            string Command = string.Join(";", (from oneGroup in Config.Group select oneGroup.Command).ToList()).Replace(";;", ";");
            if (Command.EndsWith(";")) Command = Command.Substring(0, Command.Length - 2);
            Command = Command.Replace("{stageId}", index.ToString());
            if (string.IsNullOrEmpty(Command)) return;
            TimeoutObject.Reset();
            DateTime SendData = DateTime.Now;
            Task.Run(() =>
            {
                for (int i = 0; i < Config.Trigger.CommandSendTimes; i++)
                {
                    visionClient.SendAsync(Command);
                    OnLog?.Invoke($"SEND:{Command}");
                    LogRead.Log.Info($"发送读码命令：{Command}");
                    Task.Delay(Config.Trigger.CommandSendDelay).Wait();
                }
            });
            //超时
            if (!TimeoutObject.WaitOne(Config.Trigger.ReceiveTimeOut, false))
            {
                LogRead.Log.Warn($"接收数据超时{Config.Trigger.ReceiveTimeOut}ms");
            }
        }
    }
}

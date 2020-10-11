using System;
using System.Net;
using xxw.Sockets;
using xxw.utilities;

namespace OQC_IN
{
    public class SocketHelper : BaseModel
    {
        private ConfigModel Config = App.Config;
        public ClientTcp visionClient;
        public event Action<string> OnLog;
        public string StateColor { get; set; }
        public string StateText { get; set; }
        public SocketHelper()
        {
            visionClient = new ClientTcp(IPAddress.Parse(Config.Vision.IP), Config.Vision.Port, 1024, true);
            visionClient.AddRules(new ReceiveDataRule { Prefix = "CAM" });
            visionClient.OnServerStateChangeEvent += (s) =>
            {
                StateColor = s ? "#FF11BB00" : "#FFF4F4F5";
                StateText = s ? "已连接" : "未连接";
                OnPropertyChanged(nameof(StateColor));
                OnPropertyChanged(nameof(StateText));
                OnLog?.Invoke(s ? $"[{Config.Vision.IP}:{Config.Vision.Port}] 已连接" : $"[{Config.Vision.IP}:{Config.Vision.Port}] 连接失败，尝试重新连接...");
            };
            visionClient.Connect();
        }
    }
}

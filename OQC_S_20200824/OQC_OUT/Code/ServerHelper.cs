using System;
using System.Linq;
using xxw.Logs;
using xxw.Sockets;
using xxw.utilities;

namespace OQC_OUT
{
    public class ServerHelper : BaseModel
    {
        readonly ConfigModel Config = App.Config;
        Server server;
        public string LinkState { get; set; }

        public void Init()
        {
            if (!Config.CheckIn) return;
            server = new Server(3001, 1024);
            server.OnClientStateChangeEvent += Server_OnClientStateChangeEvent;
            server.OnClientReceiveEvent += Server_OnClientReceiveEvent;
            server.Start();
        }

        private void Server_OnClientReceiveEvent(string ip, byte[] data_byte, string data_string)
        {
            // 1||>{data}\r\n2||>{data}
            Task.Run(() =>
            {
                try
                {
                    var arr = data_string.Split(new string[] { "\r\n" }, StringSplitOptions.None);
                    var db = new DbContext().InDatasDb;

                    foreach (var one in arr.Where(p => !string.IsNullOrEmpty(p)))
                    {
                        try
                        {
                            var a = one.Split(new string[] { "||>" }, StringSplitOptions.None);
                            var code = a[0];
                            try
                            {
                                var model = a[1].ToEntity<InDatas>();
                                model.Id = Guid.NewGuid().ToString();
                                model.CreateDate = DateTime.Now;
                                if (!db.IsAny(p => p.SN == model.SN))
                                    db.Insert(model);
                                server.Send($"{code}||>success\r\n");
                            }
                            catch
                            {
                                server.Send($"{code}||>error\r\n");
                            }
                        }
                        catch { }
                    }
                }
                catch
                {

                }
            });
        }

        private void Server_OnClientStateChangeEvent(string ip, bool connent)
        {
            App.Current.Dispatcher.Invoke(new Action(() =>
            {
                LinkState = connent ? "#FF11BB00" : "#FFF4F4F5";
                OnPropertyChanged(nameof(LinkState));
            }));
            if (connent)
            {
                LogInfo.Log.Info($"线头已连接：{ip}");
            }
            else
            {
                LogInfo.Log.Info($"线头已断开：{ip}");
            }
        }
    }
}

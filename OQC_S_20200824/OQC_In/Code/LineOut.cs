using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using xxw.Logs;
using xxw.Sockets;
using xxw.utilities;

namespace OQC_IN
{
    public class LineOut : BaseModel
    {
        readonly ConfigModel Config = App.Config;
        Client client;
        public string LinkState => IsConnect ? "#FF11BB00" : "#FFF4F4F5";
        public bool IsConnect { get; private set; }

        public void Init()
        {
            if (!Config.CheckOut) return;
            client = new ClientTcp(IPAddress.Parse(Config.LineOutIp), 3001, true);
            client.OnServerStateChangeEvent += Client_OnServerStateChangeEvent;
            client.Connect();
        }
        public bool Send(int index, InDatas data) {
            try
            {
                if (!IsConnect) return false;
                string s = $"{index}||>{data.ToJson().Replace("\r", "").Replace("\n", "")}\r\n";
                var r = client.Send(s);
                if (!r.Success) return false;
                var a = r.Data.Split(new string[] { "||>" }, StringSplitOptions.None);
                var code = a[0];
                if (code == index.ToString())
                {
                    if (a[1].Replace("\r","").Replace("\n", "") == "success")
                        return true;
                    else
                        return false;
                }
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }

        private void Client_OnServerStateChangeEvent(bool connent)
        {
            IsConnect = connent;
            OnPropertyChanged(nameof(LinkState));
            if (connent)
            {
                LogInfo.Log.Info($"线尾已连接");
            }
            else
            {
                LogInfo.Log.Info($"线尾已断开");
            }
        }
    }

    public class InDatas
    {
        /// <summary>
        /// Band码
        /// </summary>
        public string SN { get; set; }
        /// <summary>
        /// FG码
        /// </summary>
        public string FG { get; set; }
    }
}

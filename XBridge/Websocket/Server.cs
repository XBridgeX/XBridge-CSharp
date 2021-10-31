using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using CQ = HuajiTech.CoolQ.CurrentPluginContext;
using System.IO;
using File = System.IO.File;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Fleck;
using XBridge.Utils;

namespace XBridge.Websocket
{
    class WSSPack { 
        public string id { get; set; }
        public string action { get; set; }
        public string cause { get; set; }
        public WSSItem @params { get; set; }
    }
    class WSSItem { 
        public string text { get; set; }
        public long target { get; set; }
    }
    class WSSSetting {
        public bool enable = false;
        public int port = 9395;
        public string endport = "/xbridge";
    }
    class Server
    {
        private static WebSocketServer ws;
        private static Thread thread;
        public static WSSSetting setting = new WSSSetting();
        private static List<IWebSocketConnection> sockets = new List<IWebSocketConnection>();
        public static void init() {
            if (!File.Exists(CQ.Bot.AppDirectory.FullName + "wss/setting.json"))
            {
                Directory.CreateDirectory(CQ.Bot.AppDirectory.FullName + "wss/");
                File.WriteAllText(CQ.Bot.AppDirectory.FullName + "wss/setting.json", JsonConvert.SerializeObject(new WSSSetting()));
            }
            try { setting = JsonConvert.DeserializeObject<WSSSetting>(File.ReadAllText(CQ.Bot.AppDirectory.FullName + "/wss/setting.json")); }
            catch (Exception e) { Error.LogToFile("WSS_READ_CONFIG", e.ToString()); }
            ws = new WebSocketServer($"ws://0.0.0.0:{setting.port}{setting.endport}");
            if(setting.enable)
                Start();
        }
        private static void Start() {
            if (ws == null) return;
            if (thread == null) {
                thread = new Thread(() =>
                {
                    ws.Start(sock => {
                        sock.OnOpen = ()=>
                        {
                            try
                            {
                                logs.logToFile($"[WSS] OPEN << {sock.ConnectionInfo.ClientIpAddress}:{sock.ConnectionInfo.ClientPort}");
                                sockets.Add(sock);
                                if(sock.ConnectionInfo.Path != setting.endport)
                                    sock.Close();

                            }
                            catch (Exception ex) {
                                Error.LogToFile("WSOPEN", ex.ToString());
                            }
                        };
                        sock.OnMessage = (e) =>
                        {
                            try
                            {
                                WSMessage(sock,e);
                            }
                            catch (Exception ex)
                            {
                                Error.LogToFile("WSMESSAGE", ex.ToString());
                            }
                        };
                        sock.OnClose = () =>
                        {
                            try
                            {
                                logs.logToFile($"[WSS] CLOSE << {sock.ConnectionInfo.ClientIpAddress}:{sock.ConnectionInfo.ClientPort}");
                                sockets.Remove(sock);
                            }
                            catch (Exception ex)
                            {
                                Error.LogToFile("WSOPEN", ex.ToString());
                            }
                        };
                        sock.OnError = (e) =>
                        {
                            sock.Close();
                        };
                    });
                    
                });
                thread.Start();
            }
        }
        public static void send(object o) {
            if (!setting.enable) return;
            foreach (var i in sockets)
            {
                if (i.IsAvailable)
                    i.Send(o.ToString());
            }
        }
        private static string Pack(string cause,string id, string message) {
            var j = new
            {
                type = "pack",
                cause,
                id,
                @params = new {
                    text = message
                }
            };
            return JsonConvert.SerializeObject(j);
        }
        private static void WSMessage(IWebSocketConnection info, string msg) {
            JObject j = JObject.Parse(msg);
            if (j == null) {
                info.Send(Pack("Exception","0","bad json!"));
                return;
            }
            if (j["action"] == null) { info.Send(Pack("Exception","0","cannot find param <action>"));return; }
            var p = JsonConvert.DeserializeObject<WSSPack>(msg);
            switch (j["action"].ToString())
            {
                case "send":
                    CQ.Group(p.@params.target).Send(p.@params.text);
                    break;
            }       
        }
    }
}

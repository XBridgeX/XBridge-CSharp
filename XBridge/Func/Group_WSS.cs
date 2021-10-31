using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XBridge.Websocket;
using CQ = HuajiTech.CoolQ.CurrentPluginContext;
using Newtonsoft.Json;
using Message = XBridge.Utils.Message;
using HuajiTech.CoolQ.Events;
using ws = XBridge.Websocket.Server;
using XBridge.Utils;

namespace XBridge.Func
{
    class Group_WSS
    {
        public static void on_main(object sender, GroupMessageReceivedEventArgs e) {
            if (Setting.setting.Group.main.Contains(e.Source.Number) || Setting.setting.Group.chat.Contains(e.Source.Number)) {
                try
                {
                    logs.logToFile("[WSS] >> PACK_SEND");
                    ws.send(JsonConvert.SerializeObject(Message.GetMsgJ(e)));
                }
                catch(Exception ex)
                {
                    Error.LogToFile("GROUP_WSS", ex.ToString());
                }
            }
        }
    }
}

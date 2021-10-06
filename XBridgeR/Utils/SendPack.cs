using HuajiTech.CoolQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XBridgeR.Data;
using CQ = HuajiTech.CoolQ.CurrentPluginContext;

namespace XBridgeR.Utils
{
    public class SendPack
    {
        public static string Runcmd(string cmd)
        {
            string id = Guid.NewGuid().ToString();
            TMP.WS_Send(PackHelper.GetRunCMDPack(cmd,id));
            TMP.AddRuncmdID(id);
            CQ.Logger.Log($"cmd pack {id} send.");
            return id;
        }
        public static void SendText(string t)
        {
            TMP.WS_Send(PackHelper.GetSendTextPack(t));
            CQ.Logger.Log($"text pack send.");
        }
        public static void SendStop()
        {
            TMP.WS_Send(PackHelper.GetSTOPPack());
        }
    }
}

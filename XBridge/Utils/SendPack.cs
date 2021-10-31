using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using XBridge.WSPack;

namespace XBridge.Utils
{
    class SendPack
    {
        /// <summary>
        /// 执行命令
        /// </summary>
        /// <param name="ser">服务器名称</param>
        /// <param name="cmd">命令</param>
        /// <returns></returns>
        public static bool runcmd(string ser,string cmd,long group = 0)
        {
            if (Main.sockets.ContainsKey(ser))
            {
                string tmpid = Guid.NewGuid().ToString();
                Main.runcmdid[ser].Add(tmpid,group);
                var pack = new RunCMD()
                {
                    @params = new @params
                    {
                        cmd = cmd,
                        id = tmpid
                    }
                };
                logs.logToFile($"({tmpid})[RUNCMD] >> [{ser}]");
                Main.sockets[ser].Send(Encrypt.Encrypted(JsonConvert.SerializeObject(pack), Main.sockets[ser].getK, Main.sockets[ser].getiv));
                return true;
            }
            return false;
        }
        /// <summary>
        /// 所有服务器执行命令
        /// </summary>
        /// <param name="cmd"></param>
        public static void runcmdAll(long group,string cmd)
        {
            foreach(var i in Main.sockets)
            {
                runcmd(i.Key, cmd,group);
            }
        }
        public static void sendText(string ser, string text)
        {
            var pack = new SendText()
            {
                @params = new @params
                {
                    text = text,
                    id = Guid.NewGuid().ToString()
                }
            };
            logs.logToFile($"({pack.@params.id})[SENDTEXT] >> [{ser}]");
            Main.sockets[ser].Send(Encrypt.Encrypted(JsonConvert.SerializeObject(pack), Main.sockets[ser].getK, Main.sockets[ser].getiv));
        }
        public static void bcText(string ser, string text) {
            foreach (var i in Main.sockets)
            {
                if(i.Key != ser)
                    sendText(i.Key, text);
            }
        }
        public static void bcText(string text)
        {
            foreach(var i in Main.sockets)
            {
                sendText(i.Key, text);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using XBridgeR.Data;

namespace XBridgeR.Utils
{
    public class Pos
    {
        /// <summary>
        /// 
        /// </summary>
        public double x { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public double y { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public double z { get; set; }
    }

    public class @params
    {
        /// <summary>
        /// 
        /// </summary>
        public string cmd { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string sender { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string text { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string result { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string msg { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string xuid { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ip { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string mobtype { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string mobname { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int dmcase { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string dmname { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string srctype { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string srcname { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Pos pos { get; set; }
    }
    public class RunCMD
    {
        /// <summary>
        /// 
        /// </summary>
        public string type = "pack";
        /// <summary>
        /// 
        /// </summary>
        public string action = "runcmdrequest";
        /// <summary>
        /// 
        /// </summary>
        public @params @params { get; set; }
    }

    class SendText
    {
        /// <summary>
        /// 
        /// </summary>
        public string type = "pack";
        /// <summary>
        /// 
        /// </summary>
        public string action = "sendtext";
        /// <summary>
        /// 
        /// </summary>
        public @params @params { get; set; }
    }
    class PackHelper
    {
        public static string GetSendTextPack(string t)
        {
            var p = new SendText
            {
                @params = new @params
                {
                    id = Guid.NewGuid().ToString(),
                    text = t
                }
            };
            return Encrypt.Encrypted(JsonConvert.SerializeObject(p), TMP.GetWSKey(TMP.WebsocketKey.k), TMP.GetWSKey(TMP.WebsocketKey.iv));
        }
        public static string GetRunCMDPack(string t,string id)
        {
            var p = new RunCMD
            {
                @params = new @params
                {
                    id = id,
                    cmd = t
                }
            };
            return Encrypt.Encrypted(JsonConvert.SerializeObject(p), TMP.GetWSKey(TMP.WebsocketKey.k), TMP.GetWSKey(TMP.WebsocketKey.iv));
        }
        public static string GetSTOPPack()
        {
            var p = new
            {
                type = "pack",
                action = "stop",
                @params = new @params
                {
                }
            };
            return Encrypt.Encrypted(JsonConvert.SerializeObject(p), TMP.GetWSKey(TMP.WebsocketKey.k), TMP.GetWSKey(TMP.WebsocketKey.iv));
        }
    }
}

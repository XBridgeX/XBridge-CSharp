using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XBridgeR.Utils;
using File = System.IO.File;
using CQ = HuajiTech.CoolQ.CurrentPluginContext;
using Newtonsoft.Json;
using HuajiTech.CoolQ;
using System.Threading;
using XBridgeR.Func;

namespace XBridgeR.Data
{
    /// <summary>
    /// 运行时文件
    /// </summary>
    class TMP
    {
        public static CFG cfg;
        public static Websocket websocket;
        public static Dictionary<string, DateTime> runcmdid = new Dictionary<string, DateTime>();
        public static GroupSet GroupSet;
        public static  bool AutoRNAME
        {
            get { return cfg.AutoRename; }
        }
        public static bool Encrypt
        {
            get { return cfg.Server.encrypt; }
        }
        public static bool CMDIDContains(string id)
        {
            return runcmdid.ContainsKey(id);
        }
        public static GroupSet GetGroupSet()
        {
            return GroupSet;
        }
        public static string ChatIndex
        {
            get { return cfg.ChatIndex; }
        }
        public static int ChatSub
        {
            get { return cfg.ChatSub; }
        }
        /// <summary>
        /// 获取连接的服务器
        /// </summary>
        /// <returns>配置文件中的服务器</returns>
        public static Server GetServer()
        {
            return cfg.Server;
        }
        public static bool AutoWL
        {
            get { return cfg.AutoWhitelist; }
        }
        /// <summary>
        /// 查询玩家是否为管理员
        /// </summary>
        /// <param name="q">查询的QQ号</param>
        /// <returns>是否为管理员</returns>
        public static bool is_Admin(long q)
        {
            return cfg.Admin.Contains(q);
        }
        public enum WebsocketKey
        {
            k,
            iv
        }
        public static string GetWSKey(WebsocketKey key)
        {
            switch (key)
            {
                case WebsocketKey.k:
                    return websocket.getK;
                case WebsocketKey.iv:
                    return websocket.getiv;
                default:
                    return string.Empty;
            }
        }
        public enum GroupType
        {
            chat,
            main
        }
        public static long GetGroup(GroupType group)
        {
            switch (group)
            {
                case GroupType.chat:
                    return cfg.Group.chat;
                case GroupType.main:
                    return cfg.Group.main;
                default:
                    return 0;
            }
        }
        public static void WS_Send(string t)
        {
            websocket.Send(t);
        }
        public static void WS_Start()
        {
            websocket.Start();
        }
        public static void AddRuncmdID(string id)
        {
            runcmdid.Add(id, DateTime.Now);
        }
        public static void init()
        {
            if (CQ.Bot.AppDirectory.Exists == false)
                CQ.Bot.AppDirectory.Create();
            string AppDirectory = CQ.Bot.AppDirectory.FullName;
            if (!File.Exists(AppDirectory + "GroupData.json"))
            {
                File.WriteAllText(AppDirectory + "GroupData.json", JsonConvert.SerializeObject(new GroupSet()));
            }
            GroupSet = JsonConvert.DeserializeObject<GroupSet>(File.ReadAllText(AppDirectory + "GroupData.json"));
            if (!File.Exists(AppDirectory + "setting.json"))
            {
                var c = new CFG()
                {
                    Server = new Server
                    {
                        name = "生存服务器",
                        url = "ws://127.0.0.1:8080/mc",
                        password = "password",
                        re_connect_time = 5,
                        encrypt = true
                    },
                    Group = new Group
                    {
                        chat = 114514,
                        main = 114514
                    },
                    Admin = new List<long> { 114514 },
                    AutoWhitelist = false,
                    ChatSub = 20,
                    ChatIndex = "chat "
                };
                File.WriteAllText(AppDirectory + "setting.json", JsonConvert.SerializeObject(c, Formatting.Indented));
            }
            cfg = JsonConvert.DeserializeObject<CFG>(File.ReadAllText(AppDirectory + "setting.json"));           
            if(File.Exists(AppDirectory + "regex.json"))
            {
                CQ.Logger.Log("开始加载正则表达式文件");
                onRegexs.regexs = JsonConvert.DeserializeObject<List<onRegexs.RegexItem>>(File.ReadAllText(AppDirectory + "regex.json"));
                CQ.Logger.Log($"加载了{onRegexs.regexs.Count}条正则表达式");
            }
            if(File.Exists(AppDirectory + ".lang") == false)
                File.WriteAllBytes(AppDirectory + ".lang", Properties.Resources.lang);
            string[] l = File.ReadAllLines(AppDirectory + ".lang");
            foreach (string i in l)
            {
                if (i.StartsWith("#"))
                    continue;
                try
                {
                    var m = i.Split('=');
                    Lang.set(m[0], m[1]);
                }
                catch { }
            }
            if (!File.Exists(AppDirectory + "mobs.json"))
                File.WriteAllBytes(AppDirectory + "mobs.json", Properties.Resources.mobs);
            if (!File.Exists(AppDirectory + "die_id.json"))
                File.WriteAllBytes(AppDirectory + "die_id.json", Properties.Resources.die_id);
            DieMessage.mobs = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(AppDirectory + "mobs.json"));
            DieMessage.die_id = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(AppDirectory + "die_id.json"));
            if (!File.Exists(AppDirectory + "Playerdata.json"))
            {
                File.WriteAllText(AppDirectory + "Playerdata.json", "{}");
            }
            XBOXID.db = JsonConvert.DeserializeObject<Dictionary<long, PlayerData>>(File.ReadAllText(AppDirectory + "Playerdata.json"));
            websocket = new Websocket(cfg.Server.url, cfg.Server.name, cfg.Server.password);
            websocket.AddFunction("onMessage", WSReceive.on_ws_pack);
            websocket.Start();
            new Thread(() =>
            {
                while (true)
                {
                    Thread.Sleep(1000 * cfg.Server.re_connect_time);
                    if (websocket.IfAlive == false)
                        websocket.Start();
                }
            }).Start();
        }
    }
}

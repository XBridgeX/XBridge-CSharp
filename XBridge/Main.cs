using System.Collections.Generic;
using HuajiTech.CoolQ;
using HuajiTech.CoolQ.Events;
using XBridge.Config;
using XBridge.Func;
using MoonSharp.Interpreter;
using System.Threading;
using XBridge.Utils;
using XBridge.Websocket;
using CQ = HuajiTech.CoolQ.CurrentPluginContext;
using System.Management;
using System.Linq;

namespace XBridge
{
    /// <summary>
    /// 包含应用的逻辑。
    /// </summary>
    internal class Main : Plugin
    {
        #region 配置文件和临时文件
        public static string version = "1.2.3.8";
        public static Dictionary<long, TMP> tmp;
        /// <summary>
        /// 玩家数据
        /// </summary>
        public static XDB playerdatas = new XDB(CQ.Bot.AppDirectory.FullName + "/playerData.xdb", "20040614");
        /// <summary>
        /// 执行命令临时id
        /// </summary>
        public static Dictionary<string, Dictionary<string,long>> runcmdid = new Dictionary<string, Dictionary<string, long>>();
        /// <summary>
        /// 连接池
        /// </summary>
        public static Dictionary<string, Websocket.Websocket> sockets = new Dictionary<string, Websocket.Websocket>();
        #endregion
        /// <summary>
        /// 使用指定的事件源初始化一个 <see cref="Main"/> 类的新实例。
        /// </summary>
        public Main(INotifyGroupMessageReceived notifyGroupMessageReceived, IGroupEventSource groupEventSource)
        {
            INIT.init();
            Vcheck.init();
            foreach (var i in Setting.setting.Servers) {
                var ws = new Websocket.Websocket(i.Url, i.name, i.password);
                ws.AddFunction("onMessage", WSReceive.on_ws_pack);
                ws.Start();
                sockets.Add(i.name, ws);
                runcmdid.Add(i.name, new Dictionary<string, long>());
            }
            new Thread(() =>
            {
                while (true) {
                    Thread.Sleep(5000);
                    try
                    {
                        foreach (var i in sockets)
                        {
                            if (!i.Value.IfAlive())
                                i.Value.Start();
                        }
                    }
                    catch { }
                }
            }).Start();
            notifyGroupMessageReceived.GroupMessageReceived += Group_CMD.xbridge_cmd;
            groupEventSource.MemberLeft += MemberLeft.Member_Left;
            if (Setting.setting.enable.xb_lua)
            {
                LUAAPI.init();
                LUAAPI.LoadFile();
                notifyGroupMessageReceived.GroupMessageReceived += GroupLua.on_message;
            }
            if (Setting.setting.enable.xb_regex)
            {
                notifyGroupMessageReceived.GroupMessageReceived += Regexs.on_regex;
            }
            if (Setting.setting.enable.xb_native)
            {
                notifyGroupMessageReceived.GroupMessageReceived += GroupMain.on_main;
                notifyGroupMessageReceived.GroupMessageReceived += GroupChat.on_chat;
                Logger.Log("成功注册xb_native事件处理");
            }
            Server.init();
            if (Server.setting.enable)
            {
                notifyGroupMessageReceived.GroupMessageReceived += Group_WSS.on_main;
                logs.logToFile("Websocket服务器已开启");
            }
            Logger.LogSuccess("加载完成");
            Logger.LogSuccess("作者：Lition");
            Logger.LogSuccess("bug反馈请加QQ群：808776416");
        }


    }
}
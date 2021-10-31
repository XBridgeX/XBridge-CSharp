using HuajiTech.CoolQ;
using HuajiTech;
using System;
using HuajiTech.CoolQ.Messaging;
using XBridge.Func;
using MoonSharp.Interpreter;
using System.Collections.Generic;
using System.IO;

namespace XBridge.Utils
{
    public class LUAAPI
    {
        /// <summary>
        /// lua虚拟机
        /// </summary>
        public static Script lua = new Script();
        /// <summary>
        /// lua函数
        /// </summary>
        public static Dictionary<string, List<Closure>> func = new Dictionary<string, List<Closure>>()
        {
            {"msg",new List<Closure>() },
            {"ws",new List<Closure>() }
        };
        delegate DynValue RUNCMD(long g,object ser, object cmd);
        delegate DynValue SEND(DynValue mode, object text);
        delegate void LISTEN(object o,Closure f);
        delegate DynValue GETCONFIG(object o);
        delegate string XBCMD(object o);
        delegate DynValue WHITELIST(int m, DynValue d,DynValue b);
        /// <summary>
        /// 白名单功能
        /// </summary>
        static WHITELIST cs_whitelist = (m, p,q) =>
        {
            switch (m)
            {
                case 0:
                     return DynValue.NewBoolean(Data.wl_add((long)p.Number, q.String));
                case 1:
                    return DynValue.NewBoolean(Data.wl_remove((long)p.Number));
                case 2:
                    return DynValue.NewString(Data.get_xboxid((long)p.Number));
                default:
                    return DynValue.Nil;
            }
        };
        /// <summary>
        /// 执行命令
        /// </summary>
        static RUNCMD cs_runcmd = (g,ser, cmd) =>
        {
            if(ser.ToString() == "all")
            {
                SendPack.runcmdAll(g,cmd.ToString());
                return DynValue.True;
            }
            if (Data.is_server(ser.ToString()))
            {
                SendPack.runcmd(ser.ToString(), cmd.ToString(),g);
                return DynValue.True;
            }
            return DynValue.False;
        };
        /// <summary>
        /// 执行xb命令
        /// </summary>
        static XBCMD cs_xbcmd = (t) =>
        {
            return  XB_CMD.runcode(t.ToString());
        };
        /// <summary>
        /// 向群聊发送信息
        /// </summary>
        static SEND cs_send = (m, t) =>
        {
            switch (m.Number)
            {
                case 0:
                    foreach (long id in Setting.setting.Group.main)
                        CurrentPluginContext.Group(id).Send(t.ToString());
                    return DynValue.True;
                case 1:
                    foreach (long id in Setting.setting.Group.chat)
                        CurrentPluginContext.Group(id).Send(t.ToString());
                    return DynValue.True;
                default:
                    return DynValue.False;
            }
        };
        /// <summary>
        /// 获取配置文件
        /// </summary>
        static GETCONFIG cs_getcfg = (o) =>
        {
            switch (o.ToString())
            {
                case "admins":
                    var at = new Table(lua);
                    foreach (long q in Setting.setting.Admins)
                        at.Append(DynValue.NewNumber(q));
                    return DynValue.NewTable(at);
                case "servers":
                    var st = new Table(lua);
                    foreach (var i in Main.sockets)
                        st.Append(DynValue.NewString(i.Key));
                    return DynValue.NewTable(st);
                default:
                    return DynValue.Nil;
            }
        };
        /// <summary>
        /// 设置监听事件
        /// </summary>
        static LISTEN cs_listen = (k, f) =>
        {
            switch (k.ToString())
            {
                case "ws":
                    func["ws"].Add(f);
                    break;
                case "msg":
                    func["msg"].Add(f);
                    break;
            }
        };
        /// <summary>
        /// 初始化LUAAPI
        /// </summary>
        public static void init()
        {
            lua.Globals["send"] = cs_send;
            lua.Globals["runcmd"] = cs_runcmd;
            lua.Globals["getcfg"] = cs_getcfg;
            lua.Globals["lisen"] = cs_listen;
            lua.Globals["xbcmd"] = cs_xbcmd;
            lua.Globals["whitelist"] = cs_whitelist;
        }
        /// <summary>
        /// 读取脚本
        /// </summary>
        public static void LoadFile()
        {
            string LUAPATH = CurrentPluginContext.Bot.AppDirectory.FullName + "/scripts";
            if (!Directory.Exists(LUAPATH))
                Directory.CreateDirectory(LUAPATH);
            foreach (var i in new DirectoryInfo(LUAPATH).GetFiles("*.lua"))
            {
                try { lua.DoFile(i.FullName); }
                catch (Exception ex)
                {
                    CurrentPluginContext.Logger.LogError(ex.ToString());
                }
            }
        }
        /// <summary>
        /// 清除函数
        /// </summary>
        public static void Clear()
        {
            func["msg"].Clear();
            func["ws"].Clear();
        }
    }
}

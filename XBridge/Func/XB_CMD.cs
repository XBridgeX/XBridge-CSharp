using HuajiTech.CoolQ;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using File = System.IO.File;
using XBridge.Config;
using XBridge.Utils;

namespace XBridge.Func
{
    class XB_CMD
    {
        delegate string XBCMD(string[] p);
        public static Dictionary<string, Func<string[], string>> cmd_func = new Dictionary<string, Func<string[], string>>();
        public static string runcode(string cmd) {
            var c = cmd.Split(' ');
            try
            {
                #region xb_console解释器
                switch (c[0])
                {
                    case "wl":
                        switch (c[1])
                        {
                            case "add":
                                if (Data.wl_exsis(long.Parse(c[2])))
                                    return $"qq {c[2]} already in whitelist.";
                                Data.wl_add(long.Parse(c[2]), cmd.Substring($"wl add {c[2]} ".Length));
                                return "key add to whitelist";
                            case "remove":
                                if (Data.xboxid_exsis(c[2]))
                                {
                                    Data.wl_exsis(Data.id2qq(c[2]));
                                    return $"xboxid {c[2]} remove from whitelist.";
                                }
                                if(long.TryParse(c[2],out var foo))
                                {
                                    if (Data.wl_exsis(long.Parse(c[2])))
                                    {
                                        Data.wl_remove(long.Parse(c[2]));
                                        return $"qq {c[2]} remove from whitelist.";
                                    }
                                }
                                return $"{c[2]} not in whitelist.";
                            case "list":
                                string b = "[whitelist]";
                                foreach (var i in Main.playerdatas.getAll())
                                {
                                    b += $"\n{i.Key}:{i.Value.xboxid}";
                                }
                                return b;
                            case "get":
                                if (long.TryParse(c[2], out var fooo))
                                {
                                    if (Data.wl_exsis(long.Parse(c[2])))
                                    {
                                        return "find " + Data.get_xboxid(long.Parse(c[2]));
                                    }
                                }
                                if(Data.xboxid_exsis(cmd.Substring($"wl get ".Length)))
                                {
                                    return "find " + Data.id2qq(cmd.Substring($"wl get ".Length)).ToString();
                                }
                                return "not find";
                            default:
                                return $"command wl donot have overload >>{c[1]}<<";
                        }
                    case "lua":
                        switch (c[1])
                        {
                            case "run":
                                try
                                {
                                    return LUAAPI.lua.DoString(cmd.Substring(8)).ToDebugPrintString();
                                }
                                catch (Exception e) { return e.Message; }
                            case "call":
                                try
                                {
                                    return LUAAPI.lua.Globals.Get(c[2]).Function.Call().ToDebugPrintString();
                                }
                                catch (Exception ex) { return ex.Message; }
                            default:
                                return $"command lua donot have overload >>{c[1]}<<";
                        }
                    case "ws":
                        switch (c[1])
                        {
                            case "close":
                                if (Data.is_server(c[2]))
                                {
                                    Main.sockets[c[2]].Close();
                                    return $"socket {c[2]} close";
                                }
                                else { return $"not a socket named {c[2]}"; }
                            case "send":
                                if (Data.is_server(c[2]))
                                {
                                    Main.sockets[c[2]].Send(cmd.Substring(8));
                                    return $"pack send to {c[2]}";
                                }
                                else { return $"not a socket named {c[2]}"; }
                            default:
                                return $"command ws donot have overload >>{c[1]}<<";
                        }
                    case "admin":
                        switch (c[1])
                        {
                            case "add":
                                try { long q = long.Parse(c[2]); }
                                catch { return $"unable to convert {c[2]} to number"; }
                                if (Data.is_admin(long.Parse(c[2])))
                                    return $"member {c[2]} already in admins.";
                                else
                                    Setting.setting.Admins.Add(long.Parse(c[2]));
                                return $"member {c[2]} add to admins.";
                            case "remove":
                                try { long q = long.Parse(c[2]); }
                                catch { return $"unable to convert {c[2]} to number."; }
                                if (!Data.is_admin(long.Parse(c[2])))
                                    return $"member not in admins.";
                                else
                                    Setting.setting.Admins.Remove(long.Parse(c[2]));
                                return $"member {c[2]} remove from admins.";
                            default:
                                return $"command admin donot have overload >>{c[1]}<<";
                        }
                    case "setconfig":
                        try {long.Parse(c[1]); }
                        catch { return $"unable to convert {c[1]} to number"; }
                        if(!Setting.setting.Group.chat.Contains(long.Parse(c[1])) && !Setting.setting.Group.main.Contains(long.Parse(c[1])))
                        { return $"unable to find {c[1]} in groups"; }
                        //c[0] setconfig
                        //c[1] 1145141919
                        //c[2] chatsub
                        //c[3] 20
                        switch (c[2])
                        {
                            case "chat":
                                Main.tmp[long.Parse(c[1])].chatindex = c[3].Replace("{空格}"," ");
                                TMP.SAVE();
                                return $"{c[1]} chatindex change to" + c[3].Replace("{空格}", " ");
                            case "autowl":
                                Main.tmp[long.Parse(c[1])].autowl = bool.Parse(c[3]);
                                TMP.SAVE();
                                return $"{c[1]} auto whitelist change to " + c[3];
                            case "chatenable":
                                Main.tmp[long.Parse(c[1])].chatenable = bool.Parse(c[3]);
                                TMP.SAVE();
                                return $"{c[1]} server chat message enable change to " + c[3];
                            case "joinenable":
                                Main.tmp[long.Parse(c[1])].joinenable = bool.Parse(c[3]);
                                return $"{c[1]} server join message enable change to " + c[3];
                            case "leftenable":
                                Main.tmp[long.Parse(c[1])].leftenable = bool.Parse(c[3]);
                                TMP.SAVE();
                                return $"{c[1]} server left message enable change to " + c[3];
                            case "mobdieenable":
                                Main.tmp[long.Parse(c[1])].mobdieenable = bool.Parse(c[3]);
                                TMP.SAVE();
                                return $"{c[1]} playerdie message enable change to " + c[3];
                            case "chatsub":
                                Main.tmp[long.Parse(c[1])].chatsub = int.Parse(c[3]);
                                TMP.SAVE();
                                return $"{c[1]} Chat length intercept value change to " + c[3];
                            case "addserver":
                                if (!Data.is_server(c[3]))
                                    return $"no server names {c[3]}";
                                if (Main.tmp[long.Parse(c[1])].allowserver.Contains(c[3]))
                                    return $"server {c[3]} already in group({c[1]})'s allow list";
                                else
                                {
                                    Main.tmp[long.Parse(c[1])].allowserver.Add(c[3]);
                                    TMP.SAVE();
                                    return $"server {c[3]} add to group({c[1]})'s allow list";
                                }
                            case "removeserver":
                                if (!Data.is_server(c[3]))
                                    return $"no server names {c[3]}";
                                if (!Main.tmp[long.Parse(c[1])].allowserver.Contains(c[3]))
                                    return $"server {c[3]} not in group({c[1]})'s allow list";
                                else
                                {
                                    Main.tmp[long.Parse(c[1])].allowserver.Remove(c[3]);
                                    TMP.SAVE();
                                    return $"server {c[3]} remove form group({c[1]})'s allow list";
                                }
                            default:
                                return $"command setconfig donot have overload >>{c[2]}<<";
                        }
                    case "reload":                       
                        try
                        {
                            INIT.init();
                            return "reload success";
                        }
                        catch (Exception x) { return $"error when reload:{x}"; }
                    default:
                        return $"unknow command {c[0]}";
                }
                #endregion
            }
            catch(Exception e)
            { 
                Error.LogToFile("XBCMD",e.ToString()); return e.ToString();
            }
        }
        public static string runcodeEx(string cmd)
        {
            var c = cmd.Split(' ');
            if (cmd_func.ContainsKey(c[0]))
            {
                return cmd_func[c[0]](RemoveArray(c,c[0]));
            }
            return null;
        }
        public static void init()
        {
            cmd_func.Add(nameof(setconfig), setconfig);
            cmd_func.Add(nameof(reload), reload);
        }
        /// <summary>
        /// 移除数组中的某一位数
        /// </summary>
        /// <param name="array">需要移除的数组</param>
        /// <param name="stringIndex">移除某一个值</param>
        /// <returns></returns>
        static string[] RemoveArray(string[] array, string stringIndex)
        {
            List<string> arrayList = new List<string>();
            for (int i = 0; i < array.Length; i++)
            {
                arrayList.Add(array[i].ToString());//将数组的值加入到一个list集合
            }
            arrayList.Remove(stringIndex);//移除数组的某个值

            return arrayList.ToArray();//将list集合转换为数组返回
        }
        public static string setconfig(string[] c)
        {
            try { long.Parse(c[0]); }
            catch { return $"unable to convert {c[0]} to number"; }
            if (!Setting.setting.Group.chat.Contains(long.Parse(c[1])) && !Setting.setting.Group.main.Contains(long.Parse(c[1])))
            { return $"unable to find {c[0]} in groups"; }
            //c[0] setconfig
            //c[1] 1145141919
            //c[2] chatsub
            //c[3] 20
            switch (c[2])
            {
                case "chat":
                    Main.tmp[long.Parse(c[1])].chatindex = c[2].Replace("{空格}", " ");
                    TMP.SAVE();
                    return $"{c[0]} chatindex change to" + c[2].Replace("{空格}", " ");
                case "autowl":
                    Main.tmp[long.Parse(c[1])].autowl = bool.Parse(c[2]);
                    TMP.SAVE();
                    return $"{c[0]} auto whitelist change to " + c[2];
                case "chatenable":
                    Main.tmp[long.Parse(c[1])].chatenable = bool.Parse(c[2]);
                    TMP.SAVE();
                    return $"{c[0]} server chat message enable change to " + c[2];
                case "joinenable":
                    Main.tmp[long.Parse(c[1])].joinenable = bool.Parse(c[2]);
                    return $"{c[0]} server join message enable change to " + c[2];
                case "leftenable":
                    Main.tmp[long.Parse(c[0])].leftenable = bool.Parse(c[2]);
                    TMP.SAVE();
                    return $"{c[0]} server left message enable change to " + c[2];
                case "mobdieenable":
                    Main.tmp[long.Parse(c[0])].mobdieenable = bool.Parse(c[2]);
                    TMP.SAVE();
                    return $"{c[0]} playerdie message enable change to " + c[2];
                case "chatsub":
                    Main.tmp[long.Parse(c[0])].chatsub = int.Parse(c[2]);
                    TMP.SAVE();
                    return $"{c[0]} Chat length intercept value change to " + c[2];
                case "addserver":
                    if (!Data.is_server(c[2]))
                        return $"no server names {c[2]}";
                    if (Main.tmp[long.Parse(c[0])].allowserver.Contains(c[2]))
                        return $"server {c[2]} already in group({c[0]})'s allow list";
                    else
                    {
                        Main.tmp[long.Parse(c[0])].allowserver.Add(c[2]);
                        TMP.SAVE();
                        return $"server {c[2]} add to group({c[0]})'s allow list";
                    }
                case "removeserver":
                    if (!Data.is_server(c[2]))
                        return $"no server names {c[2]}";
                    if (!Main.tmp[long.Parse(c[0])].allowserver.Contains(c[2]))
                        return $"server {c[2]} not in group({c[0]})'s allow list";
                    else
                    {
                        Main.tmp[long.Parse(c[0])].allowserver.Remove(c[2]);
                        TMP.SAVE();
                        return $"server {c[2]} remove form group({c[0]})'s allow list";
                    }
                default:
                    return $"command setconfig donot have overload >>{c[1]}<<";
            }
        }
        public static string reload(string[] c)
        {
            try
            {
                INIT.init();
                return "reload success";
            }
            catch (Exception x) { return $"error when reload:{x}"; }
        }
    }
    class XB_CMD_RET
    {
        public bool success { get; set; }
        public string msg { get; set; }
    }
}

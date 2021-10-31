using System;
using System.Collections.Generic;
using XBridge.WSPack;
using XBridge.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using XBridge.Config;
using MoonSharp.Interpreter;
using static XBridge.Main;
using CQ = HuajiTech.CoolQ.CurrentPluginContext;

namespace XBridge.Func
{
    class WSReceive
    {
        private static Dictionary<string, DateTime> ptime = new Dictionary<string, DateTime>();
        public static void on_ws_pack(string ser, string pack) {
            var jp = JObject.Parse(pack);
            if (jp["type"] != null) {
                switch (jp["type"].ToString()) {
                    case "pack":
                        try
                        {
                            on_pack(ser, pack);
                            if (Setting.setting.enable.xb_lua)
                                lua_on_pack(ser, pack);
                        }
                        catch (Exception e) { 
                            Error.LogToFile("WSRECE",pack);
                            send(ser,"error",0, Lang.get("WSPACK_RECEIVE_ERROR", ser, e.Message)); 
                        }
                        break;
                    case "encrypted":
                        try {
                            if (jp["params"]["raw"] == null) return;
                            string unecrypt = AES.AesDecrypt(jp["params"]["raw"].ToString(), sockets[ser].getK, sockets[ser].getiv);
                            on_pack(ser, unecrypt);
                            if (Setting.setting.enable.xb_lua)
                                lua_on_pack(ser, unecrypt);
                        }
                        catch (Exception e) { 
                            Error.LogToFile("WSRECE", pack);
                            send(ser,"error",0, Lang.get("WSPACK_RECEIVE_ERROR", ser, e.ToString()));
                        }
                        break;
                }
            }
        }
        private static void send(string ser,string type,int mode, object o) {

            switch (mode) {
                case 0:
                    foreach (long id in Setting.setting.Group.main)
                    {
                        if (!tmp[id].allowserver.Contains(ser))
                            return;
                        switch (type)
                        {
                            case "error":
                            case "decodefailed":
                                if (tmp[id].errorenable)
                                    CQ.Group(id).Send(o.ToString());
                                break;
                        }
                    }
                        
                    break;
                case 1:
                    foreach (long id in Setting.setting.Group.main)
                    {
                        if (!tmp[id].allowserver.Contains(ser))
                            return;
                        switch (type)
                        {
                            case "chat":
                                if (tmp[id].chatenable)
                                    CQ.Group(id).Send(o.ToString());
                                break;
                            case "mobdie":
                                if (tmp[id].mobdieenable)
                                    CQ.Group(id).Send(o.ToString());
                                break;
                            case "join":
                                if (tmp[id].joinenable)
                                    CQ.Group(id).Send(o.ToString());
                                break;
                            case "left":
                                if (tmp[id].leftenable)
                                    CQ.Group(id).Send(o.ToString());
                                break;
                        }
                    }
                    break;
                    
            }
        }
        private static void setTime(string p,DateTime time)
        {
            if (!ptime.ContainsKey(p))
            {
                ptime.Add(p, time);
            }
            else
            {        
                ptime[p] = time;
            }
        }
        private static int getTime(string p,DateTime time)
        {
            if (ptime.ContainsKey(p))
                return ((int)(time - ptime[p]).TotalMinutes);
            return 0;
        }
        private static void on_pack(string ser,string p) {
            var type = JObject.Parse(p);
            var param = JsonConvert.DeserializeObject<@params>(type["params"].ToString());
            if (type["cause"] == null)
                return;
            logs.logToFile($"{ser} << [{type["cause"]}]");
            switch (type["cause"].ToString())
            {
                case "join":
                    send2Other(ser, Lang.get("SERVER_MEMBER_JOIN", ser, param.sender));
                    setTime(param.sender, DateTime.Now);
                    playerdatas.add_time(0, param.sender, 1);
                    send(ser,type["cause"].ToString(),1, Lang.get("MEMBER_JOIN", ser, param.sender,Data.CheckJoinTime(param.sender).ToString()));                                       
                    break;
                case "left":
                    send2Other(ser, Lang.get("SERVER_MEMBER_LEFT", ser, param.sender));
                    playerdatas.add_time(2, param.sender, getTime(param.sender, DateTime.Now));
                    send(ser,type["cause"].ToString(),1, Lang.get("MEMBER_LEFT", ser, param.sender));               
                    break;
                case "chat":
                    send2Other(ser, Lang.get("SERVER_MEMBER_CHAT", ser, param.sender, param.text));
                    send(ser,type["cause"].ToString(),1, Lang.get("MEMBER_CHAT", ser, param.sender,param.text));    
                    break;
                case "runcmdfeedback":
                    if (runcmdid[ser].ContainsKey(param.id)) {
                        if(Main.runcmdid[ser][param.id]!=0)
                            CQ.Group(Main.runcmdid[ser][param.id]).Send(Lang.get("CMD_FEEDBACK", ser, param.result));
                    }
                    break;
                case "mobdie":
                    if (param.mobtype != "" && param.mobname != "")
                    {
                        string mob = "entity." + param.srctype.ToLower() + ".name";
                        if (DieMessage.mobs.ContainsKey(mob) && param.srctype.ToLower() != "unknown")
                        {
                            send(ser,type["cause"].ToString(),1, Lang.get("MEMBER_KILL_BY_MOBS", ser, param.mobname,DieMessage.mobs[mob]));
                        }
                        else if (DieMessage.die_id.ContainsKey(param.dmcase.ToString()))
                        {
                            send(ser,type["cause"].ToString(),1, Lang.get("MEMBER_KILL_BY_SELF", ser, param.mobname, DieMessage.die_id[param.dmcase.ToString()]));
                        }
                        else
                        {
                            send(ser,type["cause"].ToString(),1, Lang.get("MEMBER_KILL_BY_UNKNOWN", ser, param.mobname));
                        }
                        playerdatas.add_time(1, param.mobname, 1);
                    }
                    break;
                case "decodefailed":
                    send(ser,type["cause"].ToString(),0,Lang.get("WSPACK_RECEIVE_ERROR",ser,param.msg));
                    break;
            }
            
        }
        private static void send2Other(string ser, string t) {
            foreach (var i in Setting.setting.Servers) { 
                if(i.others.Contains(ser))
                    SendPack.sendText(i.name, t);
            }
        }
        private static Table GetTable(JObject p)
        {
            var t = new Table(LUAAPI.lua);
            t.Set("cause", DynValue.NewString(p["cause"].ToString()));
            var pa = new Table(LUAAPI.lua);
            System.Reflection.MemberInfo[] members = typeof(@params).GetProperties();
            foreach (var i in members)
            {
                if(p["params"][i.Name] != null)
                {
                    try { pa.Set(i.Name, DynValue.NewString(p["params"][i.Name].ToString())); }
                    catch { }
                }
            }
            //t.Set("server", DynValue.NewString(ser));
            t.Set("params", DynValue.NewTable(pa));
            return t;
        }
        private static void lua_on_pack(string ser, string p)
        {
            var type = JObject.Parse(p);
            var pa = JsonConvert.DeserializeObject<@params>(type["params"].ToString());
            if (type["cause"] == null)
                return;       
            LUAAPI.func["ws"].ForEach(s =>
            {
                try
                {
                    var t = GetTable(type);
                    s.Call(ser,t);
                }
                catch (Exception ex) {send(ser,"error",0,$"[Error][xb_lua] {ex}"); }
            });
        }
    }
}

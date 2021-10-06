using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CQ = HuajiTech.CoolQ.CurrentPluginContext;
using XBridgeR.Data;
using XBridgeR.Utils;
using Newtonsoft.Json;
using static XBridgeR.Data.TMP;

namespace XBridgeR.Func
{
    class WSReceive
    {
        private static Dictionary<string, DateTime> ptime = new Dictionary<string, DateTime>();
        public static void on_ws_pack(string ser, string pack)
        {
            var jp = JObject.Parse(pack);
            if (jp["type"] != null)
            {
                switch (jp["type"].ToString())
                {
                    case "pack":
                        try
                        {
                            on_pack(ser, pack);
                        }
                        catch (Exception e)
                        {
                            send("error", 0, Lang.get("WSPACK_RECEIVE_ERROR", ser, e.Message));
                        }
                        break;
                    case "encrypted":
                        try
                        {
                            if (jp["params"]["raw"] == null) return;
                            string unecrypt = AES.AesDecrypt(jp["params"]["raw"].ToString(),TMP.GetWSKey(TMP.WebsocketKey.k),TMP.GetWSKey(TMP.WebsocketKey.iv));
                            on_pack(ser, unecrypt);
                        }
                        catch (Exception e)
                        {
                            send("error", 0, Lang.get("WSPACK_RECEIVE_ERROR", ser, e.ToString()));
                        }
                        break;
                }
            }
        }

        private static void send(string type, GroupType gtype, object o)
        {
            switch (gtype)
            {
                case GroupType.main:
                    switch (type)
                    {
                        case "error":
                        case "decodefailed":
                            if(TMP.GetGroupSet().errorenable)
                                CQ.Group(TMP.GetGroup(GroupType.main)).Send(o.ToString());
                            break;
                    }

                    break;
                case GroupType.chat:
                    long id = TMP.cfg.Group.chat;
                    switch (type)
                    {
                        case "chat":
                            if (TMP.GetGroupSet().chatenable)
                                CQ.Group(id).Send(o.ToString());
                            break;
                        case "mobdie":
                            if (TMP.GetGroupSet().mobdieenable)
                                CQ.Group(id).Send(o.ToString());
                            break;
                        case "join":
                            if (TMP.GetGroupSet().joinenable)
                                CQ.Group(id).Send(o.ToString());
                            break;
                        case "left":
                            if (TMP.GetGroupSet().leftenable)
                                CQ.Group(id).Send(o.ToString());
                            break;
                    }
                    break;
            }
            
        }
        private static void setTime(string p, DateTime time)
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
        private static int getTime(string p, DateTime time)
        {
            if (ptime.ContainsKey(p))
                return ((int)(time - ptime[p]).TotalMinutes);
            return 0;
        }

        private static void on_pack(string ser, string p)
        {
            var type = JObject.Parse(p);
            var param = JsonConvert.DeserializeObject<@params>(type["params"].ToString());
            if (type["cause"] == null)
                return;
            switch (type["cause"].ToString())
            {
                case "join":
                    setTime(param.sender, DateTime.Now);
                    XBOXID.AddTime(0, param.sender, 1);
                    send(type["cause"].ToString(), GroupType.chat, Lang.get("MEMBER_JOIN", ser, param.sender, XBOXID.GetPlayerData(param.sender).count.join.ToString()));
                    break;
                case "left":
                    XBOXID.AddTime(2, param.sender, getTime(param.sender, DateTime.Now));
                    send(type["cause"].ToString(), GroupType.chat, Lang.get("MEMBER_LEFT", ser, param.sender));
                    break;
                case "chat":
                    send(type["cause"].ToString(), GroupType.chat, Lang.get("MEMBER_CHAT", ser, param.sender, param.text));
                    break;
                case "runcmdfeedback":
                    CQ.Group(TMP.GetGroup(GroupType.main)).Send(Lang.get("CMD_FEEDBACK", ser, param.result));
                    break;
                case "mobdie":
                    if (param.mobtype != "" && param.mobname != "")
                    {
                        string mob = "entity." + param.srctype.ToLower() + ".name";
                        if (DieMessage.mobs.ContainsKey(mob) && param.srctype.ToLower() != "unknown")
                        {
                            send(type["cause"].ToString(), GroupType.chat, Lang.get("MEMBER_KILL_BY_MOBS", ser, param.mobname, DieMessage.mobs[mob]));
                        }
                        else if (DieMessage.die_id.ContainsKey(param.dmcase.ToString()))
                        {
                            send(type["cause"].ToString(), GroupType.chat, Lang.get("MEMBER_KILL_BY_SELF", ser, param.mobname, DieMessage.die_id[param.dmcase.ToString()]));
                        }
                        else
                        {
                            send(type["cause"].ToString(), GroupType.chat, Lang.get("MEMBER_KILL_BY_UNKNOWN", ser, param.mobname));
                        }
                        XBOXID.AddTime(1, param.mobname, 1);
                    }
                    break;
                case "plantext":
                    CQ.Group(TMP.GetGroup(GroupType.main)).Send(param.text);
                    break;
                case "start":
                    CQ.Group(TMP.GetGroup(GroupType.main)).Send(Lang.get("SERVER_START",ser));
                    break;
                case "stop":
                    CQ.Group(TMP.GetGroup(GroupType.main)).Send(Lang.get("SERVER_STOP", ser));
                    break;
                case "decodefailed":
                    send(type["cause"].ToString(), 0, Lang.get("WSPACK_RECEIVE_ERROR", ser, param.msg));
                    break;
            }

        }
    }
}

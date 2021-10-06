using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HuajiTech.CoolQ.Events;
using CQ = HuajiTech.CoolQ.CurrentPluginContext;
using XBridgeR.Data;
using HuajiTech.CoolQ.Messaging;
using XBridgeR.Utils;

namespace XBridgeR.Func
{
    public class GroupMEssage
    {
        public static void Group_Main_Message(object sender, GroupMessageReceivedEventArgs e)
        {
            if (e.Source.Number != TMP.GetGroup(TMP.GroupType.main))
                return;
            long id = e.Sender.Number;
            var plain = e.Message.Parse().GetPlainText();
            var pl = plain.Split(' ');
            switch (pl[0])
            {
                case "/bind":
                    string xbox = plain.Substring(6);
                    if (pl.Length > 1)
                    {
                        if (XBOXID.QQExists(id))
                        {
                            e.Reply(Lang.get("MEMBER_ALREADY_IN_WHITELIST", XBOXID.GetXboxIDByQQ(id)));
                            return;
                        }
                        if (XBOXID.GetQQByXBOXID(xbox).find)
                        {
                            e.Reply(Lang.get("XBOXID_ALREADY_BIND"));
                            return;
                        }
                        XBOXID.Add(id, xbox);
                        if (e.Sender.CanEditAlias && TMP.AutoRNAME)
                            e.Sender.SetAlias(Lang.get("RENAME_AFTER_BIND",xbox));
                        e.Reply(Lang.get("MEMBER_BIND_SUCCESS", xbox));
                        if (TMP.AutoWL)
                        {
                            string cmdid = SendPack.Runcmd($"whitelist add \"{xbox}\"");
                            
                            e.Reply(Lang.get("XBOXID_ADD_TO_SERVER_SUCCESS"));
                        }
                    }
                    break;
                case "/unbind":
                    if (XBOXID.QQExists(id))
                    {
                        SendPack.Runcmd($"whitelist remove \"{XBOXID.GetXboxIDByQQ(id)}\"");
                        XBOXID.Remove(XBOXID.GetXboxIDByQQ(id));
                        e.Reply(Lang.get("MEMBER_UNBIND"));
                    }
                    else
                    {
                        e.Reply(Lang.get("MEMBER_NOT_BIND"));
                    };
                    break;
                case "wl+":
                    if (Msg.GetAt(e).Length != 0)
                    {
                        if (!TMP.is_Admin(id))
                        {
                            e.Reply(Lang.get("MEMBER_NOT_ADMIN"));
                            return;
                        }
                        foreach (long q in Msg.GetAt(e))
                        {
                            if (XBOXID.QQExists(q))
                            {
                                e.Reply(Lang.get("ADD_WL_TO_SERVER", q.ToString(), XBOXID.GetXboxIDByQQ(q)));
                                SendPack.Runcmd($"whitelist add \"{XBOXID.GetXboxIDByQQ(q)}\"");
                            }
                            else
                            {
                                e.Reply(Lang.get("MEMBER_NOT_BIND_WHEN_REMOVE", q.ToString()));
                            }
                        }
                    }
                    break;
                case "wl-":
                    if (Msg.GetAt(e).Length != 0)
                    {
                        if (!TMP.is_Admin(id))
                        {
                            e.Reply(Lang.get("MEMBER_NOT_ADMIN"));
                            return;
                        }
                        foreach (long q in Msg.GetAt(e))
                        {
                            if (XBOXID.QQExists(q))
                            {
                                e.Reply(Lang.get("REMOVE_WL_TO_SERVER", q.ToString(), XBOXID.GetXboxIDByQQ(q)));
                                SendPack.Runcmd($"whitelist remove \"{XBOXID.GetXboxIDByQQ(q)}\"");
                                XBOXID.Remove(q);
                            }
                            else
                            {
                                e.Reply(Lang.get("MEMBER_NOT_BIND_WHEN_REMOVE", q.ToString()));
                            }
                        }
                    }
                    break;
                case "/cmd":
                    if (!TMP.is_Admin(e.Sender.Number))
                    {
                        e.Reply(Lang.get("MEMBER_NOT_ADMIN"));
                        return;
                    }
                    e.Reply(Lang.get("COMMAND_SENDTO_SERVER", plain.Substring(5)));
                    SendPack.Runcmd(plain.Substring(5));
                    break;
                case "查服":
                    SendPack.Runcmd("list");
                    break;
                case "我的统计":
                    var dt = XBOXID.GetPlayerData(id);
                    if (dt == null)
                    {
                        e.Reply(Lang.get("MEMBER_NOT_BIND"));
                        return;
                    }
                    var bb = new StringBuilder($"[{dt.xboxid}]\n");
                    bb.AppendLine($"加入次数:{dt.count.join}");
                    bb.AppendLine($"死亡次数:{dt.count.death}");
                    bb.Append($"总时长:{dt.count.duration}(分钟)");
                    e.Source.Send(bb.ToString());
                    break;
            }
        }
        private static string name2xboxid(string name, long q)
        {
            if (XBOXID.QQExists(q))
                return XBOXID.GetXboxIDByQQ(q);
            return name;
        }
        public static void on_chat(object sender, GroupMessageReceivedEventArgs e)
        {
            if (TMP.cfg.Group.chat != e.Source.Number)
                return;
            long id = e.Source.Number;
            string plain = e.Message.Parse().GetPlainText();
            if (TMP.ChatIndex == "*")
            {
                if (Msg.GetMsg(e) != string.Empty)
                    SendPack.SendText(Lang.get("GROUP_MEMBER_CHAT", name2xboxid(e.Sender.DisplayName, e.Sender.Number), Msg.GetMsg(e)));
            }
            else
            {
                if (plain.Substring(0, TMP.ChatIndex.Length) == TMP.ChatIndex)
                {
                    if (plain.Length > TMP.ChatSub + 1)
                        SendPack.SendText(Lang.get("GROUP_MEMBER_CHAT", name2xboxid(e.Sender.DisplayName, e.Sender.Number), plain.Substring(TMP.ChatIndex.Length)));
                }
            }
        }
        public static void on_group_cmd(object sender,GroupMessageReceivedEventArgs e)
        {
            if (e.Source.Number != TMP.cfg.Group.main)
                return;
            var msg = e.Message.Parse().GetPlainText().Split(' ');
            if (msg[0] == "/xb")
            {
                if (msg.Length >= 2)
                {
                    switch (msg[1])
                    {
                        case "info":
                            string m = "XBridgeR v1.0.2.4\n";
                            m += "作者：Lition\n";
                            m += "唯一官方QQ群：808776416\n";
                            m += "本机器人模块依赖bug驱动";
                            e.Source.Send(m);
                            break;
                        case "run":
                            if (TMP.is_Admin(e.Sender.Number))
                                e.Source.Send(XBR_CMD.runcode(e.Message.Parse().GetPlainText().Substring("/xb run ".Length)).text);
                            else
                                e.Reply(Lang.get("MEMBER_NOT_ADMIN"));
                            break;
                    }
                }
            }
        }
    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CQ = HuajiTech.CoolQ.CurrentPluginContext;
using XBridgeR.Data;
using HuajiTech.CoolQ.Events;
using XBridgeR.Utils;
using HuajiTech.CoolQ;
using System.Text.RegularExpressions;
using HuajiTech.CoolQ.Messaging;

namespace XBridgeR.Func
{
    class onRegexs
    {
        /// <summary>
        /// 正则表达式组
        /// </summary>
        public static List<RegexItem> regexs;
        private static string buildString(GroupMessageReceivedEventArgs e, string input)
        {
            long qq = e.Sender.Number;
            StringBuilder builder = new StringBuilder(input);
            var mb = CQ.Member(qq, e.Source.Number);
            if (XBOXID.QQExists(qq))
                builder.Replace("%xboxid%", XBOXID.GetXboxIDByQQ(qq));
            builder.Replace("%qqid%", qq.ToString());
            builder.Replace("%qqnick%", mb.Nickname ?? mb.DisplayName);
            builder.Replace("%random%", new Random().Next(1, 100).ToString());
            if (Msg.GetAt(e).Length != 0)
            {
                try
                {
                    long atqq = Msg.GetAt(e)[0];
                    var mmb = CQ.Member(atqq, e.Source.Number);
                    if (XBOXID.QQExists(atqq))
                        builder.Replace("%atxboxid%", XBOXID.GetXboxIDByQQ(atqq));
                    builder.Replace("%atqqid%", atqq.ToString());
                    builder.Replace("%atqqnick%", mmb.Nickname ?? mmb.DisplayName);
                }
                catch (Exception ex) { CQ.Logger.LogError(ex.ToString()); }
            }
            builder.Replace("%datetime%", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            return builder.ToString();
        }
        public static void on_regex(GroupMessageReceivedEventArgs e)
        {
            long groupid = e.Source.Number;
            if (TMP.cfg.Group.main==groupid)
            {
                string input = Msg.GetPlanText(e);
                if (string.IsNullOrEmpty(input))
                    return;
                CQ.Logger.Log(input);
                long qq = e.Sender.Number;
                foreach (var i in regexs)
                {
                    Match mat = Regex.Match(input, i.Regex);
                    if (mat.Success)
                    {
                        if (i.permission == 1 && !TMP.is_Admin(qq))
                            continue;
                        foreach (var it in i.@out)
                        {
                            string o = it.text;
                            for (int io = 1; io < mat.Groups.Count; io++)
                            {
                                o = o.Replace($"${io}", mat.Groups[io].Value);
                            }
                            o = buildString(e, o);
                            bool cou = false;
                            try
                            {
                                cou = on_regex_item(groupid, it, o, qq);
                            }
                            catch (Exception exx) { e.Source.Send($"执行[{i.Regex}]时出错:{exx.Message}"); }
                            if (!cou)
                                break;
                        }

                    }
                }
            }
        }
        public static void on_chat_regex(object sender, GroupMessageReceivedEventArgs e)
        {
            try
            {
                on_regex(e);

            }catch(Exception ex)
            {
                CQ.Logger.LogError(ex.ToString());
            }
        }
        private static bool on_regex_item(long group, Out it, string o, long qq)
        {
            switch (it.type)
            {
                case "runcmd":
                    SendPack.Runcmd(o);
                    return true;
                case "group":
                    CQ.Group(group).Send(o);
                    return true;
                case "textall":
                    SendPack.SendText(o);
                    return true;
                case "xb_wl_remove":
                    if (XBOXID.QQExists(qq))
                    {
                        XBOXID.Remove(qq);
                        return true;
                    }
                    return false;
                case "xb_wl_add":
                    if (!XBOXID.QQExists(qq))
                    {
                        XBOXID.Add(qq, o);
                        return true;
                    }
                    return false;
                case "xb_cmd":
                    CQ.Group(group).Send(XBR_CMD.runcode(o).text);
                    return true;
                case "http_get":
                    Task.Run(() =>
                    {
                        try
                        {
                           CQ.Group(TMP.GetGroup(TMP.GroupType.main)).Send(Encoding.UTF8.GetString(new System.Net.WebClient().DownloadData(o)));
                        }
                        catch(Exception e) { CQ.Group(TMP.GetGroup(TMP.GroupType.main)).Send(e.ToString()); }
                    });
                    return true;
                case "stop_server":
                    SendPack.Runcmd("stop");
                    return true; ;
                default:
                    return false;
            }
        }
        public class Out
        {
            /// <summary>
            /// 执行模式
            /// </summary>
            public string type { get; set; }
            /// <summary>
            /// 执行命令
            /// </summary>
            public string text { get; set; }
        }

        public class RegexItem
        {
            /// <summary>
            /// 正则主体
            /// </summary>
            public string Regex { get; set; }
            /// <summary>
            /// 权限
            /// </summary>
            public int permission { get; set; }
            /// <summary>
            /// 响应组
            /// </summary>
            public List<Out> @out { get; set; }
        }
    }
}

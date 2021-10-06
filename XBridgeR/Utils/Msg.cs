using HuajiTech.CoolQ.Messaging;
using HuajiTech.CoolQ.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XBridgeR.Data;
using HuajiTech.CoolQ;

namespace XBridgeR.Utils
{
    /// <summary>
    /// 用于解析消息
    /// </summary>
    class Msg
    {
        /// <summary>
        /// 获取消息链中所有被At的id
        /// </summary>
        /// <param name="e">消息链</param>
        /// <returns></returns>
        public static long[] GetAt(GroupMessageReceivedEventArgs e)
        {
            List<long> qs = new List<long>();
            foreach (var i in e.Message.Parse().CombinePlainText())
            {
                if (i is Mention men)
                {
                    qs.Add(men.TargetNumber);
                }
            }
            return qs.ToArray();
        }
        public static string GetMsg(GroupMessageReceivedEventArgs e)
        {
            string msg = string.Empty;
            foreach (var i in e.Message.Parse().CombinePlainText())
            {
                switch (i)
                {
                    case Mention men:
                        msg += Lang.get("MESSAGE_AT", id2name(e.Source.Number, men.TargetNumber));
                        break;
                    case MentionAll menall:
                        msg += Lang.get("MESSAGE_AT_ALL");
                        break;
                    case PlainText plain:
                        if (plain.Content.IndexOf("不支持") == -1)
                            msg += plain.Content;
                        break;
                    case Image image:
                        msg += Lang.get("MESSAGE_IMAGE");
                        break;
                    case Emoticon em:
                        msg += Lang.get("MESSAGE_FACE");
                        break;
                }
            }
            return msg;
        }
        public static string GetPlanText(GroupMessageReceivedEventArgs e)
        {
            string msg = string.Empty;
            foreach (var i in e.Message.Parse().CombinePlainText())
            {
                switch (i)
                {
                    case PlainText plain:
                        if (plain.Content.IndexOf("不支持") == -1)
                            msg += plain.Content;
                        break;
                }
            }
            return msg;
        }
        private static string id2name(long group, long q)
        {
            if (XBOXID.QQExists(q))
                return XBOXID.GetXboxIDByQQ(q);
            var g = CurrentPluginContext.Group(group);
            foreach (var i in g.GetMembers())
            {
                if (i.Number == q)
                {
                    return i.Nickname ?? i.DisplayName;
                }
            }
            return string.Empty;
        }
    }
}
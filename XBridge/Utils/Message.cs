using HuajiTech.CoolQ.Messaging;
using HuajiTech.CoolQ.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HuajiTech.CoolQ;
using XBridge.Config;

namespace XBridge.Utils
{
    /// <summary>
    /// 用于解析消息
    /// </summary>
    class Message
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
                        msg += Lang.get("MESSAGE_AT",id2name(e.Source.Number,men.TargetNumber));
                        break;
                    case MentionAll menall:
                        msg += Lang.get("MESSAGE_AT_ALL");
                        break;
                    case PlainText plain:
                        if(plain.Content.IndexOf("不支持") == -1)
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
        public static object GetMsgJ(GroupMessageReceivedEventArgs e)
        {
            var msg = new List<object>();
            foreach (var i in e.Message.Parse().CombinePlainText())
            {
                switch (i)
                {
                    case Mention men:
                        msg.Add(new
                        {
                            type = "At",
                            target = men.TargetNumber
                        });
                        break;
                    case MentionAll menall:
                        msg.Add(new
                        {
                            type = "AtAll"
                        });
                        break;
                    case PlainText plain:
                        if (plain.Content.IndexOf("不支持") == -1)
                            msg.Add(new
                            {
                                type = "PlanText",
                                text = plain.Content
                            });
                        break;
                    case Image image:
                        msg.Add(new
                        {
                            type = "Image"
                        });
                        break;
                    case Emoticon em:
                        msg.Add(new
                        {
                            type = "Face",
                            id = em.Id,
                            
                        });
                        break;
                }
            }
            
            return new {
                from = new
                {
                    member = e.Sender.Number,
                    xboxid = Data.get_xboxid(e.Sender.Number),
                    group = e.Source.Number
                },
                messageChain= msg
            };
        }
        private static string id2name(long group,long q)
        {
            if (Data.wl_exsis(q))
                return Data.get_xboxid(q);
            var g = CurrentPluginContext.Group(group);
            foreach(var i in g.GetMembers())
            {
                if(i.Number == q)
                {
                    return i.Nickname ?? i.DisplayName;
                }
            }
            return string.Empty;
        }
    }
}

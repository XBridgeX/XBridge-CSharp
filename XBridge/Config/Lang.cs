﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using XBridge.Utils;

namespace XBridge.Config
{
    class Lang
    {
        public static Dictionary<string, string> lang = new Dictionary<string, string>()
        {
            {"MEMBER_NOT_ADMIN","你不是管理员，无法进行此操作！" },
            {"MEMBER_ALREADY_IN_WHITELIST","你已经绑定过一个白名单了({0})" },
            {"MEMBER_NOT_BIND","你还没有绑定白名单！" },
            {"MEMBER_NOT_BIND_WHEN_REMOVE","成员{0}还没有绑定白名单！" },
            {"MEMBER_BIND_SUCCESS","你的白名单({0})绑定成功" },
            {"MEMBER_UNBIND","你的白名单已解绑" },
            {"XBOXID_ADD_TO_SERVER_SUCCESS","你的白名单已成功添加到服务器" },
            {"XBOXID_ALREADY_BIND","这个xboxid已经被别人绑定过了"},
            {"ADD_WL_TO_SERVER" ,"尝试添加{0}的白名单({1})到所有服务器"},
            {"REMOVE_WL_TO_SERVER" ,"尝试移除{0}的白名单({1})到所有服务器"},
            {"COMMAND_OVERLOAD_NOTFIND","指令参数不足!" },
            {"COMMAND_SENDTO_SERVER" ,"指令{0}已发送到服务器[{1}]"},
            {"COMMAND_SENDTO_ALL_SERVER", "指令{0}已发送到服务器"},
            {"MEMBER_LEFT_GROUP","成员{0}退出了群聊，尝试撤销其白名单" },
            {"MEMBER_JOIN","{1} 加入了 {0}，这是第{2}次加入服务器" },
            {"MEMBER_LEFT","{1} 离开了 {0}" },
            {"MEMBER_CHAT","[{0}]<{1}> {2}" },
            {"MEMBER_KILL_BY_MOBS","[{0}] {1} 被 {2} 杀死了" },
            {"MEMBER_KILL_BY_SELF","[{0}] {1} {2}" },
            {"MEMBER_KILL_BY_UNKNOWN","[{0}] {1} 已死亡，原因未知" },
            {"SERVER_MEMBER_JOIN","{1} 加入了 {0}" },
            {"SERVER_MEMBER_LEFT","{1} 离开了 {0}" },
            {"SERVER_MEMBER_CHAT","[{0}]<{1}> {2}" },
            {"GROUP_MEMBER_CHAT","[群聊]<{0}> {1}" },
            {"CMD_FEEDBACK","[cmd][{0}] {1}" },
            {"WSPACK_RECEIVE_ERROR","与服务器[{0}]的收信文本解析失败:{1}" },
            {"MESSAGE_AT","@{0}" },
            {"MESSAGE_AT_ALL","@全体成员" },
            {"MESSAGE_IMAGE","[图片]" },
            {"MESSAGE_FACE","[表情]" }
        };
        public static string get(string k, params string[] p)
        {
            if (lang.ContainsKey(k))
                return build(format(lang[k],p));
            return k;
        }
        private static string format(string input,params string[] p)
        {
            return string.Format(input, p);
        }
        public static void set(string k,string v)
        {
            if (lang.ContainsKey(k))
                lang[k] = v;
        }
        private static string build(string t)
        {
            var builder = new StringBuilder(t);
            builder.Replace("%DATETIME_NOW%", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            builder.Replace("%UTILS_RANDOM", new Random().Next(1, 100).ToString());
            builder.Replace("%UTILS_NEWLINE%", "\n");
            return builder.ToString();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using HuajiTech.CoolQ;
using HuajiTech.CoolQ.Events;
using HuajiTech.CoolQ.Messaging;
using XBridgeR.Data;
using XBridgeR.Func;

namespace XBridgeR
{
    /// <summary>
    /// 包含应用的逻辑。
    /// </summary>
    internal class Main : Plugin
    {
        /// <summary>
        /// 使用指定的事件源初始化一个 <see cref="Main"/> 类的新实例。
        /// </summary>
        public Main(IMessageEventSource messageEventSource)
        {
            TMP.init();
            XBRLoader.init();
            messageEventSource.GroupMessageReceived += GroupMEssage.Group_Main_Message;
            messageEventSource.GroupMessageReceived += GroupMEssage.on_chat;
            messageEventSource.GroupMessageReceived += GroupMEssage.on_group_cmd;
            messageEventSource.GroupMessageReceived += onRegexs.on_chat_regex;
            Logger.LogSuccess("加载完毕");
        }
    }
}
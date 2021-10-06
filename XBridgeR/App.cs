using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using HuajiTech.CoolQ.Events;

namespace XBridgeR
{
    /// <summary>
    /// 提供对应用的初始化。
    /// </summary>
    internal static class App
    {
        /// <summary>
        /// 初始化应用。
        /// </summary>
        /// <remarks>
        /// 该方法会在酷Q主线程内被调用，不允许调用酷Q API，且不应长时间阻塞线程。
        /// 在该方法内引发的异常将会导致酷Q主程序停止运行。
        /// </remarks>
        public static void Init()
        {
            IBotEventSource botEventSource = BotEventSource.Instance;
            ICurrentUserEventSource currentUserEventSource = CurrentUserEventSource.Instance;
            IGroupEventSource groupEventSource = GroupEventSource.Instance;


            // 使用下面的代码在酷Q初始化后创建 Main 类的实例。
            // 需要在 app.json 中注册对应事件。

            botEventSource.BotStarted += (sender, e) =>
            {
                try
                {
                    new Main(currentUserEventSource);
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }

            };

        }
    }
}
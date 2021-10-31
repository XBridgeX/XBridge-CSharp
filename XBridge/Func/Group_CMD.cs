using HuajiTech.CoolQ.Events;
using HuajiTech.CoolQ.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using HuajiTech.CoolQ;
using XBridge.Utils;
using System.Text;
using System.Threading.Tasks;
using File = System.IO.File;
using XBridge.Config;
using Newtonsoft.Json;

namespace XBridge.Func
{
    public class Group_CMD
    {
        public  static void xbridge_cmd(object sender, MessageReceivedEventArgs e)
        {
            if (e is GroupMessageReceivedEventArgs group)
            {
                long groupid = e.Source.Number;
                if (Setting.setting.Group.main.Contains(groupid))
                {
                    var msg = e.Message.Parse().GetPlainText().Split(' ');
                    if (msg[0] == "/xb")
                    {
                        if (msg.Length >= 2)
                        {
                            switch (msg[1])
                            {
                                case "info":
                                    string m = $"XBridge v{Main.version}\n";
                                    m += "作者：Lition\n";
                                    m += "唯一官方QQ群：808776416\n";
                                    m += "本机器人模块依赖bug驱动";
                                    group.Source.Send(m);
                                    break;
                                case "update":
                                    e.Reply("\n"+Vcheck.updateM());
                                    break;
                                case "status":
                                    break;
                                case "run":
                                    if (Data.is_admin(group.Sender.Number))
                                        group.Source.Send(XB_CMD.runcode(e.Message.Parse().GetPlainText().Substring("/xb run ".Length)));
                                    else
                                        e.Reply(Lang.get("MEMBER_NOT_ADMIN"));
                                    break;
                            }
                        }
                    }
                }
            }
        }
    }
}

using HuajiTech.CoolQ;
using HuajiTech;
using HuajiTech.CoolQ.Events;
using HuajiTech.CoolQ.Messaging;
using XBridge.Utils;
using System;
using XBridge.Config;

namespace XBridge.Func
{
    public class MemberLeft
    {
        public static void Member_Left(object sender,GroupEventArgs e) {
            
            if (Setting.setting.Group.main.Contains(e.Source.Number)) {
                e.Source.Send($"{e.Operatee.Number} 离开了群聊");
                if (Data.wl_exsis(e.Operatee.Number))
                {
                    try
                    {
                        e.Source.Send(Lang.get("MEMBER_LEFT_GROUP", Data.get_xboxid(e.Operatee.Number)));
                        SendPack.runcmdAll(e.Source.Number, $"whitelist remove \"{Data.get_xboxid(e.Operatee.Number)}\"");
                        Data.wl_remove(e.Operatee.Number);
                        Data.SAVE();
                    }
                    catch (Exception ex) { e.Source.Send($"处理退群事件时出现异常{ex}"); }
                }
            }
        }
            
    }
}

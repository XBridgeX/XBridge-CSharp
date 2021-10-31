using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XBridge.Utils;
using HuajiTech.CoolQ.Events;
using HuajiTech.CoolQ.Messaging;
using MoonSharp.Interpreter;

namespace XBridge.Func
{
    class GroupLua
    {
        public static void on_message(object sender,GroupMessageReceivedEventArgs e)
        {
            long groupid = e.Source.Number;
            if (Setting.setting.Group.main.Contains(groupid) || Setting.setting.Group.chat.Contains(groupid))
            {
                var t = new Table(LUAAPI.lua);
                var gt = new Table(LUAAPI.lua);
                var mt = new Table(LUAAPI.lua);
                var st = new Table(LUAAPI.lua);
                st.Set("id", DynValue.NewNumber(e.Sender.Number));
                st.Set("DisplayName", DynValue.NewString(e.Sender.DisplayName));
                gt.Set("DisplayName", DynValue.NewString(e.Source.DisplayName));
                gt.Set("id", DynValue.NewNumber(groupid));
                mt.Set("content", DynValue.NewString(e.Message.Content));
                mt.Set("plain", DynValue.NewString(e.Message.Parse().GetPlainText()));
                t.Set("message", DynValue.NewTable(mt));
                t.Set("sender", DynValue.NewTable(st));
                t.Set("group", DynValue.NewTable(gt));
                LUAAPI.func["msg"].ForEach(s =>
                {
                    try
                    {
                        s.Call(t);
                    }catch(Exception ex) { e.Source.Send($"[Error][xb_lua] {ex}"); }
                });
            }
        }
    }
}

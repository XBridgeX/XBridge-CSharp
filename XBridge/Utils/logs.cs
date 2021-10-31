using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CQ = HuajiTech.CoolQ.CurrentPluginContext;
using file = System.IO.File;
using HuajiTech.CoolQ;

namespace XBridge.Utils
{
    class logs
    {
        public static void logToFile(object o)
        {
            CQ.Logger.Log(o.ToString());
            file.AppendAllText(CQ.Bot.AppDirectory.FullName + $"logs/{DateTime.Now.ToString("yyyy-M-dd")}.log",$"[{DateTime.Now}] "+o.ToString()+"\n");
        }
        public static void log(object o)
        {
            CQ.Logger.Log(o.ToString());
        }
    }
}

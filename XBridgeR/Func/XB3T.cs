using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XBridgeR.Data;

namespace XBridgeR.Func
{
    class XB3T
    {
        public static Dictionary<string, code> codes = new Dictionary<string, code>();
        public static code BuildCode(string code)
        {
            
            string[] c = code.Replace("\n", "").Replace("\t", "").Replace("\r", "").Split(new[] {"if","then","else" },StringSplitOptions.RemoveEmptyEntries);
            if(c.Length != 3)
            {
                return null;
            }
            var xbcode = new code();
            xbcode.@if = GetPara1(c[0]);
            xbcode.then = GetParam2(c[1]);
            xbcode.@else = GetParam2(c[2]);
            return xbcode;
        }
        public static string[] GetPara1(string str)
        {
            return str.Substring(str.IndexOf("(") + 1, str.IndexOf(")") - 1 - str.IndexOf("(")).Split(',');
        }
        public static string[] GetParam2(string str)
        {
            return str.Substring(str.IndexOf("{") + 1, str.IndexOf("}") - 1 - str.IndexOf("{")).Split(';');
        }
        public static bool RunCode(string[] c)
        {
            bool re = true;
            foreach(var o in c)
            {
                string oo = o.Trim();
                switch (oo.Substring(0, oo.IndexOf("(")))
                {
                    case "getXbox":
                        if (!XBOXID.QQExists(long.Parse(GetPara1(oo)[0])))
                            return false;
                        break;
                    case "xb_cmd":
                        if (!XBR_CMD.runcode(GetPara1(oo)[0]).success)
                            return false;
                        break;
                    default:
                        return false;
                }
            }
            return re;
        }

        public static void CodeRunTime(code cx)
        {
            if (RunCode(cx.@if))
            {
                RunCode(cx.then);
            }
            else
            {
                RunCode(cx.@else);
            }
        }
        public class code
        {
            public string[] @if { get; set; }
            public string[] then { get; set; }
            public string[] @else { get; set; }
        }
    }
}
/*
if()then
{
    
}
else
{
    
}


 */
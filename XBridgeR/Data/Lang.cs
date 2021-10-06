using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XBridgeR.Data
{
    class Lang
    {
        static Dictionary<string, string> lang = new Dictionary<string, string>();
        public static string get(string k, params string[] p)
        {
            if (lang.ContainsKey(k))
                return format(lang[k], p);
            return k;
        }
        private static string format(string input, params string[] p)
        {
            return string.Format(input, p);
        }
        public static void set(string k, string v)
        {
            if (lang.ContainsKey(k))
                lang[k] = v;
            else
                lang.Add(k, v);
        }
    }
}

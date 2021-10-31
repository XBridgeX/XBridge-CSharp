using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using File = System.IO.File;
using HuajiTech.CoolQ;
using System.IO;
using XBridge.Config;

namespace XBridge.Utils
{
    class Data
    {
        /// <summary>
        /// 判断是否是服务器
        /// </summary>
        /// <param name="ser">名称</param>
        /// <returns></returns>
        public  static bool is_server(string ser)
        {
            return Main.sockets.ContainsKey(ser);
        }
        public static bool wl_exsis(long q)
        {
            return Main.playerdatas.wl_exsis(q);
        }
        public static bool wl_add(long q,string xboxid)
        {
            return Main.playerdatas.wl_add(q, xboxid);
        }
        public static bool wl_remove(long q)
        {
            return Main.playerdatas.wl_remove(q);
        }
        public static bool xboxid_exsis(string id)
        {
            return Main.playerdatas.xboxidExsis(id);
        }
        public static string get_xboxid(long q)
        {
            return Main.playerdatas.getXboxid(q);
        }
        public static bool is_admin(long q)
        {
            foreach (var i in Setting.setting.Admins)
            {
                if (i == q)
                    return true;
            }
            return false;
        }
        public static long id2qq(string id) {
            return Main.playerdatas.xboxid2QQ(id);
        }
        public static int CheckJoinTime(string xboxid)
        {
            return Main.playerdatas.CheckJoinTime(xboxid);
        }
        public static void SAVE()
        {
            Main.playerdatas.SAVE();
        }
    }
}

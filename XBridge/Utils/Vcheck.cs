using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using System.Net;
using CQ = HuajiTech.CoolQ.CurrentPluginContext;
using System.Threading.Tasks;

namespace XBridge.Utils
{
    public class Vcheck
    {
        public static void init() {
            check("");
        }
        private static void AddTimer() {
            DateTime now = DateTime.Now;
            DateTime oneclock = DateTime.Today.AddHours(1.0);
            if (now > oneclock) {
                oneclock = oneclock.AddDays(1.0);
            }
            int usm = (int)(oneclock - now).TotalMilliseconds;
            var timer = new Timer(check);
            timer.Change(usm, Timeout.Infinite);
        }
        private static void check(object state) {
            Task.Run(() => {
                try
                {
                    var w = JsonConvert.DeserializeObject<apidata>(httpget("https://xbridgex.cn/api/xbcs.json"));
                    if (w.version != Main.version)
                    {
                        string msg = $"[XBridgeUpdate]\n获取到新版本:{w.version}\n"
                            + $"更新时间：{w.time}\n{w.updateinfo}";
                        foreach (var i in Main.tmp)
                            CQ.Group(i.Key).Send(msg);

                    }
                    logs.logToFile("自动更新获取完成");
                }
                catch (Exception e)
                {

                    Error.LogToFile("checkup", e.ToString());
                }
                AddTimer();
            });

        }
        private static string httpget(string url)
        {
            return Encoding.UTF8.GetString(new WebClient().DownloadData(url));
        }
        public static string updateM()
        {
            try
            {
                var w = JsonConvert.DeserializeObject<apidata>(httpget("https://xbridgex.cn/api/xbcs.json"));
                string msg = $"[XBridgeUpdate]\n最新版本:{w.version}\n当前版本：{Main.version}\n"
                       + $"更新时间：{w.time}\n{w.updateinfo}";
                return msg;
            }
            catch (Exception e) {
                return $"get update failed({e.Message})";
            }
        }
        private class apidata  {
            public string version { get; set; }
            public string updateinfo { get; set; }
            public string time { get; set; }
        }
    }
}

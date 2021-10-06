using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Web.Script.Serialization;
using System.Net;
using System.Windows.Forms;

namespace MCSM.XBR
{
    public class MCSM
    {
    }
}

namespace XBRLoader
{
    partial class Plugin
    {
        public const string AssemblyName = "io.mcsm.xb";
        static JavaScriptSerializer ser = new JavaScriptSerializer();
        static CFG cfg;
        public static string[] onLoad(string path)
        {
            if (Directory.Exists(path) == false)
            {
                Directory.CreateDirectory(path);
            }
            if (File.Exists(path + "README.txt"))
            {
                File.WriteAllText(path + "README.txt", MCSM.XBR.Properties.Resources.README);
            }
            string path2 = path + "config.json";
            if (!File.Exists(path2))
            {
                var p = new CFG
                {
                    APIKEY = "114514",
                    URL = "http://127.0.0.1:23333",
                    Servers = new Dictionary<string, Test>()
                };
                p.Servers.Add("test", new Test { GUID = "114514", UUID = "114514" });
                File.WriteAllText(path2, ser.Serialize(p));
            }
            cfg = ser.Deserialize<CFG>(File.ReadAllText(path2));
            return new string[] { "-mcsm",nameof(onMcsm) };
        }
        public static string onMcsm(string[] arg)
        {
            switch (arg[0])
            {
                case "open":
                    if (cfg.Servers.ContainsKey(arg[1]))
                    {
                        var s = cfg.Servers[arg[1]];
                        MCSMResult re=  HttpGet($"{cfg.URL}/api/protected_instance/open/?uuid={s.UUID}&remote_uuid={s.GUID}&apikey={cfg.APIKEY}");
                        if (re.status == 200)
                            return "实例启动请求成功";
                        return "执行出错,状态码：" + re.status;
                    }
                    else
                    {
                        return $"没有名为{arg[1]} 的实例";
                    }
                case "stop":
                    if (cfg.Servers.ContainsKey(arg[1]))
                    {
                        var s = cfg.Servers[arg[1]];
                        MCSMResult re = HttpGet($"{cfg.URL}/api/protected_instance/stop/?uuid={s.UUID}&remote_uuid={s.GUID}&apikey={cfg.APIKEY}");
                        if (re.status == 200)
                            return "实例关闭请求成功";
                        return "执行出错,状态码：" + re.status;
                    }
                    else
                    {
                        return $"没有名为{arg[1]} 的实例";
                    }
                case "restart":
                    if (cfg.Servers.ContainsKey(arg[1]))
                    {
                        var s = cfg.Servers[arg[1]];
                        MCSMResult re = HttpGet($"{cfg.URL}/api/protected_instance/restart/?uuid={s.UUID}&remote_uuid={s.GUID}&apikey={cfg.APIKEY}");
                        if (re.status == 200)
                            return "实例重启请求成功";
                        return "执行出错,状态码：" + re.status;
                    }
                    else
                    {
                        return $"没有名为{arg[1]} 的实例";
                    }
                case "kill":
                    if (cfg.Servers.ContainsKey(arg[1]))
                    {
                        var s = cfg.Servers[arg[1]];
                        MCSMResult re = HttpGet($"{cfg.URL}/api/protected_instance/kill/?uuid={s.UUID}&remote_uuid={s.GUID}&apikey={cfg.APIKEY}");
                        if (re.status == 200)
                            return "实例强制关闭请求成功";
                        return "执行出错,状态码：" + re.status;
                    }
                    else
                    {
                        return $"没有名为{arg[1]} 的实例";
                    }
            }
            return "没有这个命令参数";
        }
        public static MCSMResult HttpGet(string url)
        {
            try
            {
                string re = Encoding.UTF8.GetString(new WebClient().DownloadData(url));
                return ser.Deserialize<MCSMResult>(re);
            }
            catch(Exception e)
            {
                return new MCSMResult {status=500 };
            }
            
        }
    }
    public class Test
    {
        /// <summary>
        /// 
        /// </summary>
        public string GUID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string UUID { get; set; }
    }

    public class CFG
    {
        /// <summary>
        /// 
        /// </summary>
        public string URL { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string APIKEY { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string,Test> Servers { get; set; }
    }

    public class MCSMResult
    {
        /// <summary>
        /// 
        /// </summary>
        public int status { get; set; }
    }
}

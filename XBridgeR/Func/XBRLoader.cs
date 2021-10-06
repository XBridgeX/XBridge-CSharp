using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CQ = HuajiTech.CoolQ.CurrentPluginContext;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using HuajiTech.CoolQ;
using Newtonsoft.Json;
using File = System.IO.File;

namespace XBridgeR.Func
{
    class XBRLoader
    {
        static List<PluginInfo> plugins;
        public static void init()
        {
            string AppDirectory = CQ.Bot.AppDirectory.FullName + "XBRLoader\\";
            CretaDir(AppDirectory + "data\\");
            CretaDir(AppDirectory + "plugins\\");
            if (!File.Exists(AppDirectory + "config.json"))
            {
                File.WriteAllText(AppDirectory + "config.json", "[]");
            }
            plugins = JsonConvert.DeserializeObject<List<PluginInfo>>(File.ReadAllText(AppDirectory + "config.json"));
            foreach(var i in Directory.GetFiles(AppDirectory + "plugins\\", "*.dll"))
            {
                Assembly ass = Assembly.LoadFile(i);//要绝对路径
                Type type = ass.GetType("XBRLoader.Plugin");//必须使用名称空间+类名称
                string name = string.Empty;
                try
                {
                    foreach (var fie in type.GetFields())
                    {
                        if(fie.Name == "AssemblyName")
                        {
                            name = (string)fie.GetValue("AssemblyName");
                        }
                    }
                    if(name == string.Empty)
                    {
                        CQ.Logger.LogError($"无法在插件[{i}]中找到[AssemblyName]字段！初始化失败");
                        break;
                    }
                    if(has_id(name) == false)
                    {
                        plugins.Add(new PluginInfo { id = name, enable = true }); ;
                    }
                    var info = GetPluginInfo(name);
                    if (!info.enable)
                        break;
                    MethodInfo method = type.GetMethod("onLoad");//方法的名称
                    string[] re = (string[])method.Invoke(null, new object[] { AppDirectory + "data\\" + name + "\\" });
                    if(re.Length < 2)
                    {
                        CQ.Logger.LogError($"插件[{i}]返回了长度为[{re.Length}的数组]！初始化失败");
                        break;
                    }
                    if (type.GetMethod(re[1]) != null)
                    {
                        CQ.Logger.LogSuccess($"{name} 成功注册 >> [{re[0]}]{re[1]}");
                        XBR_CMD.RegisterCMD(re[0], type.GetMethod(re[1]));
                    }
                    else
                    {
                        CQ.Logger.LogError($"无法在插件[{i}]中找到名为[{re[1]}]的函数！初始化失败");
                    }
                    CQ.Logger.LogSuccess($"插件[{name}]已被加载，返回值为0");
                }
                catch (Exception ex)
                {
                    CQ.Logger.LogError(ex.ToString());
                }
                File.WriteAllText(AppDirectory + "config.json", JsonConvert.SerializeObject(plugins, Formatting.Indented));
            }

        }
        private static bool has_id(string id)
        {
            foreach(var i in plugins)
            {
                if (i.id == id)
                    return true;
            }
            return false;
        }
        private static PluginInfo GetPluginInfo(string id)
        {
            foreach (var i in plugins)
            {
                if (i.id == id)
                    return i;
            }
            return null;
        }
        private static void CretaDir(string dir)
        {
            if(Directory.Exists(dir) == false)
            {
                Directory.CreateDirectory(dir);
            }
        }
        public class PluginInfo
        {
            /// <summary>
            /// 
            /// </summary>
            public string id { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public bool enable { get; set; }
        }
    }
}

using HuajiTech.CoolQ;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using File = System.IO.File;
using XBridge.Config;
using System;
using System.Text;
using System.IO;

namespace XBridge.Func
{
    class INIT
    {
        public static void init() {
            if (CurrentPluginContext.Bot.AppDirectory.Exists == false)
                CurrentPluginContext.Bot.AppDirectory.Create();
            var path = CurrentPluginContext.Bot.AppDirectory.FullName;
            if (CurrentPluginContext.Bot.AppDirectory.Exists == false)
                CurrentPluginContext.Bot.AppDirectory.Create();
            if (!File.Exists(path + "setting.json")) {
                var tmp = new Setting() {
                    Servers = new List<Servers> { 
                        new Servers(){ 
                            name = "生存服务器",
                            Url = "ws://127.0.0.1:8800/mc",
                            password = "password",
                            others = new List<string>()
                        }
                    },
                    Admins = new List<long>() { 1145141919},
                    Group = new Group() { 
                        chat = new List<long> { 1145141919},
                        main = new List<long> { 1145141919}
                    },
                    enable = new enable { 
                        xb_lua = false,
                        xb_native = true,
                        xb_regex = false
                    }
                };
                var cfg = JsonConvert.SerializeObject(tmp,Formatting.Indented);
                File.WriteAllText(path + "setting.json", cfg);
            }
            Setting.setting = JsonConvert.DeserializeObject<Setting>(File.ReadAllText(path + "setting.json"));
            if(File.Exists(path+".lang") == false)
            {
                var s = new StringBuilder("#这里是XBridge的语言文件\n#以#开头的行会被认作注释\n");
                foreach(var i in Lang.lang)
                {
                    s.Append($"{i.Key}={i.Value}\n");
                }
                File.WriteAllText(path + ".lang", s.ToString());
            }
            var l = File.ReadAllLines(path + ".lang");
            foreach(var i in l)
            {
                if (i.StartsWith("#"))
                    continue;
                try{
                    var m = i.Split('=');
                    Lang.set(m[0], m[1]);
                }
                catch { }
            }
            if (Setting.setting.enable.xb_regex)
            {
                if (File.Exists(path + "Regex.json") == false)
                    File.WriteAllBytes(path + "Regex.json", Properties.Resources.Regex);
                try
                {
                    Regexs.regexs = JsonConvert.DeserializeObject<List<RegexItem>>(File.ReadAllText(path + "Regex.json"));
                }
                catch (Exception exx) { CurrentPluginContext.Logger.LogError(exx.ToString()); }
            }
            if (!File.Exists(path + "mobs.json"))
                File.WriteAllBytes(path + "mobs.json", Properties.Resources.mobs);
            if (!File.Exists(path + "die_id.json"))
                File.WriteAllBytes(path + "die_id.json", Properties.Resources.die_id);
            if (Setting.setting.enable.xb_native)
            {
                try
                {
                    DieMessage.mobs = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(path + "mobs.json"));
                    DieMessage.die_id = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(path + "die_id.json"));
                }
                catch (Exception exx) { CurrentPluginContext.Logger.LogError(exx.ToString()); }
            }
            Directory.CreateDirectory(path + "logs/");
            Directory.CreateDirectory(path + "/logs/error");
            Main.tmp = TMP.init();
        }
    }
}

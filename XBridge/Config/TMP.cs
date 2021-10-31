using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XBridge.Utils;
using Newtonsoft.Json;
using File = System.IO.File;
using CQ = HuajiTech.CoolQ.CurrentPluginContext;

namespace XBridge.Config
{
    class TMP
    {
        public static Dictionary<long, TMP> init()
        {
            if (!File.Exists(CQ.Bot.AppDirectory.FullName + "GroupData.json"))
            {
                File.WriteAllText(CQ.Bot.AppDirectory.FullName + "GroupData.json", JsonConvert.SerializeObject(new Dictionary<long, TMP>()));
            }
            try
            {
                var t = JsonConvert.DeserializeObject<Dictionary<long, TMP>>(File.ReadAllText(CQ.Bot.AppDirectory.FullName + "GroupData.json"));
                foreach (var i in Setting.setting.Group.main)
                {
                    if (!t.ContainsKey(i))
                    {
                        t.Add(i, new TMP());
                        foreach (var it in Setting.setting.Servers)
                        {
                            t[i].allowserver.Add(it.name);
                        }
                    }
                }
                foreach (var i in Setting.setting.Group.chat)
                {
                    if (!t.ContainsKey(i))
                    {
                        t.Add(i, new TMP());
                        foreach(var it in Setting.setting.Servers)
                        {
                            t[i].allowserver.Add(it.name);
                        }
                    }
                        
                }
                return t;
            }
            catch (Exception e) { Error.LogToFile("打开TMP文件时发生读取错误", e.ToString()); }
            return new Dictionary<long, TMP>();
        }
        public static void SAVE()
        {
            File.WriteAllText(CQ.Bot.AppDirectory.FullName + "GroupData.json", JsonConvert.SerializeObject(Main.tmp,Formatting.Indented));
        }
        /// <summary>
        /// 是否转发聊天
        /// </summary>
        public bool chatenable = true;
        /// <summary>
        /// 是否转发加入服务器
        /// </summary>
        public bool joinenable = true;
        /// <summary>
        /// 是否转发离开服务器
        /// </summary>
        public bool leftenable = true;
        /// <summary>
        /// 是否转发生物死亡
        /// </summary>
        public bool mobdieenable = true;
        /// <summary>
        /// 转发的服务器
        /// </summary>
        public List<string> allowserver = new List<string>();
        /// <summary>
        /// 异常捕获提示
        /// </summary>
        public bool errorenable = true;
        /// <summary>
        /// 聊天索引
        /// </summary>
        public string chatindex = "*";
        /// <summary>
        /// 群聊长度截取
        /// </summary>
        public int chatsub = 20;
        /// <summary>
        /// 自动白名单
        /// </summary>
        public bool autowl = false;
        
    }
    
}

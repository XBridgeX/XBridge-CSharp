using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;
using File = System.IO.File;
using CQ = HuajiTech.CoolQ.CurrentPluginContext;
using HuajiTech.CoolQ;

namespace XBridge.Utils
{
    class Error
    {
        public static void LogToFile(string cause,string error) {
            string id = Guid.NewGuid().ToString();
            error = JsonConvert.SerializeObject(new
            {
                Date = DateTime.Now.ToString(),
                cause,
                raw = error
            });
            var err = Convert.ToBase64String( Encoding.UTF8.GetBytes(error));
            CQ.Logger.LogError($"运行出现异常！错误ID：{id}，已写入文件");
            try { File.WriteAllText($"{CQ.Bot.AppDirectory.FullName}/logs/error/{id}.log", err); } catch { }

        }
    }
}

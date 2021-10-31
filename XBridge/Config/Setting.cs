using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XBridge
{
    
    public class Servers
    {
        /// <summary>
        /// 服务器名字
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 服务器地址
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// 服务器连接密匙
        /// </summary>
        public string password { get; set; }
        /// <summary>
        /// 接收其他的服务器组
        /// </summary>
       public List<string> others { get; set; }
    }

    public class Group
    {
        /// <summary>
        /// 主群，可做执行命令和获取白名单
        /// </summary>
        public List<long> main { get; set; }
        /// <summary>
        /// 聊天群
        /// </summary>
        public List<long> chat { get; set; }
    }
    public class enable
    {
        /// <summary>
        /// 是否启用XB原生功能
        /// </summary>
        public bool xb_native { get; set; }
        /// <summary>
        /// 是否启用正则表达式
        /// </summary>
        public bool xb_regex { get; set; }
        /// <summary>
        /// 是否启用lua
        /// </summary>
        public bool xb_lua { get; set; }
    }

    public class Setting
    {
        /// <summary>
        /// 服务器组
        /// </summary>
        public List<Servers> Servers { get; set; }
        /// <summary>
        /// 群组
        /// </summary>
        public Group Group { get; set; }
        /// <summary>
        /// 管理员
        /// </summary>
        public List<long> Admins { get; set; }
        /// </summary>
        /// 功能项
        /// </summary>
        public enable enable { get; set; }
        /// <summary>
        /// 基本设置
        /// </summary>
        public static Setting setting = new Setting();
    }
}

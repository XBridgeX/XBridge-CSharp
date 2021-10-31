using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XBridge.Config
{
    public class Out
    {
        /// <summary>
        /// 执行模式
        /// </summary>
        public string type { get; set; }
        /// <summary>
        /// 执行命令
        /// </summary>
        public string text { get; set; }
    }

    public class RegexItem
    {
        /// <summary>
        /// 正则主体
        /// </summary>
        public string Regex { get; set; }
        /// <summary>
        /// 权限
        /// </summary>
        public int permission { get; set; }
        /// <summary>
        /// 响应组
        /// </summary>
        public List<Out> @out { get; set; }
    }
}

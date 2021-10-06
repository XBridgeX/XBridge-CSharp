using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XBridgeR.Data
{
    class GroupSet
    {
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
        /// 异常捕获提示
        /// </summary>
        public bool errorenable = true;
    }
}

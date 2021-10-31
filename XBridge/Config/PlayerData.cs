using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XBridge.Config
{
    public class Count
    {
        /// <summary>
        /// 加入次数
        /// </summary>
        public int @join { get; set; }
        /// <summary>
        /// 死亡次数
        /// </summary>
        public int death { get; set; }
        /// <summary>
        /// 游玩时长
        /// </summary>
        public int duration { get; set; }
    }

    public class PlayerData
    {
        /// <summary>
        /// 游戏id
        /// </summary>
        public string xboxid { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Count count { get; set; }
    }

}

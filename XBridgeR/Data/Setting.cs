using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XBridgeR.Data
{
    public class Server
    {
        /// <summary>
        /// 
        /// </summary>
        public string url { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string password { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int re_connect_time { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool encrypt { get; set; }
    }

    public class Group
    {
        /// <summary>
        /// 
        /// </summary>
        public int main { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int chat { get; set; }
    }

    public class CFG
    {
        /// <summary>
        /// 
        /// </summary>
        public Server Server { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Group Group { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<long> Admin { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool AutoWhitelist { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int ChatSub { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ChatIndex { get; set; }
        public bool AutoRename { get; set; }
    }

}

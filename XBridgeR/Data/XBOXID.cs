using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CQ = HuajiTech.CoolQ.CurrentPluginContext;
using Newtonsoft.Json;
namespace XBridgeR.Data

{
    class XBOXID
    {
        public  static Dictionary<long, PlayerData> db;
        public static bool QQExists(long q)
        {
            return db.ContainsKey(q);
        }
        public static string GetXboxIDByQQ(long q)
        {
            return db[q].xboxid;
        }
        public static RequestResult GetQQByXBOXID(string id)
        {
            RequestResult result = new RequestResult(false);
            foreach (var i in db)
            {
                if (i.Value.xboxid == id)
                {
                    result.find = true;
                    result.qq = i.Key;
                    return result;
                }
            }
            return result;
        }
        public static void Add(long qq,string xboxid)
        {
            db.Add(qq, new PlayerData { xboxid = xboxid,count = new XBOXIDItem() });
            SAVE();
        }
        public static bool Remove(string xboxid)
        {
            if (GetQQByXBOXID(xboxid).find)
            {
                db.Remove(GetQQByXBOXID(xboxid).qq);
                SAVE();
                return true;
            }
            return false;
        }
        public static bool Remove(long q)
        {
            if (QQExists(q))
            {
                db.Remove(q);
                SAVE();
                return true;
            }
            return false;
        }
        public static PlayerData GetPlayerData(long q)
        {
            if (QQExists(q))
            {
                return db[q];
            }
            return null;
        }
        public static PlayerData GetPlayerData(string id)
        {
            if (GetQQByXBOXID(id).find)
                return db[GetQQByXBOXID(id).qq];
            return new PlayerData();
        }
        public static void AddTime(int mode,string id,int t)
        {
            if (GetQQByXBOXID(id).find == false)
                return;
            long q = GetQQByXBOXID(id).qq;
            switch (mode)
            {
                case 0:
                    db[q].count.join += t;
                    break;
                case 1:
                    db[q].count.death += t;
                    break;
                case 2:
                    db[q].count.duration += t;
                    break;
            }
            SAVE();

        }
        public static void SAVE()
        {
            string AppDirectory = CQ.Bot.AppDirectory.FullName;
            File.WriteAllText(AppDirectory + "Playerdata.json",JsonConvert.SerializeObject(db,Formatting.Indented));
        }
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
        public XBOXIDItem count { get; set; }
    }
    public class XBOXIDItem
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
    public class RequestResult
    {
        public RequestResult(bool f)
        {
            find = f;
        }
        public bool find { get; set; }
        public string id { get; set; }
        public long qq { get; set; }
        public XBOXIDItem playerdata { get; set; }
    }

}

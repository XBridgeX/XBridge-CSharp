using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using XBridge.Config;
using CQ = HuajiTech.CoolQ.CurrentPluginContext;

namespace XBridge.Utils
{
    class XDB
    {
        private string k;
        private string iv;
        private string path;
        private Dictionary<long, PlayerData> db;
        public XDB(string path,string pwd)
        {
            this.path = path;
            k = MD5.MD5Encrypt(pwd).Substring(0, 16);
            iv = MD5.MD5Encrypt(pwd).Substring(16);
            if (File.Exists(CQ.Bot.AppDirectory.FullName + "playerdata.json"))
            {
                db = JsonConvert.DeserializeObject<Dictionary<long, PlayerData>>(File.ReadAllText(CQ.Bot.AppDirectory.FullName + "playerdata.json"));
                SAVE();
                File.Delete(CQ.Bot.AppDirectory.FullName + "playerdata.json");
            }
            if (!File.Exists(path))
                File.WriteAllText(path, AES.AesEncrypt("{}", k, iv));
            try
            {
                db = JsonConvert.DeserializeObject<Dictionary<long, PlayerData>>(AES.AesDecrypt(File.ReadAllText(path),k,iv));
            }catch(Exception e)
            {
                Error.LogToFile("OPEN_XDB", e.ToString());
                db = new Dictionary<long, PlayerData>();
            }
        }
        public void add_time(int mode, string id, int data)
        {
            if (Data.id2qq(id) != 0)
            {
                long q = xboxid2QQ(id);
                switch (mode)
                {
                    case 0:
                        db[q].count.join += data;
                        SAVE();
                        break;
                    case 1:
                        db[q].count.death += data;
                        SAVE();
                        break;
                    case 2:
                        db[q].count.duration += data;
                        SAVE();
                        break;

                }
            }
        }
        public PlayerData get(long k)
        {
            return db[k];
        }
        public Dictionary<long,PlayerData> getAll()
        {
            return db;
        }
        public void SAVE()
        {
            File.WriteAllText(path, AES.AesEncrypt(JsonConvert.SerializeObject(db), k, iv));
        }
        public bool wl_exsis(long q)
        {
            return db.ContainsKey(q);
        }
        public bool wl_add(long q, string xboxid)
        {
            if (!db.ContainsKey(q))
            {
                var pl = new PlayerData() { xboxid = xboxid, count = new Count() };
                db.Add(q, pl);
                SAVE();
                return true;
            }
            return false;
        }
        public bool wl_remove(long q)
        {
            if (db.ContainsKey(q))
            {
                db.Remove(q);
                SAVE();
                return true;
            }
            return false;
        }
        public string getXboxid(long q)
        {
            foreach (var i in db)
            {
                if (i.Key == q)
                    return i.Value.xboxid;
            }
            return null;
        }
        public long xboxid2QQ(string id)
        {
            foreach (var i in db)
            {
                if (i.Value.xboxid == id)
                    return i.Key;
            }
            return 0;
        }
        public bool xboxidExsis(string id)
        {
            foreach (var i in db)
            {
                if (i.Value.xboxid == id)
                    return true;
            }
            return false;
        }
        public int CheckJoinTime(string xboxid)
        {
            if (xboxidExsis(xboxid))
            {
                return db[xboxid2QQ(xboxid)].count.join;
            }
            return 0;
        }
    }
}

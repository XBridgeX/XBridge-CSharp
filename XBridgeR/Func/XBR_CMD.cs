using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using XBridgeR.Data;
using XBridgeR.Utils;

namespace XBridgeR.Func
{
    public class XBR_CMD
    {
        public static Dictionary<string, MethodInfo> funcs = new Dictionary<string, MethodInfo>();
        public delegate void REGCMD(string cmd, Func<string[], string> fun);
        public static void RegisterCMD(string cmd, MethodInfo fun)
        {
            if (!funcs.ContainsKey(cmd))
            {
                funcs.Add(cmd, fun);
            }
        }
        public static XBCMD_RESULT runcode(string cmd)
        {
            var re = new XBCMD_RESULT(false);
            try
            {
                return runcodeItem(cmd);

            }catch(Exception e)
            {
                re.text = e.ToString();
                return re;
            }
        }
        private static XBCMD_RESULT runcodeItem(string cmd)
        {
            var parser = CmdParser.Parse(cmd.Split(' '));
            var re = new XBCMD_RESULT(false);
            re.text = "没有这个命令";
            switch (parser.command)
            {
                case "-wl":
                    var sult = wl(parser.@params.All.ToArray());
                    re.text = sult.text;
                    re.success = sult.success;
                    break;
                case "-reload":
                    try
                    {
                        TMP.init();
                        re.success = true;
                        re.text = "配置文件重载完毕";
                    }
                    catch(Exception e)
                    {
                        re.text = "配置文件重载失败\n"+ e.ToString();
                    }
                    break;
                default:
                    if (funcs.ContainsKey(parser.command))
                    {
                        try
                        {
                            re.text = (string)funcs[parser.command].Invoke(null,new object[1] { parser.@params.All.ToArray()});
                            re.success = true;
                        }
                        catch (Exception ex)
                        {
                            re.text = ex.ToString();
                        }
                    }
                    break;


            }
            return re;
        }
        public class XBCMD_RESULT
        {
            public XBCMD_RESULT(bool ifs)
            {
                this.success = ifs;
            }
            public string text { get; set; }
            public bool success { get; set; }
        }
        private static bool is_long(string a)
        {
            return long.TryParse(a, out var foo);
        }
        private static XBCMD_RESULT wl(string[] parser)
        {
            var re = new XBCMD_RESULT(false);
            switch (parser[0])
            {
                case "add":
                    if (is_long(parser[1]) == false)
                    {
                        re.text = $"无法将{parser[1]}转换为QQ号";
                        break;
                    }
                    XBOXID.Add(long.Parse(parser[1]), parser[2]);
                    re.success = true;
                    re.text = $"成功添加{parser[1]}({parser[2]})到白名单";
                    break;
                case "remove":
                    if (is_long(parser[1]) == false)
                    {
                        if (XBOXID.GetQQByXBOXID(parser[1]).find)
                        {
                            XBOXID.Remove(parser[1]);
                            re.success = true;
                            re.text = $"成功移除{parser[1]}";
                            break;
                        }
                        else
                        {
                            re.text = $"{parser[1]}既不是QQ号也不是xboxid";
                            break;
                        }
                    }
                    else
                    {
                        long q = long.Parse(parser[1]);
                        if (XBOXID.QQExists(q))
                        {
                            XBOXID.Remove(parser[1]);
                            re.success = true;
                            re.text = $"成功移除{parser[1]}";
                            break;
                        }
                        else
                        {
                            re.text = $"{parser[1]}没有登记过的白名单记录";
                            break;
                        }
                    }
                case "get":
                    if (is_long(parser[1]) == false)
                    {
                        if (XBOXID.GetQQByXBOXID(parser[1]).find)
                        {
                            re.success = true;
                            re.text = XBOXID.GetQQByXBOXID(parser[1]).qq.ToString();
                            break;
                        }
                        else
                        {
                            re.text = $"{parser[1]}既不是QQ号也不是登记过的xboxid";
                            break;
                        }
                    }
                    else
                    {
                        long q = long.Parse(parser[1]);
                        if (XBOXID.QQExists(q))
                        {
                            re.success = true;
                            re.text = XBOXID.GetXboxIDByQQ(q);
                            break;
                        }
                        else
                        {
                            re.text = $"{parser[1]}没有登记过的白名单记录";
                            break;
                        }
                    }
                default:
                    re.text = "未知的参数";
                    break;
            }
            return re;
        }
    }
    public class CmdParser
    {
        public Command @params;

        static public CmdParser Parse(string[] args)
        {
            return new CmdParser(args.ToList());
        }

        static public CmdParser Parse(List<string> args)
        {
            return new CmdParser(args);
        }

        private CmdParser(List<string> args)
        {
            if (args.Count <= 0) return;
            for (int keyIndex = 0; keyIndex < args.Count; keyIndex++)
            {
                var key = args[keyIndex];
                if (!key.StartsWith("-")) continue;
                List<string> values = new List<string>();
                for (int valueIndex = keyIndex + 1; valueIndex < args.Count; valueIndex++)
                {
                    if (args[valueIndex].StartsWith("-")) break;
                    values.Add(args[valueIndex]);
                }
                Command cmd = new Command(values);
                command = key;
                @params = cmd;
            }
        }

        public bool Has(string key)
        {
            foreach (string i in @params.All)
            {
                if (i == key)
                    return true;
            }
            return false;
        }

        public string this[int index]
        {
            get
            {
                return @params.All[index];
            }
        }
        public int lenght
        {
            get { return @params.All.Count; }
        }
        public string command = string.Empty;
    }
    public class Command
    {
        private List<string> mCmds;
        private int mIndex = 0;

        public Command(List<string> cmds)
        {
            this.mCmds = cmds;
        }

        public string First
        {
            get
            {
                mIndex = 0;
                return mCmds[mIndex];
            }
        }

        public string Last
        {
            get
            {
                mIndex = mCmds.Count - 1;
                return mCmds[mIndex];
            }
        }

        public string Get(int index)
        {
            if (index < mCmds.Count)
            {
                mIndex = index;
                return mCmds[mIndex];
            }
            return null;
        }

        public bool HasNext
        {
            get
            {
                return mIndex < mCmds.Count - 1;
            }
        }

        public string Next
        {
            get
            {
                if (HasNext)
                    return mCmds[mIndex++];
                return null;
            }
        }

        public List<string> All
        {
            get
            {
                return mCmds;
            }
        }
    }
}

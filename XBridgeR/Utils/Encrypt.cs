﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XBridgeR.Data;
namespace XBridgeR.Utils
{
    class Encrypt
    {
        public static string Encrypted(string pack, string k, string iv)
        {
            if (!TMP.Encrypt)
            {
                return pack;
            }
            var p = new
            {
                type = "encrypted",
                @params = new
                {
                    mode = "aes_cbc_pck7padding",
                    raw = AES.AesEncrypt(pack, k, iv)
                }
            };
            return JsonConvert.SerializeObject(p);
        }
    }
}

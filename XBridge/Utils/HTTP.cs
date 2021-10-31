using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace HttpServer
{
    public class Http
    {
        public static Dictionary<String, HttpListener> https = new Dictionary<string, HttpListener>();
        private HttpListener http = new HttpListener();
        private Func<HttpListenerRequest,string> get { get; set; }
        private Func<HttpListenerRequest, string> post { get; set; }
        public string uid { get; set; }
        public Http(string ip, Func<HttpListenerRequest,string> GET, Func<HttpListenerRequest,string> POST)
        {
            http.Prefixes.Add(ip);
            http.TimeoutManager.EntityBody = TimeSpan.FromSeconds(30);
            http.TimeoutManager.RequestQueue = TimeSpan.FromSeconds(30);
            http.Start();
            http.BeginGetContext(ContextReady, null);
            get = GET;
            post = POST;
            uid = Guid.NewGuid().ToString();
        }
        private void ContextReady(IAsyncResult ar)
        {
            http.BeginGetContext(ContextReady, null);
            AcceptAsync(http.EndGetContext(ar));
        }
        private void AcceptAsync(HttpListenerContext context)
        {
            try
            {
                HttpListenerRequest request = context.Request;
                HttpListenerResponse response = context.Response;
                response.ContentEncoding = Encoding.UTF8;
                response.ContentType = "charset=UTF-8;application/json";
                string re = string.Empty;
                int status = 200;
                switch (request.HttpMethod)
                {
                    case "GET":
                        var luaret = get.Invoke(request);
                        re = luaret ?? "{\"code\":404}";
                        break;
                    case "POST":
                        var luaret1 = post.Invoke(request);
                        re = luaret1 ?? "{\"code\":404}"; ;
                        break;
                }
                context.Response.StatusCode = status;
                var data = Encoding.UTF8.GetBytes(re);
                System.IO.Stream output = response.OutputStream;
                output.Write(data, 0, data.Length);
                response.StatusCode = 200;
                output.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        public static bool stopHttpListner(string uid)
        {
            if (https.ContainsKey(uid))
            {
                https[uid].Stop();
                return true;
            }
            return false;
        }
    }
}

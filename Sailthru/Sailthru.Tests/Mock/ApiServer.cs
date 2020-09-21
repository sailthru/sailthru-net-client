using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Specialized;

namespace Sailthru.Tests.Mock
{
    public class ApiServer
    {
        private const String API_URL = "http://localhost:5555";

        private readonly HttpListener listener;
        private readonly Thread listenerThread;

        public ApiServer()
        {
            if (!HttpListener.IsSupported)
            {
                throw new NotSupportedException("HttpListener is not supported on this platform");
            }

            listener = new HttpListener();
            listener.Prefixes.Add(API_URL + "/");
            listener.Start();

            listenerThread = new Thread(HandleRequests);
            listenerThread.IsBackground = true;
            listenerThread.Start();
        }

        public String ApiUrl
        {
            get
            {
                return API_URL;
            }
        }

        private void HandleRequests()
        {
            while (listener.IsListening)
            {
                ThreadPool.QueueUserWorkItem(ProcessRequest, listener.GetContext());
            }
        }

        private void ProcessRequest(object ctxObject)
        {
            HttpListenerContext context = ctxObject as HttpListenerContext;

            int statusCode = 200;
            object response = null;

            try
            {
                response = ProcessRequestInternal(context);
            }
            catch (FileNotFoundException)
            {
                statusCode = 404;
                response = new ErrorResponse(99, "404 Not Found");
            }
            catch (ArgumentException ex)
            {
                statusCode = 400;
                response = new ErrorResponse(99, ex.Message);
            }
            catch (Exception ex)
            {
                statusCode = 500;
                response = new ErrorResponse(9, "Internal Exception");

                Console.WriteLine("Exception Occurred: " + ex);
            }

            if (response != null)
            {
                context.Response.StatusCode = statusCode;
                context.Response.AddHeader("Content-Type", "application/json");
                using (StreamWriter writer = new StreamWriter(context.Response.OutputStream))
                {
                    writer.Write(JsonConvert.SerializeObject(response));
                }
            }

            context.Response.Close();
        }

        private object ProcessRequestInternal(HttpListenerContext context)
        {
            string method = context.Request.HttpMethod;
            string path = context.Request.Url.AbsolutePath;

            JObject requestBody = DecodeRequestObject(context.Request);

            if (method == "POST" && path == "/purchase")
            {
                return PurchaseApi.ProcessPost(requestBody);
            }
            else if (method == "GET" && path == "/list")
            {
                return ListApi.ProcessGet(requestBody);
            }
            else if (method == "POST" && path == "/job")
            {
                return JobApi.ProcessPost(requestBody);
            }
            else if (method == "POST" && path == "/event")
            {
                return EventApi.ProcessPost(requestBody);
            }
            else if (method == "POST" && path == "/user")
            {
                return UserApi.ProcessPost(requestBody);
            }
            else
            {
                throw new FileNotFoundException();
            }
        }

        private JObject DecodeRequestObject(HttpListenerRequest request)
        {
            NameValueCollection dict;
            if (request.HasEntityBody)
            {
                dict = DecodeRequest(request);
            }
            else if (request.QueryString.Count != 0)
            {
                dict = request.QueryString;
            }
            else
            {
                return null;
            }

            if (dict["format"] != "json")
            {
                throw new NotSupportedException("only json format is supported");
            }

            if (dict["json"] == null || dict["api_key"] == null || dict["sig"] == null)
            {
                throw new NotSupportedException("required field is missing");
            }

            return JsonConvert.DeserializeObject(dict["json"]) as JObject;
        }

        private NameValueCollection DecodeRequest(HttpListenerRequest request)
        {
            string body;
            using (StreamReader reader = new StreamReader(request.InputStream))
            {
                body = reader.ReadToEnd();
            }

            if (request.ContentType == "application/x-www-form-urlencoded")
            {
                return DecodeQueryString(body);
            }
            else if (request.ContentType.StartsWith("multipart/form-data"))
            {
                throw new NotSupportedException("form data not supported yet");
            }
            else
            {
                throw new NotSupportedException("unsupported content type");
            }
        }

        private NameValueCollection DecodeQueryString(string query)
        {
            NameValueCollection coll = new NameValueCollection();
            foreach (string entry in query.Split('&'))
            {
                string[] parts = entry.Split(new char[] { '=' }, 2);
                string key = WebUtility.UrlDecode(parts[0]);
                string value = WebUtility.UrlDecode(parts[1]);
                coll.Add(key, value);
            }
            return coll;
        }

        public void Close()
        {
            // If this is called, the test is probably over,
            // so force-destroy all pending requests.
            listener.Abort();
        }
    }
}

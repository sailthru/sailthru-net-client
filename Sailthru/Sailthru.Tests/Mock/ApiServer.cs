using System.Collections.Specialized;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Sailthru.Tests.Mock
{
    public class ApiServer
    {
        private const string API_URL = "http://localhost:5555";
        private static readonly JsonSerializerSettings s_jsonSettings = new() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii };

        private readonly HttpListener _listener;
        private readonly Thread _listenerThread;

        public ApiServer()
        {
            if (!HttpListener.IsSupported)
            {
                throw new NotSupportedException("HttpListener is not supported on this platform");
            }

            _listener = new HttpListener();
            _listener.Prefixes.Add(API_URL + "/");
            _listener.Start();

            _listenerThread = new Thread(HandleRequests)
            {
                IsBackground = true
            };
            _listenerThread.Start();
        }

        public string ApiUrl
        {
            get
            {
                return API_URL;
            }
        }

        private void HandleRequests()
        {
            while (_listener.IsListening)
            {
                ThreadPool.QueueUserWorkItem(ProcessRequest, _listener.GetContext());
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
            catch (ApiException ex)
            {
                statusCode = ex.StatusCode;
                response = ex.Response;
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
                using (StreamWriter writer = new(context.Response.OutputStream))
                {
                    writer.Write(JsonConvert.SerializeObject(response, s_jsonSettings));
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
            else if (method == "POST" && path == "/blast")
            {
                return BlastApi.ProcessPost(requestBody);
            }
            else if (method == "POST" && path == "/content")
            {
                return ContentApi.ProcessPost(requestBody);
            }
            else if (method == "GET" && path == "/content")
            {
                return ContentApi.ProcessGet(requestBody);
            }
            else if (method == "POST" && path == "/send")
            {
                return SendApi.ProcessPost(requestBody);
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

            if (dict["api_key"] == null || dict["sig"] == null)
            {
                throw new NotSupportedException("required field is missing");
            }

            if (dict["json"] != null)
            {
                return JsonConvert.DeserializeObject(dict["json"]) as JObject;
            }
            else
            {
                JObject obj = new();
                foreach (string key in dict)
                {
                    obj.Add(key, JValue.CreateString(dict[key]));
                }

                return obj;
            }
        }

        private static NameValueCollection DecodeRequest(HttpListenerRequest request)
        {
            string body;
            using (StreamReader reader = new(request.InputStream))
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

        private static NameValueCollection DecodeQueryString(string query)
        {
            NameValueCollection coll = new();
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
            // If this is called, the test is probably over, so force-destroy all pending requests.
            _listener.Abort();
        }
    }
}

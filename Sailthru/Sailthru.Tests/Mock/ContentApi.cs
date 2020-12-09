using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Sailthru.Tests.Mock
{
    public static class ContentApi
    {
        public static object ProcessPost(JObject request)
        {
            request = ConvertUrlFieldToUrlKey(request);

            string key = GetAndValidateKey(request);
            string id = request["id"].Value<string>();

            bool exist = id == "http://example.com/product";

            if (key != "url" && !exist)
            {
                throw new ApiException(99, "You may not insert a content without url");
            }

            Dictionary<string, object> content = new Dictionary<string, object>
            {
                ["url"] = id,
                ["title"] = request["title"],
                ["tags"] = request["tags"],
                ["description"] = request["description"],
                ["site_name"] = request["site_name"],
                ["vars"] = request["vars"],
                ["images"] = request["images"],
            };

            if (request["keys"] is JObject keys && keys.ContainsKey("sku"))
            {
                content["sku"] = keys["sku"];
            }

            string date = (string)request["date"];
            if (date != null)
            {
                content["date"] = DateTime.Parse(date).ToString("ddd, dd MMM yyyy HH:mm:ss zzz");
            }

            return new Dictionary<string, object>
            {
                ["content"] = new List<Dictionary<string, object>>
                {
                    content
                }
            };
        }

        private static string GetAndValidateKey(JObject request)
        {
            string key = request.ContainsKey("key") ? request.Value<string>("key") : null;
            string id = null;

            if (key == null)
            {
                if (!request.ContainsKey("id"))
                {
                    throw new ApiException(99, "Missing required parameter: id/url");
                }

                id = request.Value<string>("id");
                if (!Uri.TryCreate(id, UriKind.Absolute, out Uri uri))
                {
                    throw new ApiException(99, "Invalid Url: " + id);
                }

                key = "url";
            }
            else
            {
                if (key != "url" && key != "sku")
                {
                    throw new ApiException(99, "Content is not enabled for " + key + " lookup");
                }
            }

            return key;
        }

        private static JObject ConvertUrlFieldToUrlKey(JObject request)
        {
            if (request.ContainsKey("url"))
            {
                request["key"] = "url";
                request["id"] = request["url"];
                request.Remove("url");
            }
            return request;
        }
    }
}

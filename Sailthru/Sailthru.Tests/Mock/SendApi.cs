using System;
using System.Collections.Generic;
using MongoDB.Bson;
using Newtonsoft.Json.Linq;

namespace Sailthru.Tests.Mock
{
    public static class SendApi
    {
        public static object ProcessPost(JObject request)
        {
            ObjectId sendId = ObjectId.GenerateNewId();
            string email = request["email"].Value<string>();
            string template = request["template"].Value<string>();

            if (template != "Sandbox Image Template")
            {
                return new ErrorResponse(14, "Unknown template: " + template);
            }

            return new Dictionary<string, object>
            {
                ["send_id"] = ToBase64StringUrl(sendId.ToByteArray()),
                ["email"] = email,
                ["template"] = template,
                ["status"] = "unknown",
                ["send_key"] = null
            };
        }

        private static string ToBase64StringUrl(byte[] inArray)
        {
            return Convert.ToBase64String(inArray)
                        .Replace('+', '-')
                        .Replace('/', '_')
                        .Replace('=', ',');
        }
    }
}

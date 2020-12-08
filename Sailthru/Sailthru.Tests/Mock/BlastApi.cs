using System;
using System.Collections.Generic;
using System.Threading;
using Newtonsoft.Json.Linq;

namespace Sailthru.Tests.Mock
{
    public static class BlastApi
    {
        private static int currentId = 1000;

        public static object ProcessPost(JObject request)
        {
            int blastId = Interlocked.Increment(ref currentId);
            string name = request["name"].Value<string>();
            string subject = request["subject"].Value<string>();
            string list = request["list"].Value<string>();
            string modifyTime = DateTime.Now.ToLocalTime().ToString("ddd, dd MMM yyyy HH:mm:ss zzz");

            return new Dictionary<string, object>
            {
                ["blast_id"] = blastId,
                ["name"] = name,
                ["subject"] = subject,
                ["list"] = list,
                ["modify_time"] = modifyTime,
                ["status"] = "created"
            };
        }
    }
}

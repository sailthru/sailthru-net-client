using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Sailthru.Tests.Mock
{
    public class ListApi
    {
        public static object ProcessGet(JObject request)
        {
            return new Dictionary<string, object>()
            {
                ["list_id"] = "56f17d1eade9c272368b458b",
                ["list"] = request["list"].Value<string>(),
                ["create_time"] = "Tue, 22 Mar 2016 13:13:02 -0400",
                ["count"] = 1,
                ["type"] = "normal",
                ["primary"] = false,
            };
        }
    }
}
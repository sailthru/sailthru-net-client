using Newtonsoft.Json.Linq;

namespace Sailthru.Tests.Mock
{
    public class EventApi
    {
        public static object ProcessPost(JObject request)
        {
            string? id = request["id"]?.Value<string>();
            string? eventName = request["event"]?.Value<string>();
            if (id == null)
            {
                throw new ArgumentException("id is required");
            }
            else if (eventName == null)
            {
                throw new ArgumentException("event is required");
            }

            if (eventName == "trigger_timeout")
            {
                Thread.Sleep(10000);
            }

            return new Dictionary<string, object>()
            {
                ["ok"] = true
            };
        }
    }
}

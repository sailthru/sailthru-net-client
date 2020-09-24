using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Sailthru.Tests.Mock
{
    public class UserApi
    {
        public static object ProcessPost(JObject request)
        {
            string id = (string)request["id"];
            string optout = (string)request["optout_email"];
            JObject vars = (JObject)request["vars"];
            JObject fields = (JObject)request["fields"];
            bool returnVars = fields != null && fields.ContainsKey("vars") && fields["vars"].Value<int>() == 1;
            bool returnOptout = fields != null && fields.ContainsKey("optout_email") && fields["optout_email"].Value<int>() == 1;
            Dictionary<string, object> result = new Dictionary<string, object>()
            {
                ["ok"] = true
            };
            if (returnVars)
            {
                if (vars == null)
                {
                    vars = new JObject();
                }
                result.Add("vars", vars);
            }
            if (returnOptout)
            {
                if (optout == null)
                {
                    optout = "none";
                }
                result.Add("optout_email", optout);
            }
            return result;
        }
    }
}
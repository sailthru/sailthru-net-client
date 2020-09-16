using System;
using System.Collections.Generic;
using MongoDB.Bson;
using Newtonsoft.Json.Linq;

namespace Sailthru.Tests.Mock
{
    public class JobApi
    {
        public static object ProcessPost(JObject request)
        {
            ObjectId jobId = ObjectId.GenerateNewId();
            string jobType = request["job"].Value<string>();

            if (jobType == "import")
            {
                string listName = request["list"].Value<string>();

                if (listName == null)
                {
                    throw new ArgumentException("list name is missing");
                }

                return new Dictionary<string, object>()
                {
                    ["job_id"] = jobId.ToString(),
                    ["name"] = "List Import: " + request["list"],
                    ["list"] = request["list"],
                    ["status"] = "pending"
                };
            }
            else if (jobType == "list_erase")
            {
                JArray lists = request["lists"] as JArray;

                return new Dictionary<string, object>()
                {
                    ["job_id"] = jobId.ToString(),
                    ["name"] = "Bulk List Erase: " + lists.Count + " requested",
                    ["status"] = "pending"
                };
            }
            else
            {
                throw new NotSupportedException("unsupported job type: " + request["job"]);
            }
        }
    }
}
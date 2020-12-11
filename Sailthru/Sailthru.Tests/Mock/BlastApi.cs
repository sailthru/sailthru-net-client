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
            string linkDomain = (string)request["link_domain"];
            string status = (string)request["status"];

            if (status == null)
            {
                if (request.ContainsKey("schedule_time"))
                {
                    status = "scheduled";
                }
                else
                {
                    status = "created";
                }
            }
            else
            {
                if (status == "scheduled")
                {
                    if (!request.ContainsKey("schedule_time"))
                    {
                        throw new ApiException(2, "Missing required field: schedule_time for blast", 200);
                    }
                }
                else
                {
                    status = "created";
                }
            }


            Dictionary<string, object> blast = new Dictionary<string, object>
            {
                ["blast_id"] = blastId,
                ["name"] = name,
                ["subject"] = subject,
                ["list"] = list,
                ["modify_time"] = modifyTime,
                ["link_domain"] = linkDomain,
                ["status"] = status
            };

            if (request.ContainsKey("previous_blast_id"))
            {
                int previousBlastId = request["previous_blast_id"].Value<int>();
                if (previousBlastId < 0)
                {
                    throw new ApiException(99, "previous_blast_id must be a positive integer", 400);
                }

                if (previousBlastId < 1001 || previousBlastId >= blastId)
                {
                    throw new ApiException(99, "No blast found with id: " + previousBlastId, 400);
                }

                blast["previous_blast_id"] = previousBlastId;
            }
            if (request.ContainsKey("message_criteria"))
            {
                string messageCriteria = request["message_criteria"].Value<string>();
                if (string.IsNullOrEmpty(messageCriteria))
                {
                    throw new ApiException(99, "invalid message criteria", 400);
                }
                blast["message_criteria"] = messageCriteria;
            }

            return blast;
        }
    }
}

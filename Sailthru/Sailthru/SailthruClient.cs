using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Newtonsoft.Json;
using Sailthru.Models;

namespace Sailthru
{
    public class SailthruClient
    {
        private static readonly string s_defaultAPIUrl = "https://api.sailthru.com";
        private static readonly OrdinalComparer s_orginalComparer = new OrdinalComparer();
        private static readonly string s_userAgent = "Sailthru API C# Client " + Assembly.GetExecutingAssembly().GetName().Version;
        private readonly string _apiHost;
        private readonly string _apiKey;
        private readonly Hashtable _lastRateLimitInfo;
        private readonly string _secret;

        /// <summary>
        /// Constructor with default API URI
        /// </summary>
        /// <param name="apiKey"></param>
        /// <param name="secret"></param>
        public SailthruClient(string apiKey, string secret)
            : this(apiKey, secret, s_defaultAPIUrl)
        {
        }

        /// <summary>
        /// Constructor with custom API URI
        /// </summary>
        /// <param name="apiKey"></param>
        /// <param name="secret"></param>
        /// <param name="apiHost"></param>
        public SailthruClient(string apiKey, string secret, string apiHost)
        {
            _apiHost = apiHost;
            _apiKey = apiKey;
            _secret = secret;
            _lastRateLimitInfo = new Hashtable();
        }

        /// <summary>
        /// Set/get the timeout for writing or reading from the connection. The default value is
        /// 300,000 milliseconds (5 minutes). <seealso cref="HttpWebRequest.ReadWriteTimeout"/>
        /// </summary>
        /// <value>The ReadWriteTimeout.</value>
        public int ReadWriteTimeout { get; set; } = 300000;

        /// <summary>
        /// Gets or sets the number of milliseconds to wait before the request times out. The
        /// default value is 100,000 milliseconds (100 seconds). <seealso cref="HttpWebRequest.Timeout"/>
        /// </summary>
        /// <value>The Timeout.</value>
        public int Timeout { get; set; } = 100000;

        /// <summary>
        /// For making API DELETE Request
        /// </summary>
        /// <param name="action"></param>
        /// <param name="parameters"></param>
        /// <returns>SailthruResponse Object</returns>
        public SailthruResponse ApiDelete(string action, Hashtable parameters)
        {
            AddAuthenticationAndFormatToParams(parameters);
            HttpWebRequest request = BuildRequest("DELETE", action, parameters);
            return SendRequest(request, action);
        }

        /// For custom API calls that wrappers above don't cover, you can use the below:
        /// <summary>
        /// For making API GET Request
        /// </summary>
        /// <param name="action">API Method String</param>
        /// <param name="parameters">API Parameter Hashtable</param>
        /// <returns>SailthruResponse Object</returns>
        public SailthruResponse ApiGet(string action, Hashtable parameters)
        {
            AddAuthenticationAndFormatToParams(parameters);
            HttpWebRequest request = BuildRequest("GET", action, parameters);
            return SendRequest(request, action);
        }

        /// <summary>
        /// For making API POST Request
        /// </summary>
        /// <param name="action"></param>
        /// <param name="parameters"></param>
        /// <returns>SailthruResponse Object</returns>
        public SailthruResponse ApiPost(string action, Hashtable parameters)
        {
            AddAuthenticationAndFormatToParams(parameters);
            HttpWebRequest request = BuildPostRequest(action, parameters);
            return SendRequest(request, action);
        }

        /// <summary>
        /// cancel a future send before it goes out.
        /// </summary>
        /// <param name="sendId"></param>
        /// <returns></returns>
        /// <seealso cref="http://docs.sailthru.com/api/send"/>
        public SailthruResponse CancelSend(string sendId)
        {
            Hashtable parameters = new Hashtable
            {
                ["send_id"] = sendId
            };
            return ApiDelete("send", parameters);
        }

        /// <summary>
        /// Get Blast
        /// </summary>
        /// <param name="strBlastId"></param>
        /// <seealso cref="http://docs.sailthru.com/api/blast"/>
        /// <returns></returns>
        public SailthruResponse GetBlast(string blastId)
        {
            Hashtable parameters = new Hashtable
            {
                ["blast_id"] = blastId
            };
            return ApiGet("blast", parameters);
        }

        /// <summary>
        /// Gets the blast by status.
        /// </summary>
        /// <param name="statusType">Type of the status.</param>
        /// <seealso cref="http://docs.sailthru.com/api/blast"/>
        /// <returns>Sailthru Response Object</returns>
        public SailthruResponse GetBlastByStatus(BlastRequest.StatusType statusType)
        {
            Hashtable parameters = new Hashtable
            {
                // Ensure the status type is in lowercase as required by the API
                ["status"] = statusType.ToString().ToLower()
            };
            return ApiGet("blast", parameters);
        }

        /// <summary>
        /// Get information about one of your urls.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <seealso cref="http://docs.sailthru.com/api/content"/>
        public SailthruResponse GetContent(string url)
        {
            Hashtable hashForPost = new Hashtable
            {
                { "url", url }
            };
            return ApiGet("content", hashForPost);
        }

        /// <summary>
        /// Gets the content Sku.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="sku">The sku.</param>
        /// <returns>Sailthru Response Object</returns>
        public SailthruResponse GetContent(string id, string sku)
        {
            Hashtable hashForPost = new Hashtable
            {
                { "id", id },
                { "key", sku }
            };

            return ApiGet("content", hashForPost);
        }

        /// <summary>
        /// Gets the most recent pieces of content.
        /// </summary>
        /// <returns>The contents.</returns>
        /// <param name="items">Number of items to return.</param>
        public SailthruResponse GetContents(int items)
        {
            Hashtable hashForPost = new Hashtable
            {
                { "items", items }
            };
            return ApiGet("content", hashForPost);
        }

        /// <summary>
        /// Get rate limit information for last API call
        /// </summary>
        /// <param name="action">API endpoint</param>
        /// <param name="method">HTTP method.</param>
        /// <returns>Hashtable|null</returns>
        public Hashtable GetLastRateLimitInfo(string action, string method)
        {
            if (_lastRateLimitInfo.ContainsKey(action))
            {
                Hashtable rateLimitPerMethod = (Hashtable)_lastRateLimitInfo[action];
                string methodUC = method.ToUpper();
                if (rateLimitPerMethod.ContainsKey(methodUC))
                {
                    return (Hashtable)rateLimitPerMethod[methodUC];
                }
            }

            return null;
        }

        /// <summary>
        /// Get rate limit information for last API call
        /// </summary>
        /// <param name="action">API endpoint</param>
        /// <param name="method">HTTP method</param>
        /// <returns>Hashtable|null</returns>
        [Obsolete("Use GetLastRateLimitInfo")]
        public Hashtable getLastRateLimitInfo(string action, string method)
        {
            return GetLastRateLimitInfo(action, method);
        }

        /// <summary>
        /// Gets the list by ID.
        /// </summary>
        /// <param name="listId">The list identifier.</param>
        /// <returns>Sailthru Response Object</returns>
        public SailthruResponse GetList(string listId)
        {
            Hashtable parameters = new Hashtable
            {
                ["list_id"] = listId
            };

            return ApiGet("list", parameters);
        }

        /// <summary>
        /// Gets the list by Name.
        /// </summary>
        /// <param name="listName">Name of the list.</param>
        /// <returns>Sailthru Response Object</returns>
        public SailthruResponse GetListByName(string listName)
        {
            Hashtable parameters = new Hashtable
            {
                ["list"] = listName
            };

            return ApiGet("list", parameters);
        }

        /// <summary>
        /// check on the status of a send
        /// </summary>
        /// <param name="sendId"></param>
        /// <returns></returns>
        /// <seealso cref="http://docs.sailthru.com/api/send"/>
        public SailthruResponse GetSend(string sendId)
        {
            Hashtable parameters = new Hashtable
            {
                ["send_id"] = sendId
            };
            return ApiGet("send", parameters);
        }

        /// <summary>
        /// Request various stats from Sailthru.
        /// </summary>
        /// <param name="stat"></param>
        /// <param name="list"></param>
        /// <param name="date"></param>
        /// <param name="htOptions"></param>
        /// <returns></returns>
        /// <seealso cref="http://docs.sailthru.com/api/stats"/>
        public SailthruResponse GetStat(string stat, string list = null, string date = null, Hashtable htOptions = null)
        {
            Hashtable parameters = new Hashtable
            {
                ["stat"] = stat
            };

            parameters["list"] = list ?? parameters["list"];
            parameters["date"] = date ?? parameters["date"];

            if (htOptions != null)
            {
                foreach (DictionaryEntry entry in htOptions)
                {
                    parameters[entry.Key.ToString()] = entry.Value.ToString();
                }
            }

            return ApiGet("stats", parameters);
        }

        /// <summary>
        /// Request various stats from Sailthru.
        /// </summary>
        /// <param name="stat"></param>
        /// <param name="template"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="htOptions"></param>
        /// <returns></returns>
        /// <seealso cref="http://docs.sailthru.com/api/stats"/>
        public SailthruResponse GetStat(string stat, string template = null, string startDate = null, string endDate = null, Hashtable htOptions = null)
        {
            Hashtable parameters = new Hashtable
            {
                ["stat"] = stat
            };

            parameters["template"] = template ?? parameters["template"];
            parameters["start_date"] = startDate ?? parameters["start_date"];
            parameters["end_date"] = endDate ?? parameters["end_date"];

            if (htOptions != null)
            {
                foreach (DictionaryEntry entry in htOptions)
                {
                    parameters[entry.Key.ToString()] = entry.Value.ToString();
                }
            }

            return ApiGet("stats", parameters);
        }

        /// <summary>
        /// Request various stats from Sailthru.
        /// </summary>
        /// <param name="stat"></param>
        /// <param name="htOptions"></param>
        /// <returns></returns>
        /// <seealso cref="http://docs.sailthru.com/api/stats"/>
        public SailthruResponse GetStat(string stat, Hashtable htOptions)
        {
            Hashtable parameters = new Hashtable
            {
                ["stat"] = stat
            };

            if (htOptions != null)
            {
                foreach (DictionaryEntry entry in htOptions)
                {
                    parameters[entry.Key.ToString()] = entry.Value.ToString();
                }
            }

            return ApiGet("stats", parameters);
        }

        /// <summary>
        /// Get Template
        /// </summary>
        /// <param name="strTemplateName"></param>
        /// <seealso cref="http://docs.sailthru.com/api/template"/>
        /// <returns></returns>
        public SailthruResponse GetTemplate(string templateName)
        {
            Hashtable parameters = new Hashtable
            {
                ["template"] = templateName
            };

            return ApiGet("template", parameters);
        }

        /// <summary>
        /// Gets the template.
        /// </summary>
        /// <param name="templateName">Name of the template.</param>
        /// <param name="revisionId">The revision identifier.</param>
        /// <returns>Sailthru Response Object</returns>
        public SailthruResponse GetTemplate(string templateName, int revision)
        {
            Hashtable parameters = new Hashtable
            {
                ["template"] = templateName,
                ["revision"] = revision
            };

            return ApiGet("template", parameters);
        }

        /// <summary>
        /// Get information about one of your users. Users are referenced by multiple keys.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <seealso cref="http://docs.sailthru.com/api/user"/>
        public SailthruResponse GetUser(UserRequest request)
        {
            Hashtable hashForPost = new Hashtable
            {
                { "json", JsonConvert.SerializeObject(request, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }) }  // If null is not ignored, user API call doesn't seem to work which is strange
            };

            return ApiGet("user", hashForPost);
        }

        /// <summary>
        /// Fetch email contacts from an address book at one of the major email providers (aol/gmail/hotmail/yahoo)
        /// </summary>
        /// <param name="strEmail">Email String</param>
        /// <param name="strPassword">Password String</param>
        /// <param name="boolIncludeNames">Boolean</param>
        /// <seealso cref="http://docs.sailthru.com/api/template"/>
        /// <returns>SailthruResponse Object</returns>
        public SailthruResponse ImportContacts(string strEmail, string strPassword, bool boolIncludeNames)
        {
            Hashtable hashForPost = new Hashtable
            {
                { "email", strEmail },
                { "password", strPassword }
            };

            if (boolIncludeNames)
            {
                hashForPost.Add("names", "1");
            }

            return ApiPost("contacts", hashForPost);
        }

        /// <summary>
        /// Send a transactional email for multiple users
        /// </summary>
        /// <remarks>
        /// Note that the <paramref name="htOptions"/> parameter does not allow passing email
        /// headers. To specify email headers, use the <see cref="Send"/> method which takes a <see
        /// cref="SendRequest"/> argument instead. Note that the <paramref name="htVars"/> parameter
        /// does not support hashtables or arrays, use the <see cref="Send"/> method which takes a
        /// <see cref="SendRequest"/> argument instead.
        /// </remarks>
        /// <param name="strTemplateName"></param>
        /// <param name="strEmail"></param>
        /// <param name="htVars">Vars to use for the send. Does not support hashtables or arrays</param>
        /// <param name="htOptions">Options to use for the send. Does not support email headers</param>
        /// <returns></returns>
        /// <seealso cref="http://docs.sailthru.com/api/send"/>
        public SailthruResponse Multisend(string strTemplateName, string[] strEmail, Hashtable htVars = null, Hashtable htOptions = null)
        {
            Hashtable hashForPost = new Hashtable();
            if (htVars != null)
            {
                foreach (DictionaryEntry entry in htVars)
                {
                    hashForPost.Add("vars[" + entry.Key + "]", entry.Value.ToString());
                }
            }

            if (htOptions != null)
            {
                foreach (DictionaryEntry entry in htOptions)
                {
                    hashForPost.Add("options[" + entry.Key + "]", entry.Value.ToString());
                }
            }

            hashForPost.Add("template", strTemplateName);
            hashForPost.Add("email", string.Join(",", strEmail));

            return ApiPost("send", hashForPost);
        }

        /// <summary>
        /// Notify Sailthru of an event.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <seealso cref="http://docs.sailthru.com/api/event"/>
        public SailthruResponse PostEvent(EventRequest request)
        {
            Hashtable hashForPost = new Hashtable
            {
                { "json", JsonConvert.SerializeObject(request, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }) }
            };

            return ApiPost("event", hashForPost);
        }

        /// <summary>
        /// Preview Email
        /// </summary>
        /// <param name="request">Preview Request parameters.</param>
        /// <seealso cref="http://docs.sailthru.com/api/preview"/>
        /// <returns>Sailthru Response Object</returns>
        public SailthruResponse PreviewEmail(PreviewRequest request)
        {
            Hashtable hashForPost = new Hashtable
            {
                { "json", JsonConvert.SerializeObject(request, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }) }
            };

            return ApiPost("preview", hashForPost);
        }

        public SailthruResponse ProcessImportJob(string listName, List<string> emails)
        {
            return ProcessImportJob(null, null, listName, emails);
        }

        public SailthruResponse ProcessImportJob(string reportEmail, string postbackUrl, string listName, List<string> emails)
        {
            Hashtable htForPost = new Hashtable
            {
                ["list"] = listName,
                ["emails"] = string.Join(",", emails)
            };

            return ProcessJob("import", reportEmail, postbackUrl, htForPost);
        }

        public SailthruResponse ProcessImportJob(string listName, string filePath)
        {
            return ProcessImportJob(null, null, listName, filePath);
        }

        public SailthruResponse ProcessImportJob(string reportEmail, string postbackUrl, string listName, string filePath)
        {
            Hashtable htForPost = new Hashtable
            {
                ["list"] = listName,
                ["file"] = filePath
            };

            return ProcessJob("import", reportEmail, postbackUrl, htForPost);
        }

        /// <summary>
        /// Start a background job, such as a data import or export.
        /// </summary>
        /// <example>
        /// E.g. update job:
        /// <code>
        ///Hashtable updateParams = new Hashtable();
        ///updateParams.Add("file", "/directory/update.json");
        ///ProcessJob("update", null, null, updateParams);
        /// </code>
        /// </example>
        /// <param name="jobType">See documentation on Job Types: https://getstarted.sailthru.com/developers/api/job/</param>
        /// <param name="reportEmail">when job is done, send a report to the email</param>
        /// <param name="postbackUrl">See documentation: https://getstarted.sailthru.com/developers/api-basics/postbacks/</param>
        /// <param name="parameters">use key "file" to pass the JSON file to process</param>
        public SailthruResponse ProcessJob(string jobType, string reportEmail, string postbackUrl, Hashtable parameters)
        {
            parameters["job"] = jobType;
            parameters["report_email"] = reportEmail ?? parameters["report_email"];
            parameters["postback_url"] = postbackUrl ?? parameters["postback_url"];

            if (parameters.ContainsKey("file"))
            {
                string filePath = (string)parameters["file"];
                parameters.Remove("file");

                Hashtable hashForPost = new Hashtable
                {
                    { "json", JsonConvert.SerializeObject(parameters, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }) }
                };

                return ApiPostWithFile("job", hashForPost, filePath);
            }
            else
            {
                Hashtable hashForPost = new Hashtable
                {
                    { "json", JsonConvert.SerializeObject(parameters, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }) }
                };

                return ApiPost("job", hashForPost);
            }
        }

        /// <summary>
        /// Submit a Purchase to Sailthru
        /// </summary>
        /// <param name="request">Purchaserequest parameters.</param>
        /// <returns></returns>
        public SailthruResponse Purchase(PurchaseRequest request)
        {
            Hashtable hashForPost = new Hashtable
            {
                { "json", JsonConvert.SerializeObject(request, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }) }
            };

            return ApiPost("purchase", hashForPost);
        }

        /// <summary>
        /// Receive the output of a Post.
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public bool ReceiveOptoutPost(NameValueCollection parameters)
        {
            List<string> requiredParams = new List<string> { "action", "email", "sig" };
            foreach (string key in requiredParams)
            {
                if (!parameters.AllKeys.Contains(key))
                {
                    return false;
                }
            }

            if (parameters.Get("email") == null || parameters.Get("optout") == null)
            {
                return false;
            }

            string providedSignatureHash = parameters["sig"];
            parameters.Remove("sig");

            if (providedSignatureHash != GetSignatureHash(parameters))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Receive and verify the output of a Post.
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public bool ReceiveVerifyPost(NameValueCollection parameters)
        {
            List<string> requiredParams = new List<string> { "action", "email", "send_id", "sig" };
            foreach (string key in parameters.Keys)
            {
                if (!requiredParams.Contains(key))
                {
                    return false;
                }
            }

            if (parameters.Get("action") != "verify" && parameters.Get("send_id") != null)
            {
                return false;
            }

            //check signature of request against parameter data
            string providedSignature = parameters["sig"];
            parameters.Remove("sig");
            if (providedSignature != GetSignatureHash(parameters))
            {
                return false;
            }

            SailthruResponse response = GetSend(parameters["send_id"]);
            Hashtable hash = response.HashtableResponse;
            if (hash.ContainsKey("email"))
            {
                return (string)hash["email"] == parameters["email"];
            }

            return false;
        }

        /// <summary>
        /// Save List
        /// </summary>
        /// <param name="request">List Request parameters.</param>
        /// <seealso cref="https://getstarted.sailthru.com/developers/api/list/"/>
        /// <returns>Sailthru Response Object</returns>
        public SailthruResponse SaveList(ListRequest request)
        {
            Hashtable hashForPost = new Hashtable
            {
                { "json", JsonConvert.SerializeObject(request, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }) }
            };

            return ApiPost("list", hashForPost);
        }

        /// <summary>
        /// Save Template
        /// </summary>
        /// <param name="strTemplateName"></param>
        /// <param name="fields"></param>
        /// <seealso cref="http://docs.sailthru.com/api/template"/>
        /// <returns></returns>
        public SailthruResponse SaveTemplate(string strTemplateName, Hashtable fields = null)
        {
            if (fields == null)
            {
                fields = new Hashtable();
            }

            fields.Add("template", strTemplateName);
            return ApiPost("template", fields);
        }

        /// <summary>
        /// Save Template
        /// </summary>
        /// <param name="request">TemplateRequest parameters.</param>
        /// <seealso cref="http://docs.sailthru.com/api/template"/>
        /// <returns></returns>
        public SailthruResponse SaveTemplate(TemplateRequest request)
        {
            Hashtable hashForPost = new Hashtable
            {
                { "json", JsonConvert.SerializeObject(request, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }) }
            };

            return ApiPost("template", hashForPost);
        }

        /// <summary>
        /// Create, update, and/or schedule a blast.
        /// </summary>
        /// <param name="strName"></param>
        /// <param name="strList"></param>
        /// <param name="strScheduleTime"></param>
        /// <param name="strFromName"></param>
        /// <param name="strFromEmail"></param>
        /// <param name="strSubject"></param>
        /// <param name="strContentHtml"></param>
        /// <param name="strContentText"></param>
        /// <param name="htOptions"></param>
        /// <seealso cref="http://docs.sailthru.com/api/blast"/>
        /// <returns></returns>
        public SailthruResponse ScheduleBlast(string strName, string strList, string strScheduleTime, string strFromName, string strFromEmail, string strSubject, string strContentHtml, string strContentText, Hashtable htOptions = null)
        {
            //if (htOptions == null) htOptions = new Hashtable();   //For Next release
            Hashtable hashForPost = new Hashtable();

            if (htOptions != null)
            {
                foreach (DictionaryEntry entry in htOptions)
                {
                    hashForPost.Add(entry.Key.ToString(), entry.Value.ToString());
                }
            }

            hashForPost.Add("name", strName);
            hashForPost.Add("list", strList);
            hashForPost.Add("schedule_time", strScheduleTime);
            hashForPost.Add("from_name", strFromName);
            hashForPost.Add("from_email", strFromEmail);
            hashForPost.Add("subject", strSubject);
            hashForPost.Add("content_html", strContentHtml);
            hashForPost.Add("content_text", strContentText);

            return ApiPost("blast", hashForPost);
        }

        /// <summary>
        /// Create, update, and/or schedule a blast.
        /// </summary>
        /// <param name="request">BlastRequest parameters.</param>
        /// <seealso cref="http://docs.sailthru.com/api/blast"/>
        /// <returns></returns>
        public SailthruResponse ScheduleBlast(BlastRequest request)
        {
            Hashtable hashForPost = new Hashtable
            {
                { "json", JsonConvert.SerializeObject(request, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }) }
            };

            return ApiPost("blast", hashForPost);
        }

        /// <summary>
        /// Send a transactional Email for a single user
        /// </summary>
        /// <remarks>
        /// Note that the <paramref name="htOptions"/> parameter does not allow passing email
        /// headers. To specify email headers, use the <see cref="Send"/> method which takes a <see
        /// cref="SendRequest"/> argument instead. Note that the <paramref name="htVars"/> parameter
        /// does not support hashtables or arrays, use the <see cref="Send"/> method which takes a
        /// <see cref="SendRequest"/> argument instead.
        /// </remarks>
        /// <param name="strTemplateName"></param>
        /// <param name="strEmail"></param>
        /// <param name="htVars">Vars to use for the send. Does not support hashtables or arrays</param>
        /// <param name="htOptions">Options to use for the send. Does not support email headers</param>
        /// <returns></returns>
        /// <seealso cref="http://docs.sailthru.com/api/send"/>
        public SailthruResponse Send(string strTemplateName, string strEmail, Hashtable htVars = null, Hashtable htOptions = null)
        {
            return Multisend(strTemplateName, new string[] { strEmail }, htVars, htOptions);
        }

        /// <summary>
        /// Send a transactional Email for a single or multiple users.
        /// </summary>
        /// <param name="request">SendRequest parameters.</param>
        /// <returns></returns>
        public SailthruResponse Send(SendRequest request)
        {
            Hashtable hashForPost = new Hashtable
            {
                { "json", JsonConvert.SerializeObject(request, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }) }
            };

            return ApiPost("send", hashForPost);
        }

        /// <summary>
        /// Set information about one of your urls.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <seealso cref="http://docs.sailthru.com/api/content"/>
        public SailthruResponse SetContent(ContentRequest request)
        {
            Hashtable hashForPost = new Hashtable
            {
                { "json", JsonConvert.SerializeObject(request, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }) }
            };

            return ApiPost("content", hashForPost);
        }

        /// <summary>
        /// Set information about one of your users. Users are referenced by multiple keys.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <seealso cref="http://docs.sailthru.com/api/user"/>
        public SailthruResponse SetUser(UserRequest request)
        {
            Hashtable hashForPost = new Hashtable
            {
                { "json", JsonConvert.SerializeObject(request, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }) }
            };

            return ApiPost("user", hashForPost);
        }

        protected SailthruResponse ApiPostWithFile(string action, Hashtable htForPost, string filePath)
        {
            AddAuthenticationAndFormatToParams(htForPost);
            HttpWebRequest request = BuildPostWithFileRequest(action, htForPost, filePath);
            return SendRequest(request, action);
        }

        protected HttpWebRequest BuildPostRequest(string action, Hashtable parameters)
        {
            HttpWebRequest request = BuildRequest("POST", action);
            request.ContentType = "application/x-www-form-urlencoded";

            string bodyString = GetParameterString(parameters);
            byte[] body = Encoding.UTF8.GetBytes(bodyString);

            request.ContentLength = body.Length;

            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(body, 0, body.Length);
                requestStream.Close();
            }

            return request;
        }

        protected HttpWebRequest BuildPostWithFileRequest(string action, Hashtable parameters, string filePath)
        {
            // Prepare web request
            HttpWebRequest request = BuildRequest("POST", action);
            string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
            request.ContentType = "multipart/form-data; boundary=" + boundary;

            // Use to build post body
            StringBuilder bodyBuilder = new StringBuilder();

            // Add form fields
            foreach (string key in parameters.Keys)
            {
                bodyBuilder.AppendFormat("\r\n--{0}\r\n", boundary);
                bodyBuilder.AppendFormat("Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}",
                                          key, parameters[key].ToString());
            }

            // Header for file data
            string fileName = Path.GetFileName(filePath);
            bodyBuilder.AppendFormat("\r\n--{0}\r\n", boundary);
            bodyBuilder.AppendFormat("Content-Disposition: form-data; name=\"file\"; filename=\"{0}\"\r\n", UrlEncode(fileName));
            bodyBuilder.Append("Content-Type: text/plain\r\n\r\n");

            // Read file and add to body
            using (StreamReader streamReader = new StreamReader(filePath))
            {
                char[] buffer = new char[1024];
                int read = 0;
                while ((read = streamReader.ReadBlock(buffer, 0, buffer.Length)) != 0)
                {
                    bodyBuilder.Append(read == 1024 ? buffer : buffer.Take(read).ToArray());
                }

                streamReader.Close();
            }

            // Finish file part
            bodyBuilder.AppendFormat("\r\n--{0}\r\n", boundary);

            // Get body
            byte[] bodyBytes = Encoding.UTF8.GetBytes(bodyBuilder.ToString());

            // Write body to request
            using (Stream stream = request.GetRequestStream())
            {
                stream.Write(bodyBytes, 0, bodyBytes.Length);
                stream.Close();
            }

            return request;
        }

        protected HttpWebRequest BuildRequest(string method, string path)
        {
            string uri = _apiHost + "/" + path;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.Method = method;
            request.UserAgent = s_userAgent;
            request.SendChunked = false;
            request.Timeout = Timeout;
            request.ReadWriteTimeout = ReadWriteTimeout;
            return request;
        }

        protected HttpWebRequest BuildRequest(string method, string action, Hashtable parameters)
        {
            return BuildRequest(method, action + "?" + GetParameterString(parameters));
        }

        protected string GetParameterString(Hashtable parameters)
        {
            StringBuilder builder = new StringBuilder();

            if (parameters != null && parameters.Count > 0)
            {
                foreach (string key in parameters.Keys)
                {
                    builder.AppendFormat("{0}={1}&", UrlEncode(key), UrlEncode(parameters[key].ToString()));
                }

                builder = builder.Remove(builder.Length - 1, 1);
            }

            return builder.Length > 0 ? builder.ToString() : "";
        }

        protected SailthruResponse SendRequest(HttpWebRequest request, string action)
        {
            SailthruResponse sailthruResponse;
            try
            {
                sailthruResponse = new SailthruResponse((HttpWebResponse)request.GetResponse());
            }
            catch (WebException e)
            {
                using (HttpWebResponse errorResponse = (HttpWebResponse)e.Response)
                {
                    sailthruResponse = new SailthruResponse(errorResponse);
                }
            }

            Hashtable rateLimitInfo = sailthruResponse.getRateLimitInfo();
            if (rateLimitInfo.Count > 0)
            {
                if (_lastRateLimitInfo.ContainsKey(action))
                {
                    Hashtable rateLimitPerMethod = (Hashtable)_lastRateLimitInfo[action];
                    rateLimitPerMethod[request.Method] = rateLimitInfo;
                }
                else
                {
                    Hashtable rateLimitPerMethod = new Hashtable
                    {
                        { request.Method, rateLimitInfo }
                    };

                    _lastRateLimitInfo.Add(action, rateLimitPerMethod);
                }
            }

            return sailthruResponse;
        }

        /// <summary>
        /// Generates an MD5 hash of the string.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static string md5(string value)
        {
            byte[] original_bytes = System.Text.Encoding.UTF8.GetBytes(value);
            byte[] encoded_bytes = new MD5CryptoServiceProvider().ComputeHash(original_bytes);
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < encoded_bytes.Length; i++)
            {
                result.Append(encoded_bytes[i].ToString("x2"));
            }

            return result.ToString();
        }

        private void AddAuthenticationAndFormatToParams(Hashtable parameters)
        {
            if (!parameters.Contains("api_key"))
            {
                parameters["api_key"] = _apiKey;
            }

            if (!parameters.Contains("format"))
            {
                parameters["format"] = "json";
            }

            if (!parameters.Contains("sig"))
            {
                parameters["sig"] = GetSignatureHash(parameters.Values);
            }
        }

        private string GetSignatureHash(NameValueCollection col)
        {
            return GetSignatureHash(col.Cast<string>().Select(e => col[e]));
        }

        private string GetSignatureHash(IEnumerable values)
        {
            List<string> stringValues = new List<string>();
            foreach (object value in values)
            {
                stringValues.Add(value.ToString());
            }

            string[] valuesArray = stringValues.ToArray();
            Array.Sort(valuesArray, s_orginalComparer);
            string valuesString = string.Join("", valuesArray);
            return md5(_secret + valuesString);
        }

        /// <summary>
        /// URL Encode String
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private string UrlEncode(string s)
        {
            return HttpUtility.UrlEncode(s ?? string.Empty);
        }
    }
}

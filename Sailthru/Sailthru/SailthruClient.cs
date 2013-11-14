using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Security;
using System.Collections;
using System.Security.Cryptography;
using System.Collections.Specialized;
using System.Web;
using Sailthru.Models;
using Newtonsoft.Json;

namespace Sailthru
{
    public class SailthruClient
    {
        #region Properties
        private static OrdinalComparer ORDINAL_COMPARER = new OrdinalComparer();
        private static string DEFAULT_API_URL = "https://api.sailthru.com";
        private string apiHost;
        private string apiKey;
        private string secret;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor with default API URI
        /// </summary>
        /// <param name="apiKey"></param>
        /// <param name="secret"></param>
        public SailthruClient(string apiKey, string secret)
        {
            this.apiHost = DEFAULT_API_URL;
            this.apiKey = apiKey;
            this.secret = secret;
        }


        /// <summary>
        /// Constructor with custom API URI
        /// </summary>
        /// <param name="apiKey"></param>
        /// <param name="secret"></param>
        /// <param name="apiHost"></param>
        public SailthruClient(string apiKey, string secret, string apiHost)
        {
            this.apiHost = apiHost;
            this.apiKey = apiKey;
            this.secret = secret;
        }

        
        #endregion

        #region Public Methods

        /// <summary>
        /// Receive the output of a Post.
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public bool ReceiveOptoutPost(NameValueCollection parameters)
        {
            List<string> requiredParams = new List<string> { "action", "email", "sig" };
            foreach (String key in requiredParams) {
                if (!parameters.AllKeys.Contains(key)) {
                    return false;
                }
            }

            if (parameters.Get("email") == null || parameters.Get("optout") == null) {
                return false;
            }

            string providedSignatureHash = parameters["sig"];
            parameters.Remove("sig");

            if (providedSignatureHash != GetSignatureHash(parameters)) {
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
            foreach (String key in parameters.Keys) {
                if (!requiredParams.Contains(key)) {
                    return false;
                }
            }

            if (parameters.Get("action") != "verify" && parameters.Get("send_id") != null) {
                return false;
            }

            //check signature of request against parameter data
            string providedSignature = parameters["sig"];
            parameters.Remove("sig");
            if (providedSignature != GetSignatureHash(parameters)) {
                return false;
            }

            SailthruResponse response = GetSend(parameters["send_id"]);
            var hash = response.HashtableResponse;
            if (hash.ContainsKey("email"))
            {
                return ((String)hash["email"] == parameters["email"]);
            }
            return false;
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
            return this.ApiPost("template", fields);
        }

        /// <summary>
        /// Save Template
        /// </summary>
        /// <param name="request">TemplateRequest parameters.</param>
        /// <seealso cref="http://docs.sailthru.com/api/template"/>
        /// <returns></returns>
        public SailthruResponse SaveTemplate(TemplateRequest request)
        {
            Hashtable hashForPost = new Hashtable();
            hashForPost.Add("json", JsonConvert.SerializeObject(request, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
            return this.ApiPost("template", hashForPost);
        }
        
        /// <summary>
        /// Get Template
        /// </summary>
        /// <param name="strTemplateName"></param>
        /// <seealso cref="http://docs.sailthru.com/api/template"/>
        /// <returns></returns>
        public SailthruResponse GetTemplate(string templateName)
        {
            Hashtable parameters = new Hashtable();
            parameters["template"] = templateName;
            return this.ApiGet("template", parameters);     
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
            Hashtable hashForPost = new Hashtable();
            hashForPost.Add("email", strEmail);
            hashForPost.Add("password", strPassword);

            if (boolIncludeNames)
            {
                hashForPost.Add("names", "1");
            }

            return this.ApiPost("contacts", hashForPost);
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

            return this.ApiPost("blast", hashForPost);
        }

        /// <summary>
        /// Create, update, and/or schedule a blast.
        /// </summary>
        /// <param name="request">BlastRequest parameters.</param>
        /// <seealso cref="http://docs.sailthru.com/api/blast"/>
        /// <returns></returns>
        public SailthruResponse ScheduleBlast(BlastRequest request)
        {
            Hashtable hashForPost = new Hashtable();
            hashForPost.Add("json", JsonConvert.SerializeObject(request, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
            return this.ApiPost("blast", hashForPost);
        }
        
        /// <summary>
        /// Get Blast
        /// </summary>
        /// <param name="strBlastId"></param>
        /// <seealso cref="http://docs.sailthru.com/api/blast"/>
        /// <returns></returns>
        public SailthruResponse GetBlast(string blastId)
        {
            Hashtable parameters = new Hashtable();
            parameters["blast_id"] = blastId;
            return this.ApiGet("blast", parameters);
        }
        
        /// <summary>
        /// Get information about one of your users.
        /// </summary>
        /// <param name="strEmail"></param>
        /// <returns></returns>
        /// <seealso cref="http://docs.sailthru.com/api/email"/>
        public SailthruResponse GetEmail(string email)
        {
            Hashtable parameters = new Hashtable();
            parameters["email"] = email;
            return this.ApiGet("email", parameters);
        }


		/// <summary>
		/// Get information about one of your users.
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		/// <seealso cref="http://docs.sailthru.com/api/email"/>
		public SailthruResponse GetEmail (EmailRequest request)
		{
			Hashtable hashForPost = new Hashtable();
			hashForPost.Add("json", JsonConvert.SerializeObject(request, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
			return this.ApiGet("email", hashForPost);
		}

        /// <summary>
        /// Update information about one of your users, including adding and removing the user from lists.
        /// </summary>
        /// <param name="strEmail"></param>
        /// <param name="htVars"></param>
        /// <param name="htLists"></param>
        /// <param name="htTemplates"></param>
        /// <param name="verified"></param>
        /// <param name="optout"></param>
        /// <param name="send"></param>
        /// <param name="sendVars"></param>
        /// <returns></returns>
        /// <seealso cref="http://docs.sailthru.com/api/email"/>
        public SailthruResponse SetEmail(string strEmail, 
            Hashtable htVars = null, 
            Hashtable htLists = null, 
            Hashtable htTemplates = null, 
            int verified = 0, 
            String optout = null, 
            string send = null, 
            Hashtable sendVars = null,
            String sms = null,
            String twitter = null,
            String changeEmail = null)
        {
            if (htVars == null) htVars = new Hashtable();
            if (htLists == null) htLists = new Hashtable();
            if (htTemplates == null) htTemplates = new Hashtable();

            Hashtable hashForPost = new Hashtable();
            foreach (DictionaryEntry entry in htVars)
            {
                hashForPost.Add("vars[" + entry.Key.ToString() + "]", entry.Value.ToString());
            }

            foreach (DictionaryEntry entry in htLists)
            {
                hashForPost.Add("lists[" + entry.Key.ToString() + "]", entry.Value.ToString());
            }

            foreach (DictionaryEntry entry in htTemplates)
            {
                hashForPost.Add("templates[" + entry.Key.ToString() + "]", entry.Value.ToString());
            }

            hashForPost.Add("email", strEmail);
            hashForPost.Add("verified", verified);

            if (optout != null)
            {
                hashForPost.Add("optout", optout);
            }

            if (send != null)
            {
                hashForPost.Add("send", send);
            }

            if (sendVars != null)
            {
                foreach (DictionaryEntry entry in sendVars)
                {
                    hashForPost.Add("send_vars[" + entry.Key.ToString() + "]", entry.Value.ToString());
                }
            }

            if (sms != null)
            {
                hashForPost.Add("sms", sms);
            }

            if (twitter != null)
            {
                hashForPost.Add("twitter", twitter);
            }

            if (changeEmail != null)
            {
                hashForPost.Add("change_email", changeEmail);
            }

            return this.ApiPost("email", hashForPost);
        }

        /// <summary>
        /// Update information about one of your users, including adding and removing the user from lists.
        /// </summary>
        /// <param name="request">EmailRequest parameters.</param>
        /// <returns></returns>
        /// <seealso cref="http://docs.sailthru.com/api/email"/>
        public SailthruResponse SetEmail(EmailRequest request)
        {
            Hashtable hashForPost = new Hashtable();
            hashForPost.Add("json", JsonConvert.SerializeObject(request, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));

            return this.ApiPost("email", hashForPost);
        }

        /// <summary>
        /// Send a transactional email for multiple users
        /// </summary>
        /// <param name="strTemplateName"></param>
        /// <param name="strEmail"></param>
        /// <param name="htVars"></param>
        /// <param name="htOptions"></param>
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
            
            return this.ApiPost("send", hashForPost);
        }

        /// <summary>
        /// Send a transactional Email for a single user
        /// </summary>
        /// <param name="strTemplateName"></param>
        /// <param name="strEmail"></param>
        /// <param name="htVars"></param>
        /// <param name="htOptions"></param>
        /// <returns></returns>
        /// <seealso cref="http://docs.sailthru.com/api/send"/>
        public SailthruResponse Send(string strTemplateName, string strEmail, Hashtable htVars = null, Hashtable htOptions = null)
        {
            return this.Multisend(strTemplateName, new string[] { strEmail }, htVars, htOptions);
        }

        /// <summary>
        /// Send a transactional Email for a single or multiple users.
        /// </summary>
        /// <param name="request">SendRequest parameters.</param>
        /// <returns></returns>
        public SailthruResponse Send(SendRequest request)
        {
            Hashtable hashForPost = new Hashtable();
            hashForPost.Add("json", JsonConvert.SerializeObject(request, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));  
            return this.ApiPost("send", hashForPost);
        }

        /// <summary>
        /// cancel a future send before it goes out.
        /// </summary>
        /// <param name="sendId"></param>
        /// <returns></returns>
        /// <seealso cref="http://docs.sailthru.com/api/send"/>
        public SailthruResponse CancelSend(string sendId)
        {
            Hashtable parameters = new Hashtable();
            parameters["send_id"] = sendId;
            return this.ApiDelete("send", parameters);
        }
        
        /// <summary>
        /// check on the status of a send
        /// </summary>
        /// <param name="sendId"></param>
        /// <returns></returns>
        /// <seealso cref="http://docs.sailthru.com/api/send"/>
        public SailthruResponse GetSend(string sendId)
        {
            Hashtable parameters = new Hashtable();
            parameters["send_id"] = sendId;
            return this.ApiGet("send", parameters); 
        }
        /// <summary>
        /// Submit a Purchase to Sailthru
        /// </summary>
        /// <param name="request">Purchaserequest parameters.</param>
        /// <returns></returns>
        public SailthruResponse Purchase(PurchaseRequest request)
        {
            Hashtable hashForPost = new Hashtable();
            hashForPost.Add("json", JsonConvert.SerializeObject(request, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));  
            return this.ApiPost("purchase", hashForPost);
        }

        public SailthruResponse ProcessJob(String jobType, String reportEmail, String postbackUrl, Hashtable parameters)
        {
            parameters["job"] = jobType;
            if (reportEmail != null) {
                parameters["report_email"] = reportEmail;
            }
            if (postbackUrl != null) {
                parameters["postback_url"] = postbackUrl;
            }
            if (parameters.ContainsKey("file")) {
                String filePath = (String)parameters["file"];
                parameters.Remove("file");
                return this.ApiPostWithFile("job", parameters, filePath);
            } else {
                return this.ApiPost("job", parameters);
            }
        }

        public SailthruResponse ProcessImportJob(String listName, List<String> emails)
        {
            return ProcessImportJob(null, null, listName, emails);
        }

        public SailthruResponse ProcessImportJob(String reportEmail, String postbackUrl, String listName, List<String> emails)
        {
            Hashtable htForPost = new Hashtable();
            htForPost["list"] = listName;
            htForPost["emails"] = String.Join(",", emails);
            return ProcessJob("import", reportEmail, postbackUrl, htForPost);
        }

        public SailthruResponse ProcessImportJob(String listName, String filePath)
        {
            return ProcessImportJob(null, null, listName, filePath);
        }

        public SailthruResponse ProcessImportJob(String reportEmail, String postbackUrl, String listName, String filePath)
        {
            Hashtable htForPost = new Hashtable();
            htForPost["list"] = listName;
            htForPost["file"] = filePath;
            return ProcessJob("import", reportEmail, postbackUrl, htForPost);
        }

        /// <summary>
        /// Request various stats from Sailthru.
        /// </summary>
        /// <param name="stat"></param>
        /// <param name="list"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        /// <seealso cref="http://docs.sailthru.com/api/stats"/>
        public SailthruResponse GetStat(String stat, String list = null, String date = null)
        {
            Hashtable parameters = new Hashtable();
            parameters["stat"] = stat;
            if (list != null)
            {
                parameters["list"] = list;
            }

            if (date != null)
            {
                parameters["date"] = date;
            }

            return this.ApiGet("stats", parameters);
        }


		/// <summary>
		/// Set information about one of your users. Users are referenced by multiple keys.
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		/// <seealso cref="http://docs.sailthru.com/api/user"/>
		public SailthruResponse SetUser (UserRequest request)
		{
			Hashtable hashForPost = new Hashtable();
			hashForPost.Add("json", JsonConvert.SerializeObject(request, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));  
			return this.ApiPost("user", hashForPost);
		}


		/// <summary>
		/// Get information about one of your users. Users are referenced by multiple keys.
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		/// <seealso cref="http://docs.sailthru.com/api/user"/>
		public SailthruResponse GetUser (UserRequest request)
		{
			Hashtable hashForPost = new Hashtable();
			hashForPost.Add("json", JsonConvert.SerializeObject(request, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));  // If null is not ignored, user API call doesn't seem to work which is strange
			return this.ApiGet("user", hashForPost);
		}

		/// <summary>
		/// Get information about one of your urls.
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		/// <seealso cref="http://docs.sailthru.com/api/content"/>
		public SailthruResponse GetContent (string url)
		{
			Hashtable hashForPost = new Hashtable();
			hashForPost.Add ("url", url);
			return this.ApiGet("content", hashForPost);
		}

		/// <summary>
		/// Set information about one of your urls.
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		/// <seealso cref="http://docs.sailthru.com/api/content"/>
		public SailthruResponse SetContent (ContentRequest request)
		{
			Hashtable hashForPost = new Hashtable();
			hashForPost.Add("json", JsonConvert.SerializeObject(request, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));  
			return this.ApiPost("content", hashForPost);
		}

        #endregion

        #region Protected Methods

        protected HttpWebRequest BuildRequest(String method, String path)
        {
            String uri = this.apiHost + "/" + path;
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(uri);
            request.Method = method;
            request.UserAgent = "Sailthru API C# Client";
            request.SendChunked = false;
            return request;
        }

        protected HttpWebRequest BuildRequest(String method, String action, Hashtable parameters)
        {        
            return BuildRequest(method, action + "?" + GetParameterString(parameters));
        }

        protected HttpWebRequest BuildPostRequest(String action, Hashtable parameters)
        {
            HttpWebRequest request = BuildRequest("POST", action);
            request.ContentType = "application/x-www-form-urlencoded";

            String bodyString = GetParameterString(parameters);
            byte[] body = Encoding.UTF8.GetBytes(bodyString);

            request.ContentLength = body.Length;

            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(body, 0, body.Length);
                requestStream.Close();
            }

            return request;
        }

        protected HttpWebRequest BuildPostWithFileRequest(String action, Hashtable parameters, String filePath)
        {
            // Prepare web request
            HttpWebRequest request = BuildRequest("POST", action);
            String boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
            request.ContentType = "multipart/form-data; boundary=" + boundary;

            // Use to build post body
            StringBuilder bodyBuilder = new StringBuilder();

            // Add form fields
            foreach (string key in parameters.Keys) {
                bodyBuilder.AppendFormat("\r\n--{0}\r\n", boundary);
                bodyBuilder.AppendFormat("Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}", 
                                          UrlEncode(key), UrlEncode(parameters[key].ToString()));
            }

            // Header for file data
            String fileName = Path.GetFileName(filePath);
            bodyBuilder.AppendFormat("\r\n--{0}\r\n", boundary);
            bodyBuilder.AppendFormat("Content-Disposition: form-data; name=\"file\"; filename=\"{0}\"\r\n", UrlEncode(fileName));
            bodyBuilder.Append("Content-Type: text/plain\r\n\r\n");

            // Read file and add to body
            using (StreamReader streamReader = new StreamReader(filePath)) {
                char[] buffer = new char[1024];
                int read = 0;
                while ((read = streamReader.ReadBlock(buffer, 0, buffer.Length)) != 0) {
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

        protected SailthruResponse SendRequest(HttpWebRequest request)
        {
            try 
            {
                return new SailthruResponse((HttpWebResponse)request.GetResponse());
            } 
            catch (WebException e) 
            {
                using (HttpWebResponse errorResponse = (HttpWebResponse)e.Response)
                {
                    return new SailthruResponse((HttpWebResponse)errorResponse);
                }
            }
        }
        
        /// For custom API calls that wrappers above don't cover, you can use the below:
        ///
        /// <summary>
        /// For making  API GET Request
        /// </summary>
        /// <param name="action">API Method String</param>
        /// <param name="parameters">API Parameter Hashtable</param>
        /// <returns>SailthruResponse Object</returns>
        public SailthruResponse ApiGet(String action, Hashtable parameters)
        {
            AddAuthenticationAndFormatToParams(parameters);
            HttpWebRequest request = BuildRequest("GET", action, parameters);
            return SendRequest(request);
        }

        /// <summary>
        /// For making  API DELETE Request
        /// </summary>
        /// <param name="action"></param>
        /// <param name="strParams"></param>
        /// <returns>SailthruResponse Object</returns>
        public SailthruResponse ApiDelete(string action, Hashtable parameters)
        {
            AddAuthenticationAndFormatToParams(parameters);
            HttpWebRequest request = BuildRequest("DELETE", action, parameters);
            return SendRequest(request);
        }

        /// <summary>
        /// For making  API POST Request
        /// </summary>
        /// <param name="action"></param>
        /// <param name="parameters"></param>
        /// <returns>SailthruResponse Object</returns>
        public SailthruResponse ApiPost(string action, Hashtable parameters)
        {
            AddAuthenticationAndFormatToParams(parameters);
            HttpWebRequest request = BuildPostRequest(action, parameters);
            return SendRequest(request);
        }

        protected SailthruResponse ApiPostWithFile(string action, Hashtable htForPost, String filePath)
        {
            AddAuthenticationAndFormatToParams(htForPost);
            HttpWebRequest request = BuildPostWithFileRequest(action, htForPost, filePath);
            return SendRequest(request);
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

        #endregion

        #region Private Methods

        private void AddAuthenticationAndFormatToParams(Hashtable parameters)
        {
            if (!parameters.Contains("api_key")) {
                parameters["api_key"] = this.apiKey;
            }
            if (!parameters.Contains("format")) {
                parameters["format"] = "json";
            }
            if (!parameters.Contains ("sig")) {
                parameters["sig"] = GetSignatureHash(parameters.Values);
            }
        }

        private String GetSignatureHash(ICollection values)
        {
            List<String> stringValues = new List<String>();
            foreach(Object value in values) {
                stringValues.Add(value.ToString());
            }
            String[] valuesArray = stringValues.ToArray();
            Array.Sort(valuesArray, ORDINAL_COMPARER);
            String valuesString = String.Join("", valuesArray);
            return md5(secret + valuesString);
        }

        /// <summary>
        /// URL Encode String
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private string UrlEncode(string s)
        {
            return HttpUtility.UrlEncode(s == null ? "" : s);
        }

        private static void OrdinalSort(Object[] values)
        {
            Array.Sort(values, ORDINAL_COMPARER);
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

        #endregion

    }
}

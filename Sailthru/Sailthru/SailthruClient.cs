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

        private string strAPIUri = "https://api.sailthru.com";
        private string strAPIKey;
        private string strSecret;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor with default API URI
        /// </summary>
        /// <param name="strAPIKey"></param>
        /// <param name="strSecret"></param>
        public SailthruClient(string strAPIKey, string strSecret)
        {
            this.strAPIKey = strAPIKey;
            this.strSecret = strSecret;
        }


        /// <summary>
        /// Constructor with custom API URI
        /// </summary>
        /// <param name="strAPIKey"></param>
        /// <param name="strSecret"></param>
        /// <param name="strAPIUri"></param>
        public SailthruClient(string strAPIKey, string strSecret, string strAPIUri)
        {
            this.strAPIUri = strAPIUri;
            this.strAPIKey = strAPIKey;
            this.strSecret = strSecret;
        }

        
        #endregion

        #region Public Methods

        /// <summary>
        /// REceive the output of a Post.
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public bool ReceiveOptoutPost(NameValueCollection parameters)
        {
            if (parameters.Get("email") == null || parameters.Get("optout") == null)
                return false;

            string sig = parameters["sig"];
            string paramsAsString = extractValuesFromCollection(parameters);
            if (sig != getSignatureHash(paramsAsString, this.strSecret))
                return false;

            return true;
        }

        /// <summary>
        /// Receive and verify the output of a Post.
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public bool ReceiveVerifyPost(NameValueCollection parameters)
        {
            if (parameters.Get("action") != "verify" && parameters.Get("send_id") != null)
                return false;

            //check signature of request against parameter data
            string sig = parameters["sig"];
            string paramsAsString = extractValuesFromCollection(parameters);
            if (sig != getSignatureHash(paramsAsString, this.strSecret))
                return false;

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
            hashForPost.Add("json", JsonConvert.SerializeObject(request));
            return this.ApiPost("template", hashForPost);
        }
        
        /// <summary>
        /// Get Template
        /// </summary>
        /// <param name="strTemplateName"></param>
        /// <seealso cref="http://docs.sailthru.com/api/template"/>
        /// <returns></returns>
        public SailthruResponse GetTemplate(string strTemplateName)
        {
            return this.ApiGet("template", "template=" + strTemplateName);     
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
        /// Fetch email contacts from an address book at one of the major email providers (aol/gmail/hotmail/yahoo) 
        /// </summary>
        /// <param name="strEmail">ImportContactRequest parameters.</param>
        /// <seealso cref="http://docs.sailthru.com/api/template"/>
        /// <returns>SailthruResponse Object</returns>
        public SailthruResponse ImportContacts(ImportContactRequest request)
        {
            Hashtable hashForPost = new Hashtable();
            hashForPost.Add("json", JsonConvert.SerializeObject(request));
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
            hashForPost.Add("json", JsonConvert.SerializeObject(request));
            return this.ApiPost("blast", hashForPost);
        }
        
        /// <summary>
        /// Get Blast
        /// </summary>
        /// <param name="strBlastId"></param>
        /// <seealso cref="http://docs.sailthru.com/api/blast"/>
        /// <returns></returns>
        public SailthruResponse GetBlast(string strBlastId)
        {
            return this.ApiGet("blast", "blast_id=" + strBlastId);
        }
        
        /// <summary>
        /// Get information about one of your users.
        /// </summary>
        /// <param name="strEmail"></param>
        /// <returns></returns>
        /// <seealso cref="http://docs.sailthru.com/api/email"/>
        public SailthruResponse GetEmail(string strEmail)
        {
            return this.ApiGet("email", "email=" + strEmail);
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
            hashForPost.Add("json", JsonConvert.SerializeObject(request));

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
            hashForPost.Add("json", JsonConvert.SerializeObject(request));  
            return this.ApiPost("send", hashForPost);
        }

        /// <summary>
        /// cancel a future send before it goes out.
        /// </summary>
        /// <param name="strSendId"></param>
        /// <returns></returns>
        /// <seealso cref="http://docs.sailthru.com/api/send"/>
        public SailthruResponse CancelSend(string strSendId)
        {
            return this.ApiDelete("send", "send_id=" + strSendId);
        }
        
        /// <summary>
        /// check on the status of a send
        /// </summary>
        /// <param name="strSendId"></param>
        /// <returns></returns>
        /// <seealso cref="http://docs.sailthru.com/api/send"/>
        public SailthruResponse GetSend(string strSendId)
        {
            return this.ApiGet("send", "send_id=" + strSendId + "&api_key=" + strAPIKey); 
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
            String parameters = "stat=" + stat;
            if (list != null)
            {
                parameters += "&list=" + list;
            }

            if (date != null)
            {
                parameters += "&date=" + date;
            }

            return this.ApiGet("stats", parameters);
        }

        #endregion

        #region Protected Methods

        /** 
        * Generic HTTP request function for POST, GET and DELETE
        * 
        */
        protected SailthruResponse HttpRequest(string strURI, string strData, string strMethod)
        {
            HttpWebRequest httpWebRequest;
            try
            {
                if (strMethod == "POST")
                {
                    httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(strURI);
                    byte[] byteArray = Encoding.UTF8.GetBytes(strData);

                    httpWebRequest.ContentType = "application/x-www-form-urlencoded";
                    httpWebRequest.SendChunked = false;
                    httpWebRequest.UserAgent = "Sailthru API C# Client";
                    httpWebRequest.Method = strMethod;
                    httpWebRequest.ContentLength = byteArray.Length;

                    // write POST body
                    using (Stream requestStream = httpWebRequest.GetRequestStream())
                    {
                        requestStream.Write(byteArray, 0, byteArray.Length);
                        requestStream.Close();
                    }
                }
                else
                {
                    httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(strURI + "?" + strData);
                    httpWebRequest.ContentType = "application/x-www-form-urlencoded";
                    httpWebRequest.SendChunked = false;
                    httpWebRequest.UserAgent = "Sailthru API C# Client";
                    httpWebRequest.Method = strMethod;
                }

                return new SailthruResponse((HttpWebResponse)httpWebRequest.GetResponse());
            }
            catch (WebException e)
            {
                using (HttpWebResponse errorResponse = (HttpWebResponse)e.Response)
                {
                    return new SailthruResponse((HttpWebResponse)errorResponse);
                }
            }
            
        }

        /// <summary>
        /// For making  API GET Request
        /// </summary>
        /// <param name="strAction">API Method String</param>
        /// <param name="strParams">API Parameter String</param>
        /// <returns>SailthruResponse Object</returns>
        protected SailthruResponse ApiGet(string strAction, string strParams)
        {
            Hashtable htForPost = ExtractHashtableFromParam(strParams);

            return this.ApiGet(strAction, htForPost);
        }

        /// <summary>
        /// For making  API GET Request
        /// </summary>
        /// <param name="strAction">API Method String</param>
        /// <param name="htForPost">API Parameter Hashtable</param>
        /// <returns>SailthruResponse Object</returns>
        protected SailthruResponse ApiGet(String strAction, Hashtable htForPost)
        {
            htForPost = ParseHashtable(htForPost);

            string sortedValuesString = GetSortedValuesString(htForPost);
            string sigHash = getSignatureHash(sortedValuesString, this.strSecret);
            string strPostString = GetStringForPost(htForPost);

            strPostString += "&sig=" + sigHash;

            return this.HttpRequest(this.strAPIUri + "/" + strAction, strPostString, "GET");
        }

        /// <summary>
        /// For making  API DELETE Request
        /// </summary>
        /// <param name="strAction"></param>
        /// <param name="strParams"></param>
        /// <returns>SailthruResponse Object</returns>
        protected SailthruResponse ApiDelete(string strAction, string strParams)
        {
            Hashtable htForPost = ExtractHashtableFromParam(strParams);

            return this._ApiPost(strAction, htForPost, "DELETE");
        }

        /// <summary>
        /// For making  API POST Request
        /// </summary>
        /// <param name="strAction"></param>
        /// <param name="strString"></param>
        /// <returns>SailthruResponse Object</returns>
        protected SailthruResponse ApiPost(string strAction, string strString)
        {
            Hashtable htForPost = ExtractHashtableFromParam(strString);

            return ApiPost(strAction, htForPost);
        }

        /// <summary>
        /// For making  API POST Request
        /// </summary>
        /// <param name="strAction"></param>
        /// <param name="htForPost"></param>
        /// <returns>SailthruResponse Object</returns>
        protected SailthruResponse ApiPost(string strAction, Hashtable htForPost)
        {
            return this._ApiPost(strAction, htForPost, "POST");
        }

        protected string GetStringForPost(Hashtable htForPost)
        {
            string parameters = "";
            ICollection keys = htForPost.Keys;

            if (htForPost.Count > 0)
            {
                foreach (string Key in keys)
                {
                    parameters += urlEncodeString(Key) + "=" + urlEncodeString(htForPost[Key].ToString()) + "&";
                }
                parameters = parameters.Substring(0, parameters.Length - 1);
            }

            return parameters;
        }

        protected SailthruResponse _ApiPost(string strAction, Hashtable htForPost, String method)
        {
            htForPost = ParseHashtable(htForPost);
            string sortedValuesString = GetSortedValuesString(htForPost);
            string sigHash = getSignatureHash(sortedValuesString, this.strSecret);
            string strPostString = GetStringForPost(htForPost);

            strPostString += "&sig=" + sigHash;

            return this.HttpRequest(this.strAPIUri + "/" + strAction, strPostString, method);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Parse given hash table
        /// check if it contains api_key and format and if not present add them
        /// </summary>
        /// <param name="htForPost"></param>
        /// <returns></returns>
        private Hashtable ParseHashtable(Hashtable htForPost)
        {
            // Check for API key and Format
            if (htForPost.ContainsKey("api_key") == false)
            {
                htForPost.Add("api_key", this.strAPIKey);
            }

            if (htForPost.ContainsKey("format") == false)
            {
                htForPost.Add("format", "json");
            }

            return htForPost;
        }

        /// <summary>
        /// URL Encode String
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private string urlEncodeString(string s)
        {
            if (s == null)
                s = "";

            return HttpUtility.UrlEncode(s);
        }

        /// <summary>
        /// Splits up a string of key/value parameters and concatenates all values together with no delimete
        /// </summary>
        /// <param name="strParams"></param>
        /// <returns></returns>
        private static string extractParamValues(string strParams)
        {
            ArrayList list = new ArrayList();

            string values = "";

            string[] pairs = strParams.Split('&');

            //add all values to list
            foreach (string s in pairs)
            {
                //if (!String.IsNullOrEmpty(s))
                if (s != null && s != "")
                {
                    list.Add(s.Split('=')[1]);
                }
            }

            //sort the list
            string[] sortedVals = new string[list.Count];
            list.CopyTo(sortedVals);

            OrdinalComparer comparer = new OrdinalComparer();
            Array.Sort(sortedVals, comparer);

            foreach (string s in sortedVals)
            {
                values += s;
            }

            return values;
        }

        /// <summary>
        /// Extracts the values from the NameValueCollection in alphabetical order.
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        private string extractValuesFromCollection(NameValueCollection collection)
        {
            string paramString = "";

            foreach (string key in collection.Keys)
            {
                paramString += "&" + key + "=" + collection[key];
            }

            if (paramString.Length > 0)
            {
                paramString = paramString.Substring(1);
            }

            return extractParamValues(paramString);
        }

        /// <summary>
        /// Returns an MD5 hash of the secret + sorted list of parameter values for an API call.
        /// </summary>
        /// <param name="strParams"></param>
        /// <param name="strSecret"></param>
        /// <returns></returns>
        private string getSignatureHash(string strParams, string strSecret)
        {
            string strString = strSecret + strParams;
            return md5(strString);
        }

        /// <summary>
        /// Generates an MD5 hash of the string.
        /// </summary>
        /// <param name="strMd5String"></param>
        /// <returns></returns>
        private static string md5(string strMd5String)
        {
            byte[] original_bytes = System.Text.Encoding.UTF8.GetBytes(strMd5String);
            byte[] encoded_bytes = new MD5CryptoServiceProvider().ComputeHash(original_bytes);
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < encoded_bytes.Length; i++)
            {
                result.Append(encoded_bytes[i].ToString("x2"));
            }
            return result.ToString();
        }

        /// <summary>
        /// Converts the hashtable entries into a url-encoded string representation of key/value pairs.         
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        private string HashToParamString(Hashtable table)
        {
            string parms = "";

            ICollection keys = table.Keys;

            var json_format_found = false;

            foreach (string key in keys)
            {
                if (key == "format" && table[key].ToString() == "json")
                {
                    json_format_found = true;
                }
                parms += "&" + key + "=" + table[key];
            }

            if (parms.Length > 0)
            {
                parms = parms.Substring(1);
                if (json_format_found == false)
                {
                    parms += "&format=json";
                }
            }

            return parms;
        }

        /// <summary>
        /// Extract param value from hashtable
        /// </summary>
        /// <param name="strParams"></param>
        /// <returns></returns>
        private static Hashtable ExtractHashtableFromParam(string strParams)
        {
            Hashtable list = new Hashtable();

            string[] pairs = strParams.Split('&');

            //add all values to list
            foreach (string s in pairs)
            {
                if (s != null && s != "")
                {
                    list.Add(s.Split('=')[0], s.Split('=')[1]);
                }
            }

            return list;
        }

        /// <summary>
        /// Sort API Parameter Values
        /// </summary>
        /// <param name="htForPost"></param>
        /// <returns></returns>
        private string GetSortedValuesString(Hashtable htForPost)
        {
            string values = "";

            ArrayList listOfValues = new ArrayList();

            ICollection myKeys = htForPost.Keys;

            foreach (string Key in myKeys)
            {
                listOfValues.Add(htForPost[Key].ToString());
            }

            //sort the list
            string[] sortedVals = new string[listOfValues.Count];
            listOfValues.CopyTo(sortedVals);

            OrdinalComparer comparer = new OrdinalComparer();
            Array.Sort(sortedVals, comparer);

            foreach (string s in sortedVals)
            {
                values += s;
            }
            return values;
        }

        #endregion

    }
}

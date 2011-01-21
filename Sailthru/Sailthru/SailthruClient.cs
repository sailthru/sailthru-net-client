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

namespace Sailthru
{
    public class SailthruClient
    {
        #region Properties

        private string strAPIUri = "http://api.sailthru.com";
        private string strAPIKey;
        private string strSecret;

        #endregion

        #region Constructor

        /**
         * Constructor with default API URI
         */
        public SailthruClient(string strAPIKey, string strSecret)
        {
            this.strAPIKey = strAPIKey;
            this.strSecret = strSecret;
        }


        /**
         * Constructor with custom API URI
         */
        public SailthruClient(string strAPIKey, string strSecret, string strAPIUri)
        {
            this.strAPIUri = strAPIUri;
            this.strAPIKey = strAPIKey;
            this.strSecret = strSecret;
        }

        #endregion


        


        public SailthruResponse ApiGet(string strAction, string strParams)
        {
            Hashtable htForPost = ExtractHashtableFromParam(strParams);

            return this.ApiGet(strAction, htForPost);
        }


        public SailthruResponse ApiGet(String strAction, Hashtable htForPost)
        {
            htForPost = ParseHashtable(htForPost);

            string sortedValuesString = GetSortedValuesString(htForPost);
            string sigHash = getSignatureHash(sortedValuesString, this.strSecret);
            string strPostString = GetStringForPost(htForPost);

            strPostString += "&sig=" + sigHash;

            Console.WriteLine("Params: " + strPostString);

            return this.HttpRequest(this.strAPIUri + "/" + strAction, strPostString, "GET");
        }

        public SailthruResponse ApiDelete(string strAction, string strParams)
        {
            /**
            Hashtable htForPost = ExtractHashtableFromParam(strParams);
            htForPost = parseHashtable(htForPost);

            string sortedValuesString = GetSortedValuesString(htForPost);
            string sigHash = getSignatureHash(sortedValuesString, this.strSecret);
            string strPostString = GetStringForPost(htForPost);

            strPostString += "&sig=" + sigHash;
            Console.WriteLine("Params: " + strPostString);
            return this.HttpRequest(this.strAPIUri + "/" + strAction, strPostString, "DELETE");
            **/
            Hashtable htForPost = ExtractHashtableFromParam(strParams);

            return this._ApiPost(strAction, htForPost, "DELETE");
        }


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


        public SailthruResponse ApiPost(string strAction, string strString)
        {
            Hashtable htForPost = ExtractHashtableFromParam(strString);

            return ApiPost(strAction, htForPost);
        }


        public SailthruResponse ApiPost(string strAction, Hashtable htForPost)
        {
            return this._ApiPost(strAction, htForPost, "POST");
        }


        

        public bool receiveOptoutPost(NameValueCollection parameters)
        {
            if (parameters.Get("email") == null || parameters.Get("optout") == null)
                return false;

            string sig = parameters["sig"];
            string paramsAsString = extractValuesFromCollection(parameters);
            if (sig != getSignatureHash(paramsAsString, this.strSecret))
                return false;

            return true;
        }


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

        
        public SailthruResponse SaveTemplate(string strTemplateName, Hashtable fields = null)
        {
            if (fields == null)
            {
                fields = new Hashtable();
            }
            fields.Add("template", strTemplateName);
            return this.ApiPost("template", fields);
        }

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


        public SailthruResponse ScheduleBlast(string strName, string strList, string strScheduleTime, string strFromName, string strFromEmail, string strSubject, string strContentHtml, string strContentText, Hashtable htOptions = null)
        {
            //if (htOptions == null) htOptions = new Hashtable();   //For Next release
            Hashtable hashForPost = new Hashtable();

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


        public SailthruResponse GetBlast(string strBlastId)
        {
            return this.ApiGet("blast", "blast_id=" + strBlastId);
        }
        

        public SailthruResponse GetEmail(string strEmail)
        {
            return this.ApiGet("email", "email=" + strEmail);
        }


        public SailthruResponse SetEmail(string strEmail, Hashtable htVars = null, Hashtable htLists = null, Hashtable htTemplates = null, int verified = 0, String optout = null, string send = null, Hashtable sendVars = null)
        {
            if (htVars == null) htVars = new Hashtable();
            if (htLists == null) htLists = new Hashtable();
            if (htTemplates == null) htTemplates = new Hashtable();

            Hashtable hashForPost = new Hashtable();
            foreach (DictionaryEntry entry in htVars)
            {
                hashForPost.Add("vars[" + entry.Key + "]", entry.Value.ToString());
            }

            foreach (DictionaryEntry entry in htLists)
            {
                hashForPost.Add("lists[" + entry.Key + "]", entry.Value.ToString());
            }

            hashForPost.Add("email", strEmail);

            return this.ApiPost("email", hashForPost);
        }

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


        public SailthruResponse Send(string strTemplateName, string strEmail, Hashtable htVars = null, Hashtable htOptions = null)
        {
            return this.Multisend(strTemplateName, new string[] { strEmail }, htVars, htOptions);
        }


        public SailthruResponse CancelSend(string strSendId)
        {
            return this.ApiDelete("send", "send_id=" + strSendId);
        }
        

        public SailthruResponse GetSend(string strSendId)
        {
            return this.ApiGet("send", "send_id=" + strSendId + "&api_key=" + strAPIKey); 
        }


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


        #region Protected Methods


        /** 
        * Generic HTTP request function for POST, GET and DELETE
        * 
        */
        protected SailthruResponse HttpRequest(string strURI, string strData, string strMethod)
        {

            WebResponse response;
            WebRequest request;
            try
            {
                if (strMethod == "POST")
                {
                    request = WebRequest.Create(strURI);
                    request.Method = strMethod;
                    byte[] byteArray = Encoding.UTF8.GetBytes(strData);
                    request.ContentType = "application/x-www-form-urlencoded";
                    request.ContentLength = byteArray.Length;
                    Stream dataStream = request.GetRequestStream();
                    // Write the data to the request stream.
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    dataStream.Close();
                }
                else
                {
                    request = WebRequest.Create(strURI + "?" + strData);
                    request.Method = strMethod;
                }

                response = request.GetResponse();

            }
            catch (Exception e)
            {
                throw e;
            }

            return new SailthruResponse(response);
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
            Console.WriteLine(strPostString);

            return this.HttpRequest(this.strAPIUri + "/" + strAction, strPostString, method);
        }

        #endregion

        #region Private Methods

        private string urlEncodeString(string s)
        {
            if (s == null)
                s = "";

            return HttpUtility.UrlEncode(s);
        }

        /** 
	    * Splits up a string of key/value parameters and concatenates all values together with no delimeter.
        * This is a utility function for generation of the md5 hash
        *
        * @param string strParams
        *
        * @return string
	    */
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



        /**
         * Extracts the values from the NameValueCollection in alphabetical order. This is a
         * utility function for the generation of the md5 hash.
         * 
         * @param NameValueCollection collection
         * 
         * @return string
         */
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



        /** 
	    * Returns an MD5 hash of the secret + sorted list of parameter values for an API call. 
	    * 
	    * @param string strParams 
	    * @param string strSecret 
        *
	    * @return string 
	    */
        private string getSignatureHash(string strParams, string strSecret)
        {
            string strString = strSecret + strParams;
            return md5(strString);
        }


        /**
         * Generates an MD5 hash of the string.
         */
        private static string md5(string strMd5String)
        {
            byte[] original_bytes = System.Text.Encoding.ASCII.GetBytes(strMd5String);
            byte[] encoded_bytes = new MD5CryptoServiceProvider().ComputeHash(original_bytes);
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < encoded_bytes.Length; i++)
            {
                result.Append(encoded_bytes[i].ToString("x2"));
            }
            return result.ToString();
        }


        /**
         * Converts the hashtable entries into a url-encoded string representation 
         * of key/value pairs.
         * 
         * @param Hashtable table
         * 
         * @return string
         */
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


        /**
         * 
         * 
        */
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
            Console.WriteLine("Sorted value: " + values);
            return values;
        }

        #endregion

    }
}

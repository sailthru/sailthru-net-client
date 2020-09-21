using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Collections;

namespace Sailthru
{
    public class SailthruResponse
    {

        private HttpWebResponse webResponse;

        private Hashtable hashtableResponse;

        private Hashtable rateLimitInfo;

        private String rawResponse;

        private Boolean validResponse;

        protected const String ERROR_KEY = "error";
        protected const String ERROR_MSG_KEY = "errormsg";

        
        /// <summary>
        /// Response from Server represented as HashTable
        /// </summary>
        public Hashtable HashtableResponse
        {
            get
            {
                return this.hashtableResponse;
            }
        }

        /// <summary>
        /// RawResponse Getter
        /// </summary>
        public String RawResponse
        {
            get
            {
                return this.rawResponse;
            }
        }

        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="response"></param>
        public SailthruResponse(HttpWebResponse response)
        {
            this.webResponse = response;
            this.hashtableResponse = null;
            this.rawResponse = "";
            this.validResponse = false;
            this.rateLimitInfo = new Hashtable();
            parseJSON();
        }

        /// <summary>
        /// Parse Response JSON
        /// </summary>
        private void parseJSON()
        {   

            if (webResponse != null)
            {
                HttpWebResponse httpWebResponse = webResponse;
                Stream dataStream = httpWebResponse.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);

                var responseStr = reader.ReadToEnd();

                WebHeaderCollection headers = webResponse.Headers;

                // Clean up the streams.
                reader.Close();
                dataStream.Close();
                webResponse.Close();

                this.rawResponse = responseStr;

                var jsonResponse = Sailthru.JSON.JsonDecode(responseStr);

                if (jsonResponse is Hashtable)
                {
                    hashtableResponse = (Hashtable)jsonResponse;
                    if (!hashtableResponse.ContainsKey(ERROR_KEY) || !hashtableResponse.ContainsKey(ERROR_MSG_KEY))
                    {
                        this.validResponse = true;
                    }
                }
                else
                {
                    this.rawResponse = responseStr;
                    this.hashtableResponse = createErrorResponse(responseStr);
                }

                // parse rate limit headers
                if (headers.Get("X-Rate-Limit-Limit") != null &&
                    headers.Get("X-Rate-Limit-Remaining") != null &&
                    headers.Get("X-Rate-Limit-Reset") != null)
                {
                    this.rateLimitInfo.Add("limit", Int32.Parse(headers.Get("X-Rate-Limit-Limit")));
                    this.rateLimitInfo.Add("remaining", Int32.Parse(headers.Get("X-Rate-Limit-Remaining")));
                    DateTime reset = new DateTime(1970, 1, 1, 0, 0, 0, 0);
                    this.rateLimitInfo.Add("reset", reset.AddSeconds(Int64.Parse(headers.Get("X-Rate-Limit-Reset"))));
                }
            }
            else
            {
                var msg = "There was a problem making request to the server";
                this.rawResponse = msg;
                this.hashtableResponse = createErrorResponse(msg);
            }
        }

        /// <summary>
        /// Check if the response is valid
        /// </summary>
        /// <returns></returns>
        public Boolean IsOK()
        {
            return this.validResponse;
        }

        
        /// <summary>
        /// create custom hastable with error and errormsg
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private Hashtable createErrorResponse(String message)
        {
            Hashtable hash = new Hashtable();
            hash.Add(ERROR_KEY, 99);
            hash.Add(ERROR_MSG_KEY, message);
            return hash;
        }

        public Hashtable getRateLimitInfo()
        {
            return this.rateLimitInfo;
        }
    }
}

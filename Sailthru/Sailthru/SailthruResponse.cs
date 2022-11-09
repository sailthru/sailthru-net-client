using System;
using System.Collections;
using System.IO;
using System.Net;

namespace Sailthru
{
    public class SailthruResponse
    {
        private readonly HttpWebResponse _webResponse;
        private readonly Hashtable _rateLimitInfo;
        private bool _validResponse;

        protected const string ERROR_KEY = "error";
        protected const string ERROR_MSG_KEY = "errormsg";

        /// <summary>
        /// Response from Server represented as HashTable
        /// </summary>
        public Hashtable HashtableResponse { get; private set; }

        /// <summary>
        /// RawResponse Getter
        /// </summary>
        public string RawResponse { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="response"></param>
        public SailthruResponse(HttpWebResponse response)
        {
            _webResponse = response;
            HashtableResponse = null;
            RawResponse = string.Empty;
            _validResponse = false;
            _rateLimitInfo = new Hashtable();
            parseJSON();
        }

        /// <summary>
        /// Parse Response JSON
        /// </summary>
        private void parseJSON()
        {
            if (_webResponse != null)
            {
                HttpWebResponse httpWebResponse = _webResponse;
                Stream dataStream = httpWebResponse.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);

                string responseStr = reader.ReadToEnd();

                WebHeaderCollection headers = _webResponse.Headers;

                // Clean up the streams.
                reader.Close();
                dataStream.Close();
                _webResponse.Close();

                RawResponse = responseStr;

                object jsonResponse = JSON.JsonDecode(responseStr);

                if (jsonResponse is Hashtable hashtable)
                {
                    HashtableResponse = hashtable;
                    if (!HashtableResponse.ContainsKey(ERROR_KEY) || !HashtableResponse.ContainsKey(ERROR_MSG_KEY))
                    {
                        _validResponse = true;
                    }
                }
                else
                {
                    RawResponse = responseStr;
                    HashtableResponse = createErrorResponse(responseStr);
                }

                // parse rate limit headers
                if (headers.Get("X-Rate-Limit-Limit") != null &&
                    headers.Get("X-Rate-Limit-Remaining") != null &&
                    headers.Get("X-Rate-Limit-Reset") != null)
                {
                    _rateLimitInfo.Add("limit", int.Parse(headers.Get("X-Rate-Limit-Limit")));
                    _rateLimitInfo.Add("remaining", int.Parse(headers.Get("X-Rate-Limit-Remaining")));
                    DateTime reset = new DateTime(1970, 1, 1, 0, 0, 0, 0);
                    _rateLimitInfo.Add("reset", reset.AddSeconds(long.Parse(headers.Get("X-Rate-Limit-Reset"))));
                }
            }
            else
            {
                string msg = "There was a problem making request to the server";
                RawResponse = msg;
                HashtableResponse = createErrorResponse(msg);
            }
        }

        /// <summary>
        /// Check if the response is valid
        /// </summary>
        /// <returns></returns>
        public bool IsOK()
        {
            return _validResponse;
        }

        /// <summary>
        /// create custom hastable with error and errormsg
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private Hashtable createErrorResponse(string message)
        {
            Hashtable hash = new Hashtable
            {
                { ERROR_KEY, 99 },
                { ERROR_MSG_KEY, message }
            };

            return hash;
        }

        public Hashtable getRateLimitInfo()
        {
            return _rateLimitInfo;
        }
    }
}

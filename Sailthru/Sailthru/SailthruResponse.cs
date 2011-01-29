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

        private WebResponse webResponse;

        private Hashtable hashtableResponse;

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
        public SailthruResponse(WebResponse response)
        {
            this.webResponse = response;
            this.hashtableResponse = null;
            this.rawResponse = "";
            this.validResponse = false;
            parseJSON();
        }

        /// <summary>
        /// Parse Response JSON
        /// </summary>
        private void parseJSON()
        {
            if (((HttpWebResponse)webResponse).StatusCode == HttpStatusCode.OK)
            {
                Stream dataStream = webResponse.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);

                var responseStr = reader.ReadToEnd();

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
    }
}

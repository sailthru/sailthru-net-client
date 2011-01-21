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

        public Hashtable HashtableResponse
        {
            get
            {
                return this.hashtableResponse;
            }
        }

        public String RawResponse
        {
            get
            {
                return this.rawResponse;
            }
        }

        public SailthruResponse(WebResponse response)
        {
            this.webResponse = response;
            this.hashtableResponse = null;
            this.rawResponse = "";
            parseJSON();
        }

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
                    Console.WriteLine(hashtableResponse.Keys.Count);
                }
            }
            else
            {
                Console.WriteLine(((HttpWebResponse)webResponse).StatusCode);
            }
        }
    }
}

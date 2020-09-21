using Newtonsoft.Json;

namespace Sailthru.Tests.Mock
{
    public class ErrorResponse
    {
        [JsonProperty]
        private int error;

        [JsonProperty]
        private string errormsg;

        public ErrorResponse(int error, string errormsg)
        {
            this.error = error;
            this.errormsg = errormsg;
        }
    }
}
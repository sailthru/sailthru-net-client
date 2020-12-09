using Newtonsoft.Json;

namespace Sailthru.Tests.Mock
{
    public class ErrorResponse
    {
        [JsonProperty]
        private readonly int error;

        [JsonProperty]
        private readonly string errormsg;

        public ErrorResponse(int error, string errormsg)
        {
            this.error = error;
            this.errormsg = errormsg;
        }
    }
}
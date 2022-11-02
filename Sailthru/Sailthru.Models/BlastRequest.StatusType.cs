using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Sailthru.Models
{
    public partial class BlastRequest
    {
        /// <summary>
        /// Blast status.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public enum StatusType
        {
            [EnumMember(Value = "draft")]
            Draft,

            [EnumMember(Value = "scheduled")]
            Scheduled
        }
    }
}

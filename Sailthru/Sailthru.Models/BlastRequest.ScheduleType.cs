using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Sailthru.Models
{
    public partial class BlastRequest
    {
        /// <summary>
        /// Blast schedule type
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public enum ScheduleType
        {
            [EnumMember(Value = "specific")]
            Specific,

            [EnumMember(Value = "personalized")]
            Personalized,

            [EnumMember(Value = "local_timezone")]
            LocalTimeZone
        }
    }
}

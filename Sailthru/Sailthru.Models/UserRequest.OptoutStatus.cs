using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Sailthru.Models
{
    public partial class UserRequest
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public enum OptoutStatus
        {
            [EnumMember(Value = "all")]
            All,

            [EnumMember(Value = "blast")]
            Blast,

            [EnumMember(Value = "basic")]
            Basic,

            [EnumMember(Value = "none")]
            None
        }
    }
}

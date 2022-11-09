using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Sailthru.Models
{
    public partial class PurchaseRequest
    {
        /// <summary>
        /// Channel Type
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public enum ChannelType
        {
            [EnumMember(Value = "app")]
            App,

            /// <summary>
            /// Online (default value)
            /// </summary>
            [EnumMember(Value = "online")]
            Online = 0,

            [EnumMember(Value = "offline")]
            Offline
        }
    }
}

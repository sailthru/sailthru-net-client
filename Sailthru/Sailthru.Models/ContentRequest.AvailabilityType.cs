using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Sailthru.Models
{
    public partial class ContentRequest
    {
        /// <summary>
        /// Availability
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public enum AvailabilityType
        {
            [EnumMember(Value = "in stock")]
            InStock,

            [EnumMember(Value = "out of stock")]
            OutOfStock,

            [EnumMember(Value = "preorder")]
            Preorder,

            [EnumMember(Value = "backorder")]
            BackOrder
        }
    }
}

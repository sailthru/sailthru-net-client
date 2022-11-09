using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Sailthru.Models
{
    public partial class BlastRequest
    {
        /// <summary>
        /// Message criteria for retargeting campaigns.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public enum MessageCriteriaType
        {
            [EnumMember(Value = "not_opened")]
            NotOpened,

            [EnumMember(Value = "not_clicked")]
            NotClicked,

            [EnumMember(Value = "not_purchased")]
            NotPurchased,

            [EnumMember(Value = "opened")]
            Opened,

            [EnumMember(Value = "clicked")]
            Clicked,

            [EnumMember(Value = "purchased")]
            Purchased
        }
    }
}

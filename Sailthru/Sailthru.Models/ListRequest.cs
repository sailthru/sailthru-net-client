using System.Collections;
using Newtonsoft.Json;

namespace Sailthru.Models
{
    /// <summary>
    /// Request object used for interaction with the LIST API.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class ListRequest
    {
        /// <summary>
        /// Gets or sets the list name.
        /// </summary>
        /// <value>The list name.</value>
        [JsonProperty(PropertyName = "list")]
        public string List { get; set; }

        /// <summary>
        /// Gets or sets the list id.
        /// </summary>
        /// <value>The list id.</value>
        [JsonProperty(PropertyName = "list_id")]
        public string ListId { get; set; }

        /// <summary>
        /// Set if this is a primary or secondary list
        /// </summary>
        /// <value>The primary list status.</value>
        [JsonProperty(PropertyName = "primary")]
        public bool Primary { get; set; }

        /// <summary>
        /// Get or set the list public name
        /// </summary>
        /// <value>The list public name.</value>
        [JsonProperty(PropertyName = "public_name")]
        public string PublicName { get; set; }

        /// <summary>
        /// Gets or sets the smart list query options.
        /// </summary>
        /// <value>The smart list query options. query examples <seealso cref="https://getstarted.sailthru.com/developers/api-basics/queries/"/></value>
        [JsonProperty(PropertyName = "query")]
        public Hashtable Query { get; set; }

        /// <summary>
        /// Get or set the list type (natural vs smart)
        /// </summary>
        /// <value>The list type.</value>
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the list vars.
        /// </summary>
        /// <value>The list vars.</value>
        [JsonProperty(PropertyName = "vars")]
        public Hashtable Vars { get; set; }
    }
}

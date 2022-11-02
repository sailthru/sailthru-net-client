using System.Collections;
using Newtonsoft.Json;

namespace Sailthru.Models
{
    /// <summary>
    /// Request object used for interaction with the content API.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public partial class ContentRequest
    {
        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        /// <value>The ID.</value>
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        /// <value>The key.</value>
        [JsonProperty(PropertyName = "key")]
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the keys.
        /// </summary>
        /// <value>The keys.</value>
        [JsonProperty(PropertyName = "keys")]
        public Hashtable Keys { get; set; }

        /// <summary>
        /// Gets or sets the url.
        /// </summary>
        /// <value>The url.</value>
        [JsonProperty(PropertyName = "url")]
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the date.
        /// </summary>
        /// <value>The date.</value>
        [JsonProperty(PropertyName = "date")]
        public string Date { get; set; }

        /// <summary>
        /// Gets or sets the expire date.
        /// </summary>
        /// <value>The expire date.</value>
        [JsonProperty(PropertyName = "expire_date")]
        public string ExpireDate { get; set; }

        /// <summary>
        /// Gets or sets the tags.
        /// </summary>
        /// <value>The tags.</value>
        [JsonProperty(PropertyName = "tags")]
        public string[] Tags { get; set; }

        /// <summary>
        /// Gets or sets the vars.
        /// </summary>
        /// <value>The vars.</value>
        [JsonProperty(PropertyName = "vars")]
        public Hashtable Vars { get; set; }

        /// <summary>
        /// Gets or sets the images.
        /// </summary>
        /// <value>The vars.</value>
        [JsonProperty(PropertyName = "images")]
        public Hashtable Images { get; set; }

        /// <summary>
        /// Gets or sets the location.
        /// </summary>
        /// <value>The location.</value>
        [JsonProperty(PropertyName = "location")]
        public ArrayList Location { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the site name.
        /// </summary>
        /// <value>The site name.</value>
        [JsonProperty(PropertyName = "site_name")]
        public string SiteName { get; set; }

        /// <summary>
        /// Gets or sets the author.
        /// </summary>
        /// <value>The author.</value>
        [JsonProperty(PropertyName = "author")]
        public string Author { get; set; }

        /// <summary>
        /// Gets or sets the spider.
        /// </summary>
        /// <value>The spider.</value>
        [JsonProperty(PropertyName = "spider")]
        public SpiderType Spider { get; set; }

        /// <summary>
        /// Gets or sets the price.
        /// </summary>
        /// <value>The price.</value>
        [JsonProperty(PropertyName = "price")]
        public int? Price { get; set; }

        /// <summary>
        /// Gets or sets the inventory.
        /// </summary>
        /// <value>The inventory</value>
        [JsonProperty(PropertyName = "inventory")]
        public int? Inventory { get; set; }

        /// <summary>
        /// Gets or sets the override exclude.
        /// </summary>
        /// <value>The override exclude.</value>
        [JsonProperty(PropertyName = "override_exclude", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public OverrideExcludeType OverrideExclude { get; set; }
    }
}

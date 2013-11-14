namespace Sailthru.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Newtonsoft.Json;
    using System.Collections;

    /// <summary>
	/// Request object used for interaction with the content API.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
	public class ContentRequest
    {
		/// <summary>
		/// Flag to determine the spider options.
		/// </summary>
		public enum SpiderType
		{
			/// <summary>
			/// Spider is disabled.
			/// </summary>
			Disabled = 0,

			/// <summary>
			/// Spider is enabled.
			/// </summary>
			Enabled = 1
		}

        /// <summary>
		/// Gets or sets the url.
        /// </summary>
        /// <value>
		/// The url.
        /// </value>
		[JsonProperty(PropertyName = "url")]
		public string Url { get; set; }

		/// <summary>
		/// Gets or sets the title.
		/// </summary>
		/// <value>
		/// The title.
		/// </value>
		[JsonProperty(PropertyName = "title")]
		public string Title { get; set; }

		/// <summary>
		/// Gets or sets the date.
		/// </summary>
		/// <value>
		/// The date.
		/// </value>
		[JsonProperty(PropertyName = "date")]
		public string Date { get; set; }

		/// <summary>
		/// Gets or sets the expire date.
		/// </summary>
		/// <value>
		/// The expire date.
		/// </value>
		[JsonProperty(PropertyName = "expire_date")]
		public string ExpireDate { get; set; }

        /// <summary>
		/// Gets or sets the tags.
        /// </summary>
        /// <value>
		/// The tags.
        /// </value>
		[JsonProperty(PropertyName = "tags")]
		public string[] Tags { get; set; }

        /// <summary>
        /// Gets or sets the vars.
        /// </summary>
        /// <value>
        /// The vars.
        /// </value>
        [JsonProperty(PropertyName = "vars")]
        public Hashtable Vars { get; set; }

		/// <summary>
		/// Gets or sets the images.
		/// </summary>
		/// <value>
		/// The vars.
		/// </value>
		[JsonProperty(PropertyName = "images")]
		public Hashtable Images { get; set; }

		/// <summary>
		/// Gets or sets the location.
		/// </summary>
		/// <value>
		/// The location.
		/// </value>
		[JsonProperty(PropertyName = "location")]
		public ArrayList Location { get; set; }

		/// <summary>
		/// Gets or sets the description.
		/// </summary>
		/// <value>
		/// The description.
		/// </value>
		[JsonProperty(PropertyName = "description")]
		public string Description { get; set; }

		/// <summary>
		/// Gets or sets the site name.
		/// </summary>
		/// <value>
		/// The site name.
		/// </value>
		[JsonProperty(PropertyName = "site_name")]
		public string SiteName { get; set; }

		/// <summary>
		/// Gets or sets the author.
		/// </summary>
		/// <value>
		/// The author.
		/// </value>
		[JsonProperty(PropertyName = "author")]
		public string Author { get; set; }

		/// <summary>
		/// Gets or sets the spider.
		/// </summary>
		/// <value>
		/// The spider.
		/// </value>
		[JsonProperty(PropertyName = "spider")]
		public SpiderType Spider { get; set; }
   }
}

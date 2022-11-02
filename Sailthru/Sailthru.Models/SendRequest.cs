using System.Collections;
using Newtonsoft.Json;

namespace Sailthru.Models
{
    /// <summary>
    /// Request object used for interaction with the send API.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class SendRequest
    {
        /// <summary>
        /// Gets or sets the template.
        /// </summary>
        /// <value>The template.</value>
        [JsonProperty(PropertyName = "template")]
        public string Template { get; set; }

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        /// <value>The email.</value>
        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the vars.
        /// </summary>
        /// <value>The vars.</value>
        [JsonProperty(PropertyName = "vars")]
        public Hashtable Vars { get; set; }

        /// <summary>
        /// Gets or sets the schedule time.
        /// </summary>
        /// <value>The schedule time.</value>
        [JsonProperty(PropertyName = "schedule_time")]
        public string ScheduleTime { get; set; }

        /// <summary>
        /// Gets or sets the options.
        /// </summary>
        /// <value>The options.</value>
        [JsonProperty(PropertyName = "options")]
        public Hashtable Options { get; set; }

        /// <summary>
        /// Gets or sets the data feed URL.
        /// </summary>
        /// <value>The data feed URL.</value>
        [JsonProperty(PropertyName = "data_feed_url")]
        public string DataFeedUrl { get; set; }
    }
}

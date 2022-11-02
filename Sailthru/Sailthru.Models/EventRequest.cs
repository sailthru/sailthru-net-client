using System.Collections;
using Newtonsoft.Json;

namespace Sailthru.Models
{
    /// <summary>
    /// Request object used for interaction with the event API.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class EventRequest
    {
        /// <summary>
        /// the key value to look up the user.
        /// </summary>
        /// <value>The id.</value>
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        /// <summary>
        /// the key type of id.
        /// </summary>
        /// <value>The key.</value>
        [JsonProperty(PropertyName = "key")]
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the vars.
        /// </summary>
        /// <value>The vars.</value>
        [JsonProperty(PropertyName = "vars")]
        public Hashtable Vars { get; set; }

        /// <summary>
        /// Gets or sets the event.
        /// </summary>
        /// <value>The event.</value>
        [JsonProperty(PropertyName = "event")]
        public string Event { get; set; }

        /// <summary>
        /// Gets or sets the schedule_time.
        /// </summary>
        /// <value>The schedule_time.</value>
        [JsonProperty(PropertyName = "schedule_time")]
        public string ScheduleTime { get; set; }
    }
}

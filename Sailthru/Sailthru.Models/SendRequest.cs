// -----------------------------------------------------------------------
// <copyright file="SendRequest.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Sailthru.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Newtonsoft.Json;
    using System.Collections;

    /// <summary>
    /// Request object used for interaction with the send API.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class SendRequest
    {
        /// <summary>
        /// Gets or sets the template.
        /// </summary>
        /// <value>
        /// The template.
        /// </value>
        [JsonProperty(PropertyName = "template")]
        public string Template { get; set; }

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        /// <value>
        /// The email.
        /// </value>
        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the vars.
        /// </summary>
        /// <value>
        /// The vars.
        /// </value>
        [JsonProperty(PropertyName = "vars")]
        public Hashtable Vars { get; set; }

        /// <summary>
        /// Gets or sets the schedule time.
        /// </summary>
        /// <value>
        /// The schedule time.
        /// </value>
        [JsonProperty(PropertyName = "schedule_time")]
        public string ScheduleTime { get; set; }

        /// <summary>
        /// Gets or sets the options.
        /// </summary>
        /// <value>
        /// The options.
        /// </value>
        [JsonProperty(PropertyName = "options")]
        public Hashtable Options { get; set; }
    }
}

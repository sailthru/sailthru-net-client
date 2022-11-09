using System;
using System.Collections;
using Newtonsoft.Json;

namespace Sailthru.Models
{
    /// <summary>
    /// Request object used for interaction with the PREVIEW API.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class PreviewRequest
    {
        [JsonProperty(PropertyName = "day")]
        private string _day;

        /// <summary>
        /// Gets or sets the blast id.
        /// </summary>
        /// <value>The blast id.</value>
        [JsonProperty(PropertyName = "blast_id")]
        public string BlastId { get; set; }

        /// <summary>
        /// Gets or sets the blast repeat id.
        /// </summary>
        /// <value>The blast repeat id.</value>
        [JsonProperty(PropertyName = "blast_repeat_id")]
        public string BlastRepeatId { get; set; }

        /// <summary>
        /// Gets or sets the data feed url to use.
        /// </summary>
        /// <value>The data feed url.</value>
        [JsonProperty(PropertyName = "data_feed_url")]
        public string DataFeedUrl { get; set; }

        /// <summary>
        /// Gets or sets the email to use in preview.
        /// </summary>
        /// <value>The test email.</value>
        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the preview day.
        /// </summary>
        /// <value>The preview day.</value>
        public DateTime? PreviewDay
        {
            get
            {
                if (!string.IsNullOrEmpty(_day))
                {
                    return Convert.ToDateTime(_day);
                }

                return null;
            }
            set
            {
                if (value.HasValue)
                {
                    _day = value.Value.ToString("ddd, dd MMMM yyyy");
                }
            }
        }

        /// <summary>
        /// Gets or sets from email to send the preview to.
        /// </summary>
        /// <value>The send email.</value>
        [JsonProperty(PropertyName = "send_email")]
        public string SendEmail { get; set; }

        /// <summary>
        /// Gets or sets the template.
        /// </summary>
        /// <value>The template.</value>
        [JsonProperty(PropertyName = "template")]
        public string Template { get; set; }

        /// <summary>
        /// Gets or sets the test vars.
        /// </summary>
        /// <value>The test vars.</value>
        [JsonProperty(PropertyName = "test_vars")]
        public Hashtable TestVars { get; set; }
    }
}

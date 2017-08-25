// -----------------------------------------------------------------------
// <copyright file="TemplateRequest.cs" company="Microsoft">
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
    /// Request object used for interaction with the template API.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class TemplateRequest
    {
        /// <summary>
        /// Flag to determine the link tracking options.
        /// </summary>
        public enum LinkTrackingType
        {
            /// <summary>
            /// Link tracking is disabled.
            /// </summary>
            Disabled = 0,

            /// <summary>
            /// Link tracking is enabled.
            /// </summary>
            Enabled = 1
        }

        /// <summary>
        /// Flag to determine the google analytics options.
        /// </summary>
        public enum GoogleAnalyticsType
        {
            /// <summary>
            /// Google analytics is disabled.
            /// </summary>
            Disabled = 0,

            /// <summary>
            /// Google analytics is enabled.
            /// </summary>
            Enabled = 1
        }

        /// <summary>
        /// Gets or sets the template.
        /// </summary>
        /// <value>
        /// The template.
        /// </value>
        [JsonProperty(PropertyName = "template")]
        public string Template { get; set; }

        /// <summary>
        /// Gets or sets the revision.
        /// </summary>
        /// <value>
        /// The revision.
        /// </value>
        [JsonProperty(PropertyName = "revision")]
        public int? Revision { get; set; }

        /// <summary>
        /// Gets or sets the sample.
        /// </summary>
        /// <value>
        /// The sample.
        /// </value>
        [JsonProperty(PropertyName = "sample")]
        public string Sample { get; set; }

        /// <summary>
        /// Gets or sets the name of the public.
        /// </summary>
        /// <value>
        /// The name of the public.
        /// </value>
        [JsonProperty(PropertyName = "public_name")]
        public string PublicName { get; set; }

        /// <summary>
        /// Gets or sets from name.
        /// </summary>
        /// <value>
        /// From name.
        /// </value>
        [JsonProperty(PropertyName = "from_name")]
        public string FromName { get; set; }

        /// <summary>
        /// Gets or sets from email.
        /// </summary>
        /// <value>
        /// From email.
        /// </value>
        [JsonProperty(PropertyName = "from_email")]
        public string FromEmail { get; set; }

        /// <summary>
        /// Gets or sets the reply to email.
        /// </summary>
        /// <value>
        /// The reply to email.
        /// </value>
        [JsonProperty(PropertyName = "replyto_email")]
        public string ReplyToEmail { get; set; }

        /// <summary>
        /// Gets or sets the subject.
        /// </summary>
        /// <value>
        /// The subject.
        /// </value>
        [JsonProperty(PropertyName = "subject")]
        public string Subject { get; set; }

        /// <summary>
        /// Gets or sets the content HTML.
        /// </summary>
        /// <value>
        /// The content HTML.
        /// </value>
        [JsonProperty(PropertyName = "content_html")]
        public string ContentHtml { get; set; }

        /// <summary>
        /// Gets or sets the content text.
        /// </summary>
        /// <value>
        /// The content text.
        /// </value>
        [JsonProperty(PropertyName = "content_text")]
        public string ContentText { get; set; }

        /// <summary>
        /// Gets or sets the content SMS.
        /// </summary>
        /// <value>
        /// The content SMS.
        /// </value>
        [JsonProperty(PropertyName = "content_sms")]
        public string ContentSms { get; set; }

        /// <summary>
        /// Gets or sets the link tracking.
        /// </summary>
        /// <value>
        /// The link tracking.
        /// </value>
        [JsonProperty(PropertyName = "is_link_tracking")]
        public LinkTrackingType LinkTracking { get; set; }

        /// <summary>
        /// Gets or sets the google analytics.
        /// </summary>
        /// <value>
        /// The google analytics.
        /// </value>
        [JsonProperty(PropertyName = "is_google_analytics")]
        public GoogleAnalyticsType GoogleAnalytics { get; set; }

        /// <summary>
        /// Gets or sets the verify post URL.
        /// </summary>
        /// <value>
        /// The verify post URL.
        /// </value>
        [JsonProperty(PropertyName = "verify_post_url")]
        public string VerifyPostUrl { get; set; }

        /// <summary>
        /// Gets or sets the link params.
        /// </summary>
        /// <value>
        /// The link params.
        /// </value>
        [JsonProperty(PropertyName = "link_params")]
        public Hashtable LinkParams { get; set; }

        /// <summary>
        /// Gets or sets the success URL.
        /// </summary>
        /// <value>
        /// The success URL.
        /// </value>
        [JsonProperty(PropertyName = "success_url")]
        public string SuccessUrl { get; set; }

        /// <summary>
        /// Gets or sets the error URL.
        /// </summary>
        /// <value>
        /// The error URL.
        /// </value>
        [JsonProperty(PropertyName = "error_url")]
        public string ErrorUrl { get; set; }

        /// <summary>
        /// Gets or sets the revision id.
        /// </summary>
        /// <value>
        /// The revision id.
        /// </value>
        [JsonProperty(PropertyName = "revision_id")]
        public int RevisionId { get; set; }

        /// <summary>
        /// Gets or sets the revision ids.
        /// </summary>
        /// <value>
        /// The revision ids.
        /// </value>
        [JsonProperty(PropertyName = "revision_ids")]
        public List<int> RevisionIds { get; set; }

        /// <summary>
        /// Gets or sets the setup.
        /// </summary>
        /// <value>
        /// The setup.
        /// </value>
        [JsonProperty(PropertyName = "setup")]
        public string Setup { get; set; }
    }
}

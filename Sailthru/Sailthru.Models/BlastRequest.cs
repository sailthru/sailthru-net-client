﻿// -----------------------------------------------------------------------
// <copyright file="BlastRequest.cs" company="Microsoft">
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
    using Newtonsoft.Json.Converters;
    using System.Runtime.Serialization;

    /// <summary>
    /// Request object used for interaction with the blast API.
    /// </summary>
    public class BlastRequest
    {
        /// <summary>
        /// Flag to determine link tracking options.
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
        /// Flag to determine google analytic options.
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
        /// Flag to determine the visibility of the blast.
        /// </summary>
        public enum VisibilityType
        {
            /// <summary>
            /// Blast is marked as private.
            /// </summary>
            Private = 0,

            /// <summary>
            /// Blast is marked as public.
            /// </summary>
            Public = 1
        }

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

        /// <summary>
        /// Blast status.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public enum StatusType
        {
            [EnumMember(Value = "draft")]
            Draft,

            [EnumMember(Value = "scheduled")]
            Scheduled
        }

        /// <summary>
        /// Flag to determine blast labels.
        /// </summary>
        public enum LabelType
        {
            /// <summary>
            /// Remove label from the blast.
            /// </summary>
            Remove = 0,

            /// <summary>
            /// Add the label to the blast.
            /// </summary>
            Add = 1
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the list.
        /// </summary>
        /// <value>
        /// The list.
        /// </value>
        [JsonProperty(PropertyName = "list")]
        public string List { get; set; }

        /// <summary>
        /// Gets or sets the schedule time.
        /// </summary>
        /// <value>
        /// The schedule time.
        /// </value>
        [JsonProperty(PropertyName = "schedule_time")]
        public string ScheduleTime { get; set; }

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
        /// Gets or sets the blast id.
        /// </summary>
        /// <value>
        /// The blast id.
        /// </value>
        [JsonProperty(PropertyName = "blast_id")]
        public string BlastId { get; set; }

        /// <summary>
        /// Gets or sets the copy blast.
        /// </summary>
        /// <value>
        /// The copy blast.
        /// </value>
        [JsonProperty(PropertyName = "copy_blast")]
        public string CopyBlast { get; set; }

        /// <summary>
        /// Gets or sets the copy template.
        /// </summary>
        /// <value>
        /// The copy template.
        /// </value>
        [JsonProperty(PropertyName = "copy_template")]
        public string CopyTemplate { get; set; }

        /// <summary>
        /// Gets or sets the eval template.
        /// </summary>
        /// <value>
        /// The eval template.
        /// </value>
        [JsonProperty(PropertyName = "eval_template")]
        public string EvalTemplate { get; set; }

        /// <summary>
        /// Gets or sets the reply to.
        /// </summary>
        /// <value>
        /// The reply to.
        /// </value>
        [JsonProperty(PropertyName = "replyto")]
        public string ReplyTo { get; set; }

        /// <summary>
        /// Gets or sets the report email.
        /// </summary>
        /// <value>
        /// The report email.
        /// </value>
        [JsonProperty(PropertyName = "report_email")]
        public string ReportEmail { get; set; }

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
        /// Gets or sets the visibility.
        /// </summary>
        /// <value>
        /// The visibility.
        /// </value>
        [JsonProperty(PropertyName = "is_public")]
        public VisibilityType Visibility { get; set; }

        /// <summary>
        /// Gets or sets the suppress list.
        /// </summary>
        /// <value>
        /// The suppress list.
        /// </value>
        [JsonProperty(PropertyName = "suppress_list")]
        public string SuppressList { get; set; }

        /// <summary>
        /// Gets or sets the test vars.
        /// </summary>
        /// <value>
        /// The test vars.
        /// </value>
        [JsonProperty(PropertyName = "test_vars")]
        public Hashtable TestVars { get; set; }

        /// <summary>
        /// Gets or sets the email hour range.
        /// </summary>
        /// <value>
        /// The email hour range.
        /// </value>
        [JsonProperty(PropertyName = "email_hour_range")]
        public int EmailHourRange { get; set; }

        /// <summary>
        /// Gets or sets the data feed URL.
        /// </summary>
        /// <value>
        /// The data feed URL.
        /// </value>
        [JsonProperty(PropertyName = "data_feed_url")]
        public string DataFeedUrl { get; set; }

        /// <summary>
        /// Gets or sets the vars.
        /// </summary>
        /// <value>
        /// The vars.
        /// </value>
        [JsonProperty(PropertyName = "vars")]
        public Hashtable Vars { get; set; }

        /// <summary>
        /// Gets or sets the setup.
        /// </summary>
        /// <value>
        /// The setup.
        /// </value>
        [JsonProperty(PropertyName = "setup")]
        public string Setup { get; set; }

        /// <summary>
        /// Gets or sets the ad plan.
        /// </summary>
        /// <value>
        /// The ad plan.
        /// </value>
        [JsonProperty(PropertyName = "ad_plan")]
        public string AdPlan { get; set; }

        /// <summary>
        /// Gets or sets the auto convert text.
        /// </summary>
        /// <value>
        /// The auto convert text.
        /// </value>
        [JsonProperty(PropertyName = "autoconvert_text")]
        public int AutoConvertText { get; set; }

        /// <summary>
        /// Gets or sets the test email.
        /// </summary>
        /// <value>
        /// The test email.
        /// </value>
        [JsonProperty(PropertyName = "test_email")]
        public string TestEmail { get; set; }

        /// <summary>
        /// Gets or sets the message criteria.
        /// </summary>
        /// <value>
        /// The message criteria.
        /// </value>
        [JsonProperty(PropertyName = "message_criteria")]
        public MessageCriteriaType? MessageCriteria { get; set; }

        /// <summary>
        /// Gets or sets the previous blast ID.
        /// </summary>
        /// <value>
        /// The previous blast ID.
        /// </value>
        [JsonProperty(PropertyName = "previous_blast_id")]
        public int? PreviousBlastId { get; set; }
        
        /// <summary>
        /// Gets or sets the link domain.
        /// </summary>
        /// <value>
        /// The link domain.
        /// </value>
        [JsonProperty(PropertyName = "link_domain")]
        public string LinkDomain { get; set; }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        [JsonProperty(PropertyName = "status")]
        public StatusType? Status { get; set; }

        /// <summary>
        /// Gets or sets the seed emails.
        /// </summary>
        /// <value>
        /// The seed emails.
        /// </value>
        [JsonProperty(PropertyName = "seed_emails")]
        public string[] SeedEmails { get; set; }

        /// <summary>
        /// Gets or sets the labels.
        /// </summary>
        /// <value>
        /// The labels.
        /// </value>
        [JsonProperty(PropertyName = "labels")]
        public Dictionary<string, LabelType> Labels { get; set; }
    }
}

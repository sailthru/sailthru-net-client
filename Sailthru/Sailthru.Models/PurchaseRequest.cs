using System.Collections;
using Newtonsoft.Json;

namespace Sailthru.Models
{
    /// <summary>
    /// Request object used for interaction with the send API.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public partial class PurchaseRequest
    {
        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        /// <value>The email.</value>
        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        /// <value>The items.</value>
        [JsonProperty(PropertyName = "items")]
        public ArrayList Items { get; set; }

        /// <summary>
        /// Gets or sets the purchase keys.
        /// </summary>
        /// <value>The purchase keys.</value>
        [JsonProperty(PropertyName = "purchase_keys")]
        public Hashtable PurchaseKeys { get; set; }

        /// <summary>
        /// Gets or sets the vars.
        /// </summary>
        /// <value>The vars.</value>
        [JsonProperty(PropertyName = "vars")]
        public Hashtable Vars { get; set; }

        /// <summary>
        /// Gets or sets the message_id.
        /// </summary>
        /// <value>The message_id.</value>
        [JsonProperty(PropertyName = "message_id")]
        public string MessageID { get; set; }

        /// <summary>
        /// Gets or sets the adjustments.
        /// </summary>
        /// <value>The adjustments.</value>
        [JsonProperty(PropertyName = "adjustments")]
        public ArrayList Adjustments { get; set; }

        /// <summary>
        /// Gets or sets the incomplete status.
        /// </summary>
        /// <value>The incomplete status.</value>
        [JsonProperty(PropertyName = "incomplete")]
        public int Incomplete { get; set; }

        /// <summary>
        /// Gets or sets the date.
        /// </summary>
        /// <value>The date.</value>
        [JsonProperty(PropertyName = "date")]
        public string Date { get; set; }

        /// <summary>
        /// Gets or sets the reminder_template.
        /// </summary>
        /// <value>The reminder_template.</value>
        [JsonProperty(PropertyName = "reminder_template")]
        public string ReminderTemplate { get; set; }

        /// <summary>
        /// Gets or sets the reminder_time.
        /// </summary>
        /// <value>The reminder_time.</value>
        [JsonProperty(PropertyName = "reminder_time")]
        public string ReminderTime { get; set; }

        /// <summary>
        /// Gets or sets the send_template.
        /// </summary>
        /// <value>The send_template.</value>
        [JsonProperty(PropertyName = "send_template")]
        public string SendTemplate { get; set; }

        /// <summary>
        /// Gets or sets the tenders.
        /// </summary>
        /// <value>The tenders.</value>
        [JsonProperty(PropertyName = "tenders")]
        public ArrayList Tenders { get; set; }

        /// <summary>
        /// Gets or sets the channel.
        /// </summary>
        /// <value>The channel.</value>
        /// <remarks>
        /// Identifies where the purchase took place. Values can be app, online (passed by default
        /// if channel is not set, or left blank), or offline.
        /// Note: When channel is used, the additional top-level parameter app_id may be used.
        /// </remarks>
        [JsonProperty(PropertyName = "channel")]
        public ChannelType Channel { get; set; }

        /// <summary>
        /// Gets or sets the application identifier.
        /// </summary>
        /// <value>The application identifier.</value>
        /// <remarks>
        /// Available when channel is set to app. The app_id is the id assigned to a mobile app
        /// within the Sailthru mobile platform. This can be found in the URL of the browser when
        /// viewing a specific app.
        /// </remarks>
        [JsonProperty(PropertyName = "app_id")]
        public string AppId { get; set; }

        /// <summary>
        /// Gets or sets the cookies.
        /// </summary>
        /// <value>The cookies.</value>
        /// <remarks>
        /// Used to submit user cookie values along with the purchase. For example, to ensure
        /// attribution for purchased products shown in Site Personalization Manager sections by
        /// including the optional sailthru_pc value as the value of the cookie named sailthru_pc.
        /// </remarks>
        [JsonProperty(PropertyName = "cookies")]
        public string Cookies { get; set; }

        /// <summary>
        /// Gets or sets the sailthru pc.
        /// </summary>
        /// <value>The sailthru pc.</value>
        /// <remarks>
        /// The value of the sailthru_pc cookie created by Site Personalization Manager. If Site
        /// Personalization Manager is enabled anywhere on your site, submit this cookie value with
        /// every purchase to ensure purchased products displayed using Site Personalization Manager
        /// are properly attributed to their section(s).
        /// </remarks>
        [JsonProperty(PropertyName = "sailthru_pc")]
        public string SailthruPC { get; set; }

        /// <summary>
        /// Gets or sets the user agent.
        /// </summary>
        /// <value>The user agent.</value>
        /// <remarks>The user agent string of the user making the purchase.</remarks>
        [JsonProperty(PropertyName = "user_agent")]
        public string UserAgent { get; set; }
    }
}

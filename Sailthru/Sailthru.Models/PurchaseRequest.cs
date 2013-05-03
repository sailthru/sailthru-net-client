// -----------------------------------------------------------------------
// <copyright file="PurchaseRequest.cs" company="Microsoft">
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
    public class PurchaseRequest
    {
        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        /// <value>
        /// The email.
        /// </value>
        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        /// <value>
        /// The items.
        /// </value>
        [JsonProperty(PropertyName = "items")]
        public ArrayList Items { get; set; }


        /// <summary>
        /// Gets or sets the vars.
        /// </summary>
        /// <value>
        /// The vars.
        /// </value>
        [JsonProperty(PropertyName = "vars")]
        public Hashtable Vars { get; set; }

        /// <summary>
        /// Gets or sets the message_id.
        /// </summary>
        /// <value>
        /// The message_id.
        /// </value>
        [JsonProperty(PropertyName = "message_id")]
        public string MessageID { get; set; }

        /// <summary>
        /// Gets or sets the adjustments.
        /// </summary>
        /// <value>
        /// The adjustments.
        /// </value>
        [JsonProperty(PropertyName = "adjustments")]
        public ArrayList Adjustments { get; set; }
 
        /// <summary>
        /// Gets or sets the incomplete status.
        /// </summary>
        /// <value>
        /// The incomplete status.
        /// </value>
        [JsonProperty(PropertyName = "incomplete")]
        public int Incomplete { get; set; }

        /// <summary>
        /// Gets or sets the date.
        /// </summary>
        /// <value>
        /// The date.
        /// </value>
        [JsonProperty(PropertyName = "date")]
        public string Date { get; set; }

        /// <summary>
        /// Gets or sets the reminder_template.
        /// </summary>
        /// <value>
        /// The reminder_template.
        /// </value>
        [JsonProperty(PropertyName = "reminder_template")]
        public string ReminderTemplate { get; set; }

        /// <summary>
        /// Gets or sets the reminder_time.
        /// </summary>
        /// <value>
        /// The reminder_time.
        /// </value>
        [JsonProperty(PropertyName = "reminder_time")]
        public string ReminderTime { get; set; }

        /// <summary>
        /// Gets or sets the send_template.
        /// </summary>
        /// <value>
        /// The send_template.
        /// </value>
        [JsonProperty(PropertyName = "send_template")]
        public string SendTemplate { get; set; }

        /// <summary>
        /// Gets or sets the tenders.
        /// </summary>
        /// <value>
        /// The tenders.
        /// </value>
        [JsonProperty(PropertyName = "tenders")]
        public ArrayList Tenders { get; set; }

   }
}

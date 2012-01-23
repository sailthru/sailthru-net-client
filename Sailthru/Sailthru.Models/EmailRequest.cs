// -----------------------------------------------------------------------
// <copyright file="Email.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Sailthru.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Collections;
    using Newtonsoft.Json;

    /// <summary>
    /// Request object used for interaction with the email API.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class EmailRequest
    {
        /// <summary>
        /// Flag to determine the list subscription options.
        /// </summary>
        public enum ListType
        {
            /// <summary>
            /// Unsubscribe the user to the list.
            /// </summary>
            Unsubscribe = 0,

            /// <summary>
            /// Subscribe the user to the list.
            /// </summary>
            Subscribe = 1
        }

        /// <summary>
        /// Flag to determine the template opt-in options.
        /// </summary>
        public enum TemplateType
        {
            /// <summary>
            /// Out-out of the template
            /// </summary>
            OptOut = 0,

            /// <summary>
            /// Opt-in to the template
            /// </summary>
            OptIn = 1
        }

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        /// <value>
        /// The email.
        /// </value>
        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the verified.
        /// </summary>
        /// <value>
        /// The verified.
        /// </value>
        [JsonProperty(PropertyName = "verified")]
        public int Verified { get; set; }

        /// <summary>
        /// Gets or sets the opt out.
        /// </summary>
        /// <value>
        /// The opt out.
        /// </value>
        [JsonProperty(PropertyName = "optout")]
        public string OptOut { get; set; }

        /// <summary>
        /// Gets or sets the lists.
        /// </summary>
        /// <value>
        /// The lists.
        /// </value>
        [JsonProperty(PropertyName = "lists")]
        public Dictionary<string, ListType> Lists { get; set; }

        /// <summary>
        /// Gets or sets the templates.
        /// </summary>
        /// <value>
        /// The templates.
        /// </value>
        [JsonProperty(PropertyName = "templates")]
        public Dictionary<string, TemplateType> Templates { get; set; }

        /// <summary>
        /// Gets or sets the send.
        /// </summary>
        /// <value>
        /// The send.
        /// </value>
        [JsonProperty(PropertyName = "send")]
        public string Send { get; set; }

        /// <summary>
        /// Gets or sets the send vars.
        /// </summary>
        /// <value>
        /// The send vars.
        /// </value>
        [JsonProperty(PropertyName = "send_vars")]
        public Hashtable SendVars { get; set; }

        /// <summary>
        /// Gets or sets the vars.
        /// </summary>
        /// <value>
        /// The vars.
        /// </value>
        [JsonProperty(PropertyName = "vars")]
        public Hashtable Vars { get; set; }

        /// <summary>
        /// Gets or sets the SMS.
        /// </summary>
        /// <value>
        /// The SMS.
        /// </value>
        [JsonProperty(PropertyName = "sms")]
        public string Sms { get; set; }

        /// <summary>
        /// Gets or sets the twitter.
        /// </summary>
        /// <value>
        /// The twitter.
        /// </value>
        [JsonProperty(PropertyName = "twitter")]
        public string Twitter { get; set; }

        /// <summary>
        /// Gets or sets the change email.
        /// </summary>
        /// <value>
        /// The change email.
        /// </value>
        [JsonProperty(PropertyName = "change_email")]
        public string ChangeEmail { get; set; }
    }
}

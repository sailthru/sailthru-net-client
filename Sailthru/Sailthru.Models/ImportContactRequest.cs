// -----------------------------------------------------------------------
// <copyright file="ImportContactRequest.cs" company="Microsoft">
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

    /// <summary>
    /// Request object used for interaction with the contact API.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class ImportContactRequest
    {
        /// <summary>
        /// Flag to determine the name options.
        /// </summary>
        public enum NamesType
        {
            /// <summary>
            /// Do not import the contact names.
            /// </summary>
            Exclude = 0,

            /// <summary>
            /// Import the contact names.
            /// </summary>
            Include = 1
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
        /// Gets or sets the password.
        /// </summary>
        /// <value>
        /// The password.
        /// </value>
        [JsonProperty(PropertyName = "password")]
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the names.
        /// </summary>
        /// <value>
        /// The names.
        /// </value>
        [JsonProperty(PropertyName = "names")]
        public NamesType Names { get; set; }
    }
}

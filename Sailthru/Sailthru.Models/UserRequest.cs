using System.Collections;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Sailthru.Models
{
    public partial class UserRequest
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
        /// the fields to return.
        /// </summary>
        /// <value>The fields.</value>
        [JsonProperty(PropertyName = "fields")]
        public Hashtable Fields { get; set; }

        /// <summary>
        /// set some or all of the user keys.
        /// </summary>
        /// <value>The keys.</value>
        [JsonProperty(PropertyName = "keys")]
        public Hashtable Keys { private get; set; }

        /// <summary>
        /// define behavior if keys conflict with existing keys.
        /// </summary>
        /// <value>The keysconflict.</value>
        [JsonProperty(PropertyName = "keysconflict")]
        public string KeysConflict { private get; set; }

        /// <summary>
        /// Gets or sets the vars.
        /// </summary>
        /// <value>The vars.</value>
        [JsonProperty(PropertyName = "vars")]
        public Hashtable Vars { private get; set; }

        /// <summary>
        /// Gets or sets the anonymous cookie.
        /// </summary>
        /// <value>The cookies.</value>
        [JsonProperty(PropertyName = "cookies")]
        public Hashtable Cookies { private get; set; }

        /// <summary>
        /// set custom variables on the user
        /// </summary>
        /// <value>Lists.</value>
        [JsonProperty(PropertyName = "lists")]
        public Hashtable Lists { private get; set; }

        private OptoutStatus _optoutStatusField;

        /// <summary>
        /// set email opt-out status to none, all, or blast
        /// </summary>
        /// <value>The optout_email.</value>
        [JsonProperty("optout_email", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(StringEnumConverter))]
        public OptoutStatus OptoutEmail
        {
            private get
            {
                return _optoutStatusField;
            }
            set
            {
                _optoutStatusField = value;
                _optoutSet = true;
            }
        }

        public bool ShouldSerializeOptoutEmail()
        {
            return _optoutSet;
        }

        private bool _optoutSet = false;

        /// <summary>
        /// mark the user as logged in. Pass siteto represent a site login, appto represent an app
        /// login, and optionally user_agentand ip
        /// </summary>
        /// <value>The login.</value>
        [JsonProperty(PropertyName = "login")]
        public string Login { private get; set; }
    }
}

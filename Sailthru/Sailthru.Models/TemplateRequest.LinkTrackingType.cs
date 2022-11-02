namespace Sailthru.Models
{
    public partial class TemplateRequest
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
    }
}

namespace Sailthru.Models
{
    public partial class TemplateRequest
    {
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
    }
}

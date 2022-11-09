namespace Sailthru.Models
{
    public partial class BlastRequest
    {
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
    }
}

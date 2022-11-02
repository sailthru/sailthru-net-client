namespace Sailthru.Models
{
    public partial class BlastRequest
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
    }
}

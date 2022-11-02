namespace Sailthru.Models
{
    public partial class ContentRequest
    {
        /// <summary>
        /// Flag to determine whether to override exclude filtering options.
        /// </summary>
        public enum OverrideExcludeType
        {
            /// <summary>
            /// Disable override exclude.
            /// </summary>
            Disabled = 0,

            /// <summary>
            /// Enable override exclude.
            /// </summary>
            Enabled = 1
        }
    }
}

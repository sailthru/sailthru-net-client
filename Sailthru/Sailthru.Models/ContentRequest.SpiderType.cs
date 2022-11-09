namespace Sailthru.Models
{
    public partial class ContentRequest
    {
        /// <summary>
        /// Flag to determine the spider options.
        /// </summary>
        public enum SpiderType
        {
            /// <summary>
            /// Spider is disabled.
            /// </summary>
            Disabled = 0,

            /// <summary>
            /// Spider is enabled.
            /// </summary>
            Enabled = 1
        }
    }
}

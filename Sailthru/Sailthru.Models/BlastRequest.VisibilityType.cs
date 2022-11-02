namespace Sailthru.Models
{
    public partial class BlastRequest
    {
        /// <summary>
        /// Flag to determine the visibility of the blast.
        /// </summary>
        public enum VisibilityType
        {
            /// <summary>
            /// Blast is marked as private.
            /// </summary>
            Private = 0,

            /// <summary>
            /// Blast is marked as public.
            /// </summary>
            Public = 1
        }
    }
}

namespace Sailthru.Models
{
    public partial class BlastRequest
    {
        /// <summary>
        /// Flag to determine blast labels.
        /// </summary>
        public enum LabelType
        {
            /// <summary>
            /// Remove label from the blast.
            /// </summary>
            Remove = 0,

            /// <summary>
            /// Add the label to the blast.
            /// </summary>
            Add = 1
        }
    }
}

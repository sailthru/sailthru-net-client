using System.Collections;

namespace Sailthru
{
    internal class OrdinalComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            string s1 = (string)x;
            string s2 = (string)y;
            return string.CompareOrdinal(s1, s2);
        }
    }
}

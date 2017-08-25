using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Sailthru
{
    internal class OrdinalComparer : IComparer
    {
        #region IComparer<string> Members

        public int Compare(object x, object y)
        {
            string s1 = (string)x;
            string s2 = (string)y;
            return String.CompareOrdinal(s1, s2);
        }

        #endregion
    }
}

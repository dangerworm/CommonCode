using System;
using System.Collections.Generic;

namespace CommonCode.BusinessLayer.Helpers
{
    public class SemiNumericComparer : IComparer<string>
    {
        public int Compare(string s1, string s2)
        {
            var s1IsNumeric = s1.IsNumeric();
            var s2IsNumeric = s2.IsNumeric();

            if (s1IsNumeric && s2IsNumeric)
            {
                if (Convert.ToInt32(s1) < Convert.ToInt32(s2)) return -1;
                if (Convert.ToInt32(s1) == Convert.ToInt32(s2)) return 0;
                if (Convert.ToInt32(s1) > Convert.ToInt32(s2)) return 1;
            }

            if (s1IsNumeric && !s2IsNumeric)
            {
                return -1;
            }

            if (!s1IsNumeric && s2IsNumeric)
            {
                return 1;
            }

            return string.Compare(s1, s2, StringComparison.OrdinalIgnoreCase);
        }
    }
}
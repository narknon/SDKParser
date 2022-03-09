using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDKParser
{
    // From FModel/CUE4Parse
    public static class StringUtils
    {
        public static string SubstringBefore(this string s, string delimiter, StringComparison comparisonType = StringComparison.Ordinal)
        {
            var index = s.IndexOf(delimiter, comparisonType);
            return index == -1 ? s : s.Substring(0, index);
        }

        public static string SubstringAfter(this string s, string delimiter, StringComparison comparisonType = StringComparison.Ordinal)
        {
            var index = s.IndexOf(delimiter, comparisonType);
            return index == -1 ? s : s.Substring(index + delimiter.Length, s.Length - index - delimiter.Length);
        }
    }
}

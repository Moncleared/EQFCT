using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace EQFCT.Extension
{
    public static class StringExtensions
    {
        public static string TakeLastLines(this string text, int count)
        {
            List<string> lines = new List<string>();
            Match match = Regex.Match(text, "^.*$", RegexOptions.Singleline | RegexOptions.RightToLeft);

            while (match.Success && lines.Count < count)
            {
                lines.Insert(0, match.Value);
                match = match.NextMatch();
            }

            return string.Join("", lines);
        }
    }
}

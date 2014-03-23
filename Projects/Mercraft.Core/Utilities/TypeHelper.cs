using System;
using System.Text.RegularExpressions;

namespace Mercraft.Core.Utilities
{
    public static class SanitizeHelper
    {
        private static Regex RegexFloat = new Regex(@"(?<number>[0-9\.]*).*", RegexOptions.Compiled);
        
        public static string SanitizeFloat(string s)
        {
            var v = RegexFloat.Match(s);

            if(!v.Success)
                throw new ArgumentException("Unable to recognize float value", s);

            return v.Groups["number"].Value;
        }
    }
}

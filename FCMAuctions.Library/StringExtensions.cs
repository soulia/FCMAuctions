using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FCMAuctions.Library
{
    public static class StringExtensions
    {
        /// <summary>
        /// PS - Practical LINQ / LINQ By Example / Extenstion Methods
        /// http://msmvps.com/blogs/deborahk/
        /// Converts a list of strings to title case
        /// </summary>
        public static string ConvertToTitleCase(this string source)
        {
            CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
            TextInfo textInfo = cultureInfo.TextInfo;

            return textInfo.ToTitleCase(source);
        }

    }
}

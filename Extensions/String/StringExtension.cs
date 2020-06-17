using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Windows.Forms;

namespace intrapp.Extensions.String
{
    public static class StringExtension
    {
        /// <summary>
        /// Truncate the string if it is longer than the desired width in pixels.
        /// </summary>
        /// <param name="desiredWidth"></param>
        /// <returns></returns>
        public static string Truncate(this string value, int desiredWidth)
        {
            Font font = new Font("Helvetica Neue", 16, FontStyle.Regular, GraphicsUnit.Point);
            int strWidth = TextRenderer.MeasureText(value, font).Width;
            int length = value.Length * desiredWidth / strWidth;
            if (length < value.Length)
                return value.Substring(0, length);

            return value;
        }
    }
}
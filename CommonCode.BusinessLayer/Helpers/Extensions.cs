using System;
using System.Linq;
using System.Security;
using System.Text;

namespace CommonCode.BusinessLayer.Helpers
{
    public static partial class Extensions
    {
        public static string Escape(this string value)
        {
            return SecurityElement.Escape(value);
        }

        public static string Unescape(this string value)
        {
            return SecurityElement.FromString($"<xml>{value}</xml>")?.Text ?? "";
        }

        public static bool IsAny<T>(this T value, params T[] values)
        {
            return values.Contains(value);
        }

        public static bool IsNumeric(this object value)
        {
            if (value == null)
            {
                return false;
            }

            try
            {
                var integer = Convert.ToInt32(value.ToString());
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        public static string ToSentenceCase(this string input)
        {
            var sentences = input.Split("!.?".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            var builder = new StringBuilder();
            foreach (var sentence in sentences)
            {
                var newSentence = sentence.Trim();

                var indexToCapitalise = 0;
                while (indexToCapitalise < newSentence.Length &&
                       newSentence[indexToCapitalise].IsAny("\"'()*@".ToCharArray()))
                {
                    indexToCapitalise++;
                }

                if (builder.Length > 0)
                {
                    builder.Append(" ");
                }

                if (indexToCapitalise == newSentence.Length)
                {
                    builder.Append(newSentence + " ");
                    continue;
                }

                if (indexToCapitalise > 0)
                {
                    builder.Append(newSentence.Substring(0, indexToCapitalise));
                }

                builder.Append(newSentence[indexToCapitalise].ToString().ToUpper());

                builder.Append(newSentence.Substring(indexToCapitalise + 1));
            }

            return builder.ToString();
        }
    }
}
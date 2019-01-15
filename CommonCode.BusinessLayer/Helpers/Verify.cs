using System;
using System.Diagnostics;

namespace CommonCode.BusinessLayer.Helpers
{
    public static class Verify
    {
        private static void IsCorrect(bool condition, string parameterName, string message, bool correctValue)
        {
            if (condition == correctValue)
            {
                return;
            }

            if (!string.IsNullOrWhiteSpace(message))
            {
                message = $"The specified condition must be {Convert.ToString(correctValue)}.";
            }

            throw new ArgumentException(message, parameterName);
        }

        public static void IsTrue(bool condition, string parameterName, string message = null)
        {
            IsCorrect(condition, parameterName, message, true);
        }

        public static void IsFalse(bool condition, string parameterName, string message = null)
        {
            IsCorrect(condition, parameterName, message, false);
        }

        public static void NotNull(object parameter, string parameterName, string message = null)
        {
            if (parameter != null)
            {
                return;
            }

            if (!string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentNullException(parameterName, message);
            }

            var sourceMethodName = new StackTrace().GetFrame(1).GetMethod().Name;
            message = $"Parameter {parameterName} provided to {sourceMethodName} had a null value.";

            throw new ArgumentNullException(parameterName, message);
        }

        public static void ValidEnumValue<TEnum>(TEnum value, string parameterName)
        {
            var enumType = typeof(TEnum);

            if (!enumType.IsEnum)
            {
                throw new ArgumentException($"The specified type '{enumType.FullName}' is not a valid Enum");
            }

            if (!Enum.IsDefined(enumType, value))
            {
                throw new ArgumentOutOfRangeException(parameterName);
            }
        }

        public static void ValidString(string parameter, string parameterName, bool allowWhitespace = false)
        {
            if (allowWhitespace && string.IsNullOrEmpty(parameter) ||
                !allowWhitespace && string.IsNullOrWhiteSpace(parameter))
            {
                throw new ArgumentException(parameterName);
            }
        }
    }
}
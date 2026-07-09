using EcoBrotes.Domain.Exceptions;

namespace EcoBrotes.Domain.Common
{
    /// <summary>
    /// Static helper class with extension methods for argument validation.
    /// All methods throw RequiredException on failure and return the original value.
    /// </summary>
    public static class ArgumentValidator
    {
        /// <summary>
        /// Base validation method that throws RequiredException when condition is false.
        /// </summary>
        private static void Require(bool condition, string message)
        {
            if (!condition)
            {
                throw new RequiredException(message);
            }
        }

        /// <summary>
        /// Validates that the value is not null.
        /// </summary>
        public static object ValidateNull(this object? value, string message)
        {
            Require(value != null, message);
            return value!;
        }

        /// <summary>
        /// Validates that the string is not null or empty.
        /// </summary>
        public static string ValidateRequired(this string value, string message)
        {
            Require(!string.IsNullOrEmpty(value), message);
            return value;
        }

        /// <summary>
        /// Validates that the string length is within the specified range.
        /// </summary>
        public static string ValidateLength(this string value, int minimum, int maximum, string message)
        {
            Require(value.Length >= minimum && value.Length <= maximum, message);
            return value;
        }

        /// <summary>
        /// Validates that the decimal value is greater than zero.
        /// </summary>
        public static decimal ValidateGreaterThanZero(this decimal value, string message)
        {
            Require(value > 0, message);
            return value;
        }

        /// <summary>
        /// Validates that the integer value is greater than zero.
        /// </summary>
        public static int ValidateGreaterThanZero(this int value, string message)
        {
            Require(value > 0, message);
            return value;
        }

        /// <summary>
        /// Validates that the enum value is defined.
        /// </summary>
        public static TEnum ValidateEnum<TEnum>(this TEnum value, string message) where TEnum : struct, Enum
        {
            Require(Enum.IsDefined(value), message);
            return value;
        }

        /// <summary>
        /// Validates that the collection is not empty.
        /// </summary>
        public static IEnumerable<T> ValidateNotEmpty<T>(this IEnumerable<T> list, string message)
        {
            Require(list.Any(), message);
            return list;
        }

        /// <summary>
        /// Validates that the Guid is not empty.
        /// </summary>
        public static Guid ValidateNotEmpty(this Guid value, string message)
        {
            Require(value != Guid.Empty, message);
            return value;
        }
    }
}
        
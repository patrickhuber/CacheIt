using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CacheIt.Diagnostics
{
    /// <summary>
    /// Defines a set of asserts
    /// </summary>
    public class Assert
    {
        public static void IsNull(object value, string message)
        {
            if (value != null)
                throw new Exception(message);
        }

        public static void IsNull(object value)
        {
            IsNull(value, DiagnosticStrings.IsNullMessage);
        }

        /// <summary>
        /// Asserts that the given value is not null. If the value is null throws an exception with the given message.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="message">The message.</param>
        /// <exception cref="System.Exception">Throws an exception if the value is null.</exception>
        public static void IsNotNull(object value, string message)
        {
            if (value == null)
                throw new Exception(message);
        }

        /// <summary>
        /// Asserts that the given value is not null.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <exception cref="System.Exception">Throws an exception if the value is null.</exception>
        public static void IsNotNull(object value)
        {
            IsNotNull(value, DiagnosticStrings.IsNotNullMessage);
        }

        /// <summary>
        /// Determines whether [is not null or whitespace] [the specified value].
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="message">The message.</param>
        /// <exception cref="System.Exception"></exception>
        public static void IsNotNullOrWhitespace(string value, string message)
        { 
            if(string.IsNullOrWhiteSpace(value))
                throw new Exception(message);
        }

        /// <summary>
        /// Determines whether [is not null or whitespace] [the specified value].
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="message">The message.</param>
        /// <exception cref="System.Exception"></exception>
        public static void IsNotNullOrWhitespace(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new Exception(DiagnosticStrings.IsNotNullOrWhiteSpaceMesage);
        }

        /// <summary>
        /// Determines whether [is not null or empty] [the specified value].
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="message">The message.</param>
        /// <exception cref="System.Exception"></exception>
        public static void IsNotNullOrWhiteSpace(string value, string message)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new Exception(message);
        }

        /// <summary>
        /// throws an exception if the specified array is null or empty
        /// </summary>
        /// <param name="array">The array.</param>
        /// <param name="message">The message.</param>
        /// <exception cref="System.Exception"></exception>
        public static void IsNotNullOrEmpty<T>(T[] array, string message)
        {
            if (array == null || !array.Any())
                throw new Exception(message);
        }

        /// <summary>
        /// Determines whether the specified value is false.
        /// </summary>
        /// <param name="value">if set to <c>true</c> [value].</param>
        public static void IsFalse(bool value)
        {
            IsFalse(value, "value must be false.");
        }

        /// <summary>
        /// Determines whether the specified value is false.
        /// </summary>
        /// <param name="value">if set to <c>true</c> [value].</param>
        /// <param name="message">The message.</param>
        /// <exception cref="System.Exception"></exception>
        public static void IsFalse(bool value, string message)
        {
            if (value)
                throw new Exception(message);
        }

        /// <summary>
        /// Determines whether the specified value is true.
        /// </summary>
        /// <param name="value">if set to <c>true</c> [value].</param>
        public static void IsTrue(bool value)
        {
            IsTrue(value, "value must be true.");
        }

        /// <summary>
        /// Determines whether the specified value is true.
        /// </summary>
        /// <param name="value">if set to <c>true</c> [value].</param>
        /// <exception cref="System.Exception">value must be true.</exception>
        public static void IsTrue(bool value, string message)
        {
            if (!value)
                throw new Exception(message);
        }
    }
}

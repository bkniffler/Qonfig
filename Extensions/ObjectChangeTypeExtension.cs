using System;
using System.Globalization;

namespace Qonfig.Extensions
{
    public static class ObjectChangeTypeExtension
    {
        /// <summary>
        /// Convert object to T
        /// </summary>
        /// <typeparam name="T">Type of result</typeparam>
        /// <param name="value">Value to convert</param>
        /// <returns>Result</returns>
        public static T ChangeType<T>(this object value)
        {
            var cultureInfo = CultureInfo.CurrentCulture;

            var toType = typeof(T);

            if (value == null) return default(T);

            if (value is string)
            {
                if (toType == typeof(Guid))
                {
                    return ChangeType<T>(new Guid(Convert.ToString(value, cultureInfo)));
                }
                if ((string)value == string.Empty && toType != typeof(string))
                {
                    return ChangeType<T>(null);
                }
            }
            else
            {
                if (typeof(T) == typeof(string))
                {
                    return ChangeType<T>(Convert.ToString(value, cultureInfo));
                }
            }

            if (toType.IsGenericType &&
                toType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                toType = Nullable.GetUnderlyingType(toType); ;
            }

            var canConvert = toType is IConvertible || (toType.IsValueType && !toType.IsEnum);
            if (canConvert)
            {
                return (T)Convert.ChangeType(value, toType, cultureInfo);
            }
            return (T)value;
        }
    }
}
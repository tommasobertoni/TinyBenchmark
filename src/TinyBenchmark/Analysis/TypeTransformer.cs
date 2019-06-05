using System;
using System.Collections.Generic;
using System.Text;

namespace TinyBenchmark.Analysis
{
    internal class TypeTransformer
    {
        public void EnsureIsCompatible(object value, Type targetType, string errorInfo = null)
        {
            this.EnsureCompatibleIfNull(value, targetType, errorInfo);

            if (value != null &&
                !this.IsAssignableTo(value, targetType) &&
                !CanConvertTo(value, targetType))
            {
                throw IncompatibleTypesException(value, targetType, errorInfo);
            }
        }

        public object ConvertFor(object value, Type targetType, string errorInfo = null)
        {
            this.EnsureCompatibleIfNull(value, targetType, errorInfo);

            // At this point, a null value is acceptable.
            if (value == null) return value;

            if (this.IsAssignableTo(value, targetType))
                return value;

            // Value is not null and is not assignable as-is.

            try
            {
                // Try to convert the value to the target type.
                var conversionType = GetConversionType(value, targetType);
                var convertedValue = Convert.ChangeType(value, conversionType);
                return convertedValue;
            }
            catch (Exception ex)
            {
                throw IncompatibleTypesException(value, targetType, errorInfo, ex);
            }
        }

        #region Type compatibility validation

        private void EnsureCompatibleIfNull(object value, Type targetType, string errorInfo = null)
        {
            if (value == null && targetType.IsValueType)
            {
                // null can only be assigned to a ref type or a nullable value type.
                var underlyingType = Nullable.GetUnderlyingType(targetType);

                if (underlyingType == null)
                    // This type is not nullable.
                    throw CannotAssignNullValueToTypeException(targetType, errorInfo);
            }
        }

        private bool IsAssignableTo(object value, Type targetType) =>
            targetType.IsAssignableFrom(value.GetType());

        private bool CanConvertTo(object value, Type targetType)
        {
            try
            {
                var conversionType = GetConversionType(value, targetType);
                var _ = Convert.ChangeType(value, conversionType);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private Type GetConversionType(object value, Type targetType)
        {
            if (targetType.IsByRef) return targetType;

            var underlyingType = Nullable.GetUnderlyingType(targetType);
            return underlyingType ?? targetType;
        }

        #endregion

        #region Exception helpers

        private InvalidOperationException IncompatibleTypesException(
            object value,
            Type targetType,
            string errorInfo = null,
            Exception innerException = null) =>
            new InvalidOperationException(
                $"Cannot assign a value of type {value.GetType().Name} to the type {targetType.Name}." +
                $"{errorInfo ?? string.Empty}".Trim(), innerException);

        private InvalidOperationException CannotAssignNullValueToTypeException(
            Type targetType,
            string errorInfo = null,
            Exception innerException = null) =>
            new InvalidOperationException(
                $"Cannot assign null to the type {targetType.Name}." +
                $"{errorInfo ?? string.Empty}".Trim(), innerException);

        #endregion
    }
}

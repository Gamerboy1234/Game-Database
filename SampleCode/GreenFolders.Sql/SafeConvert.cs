
using System;

namespace GreenFolders.Sql
{
    public static class SafeConvert
    {
        #region Public Fields

        public static DateTime MinDateTimeValue = new DateTime(1900, 1, 1);

        #endregion Public Fields


        #region Public Methods

        public static char ToChar(string value, char defaultValue = (char)0)
        {
            var result = defaultValue;

            try
            {
                if (!string.IsNullOrEmpty(value))
                {
                    result = Convert.ToChar(value);
                }
            }

            catch (Exception ex)
            {
                Logger.Error(ex);

                result = defaultValue;
            }

            return result;
        }

        public static char ToChar(object value, char defaultValue = (char)0)
        {
            var result = defaultValue;

            try
            {
                if (!string.IsNullOrEmpty(value?.ToString()))
                {
                    result = Convert.ToChar(value);
                }
            }

            catch (Exception ex)
            {
                Logger.Error(ex);

                result = defaultValue;
            }

            return result;
        }

        public static byte ToByte(string value, byte defaultValue = 0)
        {
            var result = defaultValue;

            try
            {
                if (!string.IsNullOrEmpty(value))
                {
                    result = Convert.ToByte(value);
                }
            }

            catch (Exception ex)
            {
                Logger.Error(ex);

                result = defaultValue;
            }

            return result;
        }

        public static byte ToByte(object value, byte defaultValue = 0)
        {
            var result = defaultValue;

            try
            {
                if (!string.IsNullOrEmpty(value?.ToString()))
                {
                    result = Convert.ToByte(value);
                }
            }

            catch (Exception ex)
            {
                Logger.Error(ex);

                result = defaultValue;
            }

            return result;
        }

        public static short ToInt16(string value, short defaultValue = 0)
        {
            var result = defaultValue;

            try
            {
                if (!string.IsNullOrEmpty(value))
                {
                    result = Convert.ToInt16(value);
                }
            }

            catch (Exception ex)
            {
                Logger.Error(ex);

                result = defaultValue;
            }

            return result;
        }

        public static short ToInt16(object value, short defaultValue = 0)
        {
            var result = defaultValue;

            try
            {
                if (!string.IsNullOrEmpty(value?.ToString()))
                {
                    result = Convert.ToInt16(value);
                }
            }

            catch (Exception ex)
            {
                Logger.Error(ex);

                result = defaultValue;
            }

            return result;
        }

        public static ushort ToUInt16(string value, ushort defaultValue = 0)
        {
            var result = defaultValue;

            try
            {
                if (!string.IsNullOrEmpty(value))
                {
                    result = Convert.ToUInt16(value);
                }
            }

            catch (Exception ex)
            {
                Logger.Error(ex);

                result = defaultValue;
            }

            return result;
        }

        public static ushort ToUInt16(object value, ushort defaultValue = 0)
        {
            var result = defaultValue;

            try
            {
                if (!string.IsNullOrEmpty(value?.ToString()))
                {
                    result = Convert.ToUInt16(value);
                }
            }

            catch (Exception ex)
            {
                Logger.Error(ex);

                result = defaultValue;
            }

            return result;
        }

        public static int ToInt32(string value, int defaultValue = 0)
        {
            var result = defaultValue;

            try
            {
                if (!string.IsNullOrEmpty(value))
                {
                    result = Convert.ToInt32(value);
                }
            }

            catch (Exception ex)
            {
                Logger.Error(ex);

                result = defaultValue;
            }

            return result;
        }

        public static int ToInt32(object value, int defaultValue = 0)
        {
            var result = defaultValue;

            try
            {
                if (!string.IsNullOrEmpty(value?.ToString()))
                {
                    result = Convert.ToInt32(value);
                }
            }

            catch (Exception ex)
            {
                Logger.Error(ex);

                result = defaultValue;
            }

            return result;
        }

        public static uint ToUInt32(string value, uint defaultValue = 0)
        {
            var result = defaultValue;

            try
            {
                if (!string.IsNullOrEmpty(value))
                {
                    result = Convert.ToUInt32(value);
                }
            }

            catch (Exception ex)
            {
                Logger.Error(ex);

                result = defaultValue;
            }

            return result;
        }

        public static uint ToUInt32(object value, uint defaultValue = 0)
        {
            var result = defaultValue;

            try
            {
                if (!string.IsNullOrEmpty(value?.ToString()))
                {
                    result = Convert.ToUInt32(value);
                }
            }

            catch (Exception ex)
            {
                Logger.Error(ex);

                result = defaultValue;
            }

            return result;
        }

        public static long ToInt64(string value, long defaultValue = 0)
        {
            var result = defaultValue;

            try
            {
                if (!string.IsNullOrEmpty(value))
                {
                    result = Convert.ToInt64(value);
                }
            }

            catch (Exception ex)
            {
                Logger.Error(ex);

                result = defaultValue;
            }

            return result;
        }

        public static long ToInt64(object value, long defaultValue = 0)
        {
            var result = defaultValue;

            try
            {
                if (!string.IsNullOrEmpty(value?.ToString()))
                {
                    result = Convert.ToInt64(value);
                }
            }

            catch (Exception ex)
            {
                Logger.Error(ex);

                result = defaultValue;
            }

            return result;
        }

        public static ulong ToUInt64(string value, ulong defaultValue = 0)
        {
            var result = defaultValue;

            try
            {
                if (!string.IsNullOrEmpty(value))
                {
                    result = Convert.ToUInt64(value);
                }
            }

            catch (Exception ex)
            {
                Logger.Error(ex);

                result = defaultValue;
            }

            return result;
        }

        public static ulong ToUInt64(object value, ulong defaultValue = 0)
        {
            var result = defaultValue;

            try
            {
                if (!string.IsNullOrEmpty(value?.ToString()))
                {
                    result = Convert.ToUInt64(value);
                }
            }

            catch (Exception ex)
            {
                Logger.Error(ex);

                result = defaultValue;
            }

            return result;
        }

        public static float ToSingle(string value, float defaultValue = 0)
        {
            var result = defaultValue;

            try
            {
                if (!string.IsNullOrEmpty(value))
                {
                    result = Convert.ToSingle(value);
                }
            }

            catch (Exception ex)
            {
                Logger.Error(ex);

                result = defaultValue;
            }

            return result;
        }

        public static float ToSingle(object value, float defaultValue = 0)
        {
            var result = defaultValue;

            try
            {
                if (!string.IsNullOrEmpty(value?.ToString()))
                {
                    result = Convert.ToSingle(value);
                }
            }

            catch (Exception ex)
            {
                Logger.Error(ex);

                result = defaultValue;
            }

            return result;
        }

        public static double ToDouble(string value, double defaultValue = 0)
        {
            var result = defaultValue;

            try
            {
                if (!string.IsNullOrEmpty(value))
                {
                    result = Convert.ToDouble(value);
                }
            }

            catch (Exception ex)
            {
                Logger.Error(ex);

                result = defaultValue;
            }

            return result;
        }

        public static double ToDouble(object value, double defaultValue = 0)
        {
            var result = defaultValue;

            try
            {
                if (!string.IsNullOrEmpty(value?.ToString()))
                {
                    result = Convert.ToDouble(value);
                }
            }

            catch (Exception ex)
            {
                Logger.Error(ex);

                result = defaultValue;
            }

            return result;
        }

        public static decimal ToDecimal(string value, decimal defaultValue = 0)
        {
            var result = defaultValue;

            try
            {
                if (!string.IsNullOrEmpty(value))
                {
                    result = Convert.ToDecimal(value);
                }
            }

            catch (Exception ex)
            {
                Logger.Error(ex);

                result = defaultValue;
            }

            return result;
        }

        public static decimal ToDecimal(object value, decimal defaultValue = 0)
        {
            var result = defaultValue;

            try
            {
                if (!string.IsNullOrEmpty(value?.ToString()))
                {
                    result = Convert.ToDecimal(value);
                }
            }

            catch (Exception ex)
            {
                Logger.Error(ex);

                result = defaultValue;
            }

            return result;
        }

        public static bool ToBoolean(string value)
        {
            var result = false;

            try
            {
                if (!string.IsNullOrEmpty(value))
                {
                    switch (value.ToLower())
                    {
                        case "1":
                        case "true":
                            result = true;
                            break;
                        case "0":
                        case "false":
                            break;
                        default:
                            result = Convert.ToBoolean(value);
                            break;
                    }
                }
            }

            catch (Exception ex)
            {
                Logger.Error(ex);

                result = false;
            }

            return result;
        }

        public static bool ToBoolean(object value)
        {
            var result = false;

            try
            {
                if ((value != null) && 
                    !string.IsNullOrEmpty(value.ToString()))
                {
                    if (value.ToString().Trim() == "1")
                    {
                        result = true;
                    }

                    else if (value.ToString().Trim() == "0")
                    {
                        // Already false
                    }

                    else
                    {
                        result = Convert.ToBoolean(value);
                    }
                }
            }

            catch (Exception ex)
            {
                Logger.Error(ex);

                result = false;
            }

            return result;
        }

        public static string ToString(string value, string defaultValue = "")
        {
            var result = defaultValue;

            try
            {
                if (!string.IsNullOrEmpty(value))
                {
                    result = Convert.ToString(value);
                }
            }

            catch (Exception ex)
            {
                Logger.Error(ex);

                result = defaultValue;
            }

            return result;
        }

        public static string ToString(object value, string defaultValue = "")
        {
            var result = defaultValue;

            try
            {
                if (!string.IsNullOrEmpty(value?.ToString()))
                {
                    result = Convert.ToString(value);
                }
            }

            catch (Exception ex)
            {
                Logger.Error(ex);

                result = defaultValue;
            }

            return result;
        }

        public static DateTime ToDateTime(string value, bool containsOffset = false)
        {
            var result = MinDateTimeValue;

            try
            {
                if (!string.IsNullOrEmpty(value))
                {
                    result = containsOffset ? DateTimeOffset.Parse(value).LocalDateTime : Convert.ToDateTime(value);
                }
            }

            catch (Exception ex)
            {
                Logger.Error(ex);

                result = MinDateTimeValue;
            }

            return result;
        }

        public static DateTime ToDateTime(object value)
        {
            var result = MinDateTimeValue;

            try
            {
                if (!string.IsNullOrEmpty(value?.ToString()))
                {
                    result = Convert.ToDateTime(value);
                }
            }

            catch (Exception ex)
            {
                Logger.Error(ex);

                result = MinDateTimeValue;
            }

            return result;
        }

        public static Guid ToGuid(string value)
        {
            var result = new Guid();

            try
            {
                var stringValue = (value ?? "").Trim();

                if (!string.IsNullOrEmpty(stringValue) &&
                    (stringValue.Length == 36))
                {
                    result = new Guid(stringValue);
                }
            }

            catch (Exception ex)
            {
                Logger.Error(ex);

                result = new Guid();
            }

            return result;
        }

        public static Guid ToGuid(object value)
        {
            var result = new Guid();

            try
            {
                var stringValue = (value?.ToString() ?? "").Trim();

                if (!string.IsNullOrEmpty(stringValue) &&
                    (stringValue.Length == 36))
                {
                    result = new Guid(stringValue);
                }
            }

            catch (Exception ex)
            {
                Logger.Error(ex);

                result = new Guid();
            }

            return result;
        }

        public static object ChangeType(object value, Type type)
        {
            var result = value;

            try
            {
                if ((value != null) && (type != null))
                {
                    switch (Type.GetTypeCode(type))
                    {
                        case TypeCode.Char:
                            result = ToChar(value);
                            break;
                        case TypeCode.Byte:
                            result = ToByte(value);
                            break;
                        case TypeCode.Int16:
                            result = ToInt16(value);
                            break;
                        case TypeCode.UInt16:
                            result = ToUInt16(value);
                            break;
                        case TypeCode.Int32:
                            result = ToInt32(value);
                            break;
                        case TypeCode.UInt32:
                            result = ToUInt32(value);
                            break;
                        case TypeCode.Int64:
                            result = ToInt64(value);
                            break;
                        case TypeCode.UInt64:
                            result = ToUInt64(value);
                            break;
                        case TypeCode.Single:
                            result = ToSingle(value);
                            break;
                        case TypeCode.Double:
                            result = ToDouble(value);
                            break;
                        case TypeCode.Decimal:
                            result = ToDecimal(value);
                            break;
                        case TypeCode.Boolean:
                            result = ToBoolean(value);
                            break;
                        case TypeCode.DateTime:
                            result = ToDateTime(value);
                            break;
                        default:
                            if (value is Guid)
                            {
                                result = ToGuid(value);
                            }
                            break;
                    }
                }
            }

            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            return result;
        }

        #endregion Public Methods
    }
}

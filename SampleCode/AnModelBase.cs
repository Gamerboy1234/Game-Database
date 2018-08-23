
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using ActiveNet.Core.Log;
using ActiveNet.Types.Database;

namespace ActiveNet.Types.Base
{
    [Serializable]
    public abstract class AnModelBase
    {
        #region Constants

        private const string ToDictionaryListMethodName = "ToDictionaryList";
        private const string FromDictionaryListMethodName = "FromDictionaryList";
        private const string GetDbTableNameMethodName = "GetDbTableName";
        
        #endregion Constants


        #region Private Methods

        private static readonly Dictionary<Type, string> DbTableNameCache = new Dictionary<Type, string>();
        private static readonly Dictionary<string, List<string>> ColumnNamesCache = new Dictionary<string, List<string>>();
        private static readonly Dictionary<string, Dictionary<string, Type>> PropertyTypeCache = new Dictionary<string, Dictionary<string, Type>>();
        private static readonly Dictionary<string, Dictionary<string, MethodInfo>> ToDictionaryListMethodCache = new Dictionary<string, Dictionary<string, MethodInfo>>();
        private static readonly Dictionary<string, Dictionary<string, MethodInfo>> FromDictionaryListMethodCache = new Dictionary<string, Dictionary<string, MethodInfo>>();

        #endregion Private Methods


        #region Abstract Methods

        // ReSharper disable once MemberCanBeProtected.Global (Ron Note: This method cannot be protected because reflection is used to get and invoke it.)
        public abstract string GetDbTableName();

        protected abstract string GetKeyColumnName();
        protected abstract List<string> GenerateColumnNames(bool includeIdColumn);
        protected abstract List<AnDatabaseCommandData> GenerateDatabaseCommandDataList(bool includeIdColumn);

        #endregion Abstract Methods


        #region Public Methods

        public List<string> GetColumnNames(bool includeIdColumn)
        {
            List<string> result = null;

            try
            {
                var dbTableName = GetDbTableName();

                if (!string.IsNullOrEmpty(dbTableName))
                {
                    if (!ColumnNamesCache.ContainsKey(dbTableName))
                    {
                        var columnNames = GenerateColumnNames(true);

                        if (columnNames?.Count > 0)
                        {
                            ColumnNamesCache.Add(dbTableName, columnNames);
                        }
                    }

                    if (ColumnNamesCache.ContainsKey(dbTableName))
                    {
                        result = AnCloneUtility.JsonClone(ColumnNamesCache[dbTableName]);

                        if (result?.Count > 0)
                        {
                            if (!includeIdColumn)
                            {
                                var keyColumnName = GetKeyColumnName();

                                if (!string.IsNullOrEmpty(keyColumnName) &&
                                    result.Contains(keyColumnName))
                                {
                                    result.Remove(keyColumnName);
                                }
                            }
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                AnLog.Error(ex);
            }

            return result ?? new List<string>();
        }

        public virtual string GenerateInsertStatement(AnDatabaseTypes.DriverType driverType)
        {
            var result = "";

            try
            {
                result = AnDatabaseTypes.GenerateInsertStatement(GetDbTableName(), GenerateDatabaseCommandDataList(false), driverType, false);
            }

            catch (Exception ex)
            {
                AnLog.Error(ex);
            }

            return result;
        }

        public string GenerateUpdateStatement<T>(AnDatabaseTypes.DriverType driverType, T modelObject)
        {
            var result = "";

            try
            {
                var whereClause = GeneratePrimaryKeyWhereClause(driverType);

                if (!string.IsNullOrEmpty(whereClause))
                {
                    result = AnDatabaseTypes.GenerateUpdateStatement(GetDbTableName(), whereClause, GenerateDatabaseCommandDataList(false), (modelObject as AnModelBase)?.GenerateDatabaseCommandDataList(false) ?? new AnList<AnDatabaseCommandData>(), driverType, false);
                }
            }

            catch (Exception ex)
            {
                AnLog.Error(ex);
            }

            return result;
        }

        public string GenerateDeleteStatement(AnDatabaseTypes.DriverType driverType)
        {
            var result = "";

            try
            {
                var whereClause = GeneratePrimaryKeyWhereClause(driverType);

                if (!string.IsNullOrEmpty(whereClause))
                {
                    result = $"DELETE FROM {GetDbTableName()} WHERE {whereClause}";
                }
            }

            catch (Exception ex)
            {
                AnLog.Error(ex);
            }

            return result;
        }

        public string GenerateExistsQuery(AnDatabaseTypes.DriverType driverType)
        {
            var result = "";

            try
            {
                var keyColumnName = GetKeyColumnName();

                if (!string.IsNullOrEmpty(keyColumnName))
                {
                    var whereClause = GeneratePrimaryKeyWhereClause(driverType);

                    if (!string.IsNullOrEmpty(whereClause))
                    {
                        result = AnDatabaseTypes.GenerateSelectQuery(driverType, GetDbTableName(), AnDatabaseTypes.FormatDatabaseColumnName(keyColumnName, GetDbTableName(), AnDatabaseTypes.ExecutionType.Query, driverType), whereClause);
                    }
                }
            }

            catch (Exception ex)
            {
                AnLog.Error(ex);
            }

            return result;
        }

        public string GenerateSelectQuery(AnDatabaseTypes.DriverType driverType)
        {
            return AnDatabaseTypes.GenerateSelectQuery(driverType, GetDbTableName(), AnDatabaseTypes.FormatColumnNames(GetDbTableName(), GetColumnNames(true), driverType), GeneratePrimaryKeyWhereClause(driverType));
        }

        public virtual string GeneratePrimaryKeyWhereClause(AnDatabaseTypes.DriverType driverType)
        {
            var result = "";

            try
            {
                var keyColumnName = GetKeyColumnName();

                if (!string.IsNullOrEmpty(keyColumnName))
                {
                    var columnData = GenerateDatabaseCommandDataList(true);

                    if (columnData?.Count > 0)
                    {
                        foreach (var column in columnData.Where(column => !string.IsNullOrEmpty(column?.Name) && string.Equals(column.Name, keyColumnName, StringComparison.CurrentCultureIgnoreCase)))
                        {
                            string columnValue = null;

                            switch (column.DataType)
                            {
                                case AnTypes.DataType.Character:
                                    columnValue = AnSafeConvert.ToChar(column.Value).ToString();
                                    break;
                                case AnTypes.DataType.Byte:
                                    columnValue = AnSafeConvert.ToByte(column.Value).ToString();
                                    break;
                                case AnTypes.DataType.Short:
                                    columnValue = AnSafeConvert.ToInt16(column.Value).ToString();
                                    break;
                                case AnTypes.DataType.UShort:
                                    columnValue = AnSafeConvert.ToUInt16(column.Value).ToString();
                                    break;
                                case AnTypes.DataType.Int:
                                    columnValue = AnSafeConvert.ToInt32(column.Value).ToString();
                                    break;
                                case AnTypes.DataType.UInt:
                                    columnValue = AnSafeConvert.ToUInt32(column.Value).ToString();
                                    break;
                                case AnTypes.DataType.Long:
                                    columnValue = AnSafeConvert.ToInt64(column.Value).ToString();
                                    break;
                                case AnTypes.DataType.ULong:
                                    columnValue = AnSafeConvert.ToUInt64(column.Value).ToString();
                                    break;
                                case AnTypes.DataType.Single:
                                    columnValue = AnSafeConvert.ToSingle(column.Value).ToString(CultureInfo.InvariantCulture);
                                    break;
                                case AnTypes.DataType.Double:
                                    columnValue = AnSafeConvert.ToDouble(column.Value).ToString(CultureInfo.InvariantCulture);
                                    break;
                                case AnTypes.DataType.Decimal:
                                case AnTypes.DataType.Currency:
                                    columnValue = AnSafeConvert.ToDecimal(column.Value).ToString(CultureInfo.InvariantCulture);
                                    break;
                                case AnTypes.DataType.Boolean:
                                    columnValue = AnSafeConvert.ToBoolean(column.Value).ToString(CultureInfo.InvariantCulture);
                                    break;
                                case AnTypes.DataType.DateTime:
                                case AnTypes.DataType.FileTime:
                                    columnValue = AnSafeConvert.ToDateTime(column.Value).ToString(CultureInfo.InvariantCulture);
                                    break;
                                case AnTypes.DataType.Guid:
                                    columnValue = AnSafeConvert.ToGuid(column.Value).ToString();
                                    break;
                                case AnTypes.DataType.Char:
                                case AnTypes.DataType.VarChar:
                                case AnTypes.DataType.Text:
                                case AnTypes.DataType.NChar:
                                case AnTypes.DataType.NVarChar:
                                case AnTypes.DataType.NText:
                                    columnValue = $"'{AnSafeConvert.ToString(column.Value)}'";
                                    break;
                                // These types cannot be primary keys.
                                //case AnTypes.DataType.Image:
                                //case AnTypes.DataType.Binary:
                                //case AnTypes.DataType.VarBinary:
                                //case AnTypes.DataType.Null:
                                //case AnTypes.DataType.Table:
                                //default:
                                //break;
                            }

                            if (columnValue != null)
                            {
                                result = $"{AnDatabaseTypes.FormatDatabaseColumnName(keyColumnName, GetDbTableName(), AnDatabaseTypes.ExecutionType.Query, driverType)} = {columnValue}";
                            }

                            break;
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                AnLog.Error(ex);
            }

            return result;
        }

        #endregion Public Methods


        #region Dictionary Methods

        public static Dictionary<string, object> ToDictionary<T>(T modelObject)
        {
            var result = new Dictionary<string, object>();

            try
            {
                if (modelObject != null)
                {
                    var defaultObject = Activator.CreateInstance<T>();

                    var type = typeof(T);

                    var dbTableName = "";

                    if (DbTableNameCache.ContainsKey(type))
                    {
                        dbTableName = DbTableNameCache[type];
                    }

                    else
                    {
                        var getDbTableNameMethod = type.GetMethod(GetDbTableNameMethodName);
    
                        if (getDbTableNameMethod != null)
                        {
                            dbTableName = AnSafeConvert.ToString(getDbTableNameMethod.Invoke(defaultObject, null));

                            if (!string.IsNullOrEmpty(dbTableName))
                            {
                                DbTableNameCache.Add(type, dbTableName);
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(dbTableName))
                    {
                        var columnData = (defaultObject as AnModelBase)?.GenerateDatabaseCommandDataList(true);

                        if (columnData?.Count > 0)
                        {
                            foreach (var column in columnData.Where(column => !string.IsNullOrEmpty(column?.Name)))
                            {
                                var value = type.GetProperty(column.Name)?.GetValue(modelObject, null);

                                switch (column.DataType)
                                {
                                    case AnTypes.DataType.Character:
                                        AnTypes.SetDictionaryValue(result, column.Name, AnSafeConvert.ToChar(value));
                                        break;
                                    case AnTypes.DataType.Byte:
                                        AnTypes.SetDictionaryValue(result, column.Name, AnSafeConvert.ToByte(value));
                                        break;
                                    case AnTypes.DataType.Short:
                                        AnTypes.SetDictionaryValue(result, column.Name, AnSafeConvert.ToInt16(value));
                                        break;
                                    case AnTypes.DataType.UShort:
                                        AnTypes.SetDictionaryValue(result, column.Name, AnSafeConvert.ToUInt16(value));
                                        break;
                                    case AnTypes.DataType.Int:
                                        AnTypes.SetDictionaryValue(result, column.Name, AnSafeConvert.ToInt32(value));
                                        break;
                                    case AnTypes.DataType.UInt:
                                        AnTypes.SetDictionaryValue(result, column.Name, AnSafeConvert.ToUInt32(value));
                                        break;
                                    case AnTypes.DataType.Long:
                                        AnTypes.SetDictionaryValue(result, column.Name, AnSafeConvert.ToInt64(value));
                                        break;
                                    case AnTypes.DataType.ULong:
                                        AnTypes.SetDictionaryValue(result, column.Name, AnSafeConvert.ToUInt64(value));
                                        break;
                                    case AnTypes.DataType.Single:
                                        AnTypes.SetDictionaryValue(result, column.Name, AnSafeConvert.ToSingle(value));
                                        break;
                                    case AnTypes.DataType.Double:
                                        AnTypes.SetDictionaryValue(result, column.Name, AnSafeConvert.ToDouble(value));
                                        break;
                                    case AnTypes.DataType.Decimal:
                                    case AnTypes.DataType.Currency:
                                        AnTypes.SetDictionaryValue(result, column.Name, AnSafeConvert.ToDecimal(value));
                                        break;
                                    case AnTypes.DataType.Boolean:
                                        AnTypes.SetDictionaryValue(result, column.Name, AnSafeConvert.ToBoolean(value));
                                        break;
                                    case AnTypes.DataType.DateTime:
                                    case AnTypes.DataType.FileTime:
                                        AnTypes.SetDictionaryValue(result, column.Name, AnSafeConvert.ToDateTime(value));
                                        break;
                                    case AnTypes.DataType.Guid:
                                        AnTypes.SetDictionaryValue(result, column.Name, AnSafeConvert.ToGuid(value));
                                        break;
                                    case AnTypes.DataType.Char:
                                    case AnTypes.DataType.VarChar:
                                    case AnTypes.DataType.Text:
                                    case AnTypes.DataType.NChar:
                                    case AnTypes.DataType.NVarChar:
                                    case AnTypes.DataType.NText:
                                        AnTypes.SetDictionaryValue(result, column.Name, AnSafeConvert.ToString(value));
                                        break;
                                    case AnTypes.DataType.Image:
                                    case AnTypes.DataType.Binary:
                                    case AnTypes.DataType.VarBinary:
                                        AnTypes.SetDictionaryValue(result, column.Name, value as byte[]);
                                        break;
                                    case AnTypes.DataType.Table:
                                        {
                                            if (!ToDictionaryListMethodCache.ContainsKey(dbTableName))
                                            {
                                                ToDictionaryListMethodCache.Add(dbTableName, new Dictionary<string, MethodInfo>());
                                            }

                                            if (!ToDictionaryListMethodCache[dbTableName].ContainsKey(column.Name))
                                            {
                                                ToDictionaryListMethodCache[dbTableName].Add(column.Name, null);
                                            }

                                            if (ToDictionaryListMethodCache[dbTableName][column.Name] == null)
                                            {
                                                if (!PropertyTypeCache.ContainsKey(dbTableName))
                                                {
                                                    PropertyTypeCache.Add(dbTableName, new Dictionary<string, Type>());
                                                }

                                                if (!PropertyTypeCache[dbTableName].ContainsKey(column.Name))
                                                {
                                                    PropertyTypeCache[dbTableName].Add(column.Name, null);
                                                }

                                                if (PropertyTypeCache[dbTableName][column.Name] == null)
                                                {
                                                    PropertyTypeCache[dbTableName][column.Name] = type.GetProperty(column.Name)?.PropertyType;
                                                }

                                                if (PropertyTypeCache[dbTableName][column.Name] != null)
                                                {
                                                    ToDictionaryListMethodCache[dbTableName][column.Name] = PropertyTypeCache[dbTableName][column.Name].GetMethod(ToDictionaryListMethodName);
                                                }
                                            }

                                            if (ToDictionaryListMethodCache[dbTableName][column.Name] != null)
                                            {
                                                AnTypes.SetDictionaryValue(result, column.Name, ToDictionaryListMethodCache[dbTableName][column.Name].Invoke(null, new[] {value}));
                                            }
                                        }
                                        break;
                                    //case AnTypes.DataType.Null:
                                    //default:
                                    //break;
                                }
                            }
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                AnLog.Error(ex);
            }

            return result;
        }

        public static T FromDictionary<T>(IDictionary<string, object> dictionary)
        {
            var result = default(T);

            try
            {
                if ((dictionary != null) &&
                    (dictionary.Count > 0))
                {
                    var defaultObject = Activator.CreateInstance<T>();

                    var type = typeof(T);

                    var dbTableName = "";

                    if (DbTableNameCache.ContainsKey(type))
                    {
                        dbTableName = DbTableNameCache[type];
                    }

                    else
                    {
                        var getDbTableNameMethod = type.GetMethod(GetDbTableNameMethodName);
    
                        if (getDbTableNameMethod != null)
                        {
                            dbTableName = AnSafeConvert.ToString(getDbTableNameMethod.Invoke(defaultObject, null));

                            if (!string.IsNullOrEmpty(dbTableName))
                            {
                                DbTableNameCache.Add(type, dbTableName);
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(dbTableName))
                    {
                        var columnData = (defaultObject as AnModelBase)?.GenerateDatabaseCommandDataList(true);

                        if (columnData?.Count > 0)
                        {
                            foreach (var key in dictionary.Keys)
                            {
                                foreach (var column in columnData.Where(column => !string.IsNullOrEmpty(column?.Name) && string.Equals(column.Name, key, StringComparison.CurrentCultureIgnoreCase)))
                                {
                                    if (result == null)
                                    {
                                        result = defaultObject;
                                    }

                                    object value = null;

                                    switch (column.DataType)
                                    {
                                        case AnTypes.DataType.Character:
                                            value = AnSafeConvert.ToChar(dictionary[key]);
                                            break;
                                        case AnTypes.DataType.Byte:
                                            value = AnSafeConvert.ToByte(dictionary[key]);
                                            break;
                                        case AnTypes.DataType.Short:
                                            value = AnSafeConvert.ToInt16(dictionary[key]);
                                            break;
                                        case AnTypes.DataType.UShort:
                                            value = AnSafeConvert.ToUInt16(dictionary[key]);
                                            break;
                                        case AnTypes.DataType.Int:
                                            value = AnSafeConvert.ToInt32(dictionary[key]);
                                            break;
                                        case AnTypes.DataType.UInt:
                                            value = AnSafeConvert.ToUInt32(dictionary[key]);
                                            break;
                                        case AnTypes.DataType.Long:
                                            value = AnSafeConvert.ToInt64(dictionary[key]);
                                            break;
                                        case AnTypes.DataType.ULong:
                                            value = AnSafeConvert.ToUInt64(dictionary[key]);
                                            break;
                                        case AnTypes.DataType.Single:
                                            value = AnSafeConvert.ToSingle(dictionary[key]);
                                            break;
                                        case AnTypes.DataType.Double:
                                            value = AnSafeConvert.ToDouble(dictionary[key]);
                                            break;
                                        case AnTypes.DataType.Decimal:
                                        case AnTypes.DataType.Currency:
                                            value = AnSafeConvert.ToDecimal(dictionary[key]);
                                            break;
                                        case AnTypes.DataType.Boolean:
                                            value = AnSafeConvert.ToBoolean(dictionary[key]);
                                            break;
                                        case AnTypes.DataType.DateTime:
                                        case AnTypes.DataType.FileTime:
                                            value = AnSafeConvert.ToDateTime(dictionary[key]);
                                            break;
                                        case AnTypes.DataType.Guid:
                                            value = AnSafeConvert.ToGuid(dictionary[key]);
                                            break;
                                        case AnTypes.DataType.Char:
                                        case AnTypes.DataType.VarChar:
                                        case AnTypes.DataType.Text:
                                        case AnTypes.DataType.NChar:
                                        case AnTypes.DataType.NVarChar:
                                        case AnTypes.DataType.NText:
                                            value = AnSafeConvert.ToString(dictionary[key]);
                                            break;
                                        case AnTypes.DataType.Image:
                                        case AnTypes.DataType.Binary:
                                        case AnTypes.DataType.VarBinary:
                                            value = dictionary[key] as byte[];
                                            break;
                                        case AnTypes.DataType.Table:
                                            {
                                                if (!PropertyTypeCache.ContainsKey(dbTableName))
                                                {
                                                    PropertyTypeCache.Add(dbTableName, new Dictionary<string, Type>());
                                                }

                                                if (!PropertyTypeCache[dbTableName].ContainsKey(column.Name))
                                                {
                                                    PropertyTypeCache[dbTableName].Add(column.Name, null);
                                                }

                                                if (PropertyTypeCache[dbTableName][column.Name] == null)
                                                {
                                                    PropertyTypeCache[dbTableName][column.Name] = type.GetProperty(column.Name)?.PropertyType;
                                                }

                                                if (!FromDictionaryListMethodCache.ContainsKey(dbTableName))
                                                {
                                                    FromDictionaryListMethodCache.Add(dbTableName, new Dictionary<string, MethodInfo>());
                                                }

                                                if (!FromDictionaryListMethodCache[dbTableName].ContainsKey(column.Name))
                                                {
                                                    FromDictionaryListMethodCache[dbTableName].Add(column.Name, null);
                                                }

                                                if (FromDictionaryListMethodCache[dbTableName][column.Name] == null)
                                                {
                                                    FromDictionaryListMethodCache[dbTableName][column.Name] = PropertyTypeCache[dbTableName][column.Name]?.GetMethod(FromDictionaryListMethodName);
                                                }

                                                if (FromDictionaryListMethodCache[dbTableName][column.Name] != null)
                                                {
                                                    value = dictionary[key] is List<Dictionary<string, object>> dictionaryList ? FromDictionaryListMethodCache[dbTableName][column.Name].Invoke(null, new object[] {dictionaryList}) : Activator.CreateInstance(PropertyTypeCache[dbTableName][column.Name] ?? typeof(object));
                                                }
                                            }
                                            break;
                                        //case AnTypes.DataType.Null:
                                        //default:
                                        //break;
                                    }

                                    type.GetProperty(column.Name)?.SetValue(result, value, null);

                                    break;
                                }
                            }
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                AnLog.Error(ex);
            }

            return result;
        }

        #endregion Dictionary Methods
    }

    [Serializable]
    public class AnModelBaseList
    {
        #region Dictionary Methods

        public static List<Dictionary<string, object>> ToDictionaryList<T>(List<T> modelBaseList)
        {
            var result = new List<Dictionary<string, object>>();

            try
            {
                if (modelBaseList != null)
                {
                    result = modelBaseList
                        .Where(x => x != null)
                        .Select(AnModelBase.ToDictionary)
                        .Where(x => x != null && x.Count > 0)
                        .ToList();
                }
            }

            catch (Exception ex)
            {
                AnLog.Error(ex);
            }

            return result;
        }

        public static List<T> FromDictionaryList<T>(List<Dictionary<string, object>> dictionaryList)
        {
            var result = new List<T>();

            try
            {
                if ((dictionaryList != null) &&
                    (dictionaryList.Count > 0))
                {
                    result.AddRange(dictionaryList.Where(dataDictionary => ((dataDictionary != null) && (dataDictionary.Count > 0))).Select(AnModelBase.FromDictionary<T>));
                }
            }

            catch (Exception ex)
            {
                AnLog.Error(ex);
            }

            return result;
        }

        #endregion Dictionary Methods
    }
}

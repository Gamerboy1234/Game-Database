
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using ActiveNet.Core.Log;

namespace ActiveNet.Types.Base
{
    public static class AnDataSetUtility
    {
        #region Public Methods

        public static bool ValidateQueryResults(DataSet dataSet, out DataTable dataTable)
        {
            var result = false;

            dataTable = null;

            try
            {
                if ((dataSet?.Tables.Count > 0) && 
                    (dataSet.Tables[0]?.Columns.Count > 0) &&
                    (dataSet.Tables[0]?.Rows.Count > 0))
                {
                    dataTable = dataSet.Tables[0];

                    result = true;
                }
            }

            catch (Exception ex)
            {
                AnLog.Error(ex);
            }

            return result;
        }

        public static bool ValidateDictionaryList(List<Dictionary<string, object>> dictionaryList)
        {
            return ((dictionaryList?.Count > 0) && (dictionaryList[0]?.Count > 0));
        }

        public static bool ValidateDictionaryList(List<Dictionary<string, object>> dictionaryList, out Dictionary<string, object> firstFieldDictionary)
        {
            var result = false;

            firstFieldDictionary = null;

            try
            {
                if ((dictionaryList?.Count > 0) && 
                    (dictionaryList[0]?.Count > 0))
                {
                    firstFieldDictionary = dictionaryList[0];

                    result = true;
                }
            }

            catch (Exception ex)
            {
                AnLog.Error(ex);
            }

            return result;
        }

        public static List<Dictionary<string, object>> ToDictionaryList(DataTable table)
        {
            List<Dictionary<string, object>> result = null;

            try
            {
                if ((table?.Columns.Count > 0) &&
                    (table.Rows?.Count > 0))
                {
                    result = table.Rows.Cast<DataRow>().Where(row => row != null).Select(row => table.Columns.Cast<DataColumn>().Where(column => !string.IsNullOrEmpty(column?.ColumnName)).Where(column => !row.IsNull(column)).ToDictionary(column => column.ColumnName, column => row[column.ColumnName])).ToList();
                }
            }

            catch (Exception ex)
            {
                AnLog.Error(ex);
            }

            return result ?? new List<Dictionary<string, object>>();
        }

        public static List<T> ToList<T>(DataTable table, string columnName)
        {
            var result = new List<T>();

            try
            {
                if (!string.IsNullOrEmpty(columnName) && 
                    (table.Rows?.Count > 0))
                {
                    result.AddRange(table.Rows.Cast<DataRow>().Where(row => table.Columns.Contains(columnName) && !row.IsNull(columnName)).Select(row => (T) row[columnName]));
                    
                    //result = (List<T>) table.Rows.Cast<DataRow>().Where(row => row != null).Select(row => table.Columns.Cast<DataColumn>().Where(column => !string.IsNullOrEmpty(column?.ColumnName)).Where(column => !row.IsNull(column)).ToList());
                }
            }

            catch (Exception ex)
            {
                AnLog.Error(ex);
            }

            return result;
        }

        public static bool Exists<T>(T value, int id)
        {
            try
            {
                if (value != null)
                {
                    var classType = AnTypes.GetType(value, null);

                    if (classType != null)
                    {
                        foreach (var property in classType.GetProperties())
                        {
                            var propertyType = property.PropertyType;

                            if (property.PropertyType.IsEnum)
                            {
                                propertyType = AnReflectionCache.GetUnderlyingType(propertyType);
                            }

                            var key = property.Name.ToLower();

                            if ((Type.GetTypeCode(propertyType) == TypeCode.Int32) && (key == "id"))
                            {
                                var localId = (int)property.GetValue(value, null);

                                if (localId == id)
                                {
                                    return true;
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

            return false;
        }

        public static bool ExistsInList<T>(T list, int id)
        {
            try
            {
                if (list != null)
                {
                    var classType = typeof(T);
                    var className = classType.FullName;

                    var exists = AnReflectionCache.GetExistsMethod(typeof(AnDataSetUtility));

                    if (exists != null)
                    {
                        var genericListItemType = GenericOfClassType(className);

                        if (genericListItemType != null)
                        {
                            exists = exists.MakeGenericMethod(genericListItemType);

                            if (classType.GetInterfaces().Contains(typeof(IEnumerable)))
                            {
                                if (((IEnumerable)list).Cast<object>().Any(item => (bool)exists.Invoke(null, new[] {item, id})))
                                {
                                    return true;
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

            return false;
        }

        #endregion Public Methods


        #region Private Methods

        private static string GenericOfClassName(string listFullClassName)
        {
            var result = new StringBuilder();

            try
            {
                if (!string.IsNullOrEmpty(listFullClassName))
                {
                    var startPos = listFullClassName.IndexOf("[[", 0, StringComparison.InvariantCulture);

                    if (startPos > -1)
                    {
                        var itemClass = listFullClassName.Substring(startPos + 2);

                        if (!string.IsNullOrEmpty(itemClass))
                        {
                            var classPieces = itemClass.Split(',');

                            var subClassName = ((classPieces.Length > 0) ? classPieces[0] : itemClass).Trim().Trim(',').Trim();

                            if (!string.IsNullOrEmpty(subClassName))
                            {
                                result.Append(subClassName);
                            }
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                AnLog.Error(ex);
            }

            return result.ToString();
        }

        private static Type GenericOfClassType(string listFullClassName)
        {
            Type result = null;

            try
            {
                var subClassName = GenericOfClassName(listFullClassName);

                if (!string.IsNullOrEmpty(subClassName))
                {
                    result = AnTypes.GetType(subClassName, null);
                }
            }

            catch (Exception ex)
            {
                AnLog.Error(ex);
            }

            return result;
        }

        #endregion Private Methods
    }
}

﻿
using Logger;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Utilities
{
    public static class DataSetUtility
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
                Log.Error(ex);
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
                Log.Error(ex);
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
                Log.Error(ex);
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
                }
            }

            catch (Exception ex)
            {
                Log.Error(ex);
            }

            return result;
        }

        #endregion Public Methods
    }
}

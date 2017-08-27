using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
// ReSharper disable ConvertToLocalFunction

// ReSharper disable once CheckNamespace
namespace DJO.Reporting
{
    public partial class Report
    {
        /// <summary>
        /// Create a Report from a DataTable.
        /// The column names are used as headers.
        /// </summary>
        /// <param name="dataTable">Input DataTable</param>
        /// <returns>A Report object</returns>
        public static Report FromDataTable(DataTable dataTable)
        {
            Func<DataTable, Row> getHeaderRow = (dt) =>
            {
                var headerColumns = new List<Column>();
                foreach (var column in dt.Columns)
                {
                    headerColumns.Add(new Column(column.ToString(), ColumnFormats.Header));
                }
                return new Row(headerColumns);
            };

            Func<DataTable, IEnumerable<Row>> getDataRows = (dt) =>
            {
                var dataRows = new List<Row>();
                foreach (DataRow row in dt.Rows)
                {
                    dataRows.Add(new Row(row.ItemArray.Select(val => new Column(val))));
                }
                return dataRows;
            };

            var rows = new List<Row>();
            rows.Add(getHeaderRow(dataTable));
            rows.AddRange(getDataRows(dataTable));

            var groupName = string.IsNullOrEmpty(dataTable.TableName) ? DefaultGroupName : dataTable.TableName;

            return new Report(rows, groupName);
        }
    }
}

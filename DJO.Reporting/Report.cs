using System.Collections.Generic;

namespace DJO.Reporting
{
    /// <summary>
    /// A Report object can represent most forms of tabular data. 
    /// Report objects can easily be serialized to many formats including Excel and CSV.
    /// </summary>
    public partial class Report
    {
        private const string DefaultGroupName = "Sheet1";

        /// <summary>
        /// Creates a report with multiple groups.  
        /// </summary>
        /// <param name="groups">
        /// A group contains an enumerable of Rows. 
        /// Each group will be exported as a separate worksheet when Excel is the chosen format.
        /// </param>
        public Report(IEnumerable<Group> groups)
        {
            Groups = groups;
        }

        /// <summary>
        /// Creates a report with the specified rows
        /// </summary>
        /// <param name="rows">Each row contains multiple columns.</param>
        /// <param name="groupName">The name of the group. This is used to name the Worksheet when Excel is used.</param>
        public Report(IEnumerable<Row> rows, string groupName = null)
        {
            Groups = new[]
            {
                new Group(groupName ?? DefaultGroupName, rows)
            };
        }

        public IEnumerable<Group> Groups { get; set; }

        public class Group
        {
            /// <summary>
            /// A group is the equivalent of a worksheet in Excel. It is a way of grouping rows.
            /// </summary>
            /// <param name="name">Name of the group</param>
            /// <param name="rows">Rows in the group</param>
            public Group(string name, IEnumerable<Row> rows)
            {
                Name = name;
                Rows = rows;
            }

            public string Name { get; }
            public IEnumerable<Row> Rows { get; }
        }

        public class Row
        {
            /// <summary>
            /// A row of data.
            /// </summary>
            /// <param name="columns">The columns within the row.</param>
            public Row(IEnumerable<Column> columns)
            {
                Columns = columns;
            }
            public IEnumerable<Column> Columns { get; }
        }

        public class Column
        {
            /// <summary>
            /// A column is equivalent to a cell in Excel. 
            /// It has a value and optionally a column format which can be used to apply a style or modify to the column.
            /// </summary>
            /// <param name="value">The value of the column, e.g. "Hello" or 1</param>
            /// <param name="columnFormat">
            /// The ReportSerializer can use this value to apply a style or modify the column.
            /// Defaults to ColumnFormats.Data</param>
            public Column(object value, string columnFormat = null)
            {
                Value = value;
                ColumnFormat = columnFormat ?? ColumnFormats.Data;
            }

            public string ColumnFormat { get; }
            public object Value { get; }
        }
        
        public static class ColumnFormats
        {
            public static readonly string Header = "Header";
            public static readonly string SubHeader = "SubHeader";
            public static readonly string Text = "Text";
            public static readonly string Data = "Data";
            public static readonly string SubTotal = "SubTotal";
            public static readonly string Total = "Total";
        }
    }
}

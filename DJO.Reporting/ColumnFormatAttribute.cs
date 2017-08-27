using System;

namespace DJO.Reporting
{
    /// <summary>
    /// Sets the column format which can be used to apply styles to columns in a report.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ColumnFormatAttribute : Attribute
    {
        public ColumnFormatAttribute(string columnFormat)
        {
            ColumnFormat = columnFormat;
        }

        public string ColumnFormat { get; }
    }
}

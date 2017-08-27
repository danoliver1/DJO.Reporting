using OfficeOpenXml;

namespace DJO.Reporting.Serialization.ReportSerializers.Excel.CellFormatters
{
    public interface IColumnFormatter
    {
        string ColumnFormat { get; }
        void UpdateCell(ExcelRange cell);
    }
}

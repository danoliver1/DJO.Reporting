using System.Drawing;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace DJO.Reporting.Serialization.ReportSerializers.Excel.CellFormatters
{
    public class HeaderFormatter : IColumnFormatter
    {
        public string ColumnFormat => Report.ColumnFormats.Header;
        public void UpdateCell(ExcelRange cell)
        {
            var style = cell.Style;
            style.Fill.PatternType = ExcelFillStyle.Solid;
            style.Fill.BackgroundColor.SetColor(Color.Black);
            style.Font.Color.SetColor(Color.White);
            cell.Style.Font.Bold = true;
        }
    }
}
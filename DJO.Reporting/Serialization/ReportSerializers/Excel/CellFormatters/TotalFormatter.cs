using System.Drawing;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace DJO.Reporting.Serialization.ReportSerializers.Excel.CellFormatters
{
    public class TotalFormatter : IColumnFormatter
    {
        public string ColumnFormat => Report.ColumnFormats.Total;
        public void UpdateCell(ExcelRange cell)
        {
            cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
            cell.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
        }
    }
}
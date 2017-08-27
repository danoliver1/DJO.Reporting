using System;
using System.Collections.Generic;
using System.Linq;
using DJO.Reporting.Serialization.ReportSerializers.Excel.CellFormatters;
using OfficeOpenXml;

namespace DJO.Reporting.Serialization.ReportSerializers.Excel
{
    public class ExcelReportSerializer : IReportSerializer
    {
        private const string ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        private const string FileExtension = "xlsx";

        private readonly IDictionary<string, Action<ExcelRange>> _columnFormatters;

        public ExcelReportSerializer(IEnumerable<IColumnFormatter> columnTypeFormatters)
        {
            _columnFormatters = columnTypeFormatters.ToDictionary(x => x.ColumnFormat, x => new Action<ExcelRange>(x.UpdateCell));
        }

        public string ReportFormat => "Excel";

        public SerializedReport Serialize(Report report)
        {
            using (var pkg = new ExcelPackage())
            {
                foreach (var group in report.Groups)
                {
                    var worksheet = pkg.Workbook.Worksheets.Add(group.Name);

                    var rowIndex = 1;
                    foreach (var row in group.Rows)
                    {
                        var colIndex = 1;
                        foreach (var column in row.Columns)
                        {
                            var cell = worksheet.Cells[rowIndex, colIndex];

                            var dateTime = column.Value as DateTime?;

                            cell.Value = dateTime?.ToShortDateString() ?? column.Value;

                            if (_columnFormatters.ContainsKey(column.ColumnFormat))
                                _columnFormatters[column.ColumnFormat](cell);

                            colIndex++;
                        }
                        rowIndex++;
                    }

                    worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns(0, 80);
                }

                var data = pkg.GetAsByteArray();
                return new SerializedReport(data, ContentType, FileExtension);
            }
        }
    }
}

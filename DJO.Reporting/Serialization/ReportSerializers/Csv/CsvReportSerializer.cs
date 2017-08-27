using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DJO.Reporting.Serialization.ReportSerializers.Csv
{
    public class CsvReportSerializer : IReportSerializer
    {
        public string ReportFormat => ReportFormats.Csv;

        public SerializedReport Serialize(Report report)
        {
            var csvFiles = report.Groups.Select(GetCsv).ToArray();

            if (csvFiles.Length == 1)
            {
                return new SerializedReport(csvFiles[0].Data.ToArray(), "text/csv", "csv");
            }

            // ReSharper disable once SuggestVarOrType_Elsewhere
            byte[] zipFile = CompressFiles(csvFiles);

            return new SerializedReport(zipFile, "application/zip", "zip");
        }

        private static CsvFile GetCsv(Report.Group group)
        {
            var stream = new MemoryStream();
            var csvWriter = new StreamWriter(stream, Encoding.UTF8);

            var firstRow = true;
            foreach (var row in group.Rows)
            {
                //add line breaks - this way stops there being an empty line at the end of the file
                if (firstRow)
                    firstRow = false;
                else
                    csvWriter.Write(Environment.NewLine);

                var firstCol = true;
                foreach (var column in row.Columns)
                {
                    //adds commas between columns
                    if (firstCol)
                        firstCol = false;
                    else
                        csvWriter.Write(",");

                    csvWriter.Write(CsvString(column.Value));
                }
            }

            csvWriter.Flush();

            return new CsvFile(group.Name, stream);

        }

        private static string CsvString(object value)
        {
            Func<string, string> wrapInQuotes = s => $"\"{s}\"";

            Func<object, string> getStringValue = o =>
            {
                if (value == null)
                    return string.Empty;

                var dateTime = value as DateTime?;
                if (dateTime != null)
                    return dateTime.Value.ToShortDateString();

                return value.ToString();
            };

            return wrapInQuotes(getStringValue(value).Replace("\"", "\"\""));
        }

        private static byte[] CompressFiles(IEnumerable<CsvFile> csvFiles)
        {
            var compressedFileStream = new MemoryStream();

            using (var zipArchive = new ZipArchive(compressedFileStream, ZipArchiveMode.Update, false))
            {
                foreach (var csvFile in csvFiles)
                {
                    var zipEntry = zipArchive.CreateEntry($"{csvFile.GroupName}.csv");

                    using (var zipEntryStream = zipEntry.Open())
                    {
                        csvFile.Data.Seek(0, SeekOrigin.Begin);
                        csvFile.Data.CopyTo(zipEntryStream);
                    }
                }
            }

            return compressedFileStream.ToArray();
        }

        private class CsvFile
        {
            internal CsvFile(string groupName, MemoryStream data)
            {
                GroupName = groupName;
                Data = data;
            }

            public string GroupName { get; }
            public MemoryStream Data { get; }
        }
    }
}

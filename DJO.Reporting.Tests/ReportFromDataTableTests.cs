using System.Data;
using System.Linq;
using NUnit.Framework;

namespace DJO.Reporting.Tests
{
    [TestFixture]
    public class ReportFromDataTableTests
    {
        [Test]
        public void WellFormattedDataTable_ReportGeneratedSuccessfully()
        {
            var dataTable = new DataTable();
            dataTable.Columns.Add("Name");
            dataTable.Columns.Add("Description");
            dataTable.Rows.Add("Test", "Test Description");
            dataTable.Rows.Add("Test 2", "Test Description 2");

            var report = Report.FromDataTable(dataTable);

            var rows = report.Groups.First().Rows.ToArray();
            Assert.That(rows.Length, Is.EqualTo(3));
            AssertRow(rows[0], "Name", "Description", Report.ColumnFormats.Header);
            AssertRow(rows[1], "Test", "Test Description", Report.ColumnFormats.Data);
            AssertRow(rows[2], "Test 2", "Test Description 2", Report.ColumnFormats.Data);
        }

        [Test]
        public void NullDataTable_ThrowsArgumentNullException()
        {
            DataTable dataTable = null;

            // ReSharper disable once ExpressionIsAlwaysNull
            Assert.That(() => Report.FromDataTable(dataTable), Throws.ArgumentNullException);
        }

        [Test]
        public void DataTableWithNoColumns_ThrowsArgumentException()
        {
            var dataTable = new DataTable();
            Assert.That(() => Report.FromDataTable(dataTable), Throws.ArgumentException);
        }

        [Test]
        public void DataTableWithColumnsButNoRows_CreatesReportWithHeaders()
        {
            var dataTable = new DataTable();
            dataTable.Columns.Add("Col1");

            var report = Report.FromDataTable(dataTable);

            var rows = report.Groups.First().Rows.ToArray();
            Assert.That(rows.Count, Is.EqualTo(1));

            var columns = rows.Single().Columns.ToArray();
            Assert.That(columns.Length, Is.EqualTo(1));
            Assert.That(columns[0].Value, Is.EqualTo("Col1"));
        }

        private static void AssertRow(Report.Row row, string col1Value, string col2Value, string columnFormat)
        {
            Assert.That(row.Columns.All(x => x.ColumnFormat == columnFormat));
            var columns = row.Columns.ToArray();
            Assert.That(columns[0].Value, Is.EqualTo(col1Value));
            Assert.That(columns[1].Value, Is.EqualTo(col2Value));
        }
    }
}

using System;
using System.Data;
using System.Web.Mvc;
using DJO.Reporting;
using DJO.Reporting.Serialization;
using DJO.Reporting.Serialization.ReportSerializers;
using DJO.Reporting.Serialization.ReportSerializers.Csv;
using DJO.Reporting.Serialization.ReportSerializers.Excel;
using DJO.Reporting.Serialization.ReportSerializers.Excel.CellFormatters;

namespace ReportTest.Controllers
{
    public class HomeController : Controller
    {
        private readonly ReportSerializer _reportSerializer;

        public HomeController()
        {
            _reportSerializer = new ReportSerializer(new IReportSerializer[]
            {
                new ExcelReportSerializer(new IColumnFormatter[] {new HeaderFormatter(), new TotalFormatter()}),
                new CsvReportSerializer()
            });
        }

        public class Book
        {
            [Ignore]
            public int Id { get; set; }
            
            public string Name { get; set; }

            public string Description { get; set; }

            public DateTime? PublishDate { get; set; }

            [ColumnFormat("Total")]
            public string Price { get; set; }
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Enumerable(string reportFormat = "Excel")
        {
            var books = new[]
            {
                new Book
                {
                    Name = "Lord of the Rings",
                    Description = "A very long book",
                    PublishDate = DateTime.Now,
                    Price = "£2.99"
                },
                new Book
                {
                    Name = "Game of Thrones",
                    Description ="Dragons etc.",
                    PublishDate = DateTime.Now,
                    Price = "£4.49"
                },
                new Book
                {
                    Name = "The Notebook",
                    Price = "£0.49"
                }
            };
            var report = Report.FromEnumerable(books);
            return ReportResult("Books", report, reportFormat);
        }

        public ActionResult Test(string reportFormat = "Excel", bool dt = false, bool multi = false)
        {
            var report = multi ? GetTwoTabReport() : GetSampleOneTabReport();

            if (dt)
            {
                var dataTable = new DataTable();
                dataTable.Columns.Add("Name");
                dataTable.Columns.Add("Description");
                dataTable.Rows.Add("Test", "Description yay");

                report = Report.FromDataTable(dataTable);
            }

            return ReportResult("TestReport", report, reportFormat);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        private FileContentResult ReportResult(string fileName, Report report, string reportFormat,
            bool includeDateStamp = true)
        {
            var serialized = _reportSerializer.Serialize(report, reportFormat);

            var fileDownloadName = fileName;

            if (includeDateStamp)
                fileDownloadName += $"_{DateTime.Now:yyyyMMddHHmm}";

            return new FileContentResult(serialized.Data, serialized.ContentType)
            {
                FileDownloadName = $"{fileDownloadName}.{serialized.FileExtension}"
            };
        }

        //For testing purposes
        public static Report GetSampleOneTabReport()
        {
            return new Report(new[]
            {
                new Report.Row(
                    new[]
                    {
                        new Report.Column("Kpi Name", Report.ColumnFormats.Header),
                        new Report.Column("Q1", Report.ColumnFormats.Header),
                        new Report.Column("Q2", Report.ColumnFormats.Header),
                        new Report.Column("Q3", Report.ColumnFormats.Header),
                        new Report.Column("Q4", Report.ColumnFormats.Header),
                        new Report.Column("YTD", Report.ColumnFormats.Header)
                    }
                ),
                new Report.Row(
                    new[]
                    {
                        new Report.Column("Sales vs Target", Report.ColumnFormats.Text),
                        new Report.Column(1),
                        new Report.Column((int?) null),
                        new Report.Column(0.34D),
                        new Report.Column(DateTime.Now),
                        new Report.Column(0, Report.ColumnFormats.Total)
                    }
                ),
            });
        }

        public static Report GetTwoTabReport()
        {
            return new Report(new[]
            {
                new Report.Group("Tab 1", new[]
                {
                    new Report.Row(
                        new[]
                        {
                            new Report.Column("Kpi Name", Report.ColumnFormats.Header),
                            new Report.Column("Q1", Report.ColumnFormats.Header),
                            new Report.Column("Q2", Report.ColumnFormats.Header),
                            new Report.Column("Q3", Report.ColumnFormats.Header),
                            new Report.Column("Q4", Report.ColumnFormats.Header),
                            new Report.Column("YTD", Report.ColumnFormats.Header)
                        }
                    ),
                    new Report.Row(
                        new[]
                        {
                            new Report.Column("Sales vs Target", Report.ColumnFormats.Text),
                            new Report.Column(1),
                            new Report.Column(0),
                            new Report.Column(0),
                            new Report.Column(0),
                            new Report.Column(0, Report.ColumnFormats.Total)
                        }
                    )
                }),
                new Report.Group("Tab 2", new[]
                {
                    new Report.Row(
                        new[]
                        {
                            new Report.Column("Kpi Name", Report.ColumnFormats.Header),
                            new Report.Column("Q1", Report.ColumnFormats.Header),
                            new Report.Column("Q2", Report.ColumnFormats.Header),
                            new Report.Column("Q3", Report.ColumnFormats.Header),
                            new Report.Column("Q4", Report.ColumnFormats.Header),
                            new Report.Column("YTD", Report.ColumnFormats.Header)
                        }
                    ),
                    new Report.Row(
                        new[]
                        {
                            new Report.Column(
                                "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Pellentesque erat enim, varius sed tempor sit amet, lacinia id urna. Morbi sit amet nisl sed nisi vehicula placerat id id arcu. Ut cursus eros metus, ac scelerisque dolor pretium nec. Mauris et varius eros. Curabitur tristique mauris sed felis lobortis, eget feugiat erat pretium. Duis ultrices quam a orci lacinia, id vulputate augue accumsan. Nulla pretium aliquam neque, nec imperdiet odio ultrices non.",
                                Report.ColumnFormats.Text),
                            new Report.Column(1),
                            new Report.Column(2),
                            new Report.Column(3),
                            new Report.Column(4),
                            new Report.Column(10, Report.ColumnFormats.Total)
                        }
                    )
                })
            });
        }
    }
}

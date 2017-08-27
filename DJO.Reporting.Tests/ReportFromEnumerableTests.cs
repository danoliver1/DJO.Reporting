using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using NUnit.Framework;

namespace DJO.Reporting.Tests
{
    [TestFixture]
    public class ReportFromEnumerableTests
    {
        public class Book
        {
            public string Name { get; set; }

            [Ignore]
            public string IgnoreMe { get; set; }

            [ColumnFormat("Price")]
            public double Price { get; set; }

            [Display(Name = "Published")]
            public DateTime PublishedDate { get; set; }
        }

        [Test]
        public void GivenAPoco_ReturnsReport()
        {
            //Arrange
            var books = new[]
            {
                new Book
                {
                    Name = "Lord of the Rings",
                    Price = 4.99D,
                    PublishedDate = new DateTime(2000, 1, 1)
                }
            };

            //Act
            var report = Report.FromEnumerable(books);

            //Assert
            var group = report.Groups.First();
            var rows = group.Rows.ToArray();

            Assert.That(group.Name, Is.EqualTo("Book"));

            //headers
            var headerColumns = rows[0].Columns.ToArray();
            Assert.That(headerColumns.All(x => x.ColumnFormat == Report.ColumnFormats.Header));
            Assert.That(headerColumns[0].Value, Is.EqualTo("Name"));
            Assert.That(headerColumns[1].Value, Is.EqualTo("Price"));
            Assert.That(headerColumns[2].Value, Is.EqualTo("Published")); //Set by DisplayAttribute

            //data
            var dataColumns = rows[1].Columns.ToArray();

            //column values
            Assert.That(dataColumns[0].Value, Is.EqualTo(books[0].Name));
            Assert.That(dataColumns[1].Value, Is.EqualTo(books[0].Price));
            Assert.That(dataColumns[2].Value, Is.EqualTo(books[0].PublishedDate));

            //column formats
            Assert.That(dataColumns[0].ColumnFormat, Is.EqualTo(Report.ColumnFormats.Data));
            Assert.That(dataColumns[1].ColumnFormat, Is.EqualTo("Price"));
            Assert.That(dataColumns[2].ColumnFormat, Is.EqualTo(Report.ColumnFormats.Data));
        }
    }
}

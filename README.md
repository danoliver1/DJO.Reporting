# DJO.Reporting

An extensible reporting (soon to be) NuGet package. 

Out of the box this package provides the ability to convert an enumerable of objects or a DataTable to a report in Excel or CSV.
A report can also be manually constructed by building up Groups, Rows and Columns.
This project has been written in an extensible way so new methods of creating and styling reports can be added easily.

## Set up

### In an MVC web project using StructureMap 4.x
This package contains a StructureMap 4.x registry making set up very simple when used in a StructureMap 4.x solution.
The registry needs to be registered in `IoC.cs` (`c.AddRegistry<ReportingRegistry>();`).

### In any project using an IoC container
All concrete implementations of `IReportSerializer` and `IColumnFormatter` need to be registered.
The concrete implemenation of `IReportGenerator` needs to be registered - preferably as a singleton.

## Creating a Report

### From a DataTable
`var report = Report.FromDataTable(dataTable);`

### From an enumerable of objects
`var report = Report.FromEnumerable(books);`

### Manually
```
var report = new Report(
  new[] 
  {
    new Report.Row(
      new[] 
      {
        new Report.Column("Username", ColumnFormats.Header),
        new Report.Column("LastLoginDate", ColumnFormats.Header)
      }    
    ),
    new Report.Row(
      new[] 
      {
        new Report.Column("dan"),
        new Report.Column(DateTime.Now)
      }    
    )
  }
);
```

## Serializing the report
### MVC
```
  public ActionResult ReportDataTable()
  {
    var dataTable = GetDataTable();
    var report = Report.FromDataTable(dataTable);
    return new ReportResult("Report", report, ReportFormats.Excel);
  }
  
  public ActionResult ReportEnumerable()
  {
    var books = GetBooks();
    var report = Report.FromEnumerable(books);
    return new ReportResult("Report", report, ReportFormats.Csv);
  }
```
### Byte array
The report is serialized in the requested format to a SerializedReport object which contains an array of bytes and the content type.
```
  var report = Report.FromDataTable(dataTable);
  var serializedReport = _reportGenerator.Serialize(report, ReportFormats.Excel);
```


## Report Formats

### Excel
* A report can contain multiple groups. The groups translate into separate worksheets in Excel.
* Each column can have a format applied to it and a new format can be implemented by creating a concrete implementation of IColumnFormatter. 
  See unit tests for code examples.

### CSV
* A report with multiple groups will serialize to a zip file containing a CSV file for each group.
* The column formats are ignored when exporting to CSV.

## Planned development
* Convert to NuGet package and set up NuGet feed.
* Report from a DataSet - each datatable would show as a separate worksheet in Excel or as a separate file in a csv file (bundled into a .zip file)
* More unit tests
* Improved StructureMap integration for easier set up
* Add ability to format columns in CSV file. I.e. for a decimal the developer may want to format the number to two decimal places.

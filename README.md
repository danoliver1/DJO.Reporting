# DJO.Reporting

An extensible reporting NuGet package. 

Out of the box this package provides the ability to convert an enumerable of objects or a DataTable to a report in Excel or CSV.
A report can be manually constructed by building up Groups, Rows and Columns; once built it can then be exported in Excel or CSV format.
This project has been written in an extensible way so new methods of creating and styling reports can be added easily.

## Installing the NuGet package
* Add the NuGet feed to Visual Studio https://www.myget.org/F/danoliver1/api/v3/index.json
* Install the package DJO.Reporting

## IoC container setup

### In an MVC web project using StructureMap 4.x
This package contains a StructureMap 4.x registry `ReportingRegistry` making set up very simple when used in a StructureMap 4.x solution.
*If the target web project uses `StructureMap.MVC5.Update`, the registry will simply need to be added to `IoC.cs` (`c.AddRegistry<ReportingRegistry>();`).*

### In a project using any other IoC container
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
    return new ReportResult("ReportDataTable", Report.FromDataTable(dataTable), ReportFormats.Excel);
  }
  
  public ActionResult ReportEnumerable()
  {
    IEnumerable<Book> books = GetBooks();
    return new ReportResult("ReportEnumerable", Report.FromEnumerable(books), ReportFormats.Csv);
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
* Report from a DataSet - each datatable would show as a separate worksheet in Excel or as a separate file in a csv file (bundled into a .zip file)
* More unit tests
* Add ability to format columns in CSV file. I.e. for a decimal the developer may want to format the number to two decimal places.


## Contributions
Contributions, suggestions and feedback are welcome. Please feel free to raise an issue or create a pull request.

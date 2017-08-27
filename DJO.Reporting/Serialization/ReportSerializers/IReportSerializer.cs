using System;

namespace DJO.Reporting.Serialization.ReportSerializers
{
    public interface IReportSerializer
    {
        string ReportFormat { get; }
        SerializedReport Serialize(Report report);
    }
}
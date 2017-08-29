using System;
using System.Collections.Generic;
using System.Linq;
using DJO.Reporting.Serialization.ReportSerializers;

namespace DJO.Reporting.Serialization
{
    public interface IReportGenerator
    {
        SerializedReport Serialize(Report report, string reportFormat);
    }

    /// <summary>
    /// Responsible for converting the Report object to the output format
    /// </summary>
    public class ReportGenerator : IReportGenerator
    {
        private readonly IReportSerializer[] _reportSerializers;

        public ReportGenerator(IEnumerable<IReportSerializer> reportSerializers)
        {
            _reportSerializers = reportSerializers.ToArray();
        }

        /// <summary>
        /// Serializes a Report object to a byte array in the requested report format e.g. ReportFormats.Excel
        /// </summary>
        /// <param name="report">The report object</param>
        /// <param name="reportFormat">
        /// Format of the Report e.g. ReportFormats.Excel. 
        /// This is a string to allow for extensibility without modification by creating new implementations of IReportSerializer.</param>
        /// <returns>Serialized report in the form of a byte array and the content type</returns>
        public SerializedReport Serialize(Report report, string reportFormat)
        {
            if(report == null)
                throw new ArgumentNullException(nameof(report));

            if (string.IsNullOrEmpty(reportFormat))
                throw new ArgumentNullException(nameof(reportFormat));

            var serializer = _reportSerializers.SingleOrDefault(x => x.ReportFormat.Equals(reportFormat, StringComparison.OrdinalIgnoreCase));

            if(serializer == null)
                throw new ArgumentException("Report type is not supported", nameof(reportFormat));

            return serializer.Serialize(report);
        }
    }
}

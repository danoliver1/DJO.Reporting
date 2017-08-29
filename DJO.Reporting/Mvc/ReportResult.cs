using System;
using System.Net.Mime;
using System.Text;
using System.Web.Mvc;
using DJO.Reporting.Serialization;

namespace DJO.Reporting.Mvc
{
    public class ReportResult : ActionResult
    {
        /// <summary>
        /// Serializes a Report object in the format specified.
        /// </summary>
        /// <param name="fileName">The filename without a file extension e.g. "report". The file extension is added automatically.</param>
        /// <param name="report">The report object. See Report.FromDataTable or Report.FromEnumerable methods.</param>
        /// <param name="reportFormat">The format e.g. "Excel" or "Csv".</param>
        /// <param name="includeTimeStamp">If set to true the filename will be appended with a timestamp.</param>
        public ReportResult(string fileName, Report report, string reportFormat, bool includeTimeStamp = true)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentNullException(nameof(fileName));

            if (report == null)
                throw new ArgumentNullException(nameof(report));

            if (string.IsNullOrWhiteSpace(reportFormat))
                throw new ArgumentNullException(nameof(reportFormat));

            FileName = fileName;
            Report = report;
            ReportFormat = reportFormat;

            //Load in the default ReportGenerator
            ReportGenerator = DependencyResolver.Current.GetService<IReportGenerator>();
        }

        public IReportGenerator ReportGenerator { get; set; }
        public string FileName { get; }
        public Report Report { get; }
        public string ReportFormat { get; }

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var serializedReport = ReportGenerator.Serialize(Report, ReportFormat);

            var response = context.HttpContext.Response;
            response.ContentType = serializedReport.ContentType;

            var fileNameWithExtension = $"{FileName}.{serializedReport.FileExtension}";
            var headerValue = ContentDispositionUtil.GetHeaderValue(fileNameWithExtension);
            context.HttpContext.Response.AddHeader("Content-Disposition", headerValue);

            response.OutputStream.Write(serializedReport.Data, 0, serializedReport.Data.Length);
        }

        #region Code borrowed from System.Web.Mvc.FileResult
        internal static class ContentDispositionUtil
        {
            private const string HexDigits = "0123456789ABCDEF";

            private static void AddByteToStringBuilder(byte b, StringBuilder builder)
            {
                builder.Append('%');

                int i = b;
                AddHexDigitToStringBuilder(i >> 4, builder);
                AddHexDigitToStringBuilder(i % 16, builder);
            }

            private static void AddHexDigitToStringBuilder(int digit, StringBuilder builder)
            {
                builder.Append(HexDigits[digit]);
            }

            private static string CreateRfc2231HeaderValue(string filename)
            {
                StringBuilder builder = new StringBuilder("attachment; filename*=UTF-8''");

                byte[] filenameBytes = Encoding.UTF8.GetBytes(filename);
                foreach (byte b in filenameBytes)
                {
                    if (IsByteValidHeaderValueCharacter(b))
                    {
                        builder.Append((char) b);
                    }
                    else
                    {
                        AddByteToStringBuilder(b, builder);
                    }
                }

                return builder.ToString();
            }

            public static string GetHeaderValue(string fileName)
            {
                // If fileName contains any Unicode characters, encode according
                // to RFC 2231 (with clarifications from RFC 5987)
                foreach (char c in fileName)
                {
                    if ((int) c > 127)
                    {
                        return CreateRfc2231HeaderValue(fileName);
                    }
                }

                // Knowing there are no Unicode characters in this fileName, rely on
                // ContentDisposition.ToString() to encode properly.
                // In .Net 4.0, ContentDisposition.ToString() throws FormatException if
                // the file name contains Unicode characters.
                // In .Net 4.5, ContentDisposition.ToString() no longer throws FormatException
                // if it contains Unicode, and it will not encode Unicode as we require here.
                // The Unicode test above is identical to the 4.0 FormatException test,
                // allowing this helper to give the same results in 4.0 and 4.5.         
                ContentDisposition disposition = new ContentDisposition() {FileName = fileName};
                return disposition.ToString();
            }

            // Application of RFC 2231 Encoding to Hypertext Transfer Protocol (HTTP) Header Fields, sec. 3.2
            // http://greenbytes.de/tech/webdav/draft-reschke-rfc2231-in-http-latest.html
            private static bool IsByteValidHeaderValueCharacter(byte b)
            {
                if ((byte) '0' <= b && b <= (byte) '9')
                {
                    return true; // is digit
                }
                if ((byte) 'a' <= b && b <= (byte) 'z')
                {
                    return true; // lowercase letter
                }
                if ((byte) 'A' <= b && b <= (byte) 'Z')
                {
                    return true; // uppercase letter
                }

                switch (b)
                {
                    case (byte) '-':
                    case (byte) '.':
                    case (byte) '_':
                    case (byte) '~':
                    case (byte) ':':
                    case (byte) '!':
                    case (byte) '$':
                    case (byte) '&':
                    case (byte) '+':
                        return true;
                }

                return false;
            }
        }
    }

    #endregion
}
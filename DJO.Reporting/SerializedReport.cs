namespace DJO.Reporting
{
    public class SerializedReport
    {
        internal SerializedReport(byte[] data, string contentType, string fileExtension)
        {
            Data = data;
            ContentType = contentType;
            FileExtension = fileExtension;
        }

        public byte[] Data { get; }
        public string ContentType { get; }
        public string FileExtension { get; }
    }
}


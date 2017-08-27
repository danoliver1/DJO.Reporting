using System;

namespace DJO.Reporting
{
    /// <summary>
    /// Prevents column from appearing in reports.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class IgnoreAttribute : Attribute
    {
    }
}

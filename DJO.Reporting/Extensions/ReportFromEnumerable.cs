using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

// ReSharper disable ConvertToLocalFunction

// ReSharper disable once CheckNamespace
namespace DJO.Reporting
{
    public partial class Report
    {
        public static Report FromEnumerable<T>(IEnumerable<T> enumerable)
        {
            var type = typeof(T);
            var properties = type
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .ExcludeIgnoreAttribute();

            if (properties.Length == 0 || type == typeof(object))
                throw new ArgumentException("The report type has no public instance properties.", nameof(type));

            var rows = new List<Row>();
            rows.Add(new Row(properties.Select(p => new Column(GetDisplayName(p), ColumnFormats.Header))));
            rows.AddRange(enumerable.Select(o => new Row(properties.Select(p => new Column(p.GetValue(o), GetColumnFormat(p))))));

            return new Report(rows, type.Name);
        }

        private static string GetDisplayName(PropertyInfo info)
        {
            return (info.GetCustomAttribute(typeof(DisplayAttribute)) as DisplayAttribute)?.GetName()
                ?? info.Name;
        }

        private static string GetColumnFormat(PropertyInfo info)
        {
            return (info.GetCustomAttribute(typeof(ColumnFormatAttribute)) as ColumnFormatAttribute)?.ColumnFormat;
        }
    }

    internal static class PropertyInfoExtensions
    {
        public static PropertyInfo[] ExcludeIgnoreAttribute(this PropertyInfo[] propertyInfos)
        {
            return propertyInfos
                .Where(i => (i.GetCustomAttribute(typeof(IgnoreAttribute)) as IgnoreAttribute) == null)
                .ToArray();
        }
    }

}

using System.Reflection;
using DJO.Reporting.Serialization;
using DJO.Reporting.Serialization.ReportSerializers;
using DJO.Reporting.Serialization.ReportSerializers.Excel.CellFormatters;
using StructureMap;
using StructureMap.Pipeline;

namespace DJO.Reporting.Structuremap
{
    public class ReportingRegistry : Registry
    {
        public ReportingRegistry()
        {
            //TODO: find a better way of doing this
            Scan(cfg =>
            {
                cfg.Assembly(Assembly.Load("DJO.Reporting"));
                cfg.AddAllTypesOf<IColumnFormatter>();
                cfg.AddAllTypesOf<IReportSerializer>();
                cfg.WithDefaultConventions();
            });

            For<IReportGenerator>().LifecycleIs<SingletonLifecycle>();
        }
    }
}

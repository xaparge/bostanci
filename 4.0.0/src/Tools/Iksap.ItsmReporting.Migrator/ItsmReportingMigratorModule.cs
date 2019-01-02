using System.Data.Entity;
using System.Reflection;
using Abp.Modules;
using Iksap.ItsmReporting.EntityFramework;

namespace Iksap.ItsmReporting.Migrator
{
    [DependsOn(typeof(ItsmReportingDataModule))]
    public class ItsmReportingMigratorModule : AbpModule
    {
        public override void PreInitialize()
        {
            Database.SetInitializer<ItsmReportingDbContext>(null);

            Configuration.BackgroundJobs.IsJobExecutionEnabled = false;
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
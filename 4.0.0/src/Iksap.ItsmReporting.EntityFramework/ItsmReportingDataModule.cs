using System.Data.Entity;
using System.Reflection;
using Abp.Modules;
using Abp.Zero.EntityFramework;
using Iksap.ItsmReporting.EntityFramework;

namespace Iksap.ItsmReporting
{
    [DependsOn(typeof(AbpZeroEntityFrameworkModule), typeof(ItsmReportingCoreModule))]
    public class ItsmReportingDataModule : AbpModule
    {
        public override void PreInitialize()
        {
            Database.SetInitializer(new CreateDatabaseIfNotExists<ItsmReportingDbContext>());

            Configuration.DefaultNameOrConnectionString = "Default";
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}

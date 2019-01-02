using System.Data.Entity.Migrations;
using Abp.MultiTenancy;
using Abp.Zero.EntityFramework;
using Iksap.ItsmReporting.Migrations.SeedData;
using EntityFramework.DynamicFilters;

namespace Iksap.ItsmReporting.Migrations
{
    public sealed class Configuration : DbMigrationsConfiguration<ItsmReporting.EntityFramework.ItsmReportingDbContext>, IMultiTenantSeed
    {
        public AbpTenantBase Tenant { get; set; }

        public Configuration()
        {
            SetSqlGenerator("MySql.Data.MySqlClient", new MySql.Data.EntityFramework.MySqlMigrationSqlGenerator());

            AutomaticMigrationsEnabled = false;
            ContextKey = "ItsmReporting";
        }

        protected override void Seed(ItsmReporting.EntityFramework.ItsmReportingDbContext context)
        {
            context.DisableAllFilters();

            if (Tenant == null)
            {
                //Host seed
                new InitialHostDbBuilder(context).Create();

                //Default tenant seed (in host database).
                new DefaultTenantCreator(context).Create();
                new TenantRoleAndUserBuilder(context, 1).Create();
            }
            else
            {
                //You can add seed for tenant databases and use Tenant property...
            }

            context.SaveChanges();
        }
    }
}

using System.Data.Common;
using System.Data.Entity;
using Abp.Zero.EntityFramework;
using Iksap.ItsmReporting.Authorization.Roles;
using Iksap.ItsmReporting.Authorization.Users;
using Iksap.ItsmReporting.MultiTenancy;

namespace Iksap.ItsmReporting.EntityFramework
{
    public class ItsmReportingDbContext : AbpZeroDbContext<Tenant, Role, User>
    {
        //TODO: Define an IDbSet for your Entities...

        /* NOTE: 
         *   Setting "Default" to base class helps us when working migration commands on Package Manager Console.
         *   But it may cause problems when working Migrate.exe of EF. If you will apply migrations on command line, do not
         *   pass connection string name to base classes. ABP works either way.
         */
        public ItsmReportingDbContext()
            : base("Default")
        {

        }

        /* NOTE:
         *   This constructor is used by ABP to pass connection string defined in ItsmReportingDataModule.PreInitialize.
         *   Notice that, actually you will not directly create an instance of ItsmReportingDbContext since ABP automatically handles it.
         */
        public ItsmReportingDbContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {

        }

        //This constructor is used in tests
        public ItsmReportingDbContext(DbConnection existingConnection)
         : base(existingConnection, false)
        {

        }

        public ItsmReportingDbContext(DbConnection existingConnection, bool contextOwnsConnection)
         : base(existingConnection, contextOwnsConnection)
        {

        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

        }
    }
}

using System.Linq;
using Iksap.ItsmReporting.EntityFramework;
using Iksap.ItsmReporting.MultiTenancy;

namespace Iksap.ItsmReporting.Migrations.SeedData
{
    public class DefaultTenantCreator
    {
        private readonly ItsmReportingDbContext _context;

        public DefaultTenantCreator(ItsmReportingDbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            CreateUserAndRoles();
        }

        private void CreateUserAndRoles()
        {
            //Default tenant

            var defaultTenant = _context.Tenants.FirstOrDefault(t => t.TenancyName == Tenant.DefaultTenantName);
            if (defaultTenant == null)
            {
                _context.Tenants.Add(new Tenant {TenancyName = Tenant.DefaultTenantName, Name = Tenant.DefaultTenantName});
                _context.SaveChanges();
            }
        }
    }
}

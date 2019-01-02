using Iksap.ItsmReporting.EntityFramework;
using EntityFramework.DynamicFilters;

namespace Iksap.ItsmReporting.Migrations.SeedData
{
    public class InitialHostDbBuilder
    {
        private readonly ItsmReportingDbContext _context;

        public InitialHostDbBuilder(ItsmReportingDbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            _context.DisableAllFilters();

            new DefaultEditionsCreator(_context).Create();
            new DefaultLanguagesCreator(_context).Create();
            new HostRoleAndUserCreator(_context).Create();
            new DefaultSettingsCreator(_context).Create();
        }
    }
}

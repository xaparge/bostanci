using Abp.Domain.Entities;
using Abp.EntityFramework;
using Abp.EntityFramework.Repositories;

namespace Iksap.ItsmReporting.EntityFramework.Repositories
{
    public abstract class ItsmReportingRepositoryBase<TEntity, TPrimaryKey> : EfRepositoryBase<ItsmReportingDbContext, TEntity, TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>
    {
        protected ItsmReportingRepositoryBase(IDbContextProvider<ItsmReportingDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        //add common methods for all repositories
    }

    public abstract class ItsmReportingRepositoryBase<TEntity> : ItsmReportingRepositoryBase<TEntity, int>
        where TEntity : class, IEntity<int>
    {
        protected ItsmReportingRepositoryBase(IDbContextProvider<ItsmReportingDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        //do not add any method here, add to the class above (since this inherits it)
    }
}

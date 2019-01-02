using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.IdentityFramework;
using Abp.Runtime.Session;
using Iksap.ItsmReporting.Authorization.Users;
using Iksap.ItsmReporting.MultiTenancy;
using Iksap.ItsmReporting.Users;
using Microsoft.AspNet.Identity;

namespace Iksap.ItsmReporting
{
    /// <summary>
    /// Derive your application services from this class.
    /// </summary>
    public abstract class ItsmReportingAppServiceBase : ApplicationService
    {
        public TenantManager TenantManager { get; set; }

        public UserManager UserManager { get; set; }

        protected ItsmReportingAppServiceBase()
        {
            LocalizationSourceName = ItsmReportingConsts.LocalizationSourceName;
        }

        protected virtual Task<User> GetCurrentUserAsync()
        {
            var user = UserManager.FindByIdAsync(AbpSession.GetUserId());
            if (user == null)
            {
                throw new ApplicationException("There is no current user!");
            }

            return user;
        }

        protected virtual Task<Tenant> GetCurrentTenantAsync()
        {
            return TenantManager.GetByIdAsync(AbpSession.GetTenantId());
        }

        protected virtual void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }
    }
}
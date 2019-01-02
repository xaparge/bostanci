using System.Threading.Tasks;
using Abp.Application.Services;
using Iksap.ItsmReporting.Authorization.Accounts.Dto;

namespace Iksap.ItsmReporting.Authorization.Accounts
{
    public interface IAccountAppService : IApplicationService
    {
        Task<IsTenantAvailableOutput> IsTenantAvailable(IsTenantAvailableInput input);

        Task<RegisterOutput> Register(RegisterInput input);
    }
}

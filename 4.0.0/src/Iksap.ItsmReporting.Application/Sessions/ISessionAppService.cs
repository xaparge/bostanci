using System.Threading.Tasks;
using Abp.Application.Services;
using Iksap.ItsmReporting.Sessions.Dto;

namespace Iksap.ItsmReporting.Sessions
{
    public interface ISessionAppService : IApplicationService
    {
        Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformations();
    }
}

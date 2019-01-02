using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Iksap.ItsmReporting.MultiTenancy.Dto;

namespace Iksap.ItsmReporting.MultiTenancy
{
    public interface ITenantAppService : IAsyncCrudAppService<TenantDto, int, PagedResultRequestDto, CreateTenantDto, TenantDto>
    {
    }
}

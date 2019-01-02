using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Iksap.ItsmReporting.Roles.Dto;

namespace Iksap.ItsmReporting.Roles
{
    public interface IRoleAppService : IAsyncCrudAppService<RoleDto, int, PagedResultRequestDto, CreateRoleDto, RoleDto>
    {
        Task<ListResultDto<PermissionDto>> GetAllPermissions();
    }
}

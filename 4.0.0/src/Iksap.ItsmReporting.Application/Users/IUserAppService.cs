using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Iksap.ItsmReporting.Roles.Dto;
using Iksap.ItsmReporting.Users.Dto;

namespace Iksap.ItsmReporting.Users
{
    public interface IUserAppService : IAsyncCrudAppService<UserDto, long, PagedResultRequestDto, CreateUserDto, UpdateUserDto>
    {
        Task<ListResultDto<RoleDto>> GetRoles();
    }
}
using System.Collections.Generic;
using Iksap.ItsmReporting.Roles.Dto;
using Iksap.ItsmReporting.Users.Dto;

namespace Iksap.ItsmReporting.Web.Models.Users
{
    public class UserListViewModel
    {
        public IReadOnlyList<UserDto> Users { get; set; }

        public IReadOnlyList<RoleDto> Roles { get; set; }
    }
}
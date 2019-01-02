using System.Collections.Generic;
using Iksap.ItsmReporting.Roles.Dto;

namespace Iksap.ItsmReporting.Web.Models.Roles
{
    public class RoleListViewModel
    {
        public IReadOnlyList<RoleDto> Roles { get; set; }

        public IReadOnlyList<PermissionDto> Permissions { get; set; }
    }
}
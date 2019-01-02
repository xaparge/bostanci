using Abp.Authorization;
using Iksap.ItsmReporting.Authorization.Roles;
using Iksap.ItsmReporting.Authorization.Users;

namespace Iksap.ItsmReporting.Authorization
{
    public class PermissionChecker : PermissionChecker<Role, User>
    {
        public PermissionChecker(UserManager userManager)
            : base(userManager)
        {

        }
    }
}

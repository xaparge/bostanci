using Abp.AutoMapper;
using Iksap.ItsmReporting.Sessions.Dto;

namespace Iksap.ItsmReporting.Web.Models.Account
{
    [AutoMapFrom(typeof(GetCurrentLoginInformationsOutput))]
    public class TenantChangeViewModel
    {
        public TenantLoginInfoDto Tenant { get; set; }
    }
}
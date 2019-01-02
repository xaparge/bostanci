using System.Threading.Tasks;
using Abp.Application.Services;
using Iksap.ItsmReporting.Configuration.Dto;

namespace Iksap.ItsmReporting.Configuration
{
    public interface IConfigurationAppService: IApplicationService
    {
        Task ChangeUiTheme(ChangeUiThemeInput input);
    }
}
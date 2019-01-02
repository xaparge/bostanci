using System.Threading.Tasks;
using Abp.Authorization;
using Abp.Runtime.Session;
using Iksap.ItsmReporting.Configuration.Dto;

namespace Iksap.ItsmReporting.Configuration
{
    [AbpAuthorize]
    public class ConfigurationAppService : ItsmReportingAppServiceBase, IConfigurationAppService
    {
        public async Task ChangeUiTheme(ChangeUiThemeInput input)
        {
            await SettingManager.ChangeSettingForUserAsync(AbpSession.ToUserIdentifier(), AppSettingNames.UiTheme, input.Theme);
        }
    }
}

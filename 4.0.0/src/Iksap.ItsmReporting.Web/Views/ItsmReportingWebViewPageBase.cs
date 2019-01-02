using Abp.Web.Mvc.Views;

namespace Iksap.ItsmReporting.Web.Views
{
    public abstract class ItsmReportingWebViewPageBase : ItsmReportingWebViewPageBase<dynamic>
    {

    }

    public abstract class ItsmReportingWebViewPageBase<TModel> : AbpWebViewPage<TModel>
    {
        protected ItsmReportingWebViewPageBase()
        {
            LocalizationSourceName = ItsmReportingConsts.LocalizationSourceName;
        }
    }
}
using Abp.Application.Navigation;
using Abp.Localization;
using Iksap.ItsmReporting.Authorization;

namespace Iksap.ItsmReporting.Web
{
    /// <summary>
    /// This class defines menus for the application.
    /// It uses ABP's menu system.
    /// When you add menu items here, they are automatically appear in angular application.
    /// See Views/Layout/_TopMenu.cshtml file to know how to render menu.
    /// </summary>
    public class ItsmReportingNavigationProvider : NavigationProvider
    {
        public override void SetNavigation(INavigationProviderContext context)
        {
            context.Manager.MainMenu
                .AddItem(
                    new MenuItemDefinition(
                        PageNames.Home,
                        L("HomePage"),
                        url: "",
                        icon: "home",
                        requiresAuthentication: true
                    )
                ).AddItem(
                    new MenuItemDefinition(
                        PageNames.Tenants,
                        L("Tenants"),
                        url: "Tenants",
                        icon: "business",
                        requiredPermissionName: PermissionNames.Pages_Tenants
                    )
                ).AddItem(
                    new MenuItemDefinition(
                        PageNames.Users,
                        L("Users"),
                        url: "Users",
                        icon: "people",
                        requiredPermissionName: PermissionNames.Pages_Users
                    )
                ).AddItem(
                    new MenuItemDefinition(
                        PageNames.Roles,
                        L("Roles"),
                        url: "Roles",
                        icon: "local_offer",
                        requiredPermissionName: PermissionNames.Pages_Roles
                    )
                )
                .AddItem(
                    new MenuItemDefinition(
                        PageNames.About,
                        L("About"),
                        url: "About",
                        icon: "info"
                    )
                );
                //.AddItem( //Menu items below is just for demonstration!
                //    new MenuItemDefinition(
                //        "SettingsMenu",
                //        L("SettingsMenu"),
                //        icon: "menu"
                //    ).AddItem(
                //        new MenuItemDefinition(
                //            "E-Mail",
                //            new FixedLocalizableString("E-Mail")
                //        ).AddItem(
                //            new MenuItemDefinition(
                //                PageNames.MailScheduledTask,
                //                L("mailScheduledTask"),
                //                url: "MailScheduledTask",
                //                icon: "info"
                //            )
                //        ).AddItem(
                //            new MenuItemDefinition(
                //                "Gönderilenler",
                //                new FixedLocalizableString("Gönderilenler"),
                //               url: "About",
                //                icon: "info"
                //            )
                //        )
                //    )
                //);


            //).AddItem(
            //    new MenuItemDefinition(
            //        "AspNetBoilerplateSamples",
            //        new FixedLocalizableString("Samples"),
            //        url: "https://aspnetboilerplate.com/Samples?ref=abptmpl"
            //    )
            //).AddItem(
            //    new MenuItemDefinition(
            //        "AspNetBoilerplateDocuments",
            //        new FixedLocalizableString("Documents"),
            //        url: "https://aspnetboilerplate.com/Pages/Documents?ref=abptmpl"
            //    )
            //)
            //.AddItem(
            //    new MenuItemDefinition(
            //        "AspNetZero",
            //        new FixedLocalizableString("ASP.NET Zero")
            //    ).AddItem(
            //        new MenuItemDefinition(
            //            "AspNetZeroHome",
            //            new FixedLocalizableString("Home"),
            //            url: "https://aspnetzero.com?ref=abptmpl"
            //        )
            //    ).AddItem(
            //        new MenuItemDefinition(
            //            "AspNetZeroDescription",
            //            new FixedLocalizableString("Description"),
            //            url: "https://aspnetzero.com/?ref=abptmpl#description"
            //        )
            //    ).AddItem(
            //        new MenuItemDefinition(
            //            "AspNetZeroFeatures",
            //            new FixedLocalizableString("Features"),
            //            url: "https://aspnetzero.com/?ref=abptmpl#features"
            //        )
            //    ).AddItem(
            //        new MenuItemDefinition(
            //            "AspNetZeroPricing",
            //            new FixedLocalizableString("Pricing"),
            //            url: "https://aspnetzero.com/?ref=abptmpl#pricing"
            //        )
            //    ).AddItem(
            //        new MenuItemDefinition(
            //            "AspNetZeroFaq",
            //            new FixedLocalizableString("Faq"),
            //            url: "https://aspnetzero.com/Faq?ref=abptmpl"
            //        )
            //    ).AddItem(
            //        new MenuItemDefinition(
            //            "AspNetZeroDocuments",
            //            new FixedLocalizableString("Documents"),
            //            url: "https://aspnetzero.com/Documents?ref=abptmpl"
            //        )
            //    )
        }

        private static ILocalizableString L(string name)
        {
            return new LocalizableString(name, ItsmReportingConsts.LocalizationSourceName);
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Iksap.ItsmReporting.Web.Controllers.Sla;
using Iksap.ItsmReporting.Web.Models.Sla;

using Abp.Application.Services.Dto;
using Abp.Web.Mvc.Authorization;
using Iksap.ItsmReporting.Authorization;
using Iksap.ItsmReporting.Authorization.Roles;
using Iksap.ItsmReporting.Users;
using Iksap.ItsmReporting.Web.Models.Users;
using System.Threading.Tasks;

namespace Iksap.ItsmReporting.Web.Controllers
{
    [AbpMvcAuthorize(PermissionNames.Pages_Users)]
    public class MailScheduledTaskController : ItsmReportingControllerBase
    {
        private readonly IUserAppService _userAppService;
        private readonly RoleManager _roleManager;

        public MailScheduledTaskController(IUserAppService userAppService, RoleManager roleManager)
        {
            _userAppService = userAppService;
            _roleManager = roleManager;
        }

        public async Task<ActionResult> Index()
        {
            var users = (await _userAppService.GetAll(new PagedResultRequestDto { MaxResultCount = int.MaxValue })).Items; //Paging not implemented yet
            var roles = (await _userAppService.GetRoles()).Items;
            var model = new UserListViewModel
            {
                Users = users,
                Roles = roles
            };

            return View(model);
        }

        public async Task<ActionResult> EditUserModal(long userId)
        {
            var user = await _userAppService.Get(new EntityDto<long>(userId));
            var roles = (await _userAppService.GetRoles()).Items;
            var model = new EditUserModalViewModel
            {
                User = user,
                Roles = roles
            };
            return View("_EditUserModal", model);
        }

        //public ActionResult Index()
        //{
        //    //sendMail();
        //    return View();
        //}
        public bool sendMail()
        {
            try
            {
                SlaReport sr = new SlaReport();
                List<SingleSlaTable> singleSla = new List<SingleSlaTable>();

                singleSla = sr.getSingleSlaTables("open", 0, 0,"");

                SendMail sm = new SendMail();
                sm.SendMailToUsers(singleSla);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
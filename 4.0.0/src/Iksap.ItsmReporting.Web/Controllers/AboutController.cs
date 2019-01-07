using Iksap.ItsmReporting.Web.Controllers.Sla;
using Iksap.ItsmReporting.Web.Models.Sla;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Iksap.ItsmReporting.Web.Controllers
{
    public class AboutController : ItsmReportingControllerBase
    {
        public ActionResult Index()
        {
            //sendMail();
            return View();
        }
        
    }
}
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
        public bool sendMail()
        {
            try
            {
                SlaReport sr = new SlaReport();
                List<SingleSlaTable> singleSla = new List<SingleSlaTable>();

                singleSla = sr.getSingleSlaTables("open", 0, 0);

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
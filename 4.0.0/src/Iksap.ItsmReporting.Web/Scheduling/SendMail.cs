using Iksap.ItsmReporting.Web.Controllers.Sla;
using Iksap.ItsmReporting.Web.Models.Sla;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Iksap.ItsmReporting.Web.Scheduling
{
    public class SendMail : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            try
            {
                SlaReport sr = new SlaReport();
                List<SingleSlaTable> singleSla = new List<SingleSlaTable>();

                singleSla = sr.getSingleSlaTables("open", 0, 0);

                Controllers.Sla.SendMail sm = new Controllers.Sla.SendMail();
                sm.SendMailToUsers(singleSla);
            }
            catch (Exception ex)
            { }
        }
    }
}
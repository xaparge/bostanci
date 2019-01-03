using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Iksap.ItsmReporting.Web.Scheduling
{
    public class SchedulerWrapper
    {
        public void RunJob()
        {
            try
            {
                ISchedulerFactory schedFact = new StdSchedulerFactory();
                IScheduler sched = schedFact.GetScheduler();
                if (!sched.IsStarted)
                    sched.Start();

                IJobDetail jobMail = JobBuilder.Create<SendMail>().WithIdentity("MailScheduler", null).Build();
                ISimpleTrigger triggerMail = (ISimpleTrigger)TriggerBuilder.Create().WithIdentity("MailScheduler").StartAt(DateTime.UtcNow).WithSimpleSchedule(x => x.WithIntervalInMinutes(60).RepeatForever()).Build();
                sched.ScheduleJob(jobMail, triggerMail);
            }
            catch (Exception ex)
            {
            }
        }
    }
}
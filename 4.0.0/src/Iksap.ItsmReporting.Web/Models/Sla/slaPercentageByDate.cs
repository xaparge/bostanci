using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Iksap.ItsmReporting.Web.Models.Sla
{
    public class slaPercentageByDate
    {
        public int PercentYear { get; set; }
        public int PercentMonth { get; set; }
        public double SuccessfulPercentage { get; set; }
        public double FailedPercentage { get; set; }
        public bool IsDeleted { get; set; }
        public int DeletedUserId { get; set; }
        public DateTime DeletionTime { get; set; }
        public DateTime LastModificationTime { get; set; }
        public DateTime CreationTime { get; set; }
    }
}
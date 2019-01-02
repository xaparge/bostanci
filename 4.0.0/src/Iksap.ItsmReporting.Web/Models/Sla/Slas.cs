using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Iksap.ItsmReporting.Web.Models.Sla
{
    public class Slas
    {
        public List<SlaTable> slaTable { get; set; }
        public SingleSlaTable singleSlaTable { get; set; }
    }
}
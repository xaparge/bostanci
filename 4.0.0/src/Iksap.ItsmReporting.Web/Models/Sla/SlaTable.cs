using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Iksap.ItsmReporting.Web.Models.Sla
{
    public class SlaTable
    {
        public int id { get; set; }
        public DateTime changed_on { get; set; }
        public int old_value { get; set; }
        public int value { get; set; }
        public string value_name { get; set; }
        public string subject { get; set; }
        public DateTime created_on { get; set; }
        public DateTime closed_on { get; set; }
        public Rate rate { get; set; }
        public int sla_time_hour { get; set; }
        public int sla_time_minute { get; set; }
        public int sla_time_second { get; set; }
    }
}
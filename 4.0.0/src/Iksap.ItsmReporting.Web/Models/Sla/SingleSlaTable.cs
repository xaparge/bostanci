using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Iksap.ItsmReporting.Web.Models.Sla
{
    public class SingleSlaTable
    {
        public int id { get; set; }

        public DateTime created_on { get; set; }    // sonuçları kontrol etmek için eklendi
        public DateTime closed_on { get; set; }    // sonuçları kontrol etmek için eklendi

        public string address { get; set; }     // mail adresi

        public DateTime start_time { get; set; }
        public DateTime end_time { get; set; }

        public Rate rate { get; set; }

        public int sla_time_hour { get; set; }
        public int sla_time_minute { get; set; }
        public int sla_time_second { get; set; }

        public string firstname { get; set; }
        public string lastname { get; set; }
        public double last_sent_percent { get; set; }

        public double success_rate { get; set; }
    }
}
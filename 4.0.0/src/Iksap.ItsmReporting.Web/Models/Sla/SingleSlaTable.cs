using System;

namespace Iksap.ItsmReporting.Web.Models.Sla
{
    public class SingleSlaTable
    {
        public int id { get; set; }
        public int project_id { get; set; }

        public string created_on_str { get; set; }
        public DateTime created_on { get; set; }    // sonuçları kontrol etmek için eklendi
        public string closed_on_str { get; set; }
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

        public string redmine_link { get; set; }    // index.js'de verilecek linkte kullanılacaktır.
    }
}
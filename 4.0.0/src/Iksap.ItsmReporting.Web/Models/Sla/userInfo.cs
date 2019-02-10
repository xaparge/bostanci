using System;
using System.Collections.Generic;

namespace Iksap.ItsmReporting.Web.Models.Sla
{
    public class userInfo
    {
        public int id { get; set; }
        public int old_value { get; set; }
        public int value { get; set; }
        public string value_name { get; set; }
        public int iksapUser { get; set; }
        public string prop_key { get; set; }

        public DateTime start_time { get; set; }
        public DateTime end_time { get; set; }
        public string start_time_str { get; set; }
        public string end_time_str { get; set; }

        public string firstname { get; set; }
        public string lastname { get; set; }
        public string mail_address { get; set; }

        public int sla_time_hour { get; set; }
        public int sla_time_minute { get; set; }
        public int sla_time_second { get; set; }
    }
}
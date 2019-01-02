using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Iksap.ItsmReporting.Web.Models.Sla
{
    public class Rate
    {
        public int id { get; set; }
        public DateTime work_start_time { get; set; }   // sadece saat ve dakika bilgisi kullanılacak
        public DateTime work_end_time { get; set; }   // sadece saat ve dakika bilgisi kullanılacak
        public int total_time { get; set; }
        public int time_limit { get; set; }
    }
}
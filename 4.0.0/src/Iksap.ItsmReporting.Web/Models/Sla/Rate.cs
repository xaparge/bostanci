using System;

namespace Iksap.ItsmReporting.Web.Models.Sla
{
    public class Rate
    {
        public int id { get; set; }
        public string name { get; set; }
        public DateTime work_start_time { get; set; }   // sadece saat ve dakika bilgisi kullanılacak
        public DateTime work_end_time { get; set; }   // sadece saat ve dakika bilgisi kullanılacak
        public DateTime lunch_start_time { get; set; }   // sadece saat ve dakika bilgisi kullanılacak
        public DateTime lunch_end_time { get; set; }   // sadece saat ve dakika bilgisi kullanılacak
        public int Is_7_24 { get; set; }
        public int total_time { get; set; }
        public int time_limit { get; set; }
    }
}
using System;

namespace Iksap.ItsmReporting.Web.Models.Sla
{
    public class SlaTable
    {
        public int id { get; set; }
        public DateTime changed_on { get; set; }
        public int old_value { get; set; }
        public int value { get; set; }
        public string value_name { get; set; }
        public DateTime created_on { get; set; }
        public DateTime closed_on { get; set; }
        public string prop_key { get; set; }
        public int project_id { get; set; }
        public string project_name { get; set; }
        public string subject { get; set; }
        public Rate rate { get; set; }
        public string assigns_firstname { get; set; }
        public string assigns_lastname { get; set; }
        public string assigns_mail_address { get; set; }
        public string assigned_firstname { get; set; }
        public string assigned_lastname { get; set; }
        public string assigned_mail_address { get; set; }
        public int iksapUser { get; set; }

        public int action_user_id { get; set; }
        public string action_firstname { get; set; }
        public string action_lastname { get; set; }
        public string action_mail_address { get; set; }
        
        //public int sla_time_hour { get; set; }
        //public int sla_time_minute { get; set; }
        //public int sla_time_second { get; set; }
        //public int user_id { get; set; }
        //public string user_firstname { get; set; }
        //public string user_lastname { get; set; }
    }
}
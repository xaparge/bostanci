//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Iksap.ItsmReporting.Web.Models.DataModel
{
    using System;
    using System.Collections.Generic;
    
    public partial class global_issue_templates
    {
        public int id { get; set; }
        public string title { get; set; }
        public string issue_title { get; set; }
        public Nullable<int> tracker_id { get; set; }
        public Nullable<int> author_id { get; set; }
        public string note { get; set; }
        public string description { get; set; }
        public bool enabled { get; set; }
        public Nullable<int> position { get; set; }
        public Nullable<System.DateTime> created_on { get; set; }
        public Nullable<System.DateTime> updated_on { get; set; }
        public string checklist_json { get; set; }
        public bool is_default { get; set; }
    }
}

﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class bitnami_redmineEntities1 : DbContext
    {
        public bitnami_redmineEntities1()
            : base("name=bitnami_redmineEntities1")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<attachments> attachments { get; set; }
        public virtual DbSet<auth_sources> auth_sources { get; set; }
        public virtual DbSet<boards> boards { get; set; }
        public virtual DbSet<changes> changes { get; set; }
        public virtual DbSet<changesets> changesets { get; set; }
        public virtual DbSet<comments> comments { get; set; }
        public virtual DbSet<custom_field_enumerations> custom_field_enumerations { get; set; }
        public virtual DbSet<custom_fields> custom_fields { get; set; }
        public virtual DbSet<custom_report_series> custom_report_series { get; set; }
        public virtual DbSet<custom_reports> custom_reports { get; set; }
        public virtual DbSet<custom_values> custom_values { get; set; }
        public virtual DbSet<documents> documents { get; set; }
        public virtual DbSet<email_addresses> email_addresses { get; set; }
        public virtual DbSet<enabled_modules> enabled_modules { get; set; }
        public virtual DbSet<enumerations> enumerations { get; set; }
        public virtual DbSet<global_issue_templates> global_issue_templates { get; set; }
        public virtual DbSet<import_items> import_items { get; set; }
        public virtual DbSet<imports> imports { get; set; }
        public virtual DbSet<issue_categories> issue_categories { get; set; }
        public virtual DbSet<issue_relations> issue_relations { get; set; }
        public virtual DbSet<issue_statuses> issue_statuses { get; set; }
        public virtual DbSet<issue_template_settings> issue_template_settings { get; set; }
        public virtual DbSet<issue_templates> issue_templates { get; set; }
        public virtual DbSet<issues> issues { get; set; }
        public virtual DbSet<journal_details> journal_details { get; set; }
        public virtual DbSet<journals> journals { get; set; }
        public virtual DbSet<member_roles> member_roles { get; set; }
        public virtual DbSet<members> members { get; set; }
        public virtual DbSet<messages> messages { get; set; }
        public virtual DbSet<news> news { get; set; }
        public virtual DbSet<open_id_authentication_associations> open_id_authentication_associations { get; set; }
        public virtual DbSet<open_id_authentication_nonces> open_id_authentication_nonces { get; set; }
        public virtual DbSet<projects> projects { get; set; }
        public virtual DbSet<projects_default_queries> projects_default_queries { get; set; }
        public virtual DbSet<queries> queries { get; set; }
        public virtual DbSet<repositories> repositories { get; set; }
        public virtual DbSet<roles> roles { get; set; }
        public virtual DbSet<settings> settings { get; set; }
        public virtual DbSet<time_entries> time_entries { get; set; }
        public virtual DbSet<tokens> tokens { get; set; }
        public virtual DbSet<trackers> trackers { get; set; }
        public virtual DbSet<user_preferences> user_preferences { get; set; }
        public virtual DbSet<users> users { get; set; }
        public virtual DbSet<versions> versions { get; set; }
        public virtual DbSet<watchers> watchers { get; set; }
        public virtual DbSet<wiki_content_versions> wiki_content_versions { get; set; }
        public virtual DbSet<wiki_contents> wiki_contents { get; set; }
        public virtual DbSet<wiki_pages> wiki_pages { get; set; }
        public virtual DbSet<wiki_redirects> wiki_redirects { get; set; }
        public virtual DbSet<wikis> wikis { get; set; }
        public virtual DbSet<workflows> workflows { get; set; }
        public virtual DbSet<changeset_parents> changeset_parents { get; set; }
        public virtual DbSet<changesets_issues> changesets_issues { get; set; }
        public virtual DbSet<custom_fields_projects> custom_fields_projects { get; set; }
        public virtual DbSet<custom_fields_roles> custom_fields_roles { get; set; }
        public virtual DbSet<custom_fields_trackers> custom_fields_trackers { get; set; }
        public virtual DbSet<groups_users> groups_users { get; set; }
        public virtual DbSet<projects_trackers> projects_trackers { get; set; }
        public virtual DbSet<queries_roles> queries_roles { get; set; }
        public virtual DbSet<roles_managed_roles> roles_managed_roles { get; set; }
        public virtual DbSet<schema_migrations> schema_migrations { get; set; }
    }
}

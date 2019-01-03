using Iksap.ItsmReporting.Web.Models.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Iksap.ItsmReporting.Web.Models.Home
{
    public class ProjectsListService
    {
        bitnami_redmineEntities1 context;
        public ProjectsListService()
        {
            context = new bitnami_redmineEntities1();
        }


        public IQueryable<PersonViewModel> GetProjects()
        {
            return from p in context.projects
        
                   select new PersonViewModel
                   {
                       id = p.id,
                       name = p.name

                   };
        }
        public class PersonViewModel
        {
            public int id { get; set; }
            public string name { get; set; }
        }
    }
}
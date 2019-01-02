using Iksap.ItsmReporting.Web.Models.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Iksap.ItsmReporting.Web.Models
{
    public class PersonService
    {
        bitnami_redmineEntities1 context;
        public PersonService()
        {
            context = new bitnami_redmineEntities1();
        }


        public IQueryable<PersonViewModel> GetPeople()
        {
            return from p in context.users

                   select new PersonViewModel
                   {
                       firstname = p.firstname,
                       lastname = p.lastname,
                       login = p.login
                   };
        }
    }
}
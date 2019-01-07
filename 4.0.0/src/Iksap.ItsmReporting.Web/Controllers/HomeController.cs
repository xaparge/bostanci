using System.Web.Mvc;
using Abp.Web.Mvc.Authorization;
using Iksap.ItsmReporting.Web.Models.Sla;
using Iksap.ItsmReporting.Web.Controllers.Sla;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Dynamic;
using System;
using Iksap.ItsmReporting.Web.Models;
using Iksap.ItsmReporting.Web.Models.DataModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Iksap.ItsmReporting.Web.Scheduling;
using Iksap.ItsmReporting.Web.Models.Home;
using static Iksap.ItsmReporting.Web.Models.Home.ProjetsTreeList;
using Microsoft.Ajax.Utilities;

namespace Iksap.ItsmReporting.Web.Controllers

{
    [AbpMvcAuthorize]
    public class HomeController : ItsmReportingControllerBase
    {
        //PersonService personService = new PersonService();
        //ProjectsListService projectsListService = new ProjectsListService();
        ProjetsTreeList projetsTreeList = new ProjetsTreeList();
       private static Dictionary<int, string> months = new Dictionary<int, string>(){{1,"Ocak"},{2, "Şubat"}, {3,"Mart"},
                                                                        {4,"Nisan"},  {5,"Mayıs"},{6,"Haziran"},
                                                                        {7,"Temmuz"}, {8,"Ağustos"},{9,"Eylül"},
                                                                        {10,"Ekim"}, {11,"Kasım"}, {12,"Aralık"}};
        private static Dictionary< string, int> monthsNumber = new Dictionary<string,int>(){{"Ocak",1},{"Şubat",2}, {"Mart",3},
                                                                        {"Nisan",4},  {"Mayıs",5},{"Haziran",6},
                                                                        {"Temmuz",7}, {"Ağustos",8},{"Eylül",9},
                                                                        {"Ekim",10}, {"Kasım",11}, {"Aralık",12}};


        public ActionResult Index()
        {
            return View();
        }
        //[HttpPost]
        //[System.Web.Mvc.Route("ItsmReport/Home/GetProjetsTreeList")]
        public JsonResult GetProjetsTreeList()
        {

            //var projects = projectsListService.GetProjects().ToList();

            return Json(projetsTreeList.PopulateTreeView(), JsonRequestBehavior.AllowGet);
        }
        //[HttpPost]
        //[System.Web.Mvc.Route("ItsmReport/Home/SlaMonthlyChart")]
        public JsonResult SlaMonthlyChart(string projects)
        {
            if (projects == null)
                projects = "1";
            List<int> projectsIdList = new List<int>();
            projectsIdList = projects.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();

           
            SlaReport sr = new SlaReport();
            List<SingleSlaTable> singleSla = new List<SingleSlaTable>();

            Dictionary<string, List<string>> dictionary = new Dictionary<string, List<string>>();

            double success_count;
            double fail_count;
            int month = DateTime.Now.Month;
            int year = DateTime.Now.Year;
            for (int i = 0; i < 12; i++)
            {
                success_count = 0;
                fail_count = 0;
                singleSla = sr.getSingleSlaTables("close", month, year, projects);

                for (int j = 0; j < singleSla.Count; j++)
                {
                    if (singleSla[j].success_rate < 100)
                    {
                        success_count++;
                    }
                    else if (singleSla[j].success_rate >= 100)
                    {
                        fail_count++;
                    }
                }
                double success_rate = Math.Round((success_count * 100) / (success_count + fail_count), 2);
                double fail_rate = Math.Round((fail_count * 100) / (success_count + fail_count), 2);
                List<string> successAndFail2 = new List<string>();
                successAndFail2.Add(CheckNull(success_rate));
                successAndFail2.Add(CheckNull(fail_rate));

                dictionary[months[month]] = successAndFail2;

                if (month > 1)
                    month--;
                else
                {
                    month = 12;
                    year -= 1;
                }

            }

            List<object> iData = new List<object>();
            //Creating sample data
            DataTable dt = new DataTable();
            dt.Columns.Add("ay", System.Type.GetType("System.String"));
            dt.Columns.Add("pozitif", System.Type.GetType("System.Double"));
            dt.Columns.Add("negatif", System.Type.GetType("System.Double"));

            int month_count = DateTime.Now.Month + 1;
            int yearlabel = DateTime.Now.Year-1;
            for (int i = 0; i < dictionary.Count; i++)
            {
                DataRow dr = dt.NewRow();

                dr["ay"] = yearlabel +" - "+months[month_count];
                dr["pozitif"] = Math.Round(Convert.ToDouble(dictionary[months[month_count]][0]), 2);
                dr["negatif"] = Math.Round(Convert.ToDouble(dictionary[months[month_count]][1]), 2);
                dt.Rows.Add(dr);

                
                if (month_count < 12) { 
                    month_count++;
                }
                else { 
                    month_count = 1;
                    yearlabel = DateTime.Now.Year;
                }
            }

            //Looping and extracting each DataColumn to List<Object>
            foreach (DataColumn dc in dt.Columns)
            {
                List<object> y = new List<object>();
                y = (from DataRow drr in dt.Rows select drr[dc.ColumnName]).ToList();
                iData.Add(y);
            }
            //Source data returned as JSON
            return Json(iData, JsonRequestBehavior.AllowGet);
        }
      
        public string CheckNull(double item)
        {
            if (!Double.IsNaN(item))
            {
                return item.ToString();

            }
            else return "0";


        }

       

        //[HttpPost]
        //[System.Web.Mvc.Route("ItsmReport/Home/SlaMonthlyChartDetailTable")]
        public JsonResult SlaMonthlyChartDetailTable(string projects, string month, string year)
        {

          
            SlaReport slaReport = new SlaReport();
            SlaDetailTable dataTable = new SlaDetailTable();
            //dataTable.draw = int.Parse(Request.QueryString["draw"]);

            List<SingleSlaTable> singleSla = new List<SingleSlaTable>();

            singleSla = slaReport.getSingleSlaTables("close", monthsNumber[month.Trim()], Convert.ToInt32(year.Trim()), projects);
            //singleSla = slaReport.getSingleSlaTablesPaging("close", monthsNumber[month], year, projects, 0, 10);

            string filterTicketId = Request.QueryString["ticketId"];
            string filterCreatedOn = Request.QueryString["created_on"];
            string filterClosedOn= Request.QueryString["closed_on"];
            string filterRate = Request.QueryString["rate"];
            var result = from s in singleSla
                         where (string.IsNullOrEmpty(filterTicketId) || s.id.Equals(filterTicketId))
                         && (string.IsNullOrEmpty(filterCreatedOn) || s.closed_on.Equals(filterCreatedOn))
                         && (string.IsNullOrEmpty(filterClosedOn) || s.success_rate.Equals(filterClosedOn))
                         && (string.IsNullOrEmpty(filterRate) || s.success_rate.Equals(filterRate))
                         select s;
            //select new
            //{
            //    s.id,
            //    s.closed_on,
            //    s.success_rate
            //};

            dataTable.data = result.ToArray();
            dataTable.recordsTotal = singleSla.Count;
            dataTable.recordsFiltered = result.Count();
            string link = "http://89.106.1.162/redmine/issues/";
            for (int i = 0; i < singleSla.Count; i++)
            {
                singleSla[i].redmine_link = "<a href=" + link + singleSla[i].id + " target =\"_blank\">" + singleSla[i].id + "</a>";
                singleSla[i].created_on_str = singleSla[i].created_on.ToString("dd/MM/yyyy HH:mm:ss");
                singleSla[i].closed_on_str = singleSla[i].closed_on.ToString("dd/MM/yyyy HH:mm:ss");
            }

            return Json(dataTable, JsonRequestBehavior.AllowGet);
        }

        //[HttpPost]
        //public JsonResult getPeople() 
        //{
        //    var draw = Request.Form.GetValues("draw").FirstOrDefault();
        //    var start = Request.Form.GetValues("start").FirstOrDefault();
        //    var length = Request.Form.GetValues("length").FirstOrDefault();

        
        //    //Global search field
        //    var search = Request.Form.GetValues("search[value]").FirstOrDefault();

        //    //Custom column search fields
        //    //var firstName = Request.Form.GetValues("columns[1][search][value]").FirstOrDefault();
        //    //var lastName = Request.Form.GetValues("columns[2][search][value]").FirstOrDefault();
        //    //var login = Request.Form.GetValues("columns[3][search][value]").FirstOrDefault();

        //    string sortColumnName = Request["columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]"];//Sıralama yapılacak column adı
        //    string sortDirection = Request["order[0][dir]"];//sıralama türü

        //    int pageSize = length != null ? Convert.ToInt32(length) : 0;
        //    int skip = start != null ? Convert.ToInt32(start) : 0;
        //    int recordsTotal = 0;

        //    //var people = personService.GetPeople(); // Get People IQueryble
        //    var people = personService.GetPeople();


        //    ////Start search
        //    //if (!string.IsNullOrWhiteSpace(firstName))
        //    //{
        //    //    people = people.Where(x => x.firstname.ToLower().Contains(firstName.ToLower()));

        //    //}



        //    //if (!string.IsNullOrWhiteSpace(lastName))
        //    //{
        //    //    people = people.Where(x => x.lastname.ToLower().Contains(lastName.ToLower()));
        //    //}

        //    //if (!string.IsNullOrWhiteSpace(login))
        //    //{
        //    //    people = people.Where(x => x.login.ToLower().Contains(login.ToLower()));
        //    //}



        //    if (!string.IsNullOrEmpty(search))
        //    {
        //        people = people.Where(x => x.login.ToLower().Contains(search.ToLower())
        //          || x.firstname.ToLower().Contains(search.ToLower())
        //          || x.lastname.ToLower().Contains(search.ToLower())
        //          || x.login.ToLower().Contains(search.ToLower()));
        //    }

        //    recordsTotal = people.Count();
        //    var data = people.OrderBy(x => x.firstname).Skip(skip).Take(pageSize).ToList();
        //    int FiltrelenmisKayitSayisi = data.Count;
        //    //return Json(new { data = data, draw = Request["draw"], recordsTotal = recordsTotal, recordsFiltered = FiltrelenmisKayitSayisi });



        //    return Json(data, JsonRequestBehavior.AllowGet);
        //    //return Json(data, JsonRequestBehavior.AllowGet);
        //    //return Json(new
        //    //{
        //    //    draw = draw,
        //    //    recordsFiltered = FiltrelenmisKayitSayisi,
        //    //    recordsTotal = recordsTotal,
        //    //    data = data
        //    //}, JsonRequestBehavior.AllowGet);
        //}
        public partial class isler
        {
            [Required]
            [StringLength(90)]
            public string firstname { get; set; }

            [Required]
            [StringLength(90)]
            public string lastname { get; set; }

            [Required]
            [StringLength(75)]
            public string login { get; set; }

            //[Column(TypeName = "date")]
            //public DateTime Tarih { get; set; }
        }

    }
}
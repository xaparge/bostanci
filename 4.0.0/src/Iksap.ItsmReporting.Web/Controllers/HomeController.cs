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
using System.ComponentModel.DataAnnotations;
using Abp.Runtime.Security;
using Newtonsoft.Json;
using System.Globalization;

namespace Iksap.ItsmReporting.Web.Controllers

{
    [AbpMvcAuthorize]
    public class HomeController : ItsmReportingControllerBase
    {
        private static Dictionary<int, string> months;
        private static Dictionary<string, int> monthsNumber;

        public ActionResult Index()
        {
            months = new Dictionary<int, string>(){{1, L("January")},{2, L("February")}, {3, L("March")},
                                                                        {4, L("April")},  {5, L("May")},{6, L("June")},
                                                                        {7, L("July")}, {8, L("August")},{9, L("September")},
                                                                        {10, L("October")}, {11, "November"}, {12, L("December")}};
            monthsNumber = new Dictionary<string, int>(){{L("January"), 1},{L("February"), 2}, {L("March"), 3},
                                                                        {L("April"), 4},  {L("May"), 5},{L("June"), 6},
                                                                        {L("July"), 7}, {L("August"), 8},{L("September"), 9},
                                                                        {L("October"), 10}, {L("November"), 11}, {L("December"), 12}};
            return View();
        }

        public JsonResult GetProjectsTreeList()
        {
            var currentUserId = User.Identity.GetUserId();

            GetProjectsAndSub createJson = new GetProjectsAndSub();
            var treeList = createJson.getProjects((int)currentUserId);
            dynamic json = JsonConvert.DeserializeObject(treeList);
            return Json(json, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SlaMonthlyChart(string projectsName)
        {
            SlaReport sr = new SlaReport();
            List<SingleSlaTable> singleSla = new List<SingleSlaTable>();

            GetProjectsAndSub calculate = new GetProjectsAndSub();
            string selectedProjectNumbers = calculate.getProjects(projectsName);

            Dictionary<string, List<string>> dictionary = new Dictionary<string, List<string>>();

            double success_count;
            double fail_count;
            int month = DateTime.Now.Month;
            int year = DateTime.Now.Year;
            for (int i = 0; i < 12; i++)
            {
                success_count = 0;
                fail_count = 0;
                singleSla = sr.getSingleSlaTables("close", month, year, selectedProjectNumbers);

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
            int yearlabel = DateTime.Now.Year - 1;
            for (int i = 0; i < dictionary.Count; i++)
            {
                DataRow dr = dt.NewRow();

                dr["ay"] = yearlabel + " - " + months[month_count];
                dr["pozitif"] = Math.Round(Convert.ToDouble(dictionary[months[month_count]][0]), 2);
                dr["negatif"] = Math.Round(Convert.ToDouble(dictionary[months[month_count]][1]), 2);
                dt.Rows.Add(dr);

                if (month_count < 12)
                {
                    month_count++;
                }
                else
                {
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

            List<string> slaLable = new List<string>();
            slaLable.Add(L("SlaProviders"));
            slaLable.Add(L("SlaPassed"));
            slaLable.Add(L("SlaMontlyPercent"));
            iData.Add(slaLable);
            //Source data returned as JSON
            return Json(iData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SlaMonthlyChartByProject(string projects)
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
                        success_count++;
                    else if (singleSla[j].success_rate >= 100)
                        fail_count++;
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
            int yearlabel = DateTime.Now.Year - 1;
            for (int i = 0; i < dictionary.Count; i++)
            {
                DataRow dr = dt.NewRow();

                dr["ay"] = yearlabel + " - " + months[month_count];
                dr["pozitif"] = Math.Round(Convert.ToDouble(dictionary[months[month_count]][0]), 2);
                dr["negatif"] = Math.Round(Convert.ToDouble(dictionary[months[month_count]][1]), 2);
                dt.Rows.Add(dr);

                if (month_count < 12)
                    month_count++;
                else
                {
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

            List<string> slaLable = new List<string>();
            slaLable.Add(L("SlaProviders"));
            slaLable.Add(L("SlaPassed"));
            slaLable.Add(L("SlaMontlyPercent"));
            iData.Add(slaLable);
            //Source data returned as JSON
            return Json(iData, JsonRequestBehavior.AllowGet);
        }

        public string CheckNull(double item)
        {
            if (!Double.IsNaN(item))
                return item.ToString();
            else return "0";
        }

        public JsonResult SlaMonthlyChartDetailTable(string projects, string month, string year)
        {
            SlaReport slaReport = new SlaReport();
            SlaDetailTable dataTable = new SlaDetailTable();

            GetProjectsAndSub calculate = new GetProjectsAndSub();
            string selectedProjectNumbers = calculate.getProjects(projects);

            List<SingleSlaTable> singleSla = new List<SingleSlaTable>();

            singleSla = slaReport.getSingleSlaTables("close", monthsNumber[month.Trim()], Convert.ToInt32(year.Trim()), selectedProjectNumbers);

            string filterTicketId = Request.QueryString["ticketId"];
            string filterCreatedOn = Request.QueryString["created_on"];
            string filterClosedOn = Request.QueryString["closed_on"];
            string filterRate = Request.QueryString["rate"];
            var result = from s in singleSla
                         where (string.IsNullOrEmpty(filterTicketId) || s.id.Equals(filterTicketId))
                         && (string.IsNullOrEmpty(filterCreatedOn) || s.closed_on.Equals(filterCreatedOn))
                         && (string.IsNullOrEmpty(filterClosedOn) || s.success_rate.Equals(filterClosedOn))
                         && (string.IsNullOrEmpty(filterRate) || s.success_rate.Equals(filterRate))
                         select s;

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

        public string getCurrentLanguage()
        {
            var currentCulture = CultureInfo.CurrentCulture.ToString();
            string language_url;
            if (currentCulture == "tr")
                language_url = "//cdn.datatables.net/plug-ins/1.10.19/i18n/Turkish.json";
            else if (currentCulture == "en")
                language_url = "//cdn.datatables.net/plug-ins/1.10.19/i18n/English.json";
            else if (currentCulture == "es")
                language_url = "//cdn.datatables.net/plug-ins/1.10.19/i18n/Spanish.json";
            else if (currentCulture == "fr")
                language_url = "//cdn.datatables.net/plug-ins/1.10.19/i18n/French.json";
            else if (currentCulture == "it")
                language_url = "//cdn.datatables.net/plug-ins/1.10.19/i18n/Italian.json";
            else if (currentCulture == "lt")
                language_url = "//cdn.datatables.net/plug-ins/1.10.19/i18n/Lithuanian.json";
            else if (currentCulture == "nl-NL")
                language_url = "//cdn.datatables.net/plug-ins/1.10.19/i18n/Dutch.json";
            else if (currentCulture == "pt-BR")
                language_url = "//cdn.datatables.net/plug-ins/1.10.19/i18n/Portuguese-Brasil.json";
            else if (currentCulture == "ja")
                language_url = "//cdn.datatables.net/plug-ins/1.10.19/i18n/Japanese.json";
            else if (currentCulture == "zh-CN")
                language_url = "//cdn.datatables.net/plug-ins/1.10.19/i18n/Chinese.json";
            else     // en
                language_url = "//cdn.datatables.net/plug-ins/1.10.19/i18n/English.json";
            return language_url;
        }

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
        }
    }
}
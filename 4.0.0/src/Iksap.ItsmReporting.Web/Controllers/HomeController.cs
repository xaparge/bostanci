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

namespace Iksap.ItsmReporting.Web.Controllers

{
    [AbpMvcAuthorize]
    public class HomeController : ItsmReportingControllerBase
    {
        PersonService personService = new PersonService();

        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public JsonResult SlaMonthlyChart2()
        {
            SlaReport sr = new SlaReport();
            List<SingleSlaTable> singleSla = new List<SingleSlaTable>();

            Dictionary<int, List<double>> dictionary = new Dictionary<int, List<double>>();
            double success_count = 0;
            double fail_count = 0;
            for (int i = 1; i < 13; i++)
            {
                singleSla = sr.getSingleSlaTables("close", i, 2018);

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
                double success_rate = (success_count * 100) / (success_count + fail_count);
                double fail_rate = (fail_count * 100) / (success_count + fail_count);

                List<double> successAndFail = new List<double>();

                successAndFail.Add(success_rate);
                successAndFail.Add(fail_rate);

                dictionary.Add(i, successAndFail);
                slaPercentageByDate abc = new slaPercentageByDate();
                abc.PercentMonth = i;
                abc.PercentYear = 2018;
                abc.SuccessfulPercentage = Math.Round(success_rate, 2).ToString();
                abc.FailedPercentage = Math.Round(fail_rate, 2).ToString();
                sr.insertSlaPercentageByDate(abc);
            }


            List<object> iData = new List<object>();
            //Creating sample data
            DataTable dt = new DataTable();
            dt.Columns.Add("ay", System.Type.GetType("System.String"));
            dt.Columns.Add("pozitif", System.Type.GetType("System.Int32"));
            dt.Columns.Add("negatif", System.Type.GetType("System.Int32"));


            foreach (KeyValuePair<int, List<double>> item in dictionary)
            {
                DataRow dr = dt.NewRow();
                dr["ay"] = item.Key;
                dr["pozitif"] = item.Value[0];
                dr["negatif"] = item.Value[1];
                dt.Rows.Add(dr);

            }

            //Looping and extracting each DataColumn to List<Object>
            foreach (DataColumn dc in dt.Columns)
            {
                List<object> x = new List<object>();
                x = (from DataRow drr in dt.Rows select drr[dc.ColumnName]).ToList();
                iData.Add(x);
            }
            //Source data returned as JSON
            return Json(iData, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [System.Web.Mvc.Route("ItsmReport/Home/SlaMonthlyChart")]
        public JsonResult SlaMonthlyChart()
        {
            SlaReport sr = new SlaReport();
            List<SingleSlaTable> singleSla = new List<SingleSlaTable>();

            Dictionary<int, List<string>> dictionary = new Dictionary<int, List<string>>();
            double success_count = 0;
            double fail_count = 0;

            List<slaPercentageByDate> old_reports = new List<slaPercentageByDate>();
            old_reports = sr.getSlaPercentageByDate();
            for (int i = 0; i < 12; i++)
            {
                List<string> successAndFail = new List<string>();

                successAndFail.Add(old_reports[old_reports.Count - i - 1].SuccessfulPercentage);
                successAndFail.Add(old_reports[old_reports.Count - i - 1].FailedPercentage);
                dictionary.Add(old_reports.Count - i, successAndFail);
            }

            singleSla = sr.getSingleSlaTables("close", DateTime.Now.Month, 2018);

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

            dictionary[DateTime.Now.Month] = successAndFail2;
            slaPercentageByDate x = new slaPercentageByDate();
            x.PercentYear = DateTime.Now.Year;
            x.PercentMonth = DateTime.Now.Month;
            x.SuccessfulPercentage = success_rate.ToString();
            x.FailedPercentage = fail_rate.ToString();
            sr.updateSlaPercentageByDate(x);

            List<object> iData = new List<object>();
            //Creating sample data
            DataTable dt = new DataTable();
            dt.Columns.Add("ay", System.Type.GetType("System.String"));
            dt.Columns.Add("pozitif", System.Type.GetType("System.Double"));
            dt.Columns.Add("negatif", System.Type.GetType("System.Double"));
            for (int i = 0; i < dictionary.Count; i++)
            {

                DataRow dr = dt.NewRow();
                dr["ay"] = i + 1;
                dr["pozitif"] = Math.Round(Convert.ToDouble(dictionary[i + 1][0]), 2);
                dr["negatif"] = Math.Round(Convert.ToDouble(dictionary[i + 1][1]), 2);
                dt.Rows.Add(dr);
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
        [HttpPost]
        public JsonResult getPeople()
        {
            var draw = Request.Form.GetValues("draw").FirstOrDefault();
            var start = Request.Form.GetValues("start").FirstOrDefault();
            var length = Request.Form.GetValues("length").FirstOrDefault();


            //Global search field
            var search = Request.Form.GetValues("search[value]").FirstOrDefault();

            //Custom column search fields
            //var firstName = Request.Form.GetValues("columns[1][search][value]").FirstOrDefault();
            //var lastName = Request.Form.GetValues("columns[2][search][value]").FirstOrDefault();
            //var login = Request.Form.GetValues("columns[3][search][value]").FirstOrDefault();

            string sortColumnName = Request["columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]"];//Sıralama yapılacak column adı
            string sortDirection = Request["order[0][dir]"];//sıralama türü

            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int recordsTotal = 0;

            //var people = personService.GetPeople(); // Get People IQueryble
            var people = personService.GetPeople();


            ////Start search
            //if (!string.IsNullOrWhiteSpace(firstName))
            //{
            //    people = people.Where(x => x.firstname.ToLower().Contains(firstName.ToLower()));

            //}



            //if (!string.IsNullOrWhiteSpace(lastName))
            //{
            //    people = people.Where(x => x.lastname.ToLower().Contains(lastName.ToLower()));
            //}

            //if (!string.IsNullOrWhiteSpace(login))
            //{
            //    people = people.Where(x => x.login.ToLower().Contains(login.ToLower()));
            //}



            if (!string.IsNullOrEmpty(search))
            {
                people = people.Where(x => x.login.ToLower().Contains(search.ToLower())
                  || x.firstname.ToLower().Contains(search.ToLower())
                  || x.lastname.ToLower().Contains(search.ToLower())
                  || x.login.ToLower().Contains(search.ToLower()));
            }

            recordsTotal = people.Count();
            var data = people.OrderBy(x => x.firstname).Skip(skip).Take(pageSize).ToList();
            int FiltrelenmisKayitSayisi = data.Count;
            //return Json(new { data = data, draw = Request["draw"], recordsTotal = recordsTotal, recordsFiltered = FiltrelenmisKayitSayisi });



            return Json(data, JsonRequestBehavior.AllowGet);
            //return Json(data, JsonRequestBehavior.AllowGet);
            //return Json(new
            //{
            //    draw = draw,
            //    recordsFiltered = FiltrelenmisKayitSayisi,
            //    recordsTotal = recordsTotal,
            //    data = data
            //}, JsonRequestBehavior.AllowGet);
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

            //[Column(TypeName = "date")]
            //public DateTime Tarih { get; set; }
        }

    }
}
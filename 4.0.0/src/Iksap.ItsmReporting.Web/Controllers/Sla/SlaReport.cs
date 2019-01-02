using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Iksap.ItsmReporting.Web.Models.Sla;
using MySql.Data.MySqlClient;
using System.Data;
using Iksap.ItsmReporting.Web.Models.DataModel;

namespace Iksap.ItsmReporting.Web.Controllers.Sla
{
    public class SlaReport
    {
        MySqlConnection dbConn = new MySqlConnection("server = 127.0.0.1; uid=root;pwd=12345678;database=itsmreporting_operations");

        public List<SingleSlaTable> getSingleSlaTables(string project_state, int month, int year)   // Açık projelerde month ve year parametreleri kullanılmadığı için rastgele int değer verilebilir.
        {
            MySqlCommand dbComm;
            if (project_state == "open")
            {
                dbComm = new MySqlCommand("itsmreporting_operations.slaOpenProject", dbConn);
            }
            else if (project_state == "close")
            {
                dbComm = new MySqlCommand("itsmreporting_operations.slaClosedProjectByDate", dbConn);
                dbComm.Parameters.AddWithValue("@monthvalue", month);
                dbComm.Parameters.AddWithValue("@yearvalue", year);
            }
            else
            {
                List<SingleSlaTable> sst = new List<SingleSlaTable>();
                return sst;
            }
            dbComm.CommandType = CommandType.StoredProcedure;

            var sla = slaList(dbComm);
            var singleSla = NormalizedSla(sla);

            for (int i = 0; i < singleSla.Count; i++)
            {
                Slas temp = new Slas();
                temp = MainReport(sla, singleSla[i]);
                singleSla[i] = temp.singleSlaTable;
                sla = temp.slaTable;
            }

            // UPDATE KISMI HOMECONTROLLER'DA YAPILABİLİR
            //slaPercentageByDate old_reports = new slaPercentageByDate();
            //old_reports.PercentYear = year;
            //old_reports.PercentMonth = month;
            //old_reports.SuccessfulPercentage = singleSla
            //updateSlaPercentageByDate(old_reports);

            return singleSla;
        }

        public List<slaPercentageByDate> getSlaPercentageByDate()
        {
            MySqlCommand dbComm = new MySqlCommand("Select PercentYear, PercentMonth, SuccessfulPercentage, FailedPercentage From sla_percentage_bydate", dbConn);
            DataTable dt = new DataTable();
            dbConn.Open();
            MySqlDataAdapter da = new MySqlDataAdapter(dbComm);
            da.Fill(dt);
            dbConn.Close();

            List<slaPercentageByDate> old_percentage = new List<slaPercentageByDate>();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                slaPercentageByDate temp = new slaPercentageByDate();
                temp.PercentYear = Convert.ToInt32(dt.Rows[i][0]);
                temp.PercentMonth = Convert.ToInt32(dt.Rows[i][1]);
                temp.SuccessfulPercentage = Convert.ToDouble(dt.Rows[i][2]);
                temp.FailedPercentage = Convert.ToDouble(dt.Rows[i][3]);
                old_percentage.Add(temp);
            }
            return old_percentage;
        }

        public void insertSlaPercentageByDate(slaPercentageByDate old_percentage)
        {
            try
            {
                MySqlCommand dbComm = new MySqlCommand("Insert Into sla_percentage_bydate(PercentYear, PercentMonth, SuccessfulPercentage, FailedPercentage) Values('" + old_percentage.PercentYear + "', '" + old_percentage.PercentMonth + "', '" + old_percentage.SuccessfulPercentage + "', '" + old_percentage.FailedPercentage + "')", dbConn);
                dbConn.Open();
                dbComm.ExecuteNonQuery();
                dbConn.Close();
            }
            catch (Exception ex) { }
        }

        public void updateSlaPercentageByDate(slaPercentageByDate old_percentage)
        {
            #region ayların sözlüğe eklenmesi
            Dictionary<int, string> months = new Dictionary<int, string>();
            months.Add(1, "january");
            months.Add(2, "february");
            months.Add(3, "march");
            months.Add(4, "april");
            months.Add(5, "may");
            months.Add(6, "june");
            months.Add(7, "july");
            months.Add(8, "august");
            months.Add(9, "september");
            months.Add(10, "october");
            months.Add(11, "november");
            months.Add(12, "december");
            #endregion

            try
            {
                MySqlCommand dbComm = new MySqlCommand("Update sla_percentage_bydate Set SuccessfulPercentage = " + old_percentage.SuccessfulPercentage + ", FailedPercentage = " + old_percentage.FailedPercentage + " Where PercentYear = " + old_percentage.PercentYear + " and PercentMonth = " + old_percentage.PercentMonth, dbConn);
                dbConn.Open();
                dbComm.ExecuteNonQuery();
                dbConn.Close();
            }
            catch (Exception ex)
            {

            }
        }

        private List<SlaTable> slaList(MySqlCommand cmd)
        {
            dbConn.Open();
            DataTable dt = new DataTable();
            MySqlDataAdapter da = new MySqlDataAdapter(cmd);
            da.Fill(dt);
            dbConn.Close();

            List<SlaTable> slaList = new List<SlaTable>();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                SlaTable temp = new SlaTable();
                temp.id = Convert.ToInt32(dt.Rows[i][0]);
                try
                {
                    temp.changed_on = Convert.ToDateTime(dt.Rows[i][1].ToString());
                }
                catch
                {
                    temp.changed_on = Convert.ToDateTime("1000-01-01");
                }
                try
                {
                    temp.old_value = Convert.ToInt32(dt.Rows[i][3]);
                }
                catch
                {
                    temp.old_value = 0;     // null değerler yerine 0 atandı
                }
                try
                {
                    temp.value = Convert.ToInt32(dt.Rows[i][4]);
                }
                catch
                {
                    temp.value = 0;     // null değerler yerine 0 atandı
                }
                if (dt.Rows[i][5].ToString() != "")
                {
                    temp.value_name = dt.Rows[i][5].ToString();
                }
                else { temp.value_name = "----------"; }
                temp.subject = dt.Rows[i][6].ToString();
                try
                {
                    temp.created_on = Convert.ToDateTime(dt.Rows[i][7].ToString());
                }
                catch
                {
                    temp.created_on = Convert.ToDateTime("1000-01-01");
                }
                try
                {
                    temp.closed_on = Convert.ToDateTime(dt.Rows[i][8].ToString());
                }
                catch
                {
                    temp.closed_on = Convert.ToDateTime("1000-01-01");
                }
                if (dt.Rows[i][2].ToString() == "Immediate")
                {
                    Rate r = new Rate();
                    r.id = 1;
                    r.time_limit = 4;
                    temp.rate = r;
                }
                else if (dt.Rows[i][2].ToString() == "Urgent")
                {
                    Rate r = new Rate();
                    r.id = 2;
                    r.work_start_time = new DateTime(1001, 01, 01, 09, 00, 00);     // sadece saat ve dakika kısmı kullanılacak
                    r.work_end_time = new DateTime(1001, 01, 01, 18, 00, 00);     // sadece saat ve dakika kısmı kullanılacak
                    r.total_time = 9;
                    r.time_limit = 9;
                    temp.rate = r;
                }
                else if (dt.Rows[i][2].ToString() == "High")
                {
                    Rate r = new Rate();
                    r.id = 3;
                    r.work_start_time = new DateTime(1001, 01, 01, 09, 00, 00);     // sadece saat ve dakika kısmı kullanılacak
                    r.work_end_time = new DateTime(1001, 01, 01, 18, 00, 00);     // sadece saat ve dakika kısmı kullanılacak
                    r.total_time = 9;
                    r.time_limit = 18;
                    temp.rate = r;
                }
                else if (dt.Rows[i][2].ToString() == "Normal")
                {
                    Rate r = new Rate();
                    r.id = 4;
                    r.work_start_time = new DateTime(1001, 01, 01, 09, 00, 00);     // sadece saat ve dakika kısmı kullanılacak
                    r.work_end_time = new DateTime(1001, 01, 01, 18, 00, 00);     // sadece saat ve dakika kısmı kullanılacak
                    r.total_time = 9;
                    r.time_limit = 27;
                    temp.rate = r;
                }
                else if (dt.Rows[i][2].ToString() == "Low")
                {
                    Rate r = new Rate();
                    r.id = 5;
                    r.work_start_time = new DateTime(1001, 01, 01, 09, 00, 00);     // sadece saat ve dakika kısmı kullanılacak
                    r.work_end_time = new DateTime(1001, 01, 01, 18, 00, 00);     // sadece saat ve dakika kısmı kullanılacak
                    r.total_time = 9;
                    r.time_limit = 45;
                    temp.rate = r;
                }

                slaList.Add(temp);
            }
            return slaList;
        }

        private List<SingleSlaTable> NormalizedSla(List<SlaTable> slaList)
        {
            List<SingleSlaTable> singleSla = new List<SingleSlaTable>();
            for (int i = 0; i < slaList.Count; i++)
            {
                bool flag = false;
                for (int j = 0; j < singleSla.Count; j++)
                {
                    if (slaList[i].id == singleSla[j].id)
                    {
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                {
                    SingleSlaTable temp = new SingleSlaTable();
                    temp.id = slaList[i].id;
                    singleSla.Add(temp);
                }
            }
            return singleSla;
        }

        private Slas MainReport(List<SlaTable> slaTable, SingleSlaTable singleSlaTable)
        {
            int slaIdCount = 0;     // Aynı id'deki ticketların kaç tane olduğunu saklar.
            for (int i = 0; i < slaTable.Count; i++)   // slaTable'daki en üstteki aynı id'li veri sayısını bulur.
            {
                if (slaTable[i].id == singleSlaTable.id)
                {
                    slaIdCount++;
                }
                else
                { break; }
            }

            singleSlaTable.rate = slaTable[0].rate;

            List<int> slaActiveTime = new List<int> { 0, 1, 2, 7, 10 };   // 0-null 1-yeni 2-çalışılıyor 7-efor bekleniyor 10-değişiklik bekleniyor

            bool start_time = false;

            if (slaActiveTime.Contains(slaTable[0].old_value))
            {
                singleSlaTable.start_time = slaTable[0].created_on;
                start_time = true;
            }

            for (int i = 0; i < slaIdCount; i++)
            {
                if (slaActiveTime.Contains(slaTable[i].value) && !start_time)
                {
                    singleSlaTable.start_time = slaTable[i].changed_on;
                    start_time = true;
                }
                else if (!slaActiveTime.Contains(slaTable[i].value) && start_time)
                {
                    singleSlaTable.end_time = slaTable[i].changed_on;

                    if (singleSlaTable.rate.id == 1)
                        singleSlaTable = CalculateSlaTime_Immediate(singleSlaTable);
                    else
                        singleSlaTable = CalculateSlaTime_Normal(singleSlaTable);

                    start_time = false;
                }
            }

            if (start_time)
            {
                singleSlaTable.end_time = DateTime.Now;

                if (singleSlaTable.rate.id == 1)
                    singleSlaTable = CalculateSlaTime_Immediate(singleSlaTable);
                else
                    singleSlaTable = CalculateSlaTime_Normal(singleSlaTable);
            }

            // success_rate hesaplaması:
            double tempSecond_past = (singleSlaTable.sla_time_hour * 3600) + (singleSlaTable.sla_time_minute * 60) + singleSlaTable.sla_time_second;   // yüzde hesaplama saniye üzerinden yapılması için gerekli dönüşüm yapıldı
            double tempSecond_time_limit = singleSlaTable.rate.time_limit * 3600;   // yüzde hesaplama saniye üzerinden yapılması için gerekli dönüşüm yapıldı
            singleSlaTable.success_rate = Math.Round((100 * tempSecond_past) / tempSecond_time_limit, 2);

            for (int i = 0; i < slaIdCount; i++)
            {
                slaTable.RemoveAt(0);
            }
            Slas all_of_them = new Slas();
            all_of_them.slaTable = slaTable;
            all_of_them.singleSlaTable = singleSlaTable;
            return all_of_them;
        }

        private SingleSlaTable CalculateSlaTime_Immediate(SingleSlaTable sla)
        {
            TimeSpan ts = new TimeSpan();
            ts = sla.end_time - sla.start_time;

            SingleSlaTable temp = new SingleSlaTable();
            temp = AddTime_Immediate(sla, ts);

            return sla;
        }

        private SingleSlaTable CalculateSlaTime_Normal(SingleSlaTable sla)
        {
            DateTime temp_date = sla.start_time;
            while (temp_date.Date <= sla.end_time.Date)
            {
                if (temp_date.DayOfWeek.ToString() != "Saturday" && temp_date.DayOfWeek.ToString() != "Sunday")
                {
                    if (temp_date.Date == sla.start_time.Date && temp_date.Hour >= sla.rate.work_start_time.Hour && temp_date.Hour < sla.rate.work_end_time.Hour)   // oluşturulan tarih başlangıç tarihi ise, günlük 9 saat değil, request başlangıç saatinden itibaren günün bitiş saatine kadar hesaplar.
                    {
                        TimeSpan ts;
                        sla.rate.work_end_time = new DateTime(sla.start_time.Year, sla.start_time.Month, sla.start_time.Day, sla.rate.work_end_time.Hour, sla.rate.work_end_time.Minute, sla.rate.work_end_time.Second);  // Amaç sadece work_and time'ın date kısmını başlangıç tarihi yapmak.
                        if (sla.end_time < sla.rate.work_end_time)      // bitiş saati mesai saatinden önceyse son zaman olarak bitiş saatini alır.
                        {
                            ts = sla.end_time.TimeOfDay - sla.start_time.TimeOfDay;
                        }
                        else
                        {
                            ts = sla.rate.work_end_time.TimeOfDay - sla.start_time.TimeOfDay;  // sla_time eklenecek saat hesaplaması, elde işlemleri
                        }

                        SingleSlaTable temp = new SingleSlaTable();
                        temp = AddTime_Normal(sla, ts);
                        sla.sla_time_hour = temp.sla_time_hour;
                        sla.sla_time_minute = temp.sla_time_minute;
                        sla.sla_time_second = temp.sla_time_second;

                    }
                    else if (temp_date.Date == sla.start_time.Date && temp_date.Hour > sla.rate.work_end_time.Hour)
                    {
                        // haftaiçi gün ama mesai saati geçmiş durumudur.
                    }
                    else if (temp_date.Date == sla.end_time.Date)  // request bitiş tarihindeki son günün saatini hesaplar.
                    {
                        if (sla.end_time.Hour >= sla.rate.work_start_time.Hour)
                        {
                            TimeSpan ts = sla.end_time.TimeOfDay - sla.rate.work_start_time.TimeOfDay;

                            SingleSlaTable temp = new SingleSlaTable();
                            temp = AddTime_Normal(sla, ts);
                            sla.sla_time_hour = temp.sla_time_hour;
                            sla.sla_time_minute = temp.sla_time_minute;
                            sla.sla_time_second = temp.sla_time_second;
                        }
                    }
                    else
                    {
                        sla.sla_time_hour += sla.rate.total_time;
                    }
                }
                temp_date = temp_date.AddDays(1);
            }
            return sla;
        }

        private SingleSlaTable AddTime_Normal(SingleSlaTable info, TimeSpan ts)
        {
            if (ts.Hours > 8)
            {
                DateTime dt1 = new DateTime(2018, 1, 1, 9, 0, 0);
                DateTime dt2 = new DateTime(2018, 1, 1, 0, 0, 0);
                ts = dt1 - dt2;
            }

            info.sla_time_second += ts.Seconds;
            if (info.sla_time_second > 59)
            {
                info.sla_time_second -= 60;
                info.sla_time_minute += 1;
                if (info.sla_time_minute > 59)
                {
                    info.sla_time_minute -= 60;
                    info.sla_time_hour += 1;
                }
            }

            info.sla_time_minute += ts.Minutes;
            if (info.sla_time_minute > 59)
            {
                info.sla_time_minute -= 60;
                info.sla_time_hour += 1;
            }

            info.sla_time_hour += ts.Hours;
            return info;
        }

        private SingleSlaTable AddTime_Immediate(SingleSlaTable info, TimeSpan ts)
        {
            info.sla_time_second += ts.Seconds;
            if (info.sla_time_second > 59)
            {
                info.sla_time_second -= 60;
                info.sla_time_minute += 1;
                if (info.sla_time_minute > 59)
                {
                    info.sla_time_minute -= 60;
                    info.sla_time_hour += 1;
                }
            }

            info.sla_time_minute += ts.Minutes;
            if (info.sla_time_minute > 59)
            {
                info.sla_time_minute -= 60;
                info.sla_time_hour += 1;
            }

            info.sla_time_hour += ts.Hours;

            info.sla_time_hour += ts.Days * 24;

            return info;
        }

    }
}
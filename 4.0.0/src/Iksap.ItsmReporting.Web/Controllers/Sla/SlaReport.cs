using System;
using System.Collections.Generic;
using Iksap.ItsmReporting.Web.Models.Sla;
using MySql.Data.MySqlClient;
using System.Data;

namespace Iksap.ItsmReporting.Web.Controllers.Sla
{
    public class SlaReport
    {
        MySqlConnection dbConn = new MySqlConnection("server=" + System.Configuration.ConfigurationManager.AppSettings["DbPath"].ToString() + "; uid=root;pwd=" + System.Configuration.ConfigurationManager.AppSettings["DbPassword"].ToString() + "; database=itsmreporting_operations");

        public List<SingleSlaTable> getSingleSlaTables(string project_state, int month, int year, string projectList)   // Açık projelerde month ve year parametreleri kullanılmadığı için rastgele int değer verilebilir.
        {
            MySqlCommand dbComm;
            if (project_state == "open")
            {
                dbComm = new MySqlCommand("itsmreporting_operations.slaOpenProject", dbConn);
            }
            else if (project_state == "close")
            {
                dbComm = new MySqlCommand("itsmreporting_operations.slaClosedProject_assigned", dbConn);
                dbComm.Parameters.AddWithValue("@monthvalue", month);
                dbComm.Parameters.AddWithValue("@yearvalue", year);
                dbComm.Parameters.AddWithValue("@projects_id", projectList);
            }
            else
            {
                List<SingleSlaTable> sst = new List<SingleSlaTable>();
                return sst;
            }
            dbComm.CommandType = CommandType.StoredProcedure;

            var sla = slaList(dbComm);
            var singleSla = NormalizedSla(sla);
            singleSla = getSlaRateInfos(singleSla);

            for (int i = 0; i < singleSla.Count; i++)
            {
                Slas temp = new Slas();
                temp = MainReport(sla, singleSla[i]);
                singleSla[i] = temp.singleSlaTable;
                sla = temp.slaTable;

                singleSla[i] = NormalizedUsers(singleSla[i]);
            }

            return singleSla;
        }

        public SingleSlaTable NormalizedUsers(SingleSlaTable singleSlaTable)
        {
            for (int i = 0; i < singleSlaTable.users.Count; i++)
            {
                bool repeat_control = false;
                for (int j = 0; j < singleSlaTable.singleUsers.Count; j++)
                {
                    if (singleSlaTable.users[i].id == singleSlaTable.singleUsers[j].id)
                    {
                        repeat_control = true;

                        userInfo user = new userInfo();

                        user.id = singleSlaTable.users[i].id;
                        user.firstname = singleSlaTable.users[i].firstname;
                        user.lastname = singleSlaTable.users[i].lastname;
                        user.iksapUser = singleSlaTable.users[i].iksapUser;
                        user.sla_time_hour = singleSlaTable.users[i].sla_time_hour;
                        user.sla_time_minute = singleSlaTable.users[i].sla_time_minute;
                        user.sla_time_second = singleSlaTable.users[i].sla_time_second;
                        //user = singleSlaTable.users[i];   // bu şekilde yapınca users değerleri değişiyor

                        singleSlaTable.singleUsers[j] = AddTime_Direct(user, singleSlaTable.singleUsers[j]);

                        //singleSlaTable.singleUsers[j] = user;   // singleSlaUser'da bir kişiden başkası varsa süreleri birleştirilip değiştiriliyor
                        break;    // ? (singleUsers'da zaten her bir kullanıcı benzersizdir. Birine atama yapıldıktan sonra başka atama yapılmayacağı için j döngüsünden çıkılabilir)
                    }
                }
                if (!repeat_control)
                {
                    userInfo user = new userInfo();

                    user.id = singleSlaTable.users[i].id;
                    user.firstname = singleSlaTable.users[i].firstname;
                    user.lastname = singleSlaTable.users[i].lastname;
                    user.mail_address = singleSlaTable.users[i].mail_address;
                    user.iksapUser = singleSlaTable.users[i].iksapUser;
                    user.sla_time_hour = singleSlaTable.users[i].sla_time_hour;
                    user.sla_time_minute = singleSlaTable.users[i].sla_time_minute;
                    user.sla_time_second = singleSlaTable.users[i].sla_time_second;
                    user.start_time = singleSlaTable.created_on;
                    user.start_time = singleSlaTable.closed_on;

                    //user = AddTime_Direct(user, singleSlaTable.users[i]);
                    singleSlaTable.singleUsers.Add(user);
                }
            }

            //if (singleSlaTable.id == 322)
            //{ }   // debug'ta kontrol için kullanılıyor

            //bool find_customer = false;
            for (int i = 0; i < singleSlaTable.singleUsers.Count; i++)
            {
                if (singleSlaTable.singleUsers[i].iksapUser == 1)
                {
                    singleSlaTable = AddTime_Direct(singleSlaTable, singleSlaTable.singleUsers[i]);
                }
                //if (!find_customer && singleSlaTable.singleUsers[i].iksapUser == 0)
                //{
                //    singleSlaTable.customer_firstname = singleSlaTable.singleUsers[i].firstname;
                //    singleSlaTable.customer_lastname = singleSlaTable.singleUsers[i].lastname;
                //    find_customer = true;
                //}
            }

            //for (int i = 0; i < singleSlaTable.users.Count; i++)
            //{
            //    if (!find_customer && singleSlaTable.users[i].iksapUser == 0)
            //    {
            //        singleSlaTable.customer_firstname = singleSlaTable.users[i].firstname;
            //        singleSlaTable.customer_lastname = singleSlaTable.users[i].lastname;
            //        find_customer = true;
            //    }
            //}

            //if (singleSlaTable.id == 322)
            //{ }   // debug'ta kontrol için kullanılıyor

            //if ( (singleSlaTable.customer_firstname == null || singleSlaTable.customer_firstname == "" || singleSlaTable.customer_firstname == "Anonymous") && (singleSlaTable.customer_lastname == null || singleSlaTable.customer_lastname == "" || singleSlaTable.customer_lastname == "Anonymous") )
            //{
            //    singleSlaTable.customer_firstname = "-----";
            //    singleSlaTable.customer_lastname = "";
            //}

            // success_rate hesaplaması:
            double tempsecond_past = (singleSlaTable.sla_time_hour * 3600) + (singleSlaTable.sla_time_minute * 60) + singleSlaTable.sla_time_second;   // yüzde hesaplama saniye üzerinden yapılması için gerekli dönüşüm yapıldı
            double tempsecond_time_limit = singleSlaTable.rate.time_limit * 3600;   // yüzde hesaplama saniye üzerinden yapılması için gerekli dönüşüm yapıldı
            singleSlaTable.success_rate = Math.Round((100 * tempsecond_past) / tempsecond_time_limit, 2);


            if (singleSlaTable.singleUsers.Count == 0)
            {
                singleSlaTable.singleUsers.Add(singleSlaTable.users[0]);
            }

            return singleSlaTable;
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
                temp.prop_key = dt.Rows[i][9].ToString();
                temp.project_id = Convert.ToInt32(dt.Rows[i][10]);
                temp.project_name = dt.Rows[i][11].ToString();
                temp.assigns_firstname = dt.Rows[i][12].ToString();
                temp.assigns_lastname = dt.Rows[i][13].ToString();
                temp.assigns_mail_address = dt.Rows[i][14].ToString();
                temp.assigned_firstname = dt.Rows[i][15].ToString();
                temp.assigned_lastname = dt.Rows[i][16].ToString();
                temp.assigned_mail_address = dt.Rows[i][17].ToString();
                temp.iksapUser = Convert.ToInt32(dt.Rows[i][18]);
                temp.action_user_id = Convert.ToInt32(dt.Rows[i][19]);
                temp.action_firstname = dt.Rows[i][20].ToString();
                temp.action_lastname = dt.Rows[i][21].ToString();
                temp.action_mail_address = dt.Rows[i][22].ToString();

                if (dt.Rows[i][2].ToString() == "Immediate" || dt.Rows[i][2].ToString() == "Urgent")
                {
                    Rate r = new Rate();
                    r.id = 1;
                    r.name = "Urgent";
                    temp.rate = r;
                }
                else if (dt.Rows[i][2].ToString() == "High")
                {
                    Rate r = new Rate();
                    r.id = 2;
                    r.name = dt.Rows[i][2].ToString();
                    temp.rate = r;
                }
                else if (dt.Rows[i][2].ToString() == "Normal")
                {
                    Rate r = new Rate();
                    r.id = 3;
                    r.name = dt.Rows[i][2].ToString();
                    temp.rate = r;
                }
                else if (dt.Rows[i][2].ToString() == "Low")
                {
                    Rate r = new Rate();
                    r.id = 4;
                    r.name = dt.Rows[i][2].ToString();
                    temp.rate = r;
                }

                slaList.Add(temp);
            }
            return slaList;
        }

        private List<SingleSlaTable> getSlaRateInfos(List<SingleSlaTable> slaList)
        {
            try
            {
                MySqlCommand dbComm = new MySqlCommand("SELECT * FROM sla_rate_list", dbConn);
                dbConn.Open();
                DataTable dt = new DataTable();
                MySqlDataAdapter da = new MySqlDataAdapter(dbComm);
                da.Fill(dt);
                dbConn.Close();

                for (int i = 0; i < slaList.Count; i++)
                {
                    bool assign_control = false;
                    for (int j = 4; j < dt.Rows.Count; j++)     // sla_rate_list'te ilk 4 değer default veriler olduğu için döngü 4'ten başlatıldı.
                    {
                        if (slaList[i].project_id == (int)dt.Rows[j][2] && slaList[i].rate.name == dt.Rows[j][1].ToString())
                        {
                            TimeSpan tsWorkStart = TimeSpan.Parse(dt.Rows[j][4].ToString());
                            slaList[i].rate.work_start_time = new DateTime(0001, 01, 01, tsWorkStart.Hours, tsWorkStart.Minutes, 0);

                            TimeSpan tsWorkEnd = TimeSpan.Parse(dt.Rows[j][5].ToString());
                            slaList[i].rate.work_end_time = new DateTime(0001, 01, 01, tsWorkEnd.Hours, tsWorkEnd.Minutes, 0);
                            slaList[i].rate.time_limit = (int)dt.Rows[j][3];

                            slaList[i].rate.Is_7_24 = (int)dt.Rows[j][8];

                            TimeSpan tsLunchStart = TimeSpan.Parse(dt.Rows[j][6].ToString());
                            slaList[i].rate.lunch_start_time = new DateTime(0001, 01, 01, tsLunchStart.Hours, tsLunchStart.Minutes, 0);      // Şimdilik kullanılmıyor, ileriki sürümlerde kullanılabilir diye ataması yapılmıştır.

                            TimeSpan tsLunchEnd = TimeSpan.Parse(dt.Rows[j][7].ToString());
                            slaList[i].rate.lunch_end_time = new DateTime(0001, 01, 01, tsLunchEnd.Hours, tsLunchEnd.Minutes, 0);      // Şimdilik kullanılmıyor, ileriki sürümlerde kullanılabilir diye ataması yapılmıştır.

                            TimeSpan ts = slaList[i].rate.work_end_time - slaList[i].rate.work_start_time;
                            slaList[i].rate.total_time = ts.Hours;
                            assign_control = true;
                            break;
                        }
                    }
                    if (!assign_control)     // bu koşul şirket için özel atanmış değerler yoksa default değer atamasını sağlar.
                    {
                        for (int j = 0; j < 4; j++)
                        {
                            if (slaList[i].rate.name == dt.Rows[j][1].ToString())
                            {
                                TimeSpan tsWorkStart = TimeSpan.Parse(dt.Rows[j][4].ToString());
                                slaList[i].rate.work_start_time = new DateTime(0001, 01, 01, tsWorkStart.Hours, tsWorkStart.Minutes, 0);

                                TimeSpan tsWorkEnd = TimeSpan.Parse(dt.Rows[j][5].ToString());
                                slaList[i].rate.work_end_time = new DateTime(0001, 01, 01, tsWorkEnd.Hours, tsWorkEnd.Minutes, 0);
                                slaList[i].rate.time_limit = (int)dt.Rows[j][3];

                                slaList[i].rate.Is_7_24 = (int)dt.Rows[j][8];

                                TimeSpan tsLunchStart = TimeSpan.Parse(dt.Rows[j][6].ToString());
                                slaList[i].rate.lunch_start_time = new DateTime(0001, 01, 01, tsLunchStart.Hours, tsLunchStart.Minutes, 0);      // Şimdilik kullanılmıyor, ileriki sürümlerde kullanılabilir diye ataması yapılmıştır.

                                TimeSpan tsLunchEnd = TimeSpan.Parse(dt.Rows[j][7].ToString());
                                slaList[i].rate.lunch_end_time = new DateTime(0001, 01, 01, tsLunchEnd.Hours, tsLunchEnd.Minutes, 0);      // Şimdilik kullanılmıyor, ileriki sürümlerde kullanılabilir diye ataması yapılmıştır.

                                TimeSpan ts = slaList[i].rate.work_end_time - slaList[i].rate.work_start_time;
                                slaList[i].rate.total_time = ts.Hours;
                                break;
                            }
                        }
                    }
                }
            }
            catch { }
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
                    temp.project_id = slaList[i].project_id;
                    temp.created_on = slaList[i].created_on;
                    temp.closed_on = slaList[i].closed_on;
                    temp.project_name = slaList[i].project_name;
                    temp.subject = slaList[i].subject;
                    temp.rate = slaList[i].rate;
                    //temp.id = slaList[i].user_id;
                    //temp.firstname = slaList[i].user_firstname;
                    //temp.lastname = slaList[i].user_lastname;
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

            List<int> slaActiveTime = new List<int> { 0, 1, 2, 7, 10 };   // 0-null 1-yeni 2-çalışılıyor 7-efor bekleniyor 10-değişiklik bekleniyor

            //if (singleSlaTable.id == 322)
            //{ }   // debug'ta kontrol için kullanılıyor

            userInfo user_temp_first = new userInfo();
            if (slaTable[0].prop_key == "assigned_to_id")   //  && slaTable[0].iksapUser == 1
            {
                if (slaTable[0].old_value != 0)
                {
                    user_temp_first.id = slaTable[0].old_value;
                    user_temp_first.firstname = slaTable[0].assigns_firstname;
                    user_temp_first.lastname = slaTable[0].assigns_lastname;
                    user_temp_first.mail_address = slaTable[0].assigns_mail_address;
                }
                else       // ilk atananın olmadığı durum
                {
                    user_temp_first.id = slaTable[0].action_user_id;
                    user_temp_first.firstname = slaTable[0].action_firstname;
                    user_temp_first.lastname = slaTable[0].action_lastname;
                    user_temp_first.mail_address = slaTable[0].action_mail_address;
                }

                user_temp_first.prop_key = "assigned_to_id";
                user_temp_first.value = 0;
            }
            else if (slaTable[0].prop_key == "status_id")
            {
                user_temp_first.id = slaTable[0].action_user_id;
                user_temp_first.firstname = slaTable[0].action_firstname;
                user_temp_first.lastname = slaTable[0].action_lastname;
                user_temp_first.mail_address = slaTable[0].action_mail_address;
                user_temp_first.prop_key = "status_id";
                user_temp_first.value = 0;
            }
            user_temp_first.iksapUser = 1;
            user_temp_first.value_name = "Yeni";

            user_temp_first.start_time = slaTable[0].created_on;
            user_temp_first.end_time = slaTable[0].changed_on;
            if (slaTable[0].rate.Is_7_24 == 1)
                user_temp_first = CalculateSlaTime_Immediate(user_temp_first);
            else
                user_temp_first = CalculateSlaTime_Normal(user_temp_first, singleSlaTable.rate);
            singleSlaTable.users.Add(user_temp_first);

            int previous_id;
            string previous_firstname;
            string previous_lastname;
            string previous_mail_address;
            int previous_value;
            string previous_value_name;
            string previous_prop_key;
            int previous_iksapUser;
            if (user_temp_first.prop_key == "assigned_to_id")
            {
                previous_id = slaTable[0].value;
                previous_firstname = slaTable[0].assigned_firstname;
                previous_lastname = slaTable[0].assigned_lastname;
                previous_mail_address = slaTable[0].assigned_mail_address;
                previous_value = 0;
                previous_value_name = "Yeni";
                previous_iksapUser = slaTable[0].iksapUser;
            }
            else //if (user_temp_first.prop_key == "status_id")
            {
                previous_id = slaTable[0].action_user_id;
                previous_firstname = slaTable[0].action_firstname;
                previous_lastname = slaTable[0].action_lastname;
                previous_mail_address = slaTable[0].action_mail_address;
                previous_value = slaTable[0].value;
                previous_value_name = slaTable[0].value_name;
                previous_iksapUser = 1;
            }
            //previous_prop_key = slaTable[0].prop_key;
            previous_prop_key = slaTable[0].prop_key;


            // previous değişkenler hep bir önceki değeri tutar
            for (int i = 1; i < slaIdCount; i++)
            {
                userInfo user_temp = new userInfo();
                if (previous_iksapUser == 1)
                {
                    user_temp.id = previous_id;
                    user_temp.firstname = previous_firstname;
                    user_temp.lastname = previous_lastname;
                    user_temp.mail_address = previous_mail_address;
                    user_temp.value = previous_value;
                    user_temp.value_name = previous_value_name;
                    user_temp.prop_key = previous_prop_key;

                    user_temp.start_time = singleSlaTable.users[singleSlaTable.users.Count - 1].end_time;   // en sonki kullanıcının bitiş zamanı (change_on) bir sonrakinin başlangıç zamanı oluyor
                    user_temp.end_time = slaTable[i].changed_on;
                }
                else
                {
                    user_temp.id = previous_id;
                    user_temp.firstname = previous_firstname;
                    user_temp.lastname = previous_lastname;
                    user_temp.mail_address = previous_mail_address;
                    user_temp.value = -1;
                    user_temp.value_name = previous_value_name;
                    user_temp.start_time = singleSlaTable.users[singleSlaTable.users.Count - 1].end_time;
                    user_temp.end_time = slaTable[i].changed_on;
                    user_temp.prop_key = previous_prop_key;

                    //}
                }
                if (slaTable[i].rate.Is_7_24 == 1)
                    user_temp = CalculateSlaTime_Immediate(user_temp);
                else
                    user_temp = CalculateSlaTime_Normal(user_temp, singleSlaTable.rate);

                user_temp.iksapUser = previous_iksapUser;
                singleSlaTable.users.Add(user_temp);


                if (slaTable[i].prop_key == "assigned_to_id")     // bu durum, ticket iksap'taymış ve müşteriye geçmiş durumudur.
                {
                    previous_id = slaTable[i].value;
                    previous_firstname = slaTable[i].assigned_firstname;
                    previous_lastname = slaTable[i].assigned_lastname;
                    previous_mail_address = slaTable[i].assigned_mail_address;
                    previous_value = -1;
                    previous_iksapUser = slaTable[i].iksapUser;
                }
                else if (slaTable[i].prop_key == "status_id")
                {
                    previous_value = slaTable[i].value;
                    previous_value_name = slaTable[i].value_name;
                    previous_iksapUser = user_temp.iksapUser;
                }
                previous_prop_key = slaTable[i].prop_key;
            }
            userInfo u = new userInfo();
            if (previous_iksapUser == 1)
            {
                u.id = previous_id;
                u.firstname = previous_firstname;
                u.lastname = previous_lastname;
                u.mail_address = previous_mail_address;
                u.value = previous_value;
                u.value_name = previous_value_name;
                u.start_time = singleSlaTable.users[singleSlaTable.users.Count - 1].end_time;   // en sonki kullanıcının bitiş zamanı (change_on) bir sonrakinin başlangıç zamanı oluyor
                u.end_time = DateTime.Now;
                u.prop_key = previous_prop_key;
            }
            else
            {
                u.id = previous_id;
                u.firstname = previous_firstname;
                u.lastname = previous_lastname;
                u.mail_address = previous_mail_address;
                u.value = -1;
                //u.value_name = "Süre Müşteride";
                u.value_name = "----";
                u.start_time = singleSlaTable.users[singleSlaTable.users.Count - 1].end_time;
                u.end_time = slaTable[0].closed_on;
                u.prop_key = previous_prop_key;
            }
            if (slaTable[slaIdCount - 1].rate.Is_7_24 == 1)     // bir önceki rate'e bakıyor çünkü bir sonraki sla'lere geçti
                u = CalculateSlaTime_Immediate(u);
            else
                u = CalculateSlaTime_Normal(u, singleSlaTable.rate);

            u.iksapUser = previous_iksapUser;
            singleSlaTable.users.Add(u);

            //// success_rate hesaplaması:
            //double tempSecond_past = (singleSlaTable.sla_time_hour * 3600) + (singleSlaTable.sla_time_minute * 60) + singleSlaTable.sla_time_second;   // yüzde hesaplama saniye üzerinden yapılması için gerekli dönüşüm yapıldı
            //double tempSecond_time_limit = singleSlaTable.rate.time_limit * 3600;   // yüzde hesaplama saniye üzerinden yapılması için gerekli dönüşüm yapıldı
            //singleSlaTable.success_rate = Math.Round((100 * tempSecond_past) / tempSecond_time_limit, 2);

            for (int i = 0; i < slaIdCount; i++)
            {
                slaTable.RemoveAt(0);
            }
            Slas all_of_them = new Slas();
            all_of_them.slaTable = slaTable;
            all_of_them.singleSlaTable = singleSlaTable;
            return all_of_them;
        }

        private userInfo CalculateSlaTime_Immediate(userInfo user)
        {
            TimeSpan ts = new TimeSpan();
            ts = user.end_time - user.start_time;

            //userInfo temp = new userInfo();
            //temp = AddTime_Immediate(user, ts);

            user.sla_time_hour = ts.Hours;
            user.sla_time_minute = ts.Minutes;
            user.sla_time_second = ts.Seconds;

            return user;
        }

        private userInfo CalculateSlaTime_Normal(userInfo user, Rate rate)
        {
            DateTime temp_date = user.start_time;
            while (temp_date.Date <= user.end_time.Date)
            {
                if (temp_date.DayOfWeek.ToString() != "Saturday" && temp_date.DayOfWeek.ToString() != "Sunday")
                {
                    if (temp_date.Date == user.start_time.Date && temp_date.Hour >= rate.work_start_time.Hour && temp_date.Hour < rate.work_end_time.Hour)   // oluşturulan tarih başlangıç tarihi ise, günlük 9 saat değil, request başlangıç saatinden itibaren günün bitiş saatine kadar hesaplar.
                    {
                        TimeSpan ts;
                        rate.work_end_time = new DateTime(user.start_time.Year, user.start_time.Month, user.start_time.Day, rate.work_end_time.Hour, rate.work_end_time.Minute, rate.work_end_time.Second);  // Amaç sadece work_and time'ın date kısmını başlangıç tarihi yapmak.
                        if (user.end_time < rate.work_end_time)      // bitiş saati mesai saatinden önceyse son zaman olarak bitiş saatini alır.
                        {
                            ts = user.end_time.TimeOfDay - user.start_time.TimeOfDay;
                        }
                        else
                        {
                            ts = rate.work_end_time.TimeOfDay - user.start_time.TimeOfDay;  // sla_time eklenecek saat hesaplaması, elde işlemleri
                        }

                        user = AddTime_Normal(user, ts);

                    }
                    else if (temp_date.Date == user.start_time.Date && temp_date.Hour >= rate.work_end_time.Hour)
                    {
                        // haftaiçi gün ama mesai saati geçmiş durumudur.
                    }
                    else if (temp_date.Date == user.end_time.Date)  // request bitiş tarihindeki son günün saatini hesaplar.
                    {
                        if (user.end_time.Hour >= rate.work_start_time.Hour)
                        {
                            TimeSpan ts = user.end_time.TimeOfDay - rate.work_start_time.TimeOfDay;

                            user = AddTime_Normal(user, ts);
                        }
                    }
                    else
                    {
                        user.sla_time_hour += rate.total_time;
                    }
                }
                temp_date = temp_date.AddDays(1);
            }
            return user;
        }

        private userInfo AddTime_Normal(userInfo user, TimeSpan ts)
        {
            if (ts.Hours > 8)
            {
                DateTime dt1 = new DateTime(2018, 1, 1, 9, 0, 0);
                DateTime dt2 = new DateTime(2018, 1, 1, 0, 0, 0);
                ts = dt1 - dt2;
            }

            user.sla_time_second += ts.Seconds;
            if (user.sla_time_second > 59)
            {
                user.sla_time_second -= 60;
                user.sla_time_minute += 1;
                if (user.sla_time_minute > 59)
                {
                    user.sla_time_minute -= 60;
                    user.sla_time_hour += 1;
                }
            }

            user.sla_time_minute += ts.Minutes;
            if (user.sla_time_minute > 59)
            {
                user.sla_time_minute -= 60;
                user.sla_time_hour += 1;
            }

            user.sla_time_hour += ts.Hours;
            return user;
        }

        private SingleSlaTable AddTime_Direct(SingleSlaTable singleSla, userInfo u2)
        {
            singleSla.sla_time_second += u2.sla_time_second;
            if (singleSla.sla_time_second > 59)
            {
                singleSla.sla_time_second -= 60;
                singleSla.sla_time_minute += 1;
                if (singleSla.sla_time_minute > 59)
                {
                    singleSla.sla_time_minute -= 60;
                    singleSla.sla_time_hour += 1;
                }
            }

            singleSla.sla_time_minute += u2.sla_time_minute;
            if (singleSla.sla_time_minute > 59)
            {
                singleSla.sla_time_minute -= 60;
                singleSla.sla_time_hour += 1;
            }

            singleSla.sla_time_hour += u2.sla_time_hour;

            return singleSla;
        }

        private userInfo AddTime_Direct(userInfo u1, userInfo u2)
        {
            u1.sla_time_second += u2.sla_time_second;
            if (u1.sla_time_second > 59)
            {
                u1.sla_time_second -= 60;
                u1.sla_time_minute += 1;
                if (u1.sla_time_minute > 59)
                {
                    u1.sla_time_minute -= 60;
                    u1.sla_time_hour += 1;
                }
            }

            u1.sla_time_minute += u2.sla_time_minute;
            if (u1.sla_time_minute > 59)
            {
                u1.sla_time_minute -= 60;
                u1.sla_time_hour += 1;
            }

            u1.sla_time_hour += u2.sla_time_hour;

            // yanlışlıkla sonradan kullanılmasın diye veriler temizlendi
            u1.start_time = Convert.ToDateTime("1000-01-01");
            u1.end_time = Convert.ToDateTime("1000-01-01");
            u1.prop_key = "----";
            u1.old_value = 0;
            u1.value = 0;
            u1.value_name = "----";

            return u1;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web.Http;
using MySql.Data.MySqlClient;
using Iksap.ItsmReporting.Web.Models.Sla;

namespace Iksap.ItsmReporting.Web.Controllers.Sla
{
    public class SendMail
    {

        MySqlConnection dbConn = new MySqlConnection("server = 127.0.0.1; uid = root; pwd = 12345678; database = itsmreporting_operations");
        private List<SingleSlaTable> getUsersOfMail(List<SingleSlaTable> singleSla)     // mail gönderilecek ticket'lara mail adresini ad soyad ve son gönderildiği yüzdeyi getirir.
        {
            MySqlCommand dbComm = new MySqlCommand("itsmreporting_operations.slaUsersMail", dbConn);
            dbComm.CommandType = CommandType.StoredProcedure;
            dbConn.Open();
            DataTable dt = new DataTable();
            MySqlDataAdapter da = new MySqlDataAdapter(dbComm);
            da.Fill(dt);
            dbConn.Close();

            for (int i = 0; i < singleSla.Count; i++)
            {
                for (int j = 0; j < dt.Rows.Count; j++)
                {
                    if (singleSla[i].id == Convert.ToInt32(dt.Rows[j][0]))
                    {
                        singleSla[i].firstname = dt.Rows[j][4].ToString();
                        singleSla[i].lastname = dt.Rows[j][5].ToString();
                        singleSla[i].address = dt.Rows[j][6].ToString();
                        if (dt.Rows[j][1].ToString() == "Urgent") { singleSla[i].rate.id = 1; }
                        else if (dt.Rows[j][1].ToString() == "High") { singleSla[i].rate.id = 2; }
                        else if (dt.Rows[j][1].ToString() == "Normal") { singleSla[i].rate.id = 3; }
                        else if (dt.Rows[j][1].ToString() == "Low") { singleSla[i].rate.id = 4; }
                        break;
                    }
                }
            }
            return singleSla;
        }

        private List<int> getPercentOfSendTime()
        {
            MySqlCommand dbComm = new MySqlCommand("Select * From mail_send_of_percentage", dbConn);
            dbConn.Open();
            DataTable dt = new DataTable();
            MySqlDataAdapter da = new MySqlDataAdapter(dbComm);
            da.Fill(dt);
            dbConn.Close();

            List<int> percents = new List<int>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                percents.Add(Convert.ToInt32(dt.Rows[i][1]));
            }
            return percents;
        }

        private List<SingleSlaTable> getSenderMailList(List<SingleSlaTable> singleSla)      // önceden gönderilen maillerin yüzdelerini getirir.
        {
            MySqlCommand dbComm = new MySqlCommand("Select * From mail_sent", dbConn);
            dbConn.Open();
            DataTable dt = new DataTable();
            MySqlDataAdapter da = new MySqlDataAdapter(dbComm);
            da.Fill(dt);
            dbConn.Close();

            for (int i = 0; i < singleSla.Count; i++)
            {
                for (int j = 0; j < dt.Rows.Count; j++)
                {
                    if (singleSla[i].id == Convert.ToInt32(dt.Rows[j][0]))
                    {
                        singleSla[i].last_sent_percent = Convert.ToDouble(dt.Rows[j][0]);
                        break;
                    }
                }
            }
            return singleSla;
        }

        private bool setSentMailList(int ticket_id, string address, string sent_date, double percent)
        {
            try
            {
                MySqlCommand dbComm = new MySqlCommand("Insert Into mail_sent(ticket_id, address, sent_time, last_sent_percentage) Values('" + ticket_id + "', '" + address + "', '" + sent_date + "', '" + percent + "')", dbConn);
                dbConn.Open();
                dbComm.ExecuteNonQuery();
                dbConn.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void SendMailToUsers(List<SingleSlaTable> singleSla)
        {
            var percentsForSend = getPercentOfSendTime();
            singleSla = getSenderMailList(singleSla);
            singleSla = getUsersOfMail(singleSla);

            for (int i = 0; i < singleSla.Count; i++)
            {
                for (int j = 0; j < percentsForSend.Count; j++)
                {
                    if ((singleSla[i].success_rate >= 100) || (singleSla[i].success_rate > percentsForSend[j] && singleSla[i].last_sent_percent < percentsForSend[j]))
                    {
                        string priority = "";
                        if (singleSla[i].rate.id == 1) { priority = "Acil"; }
                        else if (singleSla[i].rate.id == 2) { priority = "Yüksek"; }
                        else if (singleSla[i].rate.id == 3) { priority = "Normal"; }
                        else if (singleSla[i].rate.id == 4) { priority = "Düşük"; }
                        SendEmail(singleSla[i].address, singleSla[i].id + " no'lu Ticket Hakk.", "<p>" + singleSla[i].firstname + " " + singleSla[i].lastname + ", " + singleSla[i].id + " numaralı (" + priority + ") ticket'ın süresi %" + singleSla[i].success_rate + " olmuştur</p>");
                        setSentMailList(singleSla[i].id, singleSla[i].address, DateTime.Now.ToString(), Convert.ToDouble(singleSla[i].success_rate));
                        break;
                    }
                }
            }
        }

        [HttpPost]
        private bool SendEmail(string toEmail, string subject, string emailBody)
        {
            try
            {
                string senderEmail = System.Configuration.ConfigurationManager.AppSettings["SenderEmail"].ToString();
                string senderPassword = System.Configuration.ConfigurationManager.AppSettings["SenderPassword"].ToString();

                SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
                client.EnableSsl = true;
                client.Timeout = 100000;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(senderEmail, senderPassword);
                toEmail = "hakan.yavuzalp@iksap.com";
                MailMessage mailMessage = new MailMessage(senderEmail, toEmail, subject, emailBody);
                mailMessage.IsBodyHtml = true;
                mailMessage.BodyEncoding = UTF8Encoding.UTF8;
                //mailMessage.CC.Add("support@iksap.com");
                //mailMessage.CC.Add("ozgur.aslan@iksap.com");
                client.Send(mailMessage);
                System.Threading.Thread.Sleep(1000);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

    }
}
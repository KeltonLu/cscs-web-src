using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Net;
using System.Net.Mail;

namespace CIMSClass
{
    class MailSend:BaseLogic
    {
         public MailSend()
    {
        //
        // TODO: �b���[�J�غc�禡���{���X
        //
    }



        public static void SendMail(string MailTo, string MailSubject, string MailBody)
        {
            string strTestType = ConfigurationManager.AppSettings["TestType"].ToString();
            if (strTestType == "2")
            {

                string MailFrom = ConfigurationManager.AppSettings["MailServerFrom"];

                string MailServer = ConfigurationManager.AppSettings["MailServer"];
                string MailServerID = ConfigurationManager.AppSettings["MailServerID"];
                string MailServerPWD = ConfigurationManager.AppSettings["MailServerPWD"];
                try
                {
                    SmtpClient client = new SmtpClient();
                    client.Host = MailServer;
                    client.UseDefaultCredentials = false;
                    client.Credentials = new System.Net.NetworkCredential(MailServerID, MailServerPWD);

                    client.DeliveryMethod = SmtpDeliveryMethod.Network;

                    System.Net.Mail.MailMessage message = new MailMessage(MailFrom, MailTo);
                    message.Subject = MailSubject;
                    message.Body = MailBody;
                    message.BodyEncoding = System.Text.Encoding.GetEncoding("GB2312");
                    message.IsBodyHtml = true;



                    client.Send(message);
                }
                catch (Exception ex)
                {

                }
            }
            else
            {

                try
                {

                    MailMessage mail = new MailMessage();

                    string MailFrom = ConfigurationManager.AppSettings["MailServerFrom"];
                    string MailFromName = ConfigurationManager.AppSettings["MailFromName"];
                    string MailServer = ConfigurationManager.AppSettings["MailServer"];
                    string MailServerID = ConfigurationManager.AppSettings["MailServerID"];
                    string MailServerPWD = ConfigurationManager.AppSettings["MailServerPWD"];
                    string MailDomainName = ConfigurationManager.AppSettings["MailDomainName"];

                    //�l��o�e��
                    mail.From = new MailAddress(MailFrom, MailFromName, Encoding.Default);

                    //�l�󱵨���
                    mail.To.Add(new MailAddress(MailTo));

                    //�l����D
                    mail.Subject = MailSubject;
                    mail.SubjectEncoding = Encoding.Default;

                    //�l��D����
                    mail.IsBodyHtml = true;
                    mail.Body = MailBody;
                    mail.BodyEncoding = Encoding.Default;


                    //SMTP
                    SmtpClient smtp = new SmtpClient(MailServer, 25);
                    smtp.UseDefaultCredentials = true;



                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential(MailServerID, MailServerPWD, MailDomainName);

                    //�o�e
                    smtp.Send(mail);
                    mail.Dispose();
                }
                catch
                {
                }
            }
        }
    }
}

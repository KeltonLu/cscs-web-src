//******************************************************************
//*  �@    �̡GRay
//*  �\�໡���G�l��o�e�A�A�Τ_SMTP��ĳ�l����A��
//*  �Ыؤ���G2008-11-14
//*  �ק����G
//*  �ק�O���G
//*******************************************************************
using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Net.Mail;
using System.Net;

namespace CIMSClass.Mail
{
    internal class MailBase
    {
        private static string MailServer = ConfigurationManager.AppSettings["MailServer"]; //�l����A���a�}
        private static string ManagerMail = ConfigurationManager.AppSettings["ManagerMail"];//�����̶l�c
        private static string MailFrom = ConfigurationManager.AppSettings["MailFrom"];//�H��̶l�c
        private static string MailUser = ConfigurationManager.AppSettings["MailUser"];//�H��̶l�c
        private static string MailPassword = ConfigurationManager.AppSettings["MailPassword"];//�H��̶l�c�K�X
        private static string Subject = ConfigurationManager.AppSettings["MailSubject"];//�l��D��
        private static string Body = ConfigurationManager.AppSettings["MailBody"];//�l�󥿤�

        private static SmtpClient smtp; //SMTP��H
        private static MailMessage mail;//Mail�D���H
        
        public MailBase()
        {
                      
        }
        /// <summary>
        /// �l��o�e�A���t���t����
        /// </summary>
        /// <returns></returns>
        public static bool SendMail()
        {

            return SendMail(ManagerMail, Subject, Body);
        }
        ///// <summary>
        ///// �l��o�e�A�t����A�h�Ӫ�����|�γr�����j
        ///// </summary>
        ///// <param name="LogFile">����a�}�r�Ŧ�</param>
        ///// <returns></returns>
        //public static bool SendMail(string LogFile)
        //{
        //    try
        //    {
        //        BuildMail();
        //        AddAttach(LogFile);
        //        smtp.Send(mail);
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        WriteErrLog(ex.ToString());
        //        return false;
        //    }
        //}

        /// <summary>
        /// WEB�����l��o�e��k
        /// </summary>
        /// <param name="MailTo"></param>
        /// <param name="MailSubject"></param>
        /// <param name="MailBody"></param>
        public static bool SendMail(string MailTo, string MailSubject, string MailBody)
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
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
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
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }


        //public static bool SendMail(string MailAddress, string Subject, string Body)
        //{
        //    try
        //    {
        //        BuildMail(MailAddress, Subject, Body);
        //        smtp.Send(mail);
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        WriteErrLog(ex.ToString());
        //        return false;
        //    }
        //}
        /// <summary>
        /// �Ыضl��D��
        /// </summary>
        private static void BuildMail()
        {
            BuildMail(MailServer, MailFrom, ManagerMail, Subject, Body, MailUser, MailPassword);
        }
        private static void BuildMail(string MailAddress, string Subject, string Body)
        {
            BuildMail(MailServer, MailFrom, MailAddress, Subject, Body, MailUser, MailPassword);
        }
        /// <summary>
        /// �Ыضl��D��
        /// </summary>
        /// <param name="MailServer">���A���a�}</param>
        /// <param name="MailFrom">�o��H</param>
        /// <param name="ManagerMail">����H�A�h�Ӧa�}�γr�����j</param>
        /// <param name="Subject">�D��</param>
        /// <param name="Body">���夺�e</param>
        /// <param name="MailUser">�l�c�㸹</param>
        /// <param name="MailPassword">�l�c�K�X</param>
        private static void BuildMail(string MailServer,string MailFrom,string ManagerMail,string Subject,string Body,string MailUser,string MailPassword)
        {
            smtp = new SmtpClient(MailServer);
            mail = new MailMessage(MailFrom, ManagerMail);
            mail.Subject = Subject;
            mail.Body = Body;
            mail.Priority = MailPriority.High;
            smtp.Credentials = new NetworkCredential(MailUser, MailPassword);
        }
        private static void AddAttach(string LogFiles)
        {
            foreach (string LogFile in LogFiles.Split(','))
            {
                Attachment attatch = new Attachment(LogFile);
                mail.Attachments.Add(attatch);
            }
        }
        /// <summary>
        /// ���`�l��o�e
        /// </summary>
        /// <param name="ErrDes">�t�β��`�H��</param>
        private static void WriteErrLog(string ErrDes)
        {
            StringBuilder ErrStr = new StringBuilder("�l��o�e���~�G");
            ErrStr.Append(ErrDes);
            ErrStr.Append(";");     
            LogFactory.Write(ErrStr.ToString(), GlobalString.LogType.ErrorCategory);
        }
    }
}

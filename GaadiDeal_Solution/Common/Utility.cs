using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Text.RegularExpressions;
using System.Net;
using System.Net.Mail;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Web;

namespace Common
{
    public class Utility
    {
        public static bool isValidEmail(string EmailStr)
        {
            if (EmailStr.StartsWith("@")) return false;
            string pattern = @"^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|"
            + @"([-a-z0-9!#$%&'*+/=?^_`{|}~]|(?<!\.)\.)*)(?<!\.)"
            + @"@[a-z0-9][\w\.-]*[a-z0-9]\.[a-z][a-z\.]*[a-z]$";
            Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);
            return regex.IsMatch(EmailStr);

        }

        public static string strVarcharMode(string obj, bool useWildcard)
        {
            obj = (useWildcard) ? "'%" + obj.Trim().Replace("'", "''") + "%'"
                : "'" + obj.Trim().Replace("'", "''") + "'";
            return obj;
        }
        public static string strVarcharMode(string obj)
        {
            return strVarcharMode(obj, false);
        }
        public static void SendMail(List<String> To, string EmailSubject, string EmailBody, bool isHtml,
            string SenderEmailAddress, string SenderDisplayName, string Host, int Port, string UserName, string Password, bool enableSsl, List<String> CC = null)
        {
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(SenderEmailAddress, SenderDisplayName);
            mail.To.Add(string.Join(",", To));
            if (CC != null && CC.Count > 0)
            {
                mail.CC.Add(string.Join(",", CC));
            }

            mail.Subject = EmailSubject;
            mail.Body = EmailBody;
            mail.IsBodyHtml = isHtml;


            SmtpClient smtp = null;
            if (Port > 0)
                smtp = new SmtpClient(Host, Port);
            else
                smtp = new SmtpClient(Host);
            smtp.Credentials = new NetworkCredential(UserName, Password);
            smtp.EnableSsl = enableSsl;
            smtp.Send(mail);
        }
        /// <summary>
        /// Send email base on web config:
        /// EmailSmtp.Host: IP or Address 
        /// EmailSmtp.Port: integer
        /// EmailSmtp.enableSsl: 1 or true means enable, otherwise, disable
        /// EmailSmtp.UserName
        /// EmailSmtp.Password
        /// EmailSmtp.SenderEmailAddress
        /// EmailSmtp.SenderDisplayName
        /// </summary>
        /// <param name="To"></param>
        /// <param name="EmailSubject"></param>
        /// <param name="EmailBody"></param>
        /// <param name="isHtml"></param>
        /// <param name="CC"></param>
        public static void SendMail(List<String> To, string EmailSubject, string EmailBody, bool isHtml = true, List<String> CC = null)
        {
            string EmailHost = System.Configuration.ConfigurationManager.AppSettings.Get("EmailSmtp.Host");
            int EmailPort = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings.Get("EmailSmtp.Port"));
            bool enableSsl = System.Configuration.ConfigurationManager.AppSettings.Get("EmailSmtp.enableSsl") == "1" || System.Configuration.ConfigurationManager.AppSettings.Get("EmailSmtp.enableSsl").ToLower() == "true";
            string EmailUserName = System.Configuration.ConfigurationManager.AppSettings.Get("EmailSmtp.UserName");
            string EmailPassword = System.Configuration.ConfigurationManager.AppSettings.Get("EmailSmtp.Password");
            string SenderEmailAddress = System.Configuration.ConfigurationManager.AppSettings.Get("EmailSmtp.SenderEmailAddress");
            string SenderDisplayName = System.Configuration.ConfigurationManager.AppSettings.Get("EmailSmtp.SenderDisplayName");
            SendMail(To, EmailSubject, EmailBody, isHtml, SenderEmailAddress, SenderDisplayName, EmailHost, EmailPort, EmailUserName, EmailPassword, enableSsl, CC);
        }

        public static string GetIPAddress(HttpContext context)
        {
            HttpContextWrapper wrapper = new HttpContextWrapper(context);
            return GetIPAddress((HttpContextBase)wrapper);
        }
        public static string GetIPAddress(HttpContextBase context)
        {
            string IPAddress = "";
            try
            {
                if (!String.IsNullOrEmpty(context.Request.Headers["True-Client-IP"]))
                {
                    IPAddress = context.Request.Headers["True-Client-IP"];
                }
                else if (!String.IsNullOrEmpty(context.Request.Headers["X-Forwarded-For"]))
                {
                    string strIP = context.Request.Headers["X-Forwarded-For"];
                    if (!string.IsNullOrEmpty(strIP))
                    {
                        string[] ipRange = strIP.Split(',');
                        int le = ipRange.Length - 1;
                        IPAddress = ipRange[le];
                    }
                }
                else
                {
                    IPAddress = context.Request.ServerVariables["REMOTE_ADDR"];
                }
            }
            catch { }

            return IPAddress;
        }



        public static String GenerateUniqueId(Boolean withTimeStamp = true)
        {
            String UniqueId = GenerateUniqueId();

            if (withTimeStamp)
            {
                UniqueId = DateTimeStamp() + "_" + UniqueId;
            }

            return UniqueId;
        }
        static String GenerateUniqueId()
        {
            long i = 1;
            foreach (byte b in Guid.NewGuid().ToByteArray())
            {
                i *= ((int)b + 1);
            }

            long uid = i - DateTime.Now.Ticks;

            if (uid < 0)
            {
                uid = uid * -1;
            }

            return uid.ToString();
        }
        public static string DateTimeStamp()
        {
            return DateTimeStamp(DateTime.Now);
        }
        public static string DateTimeStamp(DateTime dateTime)
        {
            decimal stamp = Convert.ToDecimal(dateTime.ToString("yyyyMMddHHmmssfffffff"));
            return stamp.ToString();
        }
    }
}

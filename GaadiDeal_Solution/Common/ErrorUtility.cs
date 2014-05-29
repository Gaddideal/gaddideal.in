using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Configuration;
using System.Web;
using System.Reflection;

namespace Common
{
    public class ErrorUtility
    {
        public static void SendErrorEmail(HttpContextBase context, Exception ex, string title = "", string message = "")
        {
            string errorInfo = ErrorInfo(context, message);
            string exceptionInfo = ExceptionInfo(ex);
            if (string.IsNullOrEmpty(title)) title = context.Request.Url.Authority;

            SendErrorEmail(title, exceptionInfo + errorInfo);
        }
        public static void SendErrorEmail(string title, string bodyText, bool isHtml = false)
        {


            string EmailHost = null;
            int EmailPort = 0;
            bool enableSsl = false;
            string EmailUserName = null;
            string EmailPassword = null;
            string SenderEmailAddress = null;
            string SenderDisplayName = null;


            #region EmailHost
            try
            {
                EmailHost = System.Configuration.ConfigurationManager.AppSettings.Get("ErrorEmailSmtp.Host");
            }
            catch
            {
                EmailHost = "";  //server name or ip
            }

            if (String.IsNullOrWhiteSpace(EmailHost))
            {
                EmailHost = ""; //server name or ip
            }
            #endregion
            #region EmailPort
            try
            {
                int.TryParse(System.Configuration.ConfigurationManager.AppSettings.Get("ErrorEmailSmtp.Port"), out EmailPort);
            }
            catch
            {
                EmailPort = 0;
            }
            #endregion
            #region enableSsl
            try
            {
                enableSsl = System.Configuration.ConfigurationManager.AppSettings.Get("ErrorEmailSmtp.enableSsl") == "1" || System.Configuration.ConfigurationManager.AppSettings.Get("ErrorEmailSmtp.enableSsl").ToLower() == "true";
            }
            catch
            {
                enableSsl = false;
            }
            #endregion
            #region EmailUserName
            try
            {
                EmailUserName = System.Configuration.ConfigurationManager.AppSettings.Get("ErrorEmailSmtp.UserName");
            }
            catch
            {
                EmailUserName = "PS";
            }

            if (String.IsNullOrWhiteSpace(EmailUserName))
            {
                EmailUserName = "PS";
            }
            #endregion
            #region EmailPassword
            try
            {
                EmailPassword = System.Configuration.ConfigurationManager.AppSettings.Get("ErrorEmailSmtp.Password");
            }
            catch
            {
                EmailPassword = "PS";
            }
            if (String.IsNullOrWhiteSpace(EmailPassword))
            {
                EmailPassword = "PS";
            }
            #endregion
            #region SenderEmailAddress
            try
            {
                SenderEmailAddress = System.Configuration.ConfigurationManager.AppSettings.Get("ErrorEmailSmtp.SenderEmailAddress");
            }
            catch
            {
                SenderEmailAddress = "PS@PS.com";
            }
            if (String.IsNullOrWhiteSpace(SenderEmailAddress))
            {
                SenderEmailAddress = "PS@PS.com";
            }
            #endregion
            #region SenderDisplayName
            try
            {
                SenderDisplayName = System.Configuration.ConfigurationManager.AppSettings.Get("ErrorEmailSmtp.SenderDisplayName");
            }
            catch
            {
                SenderDisplayName = "PS";
            }
            if (String.IsNullOrWhiteSpace(SenderDisplayName))
            {
                SenderDisplayName = "PS";
            }
            #endregion









            List<string> toList = new List<string>();
            toList.Add("ppsiteerrors@pureprofile.com");
            //try
            //{
            //    //string email = WebConfigurationManager.AppSettings["Email.SendError"];
            //    //if (string.IsNullOrEmpty(email))
            //    //    toList.Add("ppsiteerrors@pureprofile.com");
            //    //else
            //    //    toList.Add(email);
            //}
            //catch
            //{
            //    toList.Add("ppsiteerrors@pureprofile.com");
            //}

            string emailTitle = "[PPSiteErrors] - " + title;

            Utility.SendMail(toList, emailTitle, bodyText, true, SenderEmailAddress, SenderDisplayName, EmailHost, EmailPort, EmailUserName, EmailPassword, enableSsl, new List<string>());

        }

        public static string ExceptionInfo(Exception ex)
        {
            StringBuilder sb = new StringBuilder();

            Dictionary<String, String> exceptionInfo = ObjectExtractor(ex);
            if (exceptionInfo.Count() > 0)
            {
                sb.AppendLine("============ Exception Data ===================");
                foreach (KeyValuePair<string, string> key in exceptionInfo)
                {
                    sb.AppendLine(key.Key + " : " + key.Value);
                }
                sb.AppendLine("");
                sb.AppendLine("");
            }


            return sb.ToString();
        }
        public static string ErrorInfo(HttpContext context, string message = "")
        {
            HttpContextWrapper wrapper = null;
            if (context != null)
            {
                wrapper = new HttpContextWrapper(context);
            }
            return ErrorInfo(wrapper, message);
        }
        public static string ErrorInfo(HttpContextBase context, string message = "")
        {
            StringBuilder sb = new StringBuilder();

            if (!String.IsNullOrEmpty(message))
            {
                sb.AppendLine("============ Messages ===================");
                sb.AppendLine(message);
                sb.AppendLine("");
                sb.AppendLine("");
            }

            if (context != null)
            {
                sb.AppendLine("============ Header Info ===================");
                sb.AppendLine("Access Time: " + DateTime.Now.ToString("dd/MMM/yyyy hh:mm:ss"));
                sb.AppendLine("Request url: " + context.Request.Url.AbsoluteUri);
                sb.AppendLine("QueryString url: " + context.Request.QueryString.ToString());
                try
                {
                    sb.AppendLine("Ref url: " + context.Request.UrlReferrer.AbsoluteUri);
                }
                catch { }

                try
                {
                    System.Collections.Specialized.NameValueCollection postdata = context.Request.Form;
                    if (postdata.AllKeys.Count() > 0)
                    {
                        sb.AppendLine("============ Post Data ===================");
                        foreach (string key in postdata)
                        {
                            sb.AppendLine(key + " : " + postdata[key]);
                        }
                        sb.AppendLine("");
                        sb.AppendLine("");
                    }
                }
                catch { }

                try
                {
                    sb.AppendLine("============ Header Info ===================");
                    foreach (string key in context.Request.Headers.AllKeys)
                    {
                        sb.AppendLine(key + " : " + context.Request.Headers[key]);
                    }
                    sb.AppendLine("");
                    sb.AppendLine("");
                }
                catch { }

                try
                {
                    sb.AppendLine("============ Server Variables Info ===================");
                    foreach (string key in context.Request.ServerVariables.AllKeys)
                    {
                        sb.AppendLine(key + " : " + context.Request.ServerVariables[key]);
                    }
                }
                catch { }
            }

            return sb.ToString();
        }

        public static void BuildErrorAndSend(HttpContextBase context, string title = "", string message = "")
        {
            string errorBody = ErrorInfo(context, message);
            if (string.IsNullOrEmpty(title)) title = context.Request.Url.Authority;
            SendErrorEmail(title, errorBody);
        }

        public static Dictionary<String, String> ObjectExtractor(object model)
        {
            Dictionary<String, string> Details = new Dictionary<string, string>();
            try
            {
                PropertyInfo[] PropertyInfos = model.GetType().GetProperties();
                foreach (PropertyInfo propertyInfo in PropertyInfos)
                {
                    Details.Add(propertyInfo.Name, propertyInfo.GetValue(model, null).ToString());
                }
            }
            catch { }
            return Details;
        }
    }
}

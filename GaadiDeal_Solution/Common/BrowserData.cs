using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Common
{
    public class BrowserData
    {

        HttpContextBase Context = null;

        public String IpAddress { get; private set; }
        public long MachineCookieID { get; private set; }
        public String UserAgent { get; private set; }

        public string BrowserName { get; set; }
        public bool IsMobileDevice { get; set; }
        public string Version { get; set; }
        public int MajorVersion { get; set; }
        public double MinorVersion { get; set; }


        public BrowserData(HttpContextBase Context)
        {
            this.Context = Context;
            this.IpAddress = BrowserData.GetIp(this.Context);
            this.UserAgent = this.Context.Request.UserAgent;
            try
            {
                BrowserName = this.Context.Request.Browser.Browser;
            }
            catch { }
            try
            {
                IsMobileDevice = this.Context.Request.Browser.IsMobileDevice;
            }
            catch { }
            try
            {
                MajorVersion = this.Context.Request.Browser.MajorVersion;
            }
            catch { }
            try
            {
                MinorVersion = this.Context.Request.Browser.MinorVersion;
            }
            catch { }
            try
            {
                Version = this.Context.Request.Browser.Version;
            }
            catch { }
        }

        public static string GetIp(HttpContextBase Context)
        {
            string ip = "";
            try
            {
                if (!String.IsNullOrWhiteSpace(Context.Request.ServerVariables["HTTP_VIA"])) // using proxy
                {
                    ip = Context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];  // Return real client IP.
                }


                if (string.IsNullOrEmpty(ip))
                {
                    ip = Common.WebUtility.GetIPAddress(Context);
                }

                if (string.IsNullOrEmpty(ip))
                {
                    ip = Context.Request.ServerVariables["REMOTE_ADDR"]; //While it can't get the Client IP, it will return proxy IP.
                }

                return ip;
            }
            catch { }


            return "";
        }
    }
}

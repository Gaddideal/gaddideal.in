using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Web;

namespace Common
{
    public class WebUtility
    {
        public static string GetIPAddress(HttpContext context)
        {
            HttpContextWrapper wrapper = new HttpContextWrapper(context);
            return GetIPAddress((HttpContextBase)wrapper);
        }
        public static string GetIPAddress(HttpContextBase context)
        {
            return Utility.GetIPAddress(context);
        }


        public static short GetPlatformID(HttpContextBase context)
        {
            short platformId = 1;
            string userAgent = context.Request.UserAgent.ToLower();
            if (userAgent.Contains("iphone"))
                platformId = 2;
            else if (userAgent.Contains("ipad"))
                platformId = 3;
            else if (userAgent.Contains("android"))
            {
                if (userAgent.Contains("mobile"))
                    platformId = 4;
                else
                    platformId = 5;
            }

            return platformId;

        }
    }
}

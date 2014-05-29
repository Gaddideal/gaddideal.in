using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Web;

namespace Common
{
    public class Website
    {
        public enum EnvironmentType { Local, Dev, Live }
        static string WebSiteEnvironment = System.Configuration.ConfigurationManager.AppSettings.Get("WebSite.Environment").ToLower();

        public static EnvironmentType Environment
        {
            get
            {
                if (WebSiteEnvironment == "dev")
                    return EnvironmentType.Dev;
                else if (WebSiteEnvironment == "live")
                    return EnvironmentType.Live;              
                else
                    return EnvironmentType.Local;
            }
        }

        public static bool IsLocal
        {
            get
            {
                return Environment == EnvironmentType.Local;
            }
        }
        public static bool IsDev
        {
            get
            {
                return Environment == EnvironmentType.Dev;
            }
        }
        public static bool IsLive
        {
            get
            {
                return Environment == EnvironmentType.Live;
            }
        }
        
    }

}

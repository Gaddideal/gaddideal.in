using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Web;

namespace Common
{
    public class MachineCookieID
    {

        public static long GenerateBusinessMachineCookieID(HttpContext context)
        {
            HttpContextWrapper wrapper = new HttpContextWrapper(context);
            return GenerateBusinessMachineCookieID((HttpContextBase)wrapper);
        }
        public static long GenerateBusinessMachineCookieID(HttpContextBase context)
        {
            string MachineCookieName = "devphdy";
            if (Website.IsLive) MachineCookieName = "phdy";

            long MachineCookieID = 0;

            try
            {
                MachineCookieID = Convert.ToInt64(Encryption.Decrypt16(context.Request.Cookies[MachineCookieName].Value));
                if (MachineCookieID < 1)
                {
                    throw new Exception();
                }
            }
            catch
            {

            }

            return MachineCookieID;
        }

    }
}

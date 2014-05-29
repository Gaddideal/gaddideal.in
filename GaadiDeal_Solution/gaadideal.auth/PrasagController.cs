using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace gaadideal.auth
{
    [PrasagAuthorize]
    public class PrasagController : System.Web.Mvc.Controller
    {
        public PrasagPrincipal accounts
        {
            get
            {
                return (PrasagPrincipal)this.HttpContext.User;
            }
        }

        protected bool RequestIsMobile
        {
            get
            {
                System.Text.RegularExpressions.Regex mobileRegex = new System.Text.RegularExpressions.Regex("(iphone|android 2*|windows phone)", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                if (mobileRegex.IsMatch(Request.UserAgent))
                {
                    return true;
                }
                return false;
            }
        }
    }
}

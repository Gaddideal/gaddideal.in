using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using gaadideal.auth;

namespace gaadideal.site.Controllers
{ 
    [PrasagAuthorize(AccountTypes = new AccountType[] { AccountType.members }, LocalTestMemberID = 4)]  //22404 20599  LocalTestContactID = 20599, 
    public class HomeController : PrasagController
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            return View();
        }
    }
}

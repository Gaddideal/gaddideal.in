using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Principal;
using System.Web;
using Common;
using System.Configuration;

namespace gaadideal.auth
{

    public class PrasagPrincipal : IPrincipal
    {
        public PrasagAccount member { get; set; }


        public PrasagPrincipal(HttpContextBase Context, PrasagIdentity identity, PrasagMemberManager member_am)
        {
            this.member = new PrasagAccount(Context, member_am);
            this.Identity = identity;
        }

        public IIdentity Identity
        {
            get;
            private set;
        }

        public bool IsInRole(string role)
        {
            throw new NotImplementedException();
        }
    }     
}

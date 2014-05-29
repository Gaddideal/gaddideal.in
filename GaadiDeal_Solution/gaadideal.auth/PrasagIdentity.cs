using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Principal;

namespace gaadideal.auth
{
    public class PrasagIdentity : IIdentity
    {
        public string AuthenticationType
        {
            get { return ""; }
        }

        public bool IsAuthenticated
        {
            get { return true; }
        }

        public string Name
        {
            get { return ""; }
        }
    }
}

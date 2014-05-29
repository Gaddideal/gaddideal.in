using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace gaadideal.auth
{


    public class Member
    {
        public Int32 MemberID { get; set; }
        public String EmailAddress { get; set; }
        public String Password { get; set; }
        public Int32 AccountStatusID { get; set; }
        public String FirstName { get; set; }
        public String LastName { get; set; }
        public String Photo { get; set; }
        public Int32 CountryID { get; set; }
        public DateTime RegisteredDate { get; set; }
        public String IpAddress { get; set; } 
        public String Telephone { get; set; }    

        public bool IsActive { get { return this.AccountStatusID == 1; } }
    }
}

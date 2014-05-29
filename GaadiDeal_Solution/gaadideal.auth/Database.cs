using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;

namespace gaadideal.auth
{
    public class Database
    {

        public static ConcreteDbContext PrasagDB
        {
            get
            {
                string ConnectionString = "";

                try
                {
                    ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["PRASAG"].ConnectionString;
                }
                catch
                {
                    ConnectionString = "";
                }

                if (String.IsNullOrWhiteSpace(ConnectionString))
                {
                    string LocalConnectionString = "Data Source=prasag.db.11903400.hostedresource.com;Initial Catalog=prasag;Persist Security Info=True;User ID=prasag;Password=Welcome123@";
                    string DevConnectionString = "Data Source=prasag.db.11903400.hostedresource.com;Initial Catalog=prasag;Persist Security Info=True;User ID=prasag;Password=Welcome123@";
                    string LiveConnectionString = "Data Source=prasag.db.11903400.hostedresource.com;Initial Catalog=prasag;Persist Security Info=True;User ID=prasag;Password=Welcome123@";


                    if (Website.IsDev)
                        ConnectionString = DevConnectionString;
                    else if (Website.IsLive)
                        ConnectionString = LiveConnectionString;                
                    else
                        ConnectionString = LocalConnectionString;
                }

                return new ConcreteDbContext(ConnectionString);
            }
        }
    }
}

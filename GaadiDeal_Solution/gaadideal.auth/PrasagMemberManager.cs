using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Common;
using System.Web;

namespace gaadideal.auth
{
    public class PrasagMemberManager : AbstractAccountManager
    {
        public PrasagMemberManager()
        {

            string PersistentCookieName = "devpsqhtp"
            , LiveCookieName = "devpsqhtl";

            if (Website.IsLive)
            {
                PersistentCookieName = "psqhtp";
                LiveCookieName = "psqhtl";
            }

            base.Setup(PersistentCookieName
            , LiveCookieName);
        }

        ConcreteDbContext db { get { return Database.PrasagDB; } }


        public Member GetLoggedInAccountDetail(HttpContextBase Context)
        {
            Member a = GetContact(int.Parse(Encryption.Decrypt16(GetLoginAccountID(Context))));
            return a;
        }

        public int DBLogin(string EmailAddress, string Password)
        {
            int TeamMemberID = -1;

            bool ValidEmailAddress = Utility.isValidEmail(EmailAddress);
            bool ValidPassword = Password.Length > 5 && Password.Length < 21;

            if (ValidEmailAddress && ValidPassword)
            {
                Member a = GetContactDetail(EmailAddress);

                if (a == null)
                {
                    ValidEmailAddress = false;
                }
                else
                {
                    string RealPassword = a.Password;
                    ValidPassword = Password == RealPassword;
                }


                if (ValidEmailAddress && ValidPassword && a.IsActive)
                {
                    TeamMemberID = a.MemberID;
                }
            }

            TeamMemberID = (ValidEmailAddress) ? TeamMemberID : -1;
            TeamMemberID = (ValidPassword) ? TeamMemberID : -2;


            return TeamMemberID;
        }

        protected sealed override int DBLogin(string EmailAddress, string Password, bool RememberMe, HttpContextBase Context)
        {
            return DBLogin(EmailAddress, Password);
        }
        protected sealed override List<string> DBGetRoles(int AccountID)
        {
            List<string> roles = new List<string> { };

            if (AccountID > 0)
            {
                roles.Add("NormalTeamMember");
            }

            return roles;
        }
        public override bool IsActiveAccount(int AccountID)
        {
            Member account = GetContactDetail(AccountID);
            return account != null && account.IsActive;
        }
        protected sealed override int autologin(HttpContextBase Context, int AccountID)
        {
            int ErrorCode = 0;
            Member a = GetContactDetail(AccountID);

            if (a != null)
            {
                ErrorCode = base.autologin(Context, a.MemberID);
            }

            return ErrorCode;
        }
        public override void loginlog(HttpContextBase Context, string EmailAddress, string Password, int CONTACTID, bool RememberMe, bool AutoLogin, bool SUCCESSFULLOGIN)
        {
//            try
//            {
//                if (SUCCESSFULLOGIN)
//                {
//                    EmailAddress = "";
//                    Password = "";
//                }

//                AkamaiHeader akamai = AkamaiHeader.GetAkamaiHeader(Context);

//                string sql = @"EXEC	[BusinessManager2011].[dbo].[InsertLoginLogs]
//		                    @CONTACTID = @pCONTACTID,
//		                    @ENTEREDEMAILADDRESS = @pENTEREDEMAILADDRESS,
//		                    @ENTEREDPASSWORD = @pENTEREDPASSWORD,
//		                    @AUTOLOGIN = @pAUTOLOGIN,
//		                    @REMEMBERME = @pREMEMBERME,
//		                    @SUCCESSFULLOGIN = @pSUCCESSFULLOGIN,
//		                    @MACHINECOOKIEID = @pMACHINECOOKIEID,
//		                    @IPADDRESS = @pIPADDRESS,
//		                    @BROWSER = @pBROWSER,
//		                    @USERAGENT = @pUSERAGENT,
//		                    @GEOREGION = @pGEOREGION,
//		                    @COUNTRYCODE = @pCOUNTRYCODE,
//		                    @REGIONCODE = @pREGIONCODE,
//		                    @CITY = @pCITY,
//		                    @LATITUDE = @pLATITUDE,
//		                    @LONGITUDE = @pLONGITUDE,
//		                    @TIMEZONE = @pTIMEZONE,
//		                    @NETWORK = @pNETWORK,
//		                    @CONTINENT = @pCONTINENT,
//		                    @THROUGHPUT = @pTHROUGHPUT,
//		                    @BW = @pBW,
//		                    @ASNUM = @pASNUM";

//                List<SqlParameter> param = new List<SqlParameter>();
//                param.Add(new SqlParameter { name = "pCONTACTID", type = DbType.Int32, value = CONTACTID });
//                param.Add(new SqlParameter { name = "pENTEREDEMAILADDRESS", type = DbType.String, value = EmailAddress });
//                param.Add(new SqlParameter { name = "pENTEREDPASSWORD", type = DbType.String, value = Password });
//                param.Add(new SqlParameter { name = "pAUTOLOGIN", type = DbType.Boolean, value = AutoLogin });
//                param.Add(new SqlParameter { name = "pREMEMBERME", type = DbType.Boolean, value = RememberMe });
//                param.Add(new SqlParameter { name = "pSUCCESSFULLOGIN", type = DbType.Boolean, value = SUCCESSFULLOGIN });
//                param.Add(new SqlParameter { name = "pMACHINECOOKIEID", type = DbType.Int64, value = MachineCookieID.GenerateBusinessMachineCookieID(Context) });
//                param.Add(new SqlParameter { name = "pIPADDRESS", type = DbType.String, value = WebUtility.GetIPAddress(Context) });
//                param.Add(new SqlParameter { name = "pBROWSER", type = DbType.String, value = "" });
//                param.Add(new SqlParameter { name = "pUSERAGENT", type = DbType.String, value = Context.Request.UserAgent });
//                param.Add(new SqlParameter { name = "pGEOREGION", type = DbType.Int32, value = akamai.GeoRegion });
//                param.Add(new SqlParameter { name = "pCOUNTRYCODE", type = DbType.String, value = akamai.CountryCode });
//                param.Add(new SqlParameter { name = "pREGIONCODE", type = DbType.String, value = akamai.RegionCode });
//                param.Add(new SqlParameter { name = "pCITY", type = DbType.String, value = akamai.City });
//                param.Add(new SqlParameter { name = "pLATITUDE", type = DbType.Decimal, value = akamai.Latitude });
//                param.Add(new SqlParameter { name = "pLONGITUDE", type = DbType.Decimal, value = akamai.Longitude });
//                param.Add(new SqlParameter { name = "pTIMEZONE", type = DbType.String, value = akamai.Timezone });
//                param.Add(new SqlParameter { name = "pNETWORK", type = DbType.String, value = akamai.Network });
//                param.Add(new SqlParameter { name = "pCONTINENT", type = DbType.String, value = akamai.Continent });
//                param.Add(new SqlParameter { name = "pTHROUGHPUT", type = DbType.String, value = akamai.throughput });
//                param.Add(new SqlParameter { name = "pBW", type = DbType.String, value = akamai.bw });
//                param.Add(new SqlParameter { name = "pASNUM", type = DbType.String, value = akamai.asnum });


//                db.ExecuteNonQuery(sql, param);
//            }
//            catch (Exception ex)
//            {
//                ErrorUtility.SendErrorEmail(Context, ex, "Business Loginlog Exception");
//            }
        }

        Member GetContactDetail(int MemberID)
        {
            Member account = GetContactDetail(MemberID, "");
            return account;
        }
        Member GetContactDetail(string Email)
        {
            Member account = GetContactDetail(0, Email);
            return account;
        }
        Member GetContactDetail(int MemberID, string Email)
        {
            Member member = new Member();
            string sql = "EXEC [dbo].[GetMembersDetails] @MemberID = " + MemberID + ", @EMAILADDRESS = N" + Utility.strVarcharMode(Email);
            DataTable dt = db.ExecuteDataTable(sql);
            if (dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                member = new Member();

                member.MemberID = int.Parse(dr["MemberID"].ToString()); 
                member.CountryID = int.Parse(dr["CountryID"].ToString());
                member.EmailAddress = dr["EmailAddress"].ToString();
                member.Password = dr["Password"].ToString(); 
                member.AccountStatusID = int.Parse(dr["AccountStatusID"].ToString());
                member.FirstName = dr["FirstName"].ToString();
                member.LastName = dr["LastName"].ToString();
                member.Photo = dr["Photo"].ToString(); 
                member.RegisteredDate = DateTime.Parse(dr["RegisteredDate"].ToString());
                member.IpAddress = dr["IpAddress"].ToString(); 
                member.Telephone = dr["Telephone"].ToString(); 
            }
            return member;
        }
        public Member GetContact(int ContactID)
        {
            Member account = GetContactDetail(ContactID, "");
            account.Password = "";
            return account;
        }
    }
    //    public class PrasagMemberManager
    //    {
    //        public PrasagMemberManager(string ConnectionString)
    //        {
    //            this.db = new ConcreteDbContext(ConnectionString);
    //        }

    //        ConcreteDbContext db = null;

    //        public PrasagAccount GetMember(int AccountID)
    //        {
    //            PrasagAccount psaccount = null;
    //            if (AccountID > 0)
    //            {
    //                string sql = "EXEC [BusinessManager2011].[dbo].[GetMembersDetails] @CONTACTID = " + AccountID;
    //                DataTable dt = this.db.ExecuteDataTable(sql);
    //                if (dt != null && dt.Rows.Count > 0)
    //                {
    //                    psaccount = convert(dt.Rows[0]);
    //                }
    //            }

    //            return psaccount;
    //        }
    //        public PrasagAccount GetMember(string Email)
    //        {
    //            PrasagAccount psaccount = null;
    //            if (!String.IsNullOrWhiteSpace(Email))
    //            {
    //                string sql = "EXEC [BusinessManager2011].[dbo].[GetMembersDetails] @EMAILADDRESS = N" + DBTools.FormatString(Email);
    //                DataTable dt = this.db.ExecuteDataTable(sql);
    //                if (dt != null && dt.Rows.Count > 0)
    //                {
    //                    psaccount = convert(dt.Rows[0]);
    //                }
    //            }

    //            return psaccount;
    //        }
    //        PrasagAccount convert(DataRow dr)
    //        {
    //            PrasagAccount psaccount = null;
    //            if (dr != null)
    //            {
    //                psaccount = new PrasagAccount();
    //                psaccount.AccountID = int.Parse(dr["MemberID"].ToString());
    //                psaccount.RegisteredDate = DateTime.Parse(dr["RegisteredDate"].ToString());
    //                psaccount.CountryID = int.Parse(dr["CountryID"].ToString());
    //                psaccount.EmailAddress = dr["EmailAddress"].ToString();
    //                psaccount.FirstName = dr["FirstName"].ToString();
    //                psaccount.LastName = dr["LastName"].ToString();
    //                try
    //                {
    //                    psaccount.ValidatedDate = DateTime.Parse(dr["ValidatedDate"].ToString());
    //                }
    //                catch
    //                {
    //                    psaccount.ValidatedDate = new DateTime(1900, 1, 1);
    //                }
    //                try
    //                {
    //                    psaccount.InvitedDate = DateTime.Parse(dr["InvitedDate"].ToString());
    //                }
    //                catch
    //                {
    //                    psaccount.InvitedDate = new DateTime(1900, 1, 1);
    //                }
    //                psaccount.AccountStatusID = int.Parse(dr["AccountStatusID"].ToString());


    //                psaccount.isValidated = bool.Parse(dr["isValidated"].ToString());
    //                psaccount.IpAddress = dr["IpAddress"].ToString();
    //            }
    //            return psaccount;
    //        }

    //        public bool ValidMember(PrasagAccount psaccount)
    //        {
    //            return psaccount != null && psaccount.AccountStatusID == 1;
    //        }

    //        public String GetAccountPassword(int AccountID)
    //        {
    //            string realPassword = null;

    //            string sql = @"
    //DECLARE	@Password varchar(50)
    //
    //EXEC [GetAccountPassword]
    //		@AccountID = " + AccountID + @",
    //		@Password = @Password OUTPUT
    //
    //SELECT	@Password as N'@Password'";

    //            realPassword = this.db.ExecuteString(sql);
    //            return realPassword;
    //        }
    //        public String GetAccountPassword(string EmailAddress)
    //        {
    //            string realPassword = null;
    //            string sql = @"
    //DECLARE	@Password varchar(50)
    //
    //EXEC [GetAccountPassword]
    //		@EmailAddress = " + DBTools.FormatString(EmailAddress) + @",
    //		@Password = @Password OUTPUT
    //
    //SELECT	@Password as N'@Password'";

    //            realPassword = this.db.ExecuteString(sql);
    //            return realPassword;
    //        }
    //        public int LoginLog(int AccountID, string enteredEmailAddress, string enteredPassword, bool isAutoLogin, bool isRememberMe, bool isSuccessfulLogin, BrowserData BrowserData)
    //        {
    //            //            string sql = @"
    //            //DECLARE	@Result INT;
    //            //
    //            //EXEC  [dbo].[DoLoginLogs]
    //            //@AccountID = " + AccountID + @",		                
    //            //@EnteredEmailAddress = N" + DBTools.FormatString(enteredEmailAddress) + @",		
    //            //@EnteredPassword = N" + DBTools.FormatString(enteredPassword) + @", 
    //            //@IsAutoLogin = " + ((isAutoLogin) ? 1 : 0) + @",                    
    //            //@IsRememberMe = " + ((isRememberMe) ? 1 : 0) + @",  
    //            //@IsSuccessfulLogin = " + ((isSuccessfulLogin) ? 1 : 0) + @",
    //            //@IpAddress = N" + DBTools.FormatString(BrowserData.IpAddress) + @",	    
    //            //@UserAgent = N" + DBTools.FormatString(BrowserData.UserAgent) + @",		                
    //            //@Result = @Result OUTPUT; 
    //            //
    //            //SELECT	@Result as '@Result'";

    //            //            return this.db.ExecuteInt(sql);
    //            return 1;
    //        }

    //    }
}

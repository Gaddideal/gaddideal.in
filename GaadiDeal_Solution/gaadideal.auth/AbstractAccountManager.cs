using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using System.Web;

namespace gaadideal.auth
{
    public abstract class AbstractAccountManager
    {
        public AbstractAccountManager()
        {
        }

        #region private
        class CookieEncryptionParameter
        {
            //Password:  This is this password for encryption, like a key. Eg: “Welcome123”
            public static string PrivatePassword = "onf35ina36os4fna3sn3";
            //Salt:string, just like a second password.
            public static string PrivateSalt = "s98sgyjuzgj18fg";
            //HashAlgorithm: string, can be SHA1 or MD5.
            public static string PrivateHashAlgorithm = "SHA1";
            //PasswordIterations: int is the number of times the algorithm is run on the text.
            public static int PrivatePasswordIterations = 2;
            //InitialVector: should be a string of 16 ASCII characters. Eg: “*1B2c3D4e5F6g7H8”
            public static string PrivateInitialVector = "JRGch58k@lxj79nO";
            //KeySize: int,can be 128, 192, or 256.
            public static int PrivateKeySize = 128;
        }
        String PersistentCookieName = "";
        String LiveCookieName = "";

        bool AlreadySetup = false;

        string Domain = "gaddideal.in";

        string Encrypt(string Text, bool UrlEncode = true)
        {
            String EncryptText = Encryption.Encrypt(Text, CookieEncryptionParameter.PrivatePassword, CookieEncryptionParameter.PrivateSalt, CookieEncryptionParameter.PrivateHashAlgorithm, CookieEncryptionParameter.PrivatePasswordIterations, CookieEncryptionParameter.PrivateInitialVector, CookieEncryptionParameter.PrivateKeySize);
            if (UrlEncode) EncryptText = HttpUtility.UrlEncode(EncryptText);
            return EncryptText;
        }
        string Decrypt(string Text, bool UrlDecode = true)
        {
            if (UrlDecode) Text = HttpUtility.UrlDecode(Text);
            String DecryptText = Encryption.Decrypt(Text, CookieEncryptionParameter.PrivatePassword, CookieEncryptionParameter.PrivateSalt, CookieEncryptionParameter.PrivateHashAlgorithm, CookieEncryptionParameter.PrivatePasswordIterations, CookieEncryptionParameter.PrivateInitialVector, CookieEncryptionParameter.PrivateKeySize);
            return DecryptText;
        }


        void SetLoginCookie(HttpContextBase Context, string Name, int ID, DateTime Expires, string domain)
        {
            HttpCookie TicketCookie = new HttpCookie(Name, Encrypt(ID.ToString()));
            TicketCookie.HttpOnly = true;

            if (Expires > DateTime.Now)
            {
                TicketCookie.Expires = Expires;
            }

            string Business_Domain = System.Configuration.ConfigurationManager.AppSettings.Get("business_domain");

            if (Business_Domain != null && Business_Domain != "")
            {
                domain = Business_Domain.ToLower();
            }

            if (!Context.Request.IsLocal)
            {
                TicketCookie.Domain = domain;
            }
            Context.Response.Cookies.Add(TicketCookie);
        }

        void SetPersistentCookie(HttpContextBase Context, int ID)
        {
            SetLoginCookie(Context, PersistentCookieName, ID, DateTime.Now.AddDays(14), Domain);
        }
        void SetLiveCookie(HttpContextBase Context, int ID)
        {
            SetLoginCookie(Context, LiveCookieName, ID, DateTime.Now, Domain);
        }

        string GetPersistentCookie(HttpContextBase Context)
        {
            return Context.Request.Cookies[PersistentCookieName].Value;
        }
        string GetLiveCookie(HttpContextBase Context)
        {
            return Context.Request.Cookies[LiveCookieName].Value;
        }

        void RemovePersistentCookie(HttpContextBase Context)
        {
            if (Context.Request.Cookies[PersistentCookieName] != null)
            {
                HttpCookie c = new HttpCookie(PersistentCookieName);
                c.Expires = DateTime.Now.AddDays(-1d);
                if (!Context.Request.IsLocal)
                {
                    string domain = Domain;
                    string Business_Domain = System.Configuration.ConfigurationManager.AppSettings.Get("business_domain");

                    if (Business_Domain != null && Business_Domain != "")
                    {
                        domain = Business_Domain.ToLower();
                    }
                    c.Domain = domain;
                }
                Context.Response.Cookies.Add(c);
            }
        }
        void RemoveLiveCookie(HttpContextBase Context)
        {
            if (Context.Request.Cookies[LiveCookieName] != null)
            {
                HttpCookie c = new HttpCookie(LiveCookieName);
                c.Expires = DateTime.Now.AddDays(-1d);
                if (!Context.Request.IsLocal)
                {
                    string domain = Domain;
                    string Business_Domain = System.Configuration.ConfigurationManager.AppSettings.Get("business_domain");

                    if (Business_Domain != null && Business_Domain != "")
                    {
                        domain = Business_Domain.ToLower();
                    }
                    c.Domain = domain;
                }
                Context.Response.Cookies.Add(c);
            }
        }
        #endregion

        protected virtual int autologin(HttpContextBase Context, int AccountID)
        {
            try
            {
                SetLiveCookie(Context, AccountID);
                loginlog(Context, "", "", AccountID, false, true, AccountID > 0);
            }
            catch { AccountID = 0; }

            return AccountID;
        }
        protected abstract int DBLogin(string EmailAddress, string Password, bool RememberMe, HttpContextBase Context);
        protected abstract List<string> DBGetRoles(int AccountID);
        public abstract bool IsActiveAccount(int AccountID);
        public abstract void loginlog(HttpContextBase Context, string EmailAddress, string Password, int AccountID, bool RememberMe, bool AutoLogin, bool SUCCESSFULLOGIN);

        public void Setup(string PersistentCookieName, string LiveCookieName)
        {
            if (!AlreadySetup)
            {
                this.PersistentCookieName = PersistentCookieName;
                this.LiveCookieName = LiveCookieName;
                AlreadySetup = true;
            }
        }
        public void Setup(string PersistentCookieName, string LiveCookieName, string EncryptionPrivatePassword, string EncryptionPrivateSalt, string EncryptionPrivateInitialVector)
        {
            if (!AlreadySetup)
            {
                this.PersistentCookieName = PersistentCookieName;
                this.LiveCookieName = LiveCookieName;
                CookieEncryptionParameter.PrivatePassword = EncryptionPrivatePassword;
                CookieEncryptionParameter.PrivateSalt = EncryptionPrivateSalt;
                CookieEncryptionParameter.PrivateInitialVector = EncryptionPrivateInitialVector;
                AlreadySetup = true;
            }
        }

        public void Logout(HttpContextBase Context)
        {
            var app = (HttpApplication)Context.GetService(typeof(HttpApplication));
            string Business_Domain = System.Configuration.ConfigurationManager.AppSettings.Get("business_domain");

            if (Business_Domain == null || Business_Domain == "")
            {
                Business_Domain = "pureprofile.com";
            }
            else
            {
                Business_Domain = Business_Domain.ToLower();
            }

            RemovePersistentCookie(Context);
            RemoveLiveCookie(Context);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="Context">The current HttpContextBase from the controller</param>
        /// <param name="EmailAddress">email for Login account</param>
        /// <param name="Password">password for Login account</param>
        /// <param name="RememberMe">determine whether keep the login info in cookie Persistently</param>
        /// <returns></returns>
        public int Login(HttpContextBase Context, string EmailAddress, string Password, bool RememberMe)
        {
            int AccountID = -1;
            int LogAccountID = AccountID;

            AccountID = DBLogin(EmailAddress, Password, RememberMe, Context);


            #region cookie login
            if (AccountID > 0)
            {
                LogAccountID = AccountID;
                SetLiveCookie(Context, AccountID);
                if (RememberMe)
                {
                    SetPersistentCookie(Context, AccountID);
                }
                AccountID = 1;
            }
            #endregion


            loginlog(Context, EmailAddress, Password, LogAccountID, RememberMe, false, LogAccountID > 0);


            return AccountID;
        }

        /// <summary>
        /// return Current logged in Hexadecimal Encryption Account ID
        /// </summary>
        /// <param name="Context">The current HttpContextBase from the controller</param>
        /// <returns></returns>
        string GetLoginAccountID(HttpContextBase Context, bool Encrypt)
        {
            string ID = "";


            try { ID = GetLiveCookie(Context); }
            catch { ID = ""; }

            if (String.IsNullOrEmpty(ID))
            {
                try { ID = GetPersistentCookie(Context); }
                catch { ID = ""; }

                #region auto login
                if (!String.IsNullOrEmpty(ID))
                {
                    int AccountID = int.Parse(Decrypt(ID));
                    if (autologin(Context, AccountID) < 1)
                    {
                        ID = "";
                    }
                    else
                    {
                        ID = AccountID.ToString();
                    }
                }
                #endregion
            }
            else
            {
                int AccountID = int.Parse(Decrypt(ID));
                if (IsActiveAccount(AccountID))
                {
                    ID = AccountID.ToString();
                }
                else
                {
                    ID = "";
                }
            }

            if (!String.IsNullOrEmpty(ID) && Encrypt)
            {
                ID = Encryption.Encrypt16(ID);
            }
            return ID;
        }
        public int GetLoginIntegerAccountID(HttpContextBase Context)
        {
            int account_id = 0;
            string ID = GetLoginAccountID(Context, false);

            if (!String.IsNullOrEmpty(ID))
            {
                try
                {
                    account_id = Convert.ToInt32(ID);
                }
                catch
                {
                    account_id = 0;
                }

            }
            return account_id;
        }
        public string GetLoginAccountID(HttpContextBase Context)
        {
            return GetLoginAccountID(Context, true);
        }



        /// <summary>
        /// return All roles for current logged in account
        /// </summary>
        /// <param name="Context"></param>
        /// <returns></returns>
        public List<string> GetLoginRoles(HttpContextBase Context)
        {
            List<string> roles = new List<string> { };
            if (!String.IsNullOrEmpty(GetLoginAccountID(Context)))
            {
                roles = DBGetRoles(int.Parse(Encryption.Decrypt16(GetLoginAccountID(Context))));
            }
            return roles;
        }


    }
}

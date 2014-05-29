using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web;
using Common;

namespace gaadideal.auth
{
    public enum AccountType { members }

    public class error
    {
        public int code { get; set; }
        public String redirect { get; set; }
        public String message { get; set; }
    }
    public class PrasagAuthorize : AuthorizeAttribute
    {
        #region Public Values
        public bool needProtect = false;
        public int LocalTestMemberID = 0;
        //public bool IsSkipDevTeamLogin = false;
        public AccountType[] AccountTypes = new AccountType[] { };

        #endregion

        #region Private Variable
        error currentError = null;
        List<AccountType> account_types = new List<AccountType>();
        #endregion

        #region Auth
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            account_types = AccountTypes.ToList();

            PrasagIdentity id = new PrasagIdentity();
            PrasagPrincipal User = new PrasagPrincipal(filterContext.HttpContext, id, new PrasagMemberManager());
            SetLocalTestID(User);

            filterContext.HttpContext.User = User;
            base.OnAuthorization(filterContext);
        }
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            PrasagPrincipal user = (PrasagPrincipal)httpContext.User;

            set_protect();

            if (!valid_account(user))
            {
                return false;
            }


            return true;
        }
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            handle_error(filterContext, currentError);
        }
        #endregion

        #region Validate Methods

        bool valid_login(Account a)
        {
            return a.id > 0;
        }
        bool valid_active(Account a)
        {
            return a.is_active;
        }
        bool valid_account(PrasagPrincipal user)
        {

            if (account_types.Contains(AccountType.members))
            {
                if (!valid_login(user.member))
                {
                    currentError = member_login;
                    return false;
                }
                if (!valid_active(user.member))
                {
                    currentError = member_active;
                    return false;
                }
            }
            return true;
        }
        #endregion

        #region set error data
        static error member_login = new error
        {
            code = -1001,
            redirect = System.Configuration.ConfigurationManager.AppSettings.Get("member_login_url").ToLower()
        };

        static error member_active = new error
        {
            code = -1002,
            redirect = System.Configuration.ConfigurationManager.AppSettings.Get("member_not_active_url").ToLower()
        };
          
        #endregion

        void set_protect()
        {
            if (Website.IsDev)
            {
                needProtect = true;
            } 
        }
        protected void SetLocalTestID(PrasagPrincipal User)
        {
            if (Website.IsLocal)
            {
                if (LocalTestMemberID > 0)
                {
                    User.member.SetLocalTestId(LocalTestMemberID);
                }               
            }
        }
        void handle_error(AuthorizationContext filterContext, error e)
        {
            string requestedWith = filterContext.HttpContext.Request.Headers["X-Requested-With"];
            bool isAjax = !String.IsNullOrEmpty(requestedWith) && requestedWith.ToLower().Contains("XMLHttpRequest".ToLower());

            string current_url = filterContext.HttpContext.Request.Url.ToString();

            int stringIndex = e.redirect.IndexOf("?returnUrl=");

            if (stringIndex != -1)
            {
                e.redirect = e.redirect.Substring(0, stringIndex);
            }

            e.redirect = e.redirect + "?returnUrl=" + HttpUtility.UrlEncode(current_url);

            if (isAjax)
            {
                string json = "{\"AuthorizationError\":{ \"code\": " + e.code + ",\"message\": \"" + e.message + "\",\"redirect\": \"" + e.redirect + "\" }}";
                filterContext.HttpContext.Response.Write(json);
                filterContext.HttpContext.Response.End();
            }
            else
            {
                filterContext.Result = new RedirectResult(e.redirect);
            }
        }


    }
}

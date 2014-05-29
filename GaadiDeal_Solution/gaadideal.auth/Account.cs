using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web; 

namespace gaadideal.auth
{
    public abstract class Account
    {
        Int32 _id = 0;
        bool _is_active = false;

        public Int32 id
        {
            get
            {
                if (_id == 0)
                {
                    _id = accountManager.GetLoginIntegerAccountID(this.Context);
                }

                return _id;
            }
        }
        public bool is_active
        {
            get
            {
                if (!_is_active)
                {
                    _is_active = accountManager.IsActiveAccount(id);
                }

                return _is_active;
            }
        }

        public void SetLocalTestId(int testId)
        {
            if (Common.Website.IsLocal)
            {
                this._id = testId;
            }
        }

        public AbstractAccountManager accountManager { get; protected set; }
        protected HttpContextBase Context;

        public Account(HttpContextBase Context, AbstractAccountManager am)
        {
            this.accountManager = am;
            this.Context = Context;
        }

        public abstract bool is_authorized_app(int app_id);
        public abstract bool is_authorized_group(int group_id);
    }
    public class PrasagAccount : Account
    {
        public PrasagAccount(HttpContextBase Context, PrasagMemberManager am)
            : base(Context, am)
        {
        }

        public PrasagMemberManager prasagMemberManager { get { return (PrasagMemberManager)this.accountManager; } }

        Member _Member = new Member();

        public Member Member
        {
            get
            {
                if (this.id > 0 && _Member.MemberID != this.id)
                {
                    _Member = this.prasagMemberManager.GetContact(this.id);
                }
                return _Member;
            }
        }

        public override bool is_authorized_app(int app_id) { return true; }
        public override bool is_authorized_group(int group_id) { return true; }
    }
}

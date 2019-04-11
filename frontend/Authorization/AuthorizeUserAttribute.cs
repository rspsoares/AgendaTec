using System.Web.Mvc;

namespace AgendaTech.View.Authorization
{
    public class AuthorizeUserAttribute : AuthorizeAttribute
    {
        private string _access;     

        public string AccessLevel
        {
            get
            {
                return (this._access ?? string.Empty);
            }
            set
            {
                this._access = value;
            }
        }
    }
}
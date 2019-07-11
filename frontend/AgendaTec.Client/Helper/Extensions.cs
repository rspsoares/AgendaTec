using AgendaTec.Business.Entities;
using AgendaTec.Client.Models;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;

namespace AgendaTec.Client.Helper
{
    public static class Extensions
    {
        public static string GetFullName(this IPrincipal usr)
        {
            return ((ClaimsIdentity)usr.Identity).FindFirst("FullName")?.Value;         
        }

        public static string GetFirstName(this IPrincipal usr)
        {
            return ((ClaimsIdentity)usr.Identity).FindFirst("FirstName")?.Value;
        }        

        public static string GetIdRole(this IPrincipal usr)
        {
            return ((ClaimsIdentity)usr.Identity).FindFirst("IDRole")?.Value;
        }

        public static string GetIdCustomer(this IPrincipal usr)
        {
            return ((ClaimsIdentity)usr.Identity).FindFirst("IDCustomer")?.Value.ToString();
        }

        public static string GetIdUser(this IPrincipal usr)
        {
            return ((ClaimsIdentity)usr.Identity).FindFirst("IDUser")?.Value.ToString();
        }

        public static ApplicationUser GenerateNewUserFromExternalLogin(this ExternalLoginInfo loginInfo, int idCustomer)
        {
            string firstName = string.Empty;
            string lastName = string.Empty;

            switch (loginInfo.Login.LoginProvider)
            {
                case "Google":
                    firstName = loginInfo.ExternalIdentity.Claims.ToList().Where(x => x.Type.Equals("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname")).SingleOrDefault()?.Value;
                    lastName = loginInfo.ExternalIdentity.Claims.ToList().Where(x => x.Type.Equals("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname")).SingleOrDefault()?.Value;
                    break;
                case "Facebook":
                    var fullName = loginInfo
                        .ExternalIdentity
                        .Claims
                        .ToList()
                        .Where(x => x.Type.Equals("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"))
                        .SingleOrDefault()?
                        .Value
                        .Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

                    firstName = fullName[0]?.ToString();
                    lastName = fullName[1]?.ToString();
                    break;
            }

            return new ApplicationUser
            {
                //IDCustomer = idCustomer,
                IDRole = ((int)EnUserType.Consumer).ToString(),
                FirstName = firstName,
                LastName = lastName,
                CPF = string.Empty,
                UserName = loginInfo.Email,
                Email = loginInfo.Email,
                PhoneNumber = string.Empty,
                IsEnabled = true,
                DirectMail = true,
                RootUser = false
            };
        }
    }
}
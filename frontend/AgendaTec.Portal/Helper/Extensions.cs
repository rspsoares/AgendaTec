﻿using System.Security.Claims;
using System.Security.Principal;

namespace AgendaTec.Portal.Helper
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
    }
}
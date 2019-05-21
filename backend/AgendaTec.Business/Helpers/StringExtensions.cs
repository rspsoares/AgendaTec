using System;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;

namespace AgendaTec.Business.Helpers
{
    public static class StringExtensions
    {
        public static bool IsCNPJ(this string cnpj)
        {
            if (string.IsNullOrEmpty(cnpj))
                return false;

            var multiplicador1 = new int[12] { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            var multiplicador2 = new int[13] { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            cnpj = cnpj.Trim().CleanMask();

            if (cnpj.Length != 14)
                return false;

            var tempCnpj = cnpj.Substring(0, 12);
            var soma = 0;

            for (var i = 0; i < 12; i++)
                soma += int.Parse(tempCnpj[i].ToString(CultureInfo.InvariantCulture)) * multiplicador1[i];

            var resto = soma % 11;

            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;

            var digito = resto.ToString(CultureInfo.InvariantCulture);
            tempCnpj = tempCnpj + digito;
            soma = 0;

            for (var i = 0; i < 13; i++)
                soma += int.Parse(tempCnpj[i].ToString(CultureInfo.InvariantCulture)) * multiplicador2[i];

            resto = soma % 11;
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;

            digito = digito + resto.ToString(CultureInfo.InvariantCulture);

            return cnpj.EndsWith(digito);
        }

        public static bool IsCPF(this string cpf)
        {
            if (string.IsNullOrEmpty(cpf))
                return false;

            var multiplicador1 = new int[9] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            var multiplicador2 = new int[10] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

            cpf = cpf.Trim().CleanMask();

            if (cpf.Length != 11)
                return false;

            var tempCpf = cpf.Substring(0, 9);
            var soma = 0;

            for (var i = 0; i < 9; i++)
                soma += int.Parse(tempCpf[i].ToString(CultureInfo.InvariantCulture)) * multiplicador1[i];

            var resto = soma % 11;

            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;

            var digito = resto.ToString(CultureInfo.InvariantCulture);
            tempCpf = tempCpf + digito;
            soma = 0;

            for (var i = 0; i < 10; i++)
                soma += int.Parse(tempCpf[i].ToString(CultureInfo.InvariantCulture)) * multiplicador2[i];

            resto = soma % 11;

            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;

            digito = digito + resto.ToString(CultureInfo.InvariantCulture);

            return cpf.EndsWith(digito);
        }

        public static string CleanMask(this string value)
        {
            if (value == null)
                return string.Empty;

            return value.Where(character =>
                character != '(' &&
                character != ')' &&
                character != '.' &&
                character != '-' &&
                character != ' ' &&
                character != '/').Aggregate(string.Empty, (current, character) => current + character);
        }

        public static string ToUnsecureString(this SecureString securePassword)
        {
            if (securePassword == null)
                throw new ArgumentNullException("Please, inform the Secure Password");

            var unmanagedString = IntPtr.Zero;

            try
            {
                unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(securePassword);
                return Marshal.PtrToStringUni(unmanagedString);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
            }
        }

        public static SecureString ToSecureString(this string plainPassword)
        {
            var secureString = new SecureString();

            foreach (var c in plainPassword.ToCharArray())
                secureString.AppendChar(c);

            secureString.MakeReadOnly();

            return secureString;
        }

        public static SecureString ToSecureString(this char[] charArray)
        {
            var secureString = new SecureString();

            foreach (var c in charArray)
                secureString.AppendChar(c);

            secureString.MakeReadOnly();

            return secureString;
        }
    }
}

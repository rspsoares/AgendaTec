﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AgendaTec.Client.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "Remember this browser?")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required]        
        [Display(Name = "Primeiro nome")]
        public string FirstName { get; set; }

        [Required]        
        [Display(Name = "Sobrenome")]
        public string LastName { get; set; }
       
        [Display(Name = "CPF")]
        public string CPF { get; set; }

        [Required]
        [EmailAddress]        
        [Display(Name = "E-mail")]
        public string Email { get; set; }

        [Required]        
        [Display(Name = "Celular")]
        public string Phone { get; set; }

        [Required]
        [StringLength(20, ErrorMessage = "A senha deve ter no mínimo {2} caracteres de comprimento.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Senha")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmação senha")]
        [Compare("Password", ErrorMessage = "A senha e a confirmação não conferem.")]
        public string ConfirmPassword { get; set; }
    }

    public class ResetPasswordViewModel
    {        
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(20, ErrorMessage = "A senha deve ter no mínimo {2} caracteres de comprimento.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Senha")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmação senha")]
        [Compare("Password", ErrorMessage = "A senha e a confirmação não conferem.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}

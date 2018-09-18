using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Stipendium.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "E-post")]
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
        [Display(Name = "E-post")]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {
        [Required]
        [Display(Name = "E-post")]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Lösenord")]
        public string Password { get; set; }

        [Display(Name = "Kom ihåg mig")]
        public bool RememberMe { get; set; }

        public DateTimeOffset LastActivityDate { get; set; }
    }

    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "E-post")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Lösenord måste ha minst {2} tecken.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Lösenord")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Bekräfta lösenordet")]
        [Compare("Password", ErrorMessage = "Lösenorden matchar inte")]
        public string ConfirmPassword { get; set; }

        public DateTime RegistrationDate { get { return DateTime.Now; } }
    }

    public class RegisterPrivateViewModel : RegisterViewModel
    {

        [Required]
        [Display(Name = "Förnamn")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Efternamn")]
        public string LastName { get; set; }

        public string Address { get; set; }

        [Display(Name = "Postnummer")]
        public string PostNr { get; set; }

        [Display(Name = "Ort")]
        public string City { get; set; }

        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Telefon Nr")]
        public string PhoneNumber { get; set; }
    }

    public class RegisterCompanyViewModel : RegisterViewModel
    {
        public virtual Stiftelse Stiftelse { get; set; }
    }

        public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "E-post")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Lösenord måste ha minst {2} tecken.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Nytt lösenord")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Upprepa nytt lösenord")]
        [Compare("Password", ErrorMessage = "Lösenorden matchar inte.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "E-post")]
        public string Email { get; set; }
    }
}

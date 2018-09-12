using KeysPlus.Data;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


//namespace KeysPlus.Website.Models
//{
//    public class ExternalLoginConfirmationViewModel
//    {
//        [Required]
//        [Display(Name = "User name")]
//        public string UserName { get; set; }
//    }

//    public class ManageUserViewModel
//    {
//        [Required]
//        [DataType(DataType.Password)]
//        [Display(Name = "Current password")]
//        public string OldPassword { get; set; }

//        [Required]
//        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
//        [DataType(DataType.Password)]
//        [Display(Name = "New password")]
//        public string NewPassword { get; set; }

//        [DataType(DataType.Password)]
//        [Display(Name = "Confirm new password")]
//        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
//        public string ConfirmPassword { get; set; }
//    }

//    public class ExternalLoginListViewModel
//    {
//        public string ReturnUrl { get; set; }
//    }

//    public class SendCodeViewModel
//    {
//        public string SelectedProvider { get; set; }
//        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
//        public string ReturnUrl { get; set; }
//        public bool RememberMe { get; set; }
//    }

//    public class VerifyCodeViewModel
//    {
//        [Required]
//        public string Provider { get; set; }

//        [Required]
//        [Display(Name = "Code")]
//        public string Code { get; set; }
//        public string ReturnUrl { get; set; }

//        [Display(Name = "Remember this browser?")]
//        public bool RememberBrowser { get; set; }

//        public bool RememberMe { get; set; }
//    }

//    public class ForgotViewModel
//    {
//        [Required]
//        [Display(Name = "Email")]
//        public string Email { get; set; }
//    }

//    public class LoginViewModel
//    {
//        [Required]
//        [Display(Name = "Username")]
//        public string UserName { get; set; }

//        [Required]
//        [DataType(DataType.Password)]
//        [Display(Name = "Password")]
//        public string Password { get; set; }

//        [Display(Name = "Remember me?")]
//        public bool RememberMe { get; set; }
//    }

//    public class RegisterViewModel
//    {
//        //[Required]
//        //[EmailAddress]
//        //[Display(Name = "Email")]
//        //public string Email { get; set; }

//        //[Required]
//        //[Display(Name = "First name")]
//        //public string FirstName { get; set; }
//        //[Required]
//        //[Display(Name = "Last name")]
//        //public string LastName { get; set; }


//        [Required]
//        [Display(Name = "Email Address")]
//        [System.Web.Mvc.Remote("IsEmailAvailable", "Account", ErrorMessage = "Email already in use.")]
//        [RegularExpression(@"^(?("")("".+?(?<!\\)""@)|(([0-9a-zA-Z]((\.(?!\.))|[-'\w])*)(?<=[0-9a-zA-Z])@))" +
//                @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-zA-Z][\w]*[0-9a-zA-Z]*\.)+[A-Za-z0-9]{0,22}[A-Za-z0-9]))$", ErrorMessage = "Invalid Email Address")]
//        //[RegularExpression("^[a-zA-Z0-9.]+@([a-zA-Z0-9]+.)+[a-zA-Z]{2,6}$", ErrorMessage = "Invalid Email Address")]
//        //[EmailAddress(ErrorMessage = "Invalid Email Address")]
//        public string UserName { get; set; }

//        [Required]
//        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
//        [DataType(DataType.Password)]
//        [Display(Name = "Password")]
//        public string Password { get; set; }

//        [DataType(DataType.Password)]
//        [Display(Name = "Confirm password")]
//        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
//        public string ConfirmPassword
//        { get
//            { return Password; }
//            set {  this.Password=Password; } }

//        [Required(ErrorMessage ="Please select Your category")]
//        //[Display(Name ="Category")]
//        public int RoleID { get; set; }

//        [Required (ErrorMessage="Please enter First Name")]
//        [Display(Name="First Name")]
//        [RegularExpression("^[a-zA-ZàáâäãåąčćęèéêëėįìíîïłńòóôöõøùúûüųūÿýżźñçčšžÀÁÂÄÃÅĄĆČĖĘÈÉÊËÌÍÎÏĮŁŃÒÓÔÖÕØÙÚÛÜŲŪŸÝŻŹÑßÇŒÆČŠŽ∂ð ,.'-]{2,30}$",
//            ErrorMessage = "First Name,Please enter valid name")]
//        public string FirstName { get; set; }

//        //[Required(ErrorMessage = "Please enter Middle Name")]
//        [Display(Name = "Middle Name")]
//        public string MiddleName { get; set; }

//        [Required(ErrorMessage = "Please enter Last Name")]
//        [Display(Name = "Last Name")]
//        [RegularExpression("^[a-zA-ZàáâäãåąčćęèéêëėįìíîïłńòóôöõøùúûüųūÿýżźñçčšžÀÁÂÄÃÅĄĆČĖĘÈÉÊËÌÍÎÏĮŁŃÒÓÔÖÕØÙÚÛÜŲŪŸÝŻŹÑßÇŒÆČŠŽ∂ð ,.'-]{2,30}$",
//            ErrorMessage = "Last Name,Please enter valid name")]
//        public string LastName { get; set; }

//        //[Required]
//        //[DataType(DataType.EmailAddress, ErrorMessage = "Please enter valid Email Address")]
//        //public string Email { get; set; }

//        //[Required]
//        //[DataType(DataType.PhoneNumber, ErrorMessage = "Please enter vaild Mobile Number")]
//        //[RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid Phone number")]
//        //[Display(Name = "Mobile Number")]
//        //public string MobileNum { get; set; }


//    }

//    //public class RoleCheckBox
//    //{
//    //    public int RoleId { get; set; }
//    //    public string RoleTitle { get; set; }
//    //    public bool IsChecked { get; set; }
//    //}

//    public class ResetPasswordViewModel
//    {
//        [Required]
//        [EmailAddress]
//        [Display(Name = "Email")]
//        public string Email { get; set; }

//        [Required]
//        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
//        [DataType(DataType.Password)]
//        [Display(Name = "Password")]
//        public string Password { get; set; }

//        [DataType(DataType.Password)]
//        [Display(Name = "Confirm password")]
//        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
//        public string ConfirmPassword { get; set; }

//        public string Code { get; set; }
//    }

//    public class ForgotPasswordViewModel
//    {
//        [Required]
//        [EmailAddress]
//        [Display(Name = "Email")]
//        public string Email { get; set; }
//    }
//}

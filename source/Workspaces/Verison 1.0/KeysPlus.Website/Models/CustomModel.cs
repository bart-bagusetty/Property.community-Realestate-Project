using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Reflection;


//namespace KeysPlus.Website.Models
//{
//    public class ChangePasswordModel
//    {
//        [Required(ErrorMessage = "* Current password is required.")]
//        [DataType(DataType.Password)]
//        [Display(Name = "Current password")]
//        [Remote("IsPassValid", "Account", ErrorMessage = "* Please enter correct current password.")]
//        public string OldPassword { get; set; }

//        [Required(ErrorMessage = "* New password is required.")]
//        [DataType(DataType.Password)]        
//        [Display(Name = "New password")]
//        [MinLength(7, ErrorMessage = "* The new password must be at least 7 characters.")]
//        [Remote("IsPassNew", "Account", ErrorMessage = "* New Password must differ from old password.")]
//         public string NewPassword { get; set; }

//        [Required(ErrorMessage = "* Confirm password is required.")]
//        [DataType(DataType.Password)]        
//        [Display(Name = "Confirm new password")]
//        [MinLength(7, ErrorMessage = "* The confirm password must be at least 7 characters."),]
//        [System.ComponentModel.DataAnnotations.Compare("NewPassword", ErrorMessage = "* The new password and confirmation password do not match.")]
//        public string ConfirmPassword { get; set; }

//        public bool HasError { get; set; }

//        public List<ErrorModel> ErrorList { get; set; }
//    }

//    public class ErrorModel
//    {
//        public string PropertyName { get; set; }
//        public string ErrorMessage { get; set; }
//    }

   
//}

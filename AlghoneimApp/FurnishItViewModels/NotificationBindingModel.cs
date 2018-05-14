using DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FurnishIt.FurnishItViewModels
{
    public class NotificationsBindingModel
    {
        [Required]
        [Display(Name = "User Id")]
        public int ID { get; set; }

        [Required]
        public Boolean is_Notify { get; set; }
        
    }
    public class SetPasswordBindingModel
    {

        public int ID { get; set; }

        [Required]

        [DataType(DataType.Password)]
        [Display(Name = "Old password")]
        public string OldPassword { get; set; }


        [Required]

        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

    }
    public class ResetPassword
    {

        public int ID { get; set; }

        public string email { get; set; }

        [Required]
        [Display(Name = "New password")]
        public string Password { get; set; }

    }

    public class PhoneVerificationModel
    {
        [Required]
        [Display(Name = "Request Id")]
        public string requestId { get; set; }

        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }

        public string email { get; set; }
    }
    public class StoreViewModel
    {
        public IEnumerable<Store> Stores { get; set; }

    }
    public class ProductViewModel
    {
        public IEnumerable<Product> products { get; set; }
    }
    public class CategoriesViewModel
    {
        public IEnumerable<Category> Categories { get; set; }

    }
}
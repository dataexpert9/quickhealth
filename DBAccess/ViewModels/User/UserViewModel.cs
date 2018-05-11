using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace DBAccess.ViewModels
{
    public class EditUserProfileBindingModel
    {
        public string FullName { get; set; }

        public string Gender { get; set; }

        public string Location { get; set; }

        public string PhoneNumber { get; set; }

        public string Email { get; set; }

        public string Address { get; set; }

        public DateTime DOB { get; set; }

        public string Weight { get; set; }

        public string Height { get; set; }

        public string BMI { get; set; }
    }
    
    public class UserViewModel
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string FullName { get; set; }

        public string ProfilePictureUrl { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }
        
        public string AccountType { get; set; }

        public string ZipCode { get; set; }

        public string DateofBirth { get; set; }

        public short? SignInType { get; set; }

        public string UserName { get; set; }

        public short? Status { get; set; }

        public bool EmailConfirmed { get; set; }

        public bool PhoneConfirmed { get; set; }
    }

    public class PhoneVerificationViewModel
    {
        [Required]
        [Display(Name = "Request Id")]
        public string request_id { get; set; }

        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }
    }

    public class MessageViewModel
    {
        public string StatusCode { get; set; }
        public string Details { get; set; }
    }

    public class ImagePathViewModel
    {
        [Required]
        public string Path { get; set; }
    }
}
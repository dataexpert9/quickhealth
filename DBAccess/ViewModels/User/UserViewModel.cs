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
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }
        
        public string SurName { get; set; }

        [Required]
        public int Gender { get; set; }

        public string Bio { get; set; }

        [Required]
        public int Airline_Id { get; set; }

        [Required]
        public string JobTitle { get; set; }

        [Required]
        public string Grade { get; set; }

        [Required]
        public string AircraftTrainedFor { get; set; }

        [Required]
        public string Base { get; set; }

        [Required]
        public string DateofBirth { get; set; }

        [Required]
        public int Language_Id { get; set; }

        public string PassportNo { get; set; }

        public string PassportCountryIssued { get; set; }

        public string ExpiryDate { get; set; }

        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
        
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }
        public string PassportExpiryDate { get; set; }
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
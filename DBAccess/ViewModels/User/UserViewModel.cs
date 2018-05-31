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

        public EditUserProfileBindingModel()
        {
            About = new EditProfileAbout();
            FamilyHistory = new List<EditProfileFamilyHistory>();
            LifeStyle = new EditProfileLiftStyle();
            MedicalConditions = new List<EditProfileMedicalCondition>();
            Allergies = new List<EditProfileAllergy>();
            Medications = new List<EditProfileMedications>();
            Vaccinations = new List<EditVaccinations>();
        }

        public int User_Id { get; set; }

        public string Email { get; set; }

        public EditProfileAbout About { get; set; }

        public List<EditProfileFamilyHistory> FamilyHistory { get; set; }

        public EditProfileLiftStyle LifeStyle { get; set; }

        public List<EditProfileMedicalCondition> MedicalConditions { get; set; }

        public List<EditProfileAllergy> Allergies { get; set; }

        public List<EditProfileMedications> Medications { get; set; }

        public List<EditVaccinations> Vaccinations { get; set; }
    }

    public class EditVaccinations
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }

    public class EditProfileMedications
    {
        public int Id { get; set; }

        public string Medicine_Name { get; set; }

        public string TimePeriod { get; set; }

    }

    public class EditProfileAllergy
    {
        public int Id { get; set; }

        public string Allergy_Name { get; set; }
    }

    public class EditProfileMedicalCondition
    {

        public int Id { get; set; }

        public string Condition { get; set; }

    }

    public class EditProfileLiftStyle
    {
        public int Id { get; set; }

        public string DietryRestrictions { get; set; }

        public string Alcohol { get; set; }

        public string Smoking { get; set; }

        public string SexuallyActive { get; set; }

        public string RecreationalDrugs { get; set; }

    }

    public class EditProfileVaccinations
    {
        public string Vaccination_Name { get; set; }
    }

    public class EditProfileFamilyHistory
    {

        public int Id { get; set; }

        public int FamilyMember_Id { get; set; }

        public string Relation { get; set; }

        public string Reason { get; set; }

    }

    public class EditProfileAbout
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
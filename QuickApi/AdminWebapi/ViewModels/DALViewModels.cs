using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AdminWebapi.ViewModels
{
    public class DALViewModels
    {
        
    }
    public class AdminPharmacyRequestViewModel
    {
        public int Id { get; set; }

        public int Status { get; set; } // Dispatch, Delivery In Progress , Cancel , Deliveryed 

        public string Purpose { get; set; }

        public string ChannelName { get; set; }

        public double Total { get; set; }

        public int User_Id { get; set; }

        public int? FamilyMember_Id { get; set; }
        
        public int Pharmacy_Id { get; set; }
        
        public int? Appointment_Id { get; set; }
        
    }

    public class AdminAppointmentViewModel
    {
      
        public int Id { get; set; }

        public string Symptoms { get; set; }

        public int AppointmentType { get; set; }

        public int Status { get; set; }

        public string Purpose { get; set; }

        public bool IsFever { get; set; }

        public string Temperature { get; set; }

        public DateTime AppointmentDateTime { get; set; }

        public int? FamilyMember_Id { get; set; }

        public int User_Id { get; set; }
        
        public int? Doctor_Id { get; set; }

        public string Diagnosis { get; set; }

        public AdminDoctorViewModel Doctor { get; set; }

        public List<AllergyViewModel> AllergyHistory { get; set; }

        public List<VaccinationViewModel> VaccinationHistory { get; set; }

        public List<MedicationsViewModel> MedicationHistory { get; set; }

        public List<MedicalConditionViewModel> MedicalConditionHistory { get; set; }

        public LifeStyleViewModel LifeStyleHistory { get; set; }




    }
    public class AdminPharmacyViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }

        public string Country { get; set; }

        public string ImageUrl { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public string PhoneNumber { get; set; }

        public string ZipCode { get; set; }

        public bool IsDeleted { get; set; }
    }
    public class AdminDoctorViewModel
    {
        public int Id { get; set; }

        public string FullName { get; set; }

        public string SurName { get; set; }

        public string Phone { get; set; }

        public string DateofBirth { get; set; }

        public string Email { get; set; }

        public int? SignInType { get; set; }

        public string Country { get; set; }

        public string City { get; set; }

        public int ProviderType { get; set; }

        public bool IsAvailable { get; set; }

        public string Gender { get; set; }

        public string Password { get; set; }

        public string Specialization { get; set; }

        public string Department { get; set; }

        public string LatestQualification { get; set; }

        public string Bio { get; set; }

        public string Address { get; set; }

        public short? Status { get; set; }

        public bool EmailConfirmed { get; set; }

        public bool PhoneConfirmed { get; set; }

        public bool IsNotificationsOn { get; set; }

        public bool IsDeleted { get; set; }

        public string ProfilePictureUrl { get; set; }

    }

    public class AdminUserViewModel
    {
        public int Id { get; set; }

        public string Gender { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public DateTime? DateofBirth { get; set; }

        public int? SignInType { get; set; }

        public short? Status { get; set; }

        public bool EmailConfirmed { get; set; }

        public string Location_Name { get; set; }
        
        public bool PhoneConfirmed { get; set; }

        public string Country { get; set; }

        public string City { get; set; }

        public int ProviderType { get; set; }

        public string Name { get; set; }

        public string PhoneNumber { get; set; }

        public string Address { get; set; }

        public string Weight { get; set; }

        public string Height { get; set; }

        public string BMI { get; set; }

        public string ProfilePictureUrl { get; set; }

    }
    public class AdminFamilyMemberViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string ImageUrl { get; set; }

        public string Relation { get; set; }

        public int Age { get; set; }

        public int User_Id { get; set; }
    }

    public class AdminPharmacyPrescriptionViewModel
    {
        public int Id { get; set; }

        public string Dose { get; set; }

        public string MedicineName { get; set; }

        public double Price { get; set; }

        public int PharmacyRequest_Id { get; set; }
    }

    public class AssignPharmacyRequestBindingModel
    {
        public int Pharmacy_Id { get; set; }

        public int PharmacyRequest_Id { get; set; }
    }
    
}
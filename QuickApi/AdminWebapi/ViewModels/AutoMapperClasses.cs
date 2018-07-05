using DBAccess.Models;
using DBAccess.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AdminWebapi.ViewModels
{
    public class AutoMapperClasses
    {
    }

    public class AdimPharmacyRequestViewModel
    {
        public AdimPharmacyRequestViewModel()
        {
            PharmacyPrescription = new List<AdminPharmacyPrescriptionViewModel>();
        }

        public int Id { get; set; }

        public int Status { get; set; } // Dispatch, Delivery In Progress , Cancel , Deliveryed 

        public string Purpose { get; set; }

        public string ChannelName { get; set; }

        public double Total { get; set; }

        public int User_Id { get; set; }

        public int? FamilyMember_Id { get; set; }

        public int Pharmacy_Id { get; set; }

        public int? Appointment_Id { get; set; }

        public AdminUserViewModel User { get; set; }

        public Pharmacy Pharmacy { get; set; }

        public AdminFamilyMemberViewModel FamilyMember { get; set; }

        public AdminAppointmentViewModel Appointment { get; set; }

        public List<PharmacyRequestImages> PharmacyRequestImages { get; set; }

        public List<AdminPharmacyPrescriptionViewModel> PharmacyPrescription { get; set; }


    }

    public class MapperAppointmentViewModel
    {
        public List<AppointmentHistoryViewModel> Inquiries { get; set; }
    }


    public class GetMyCases
    {
        public List<AppointmentHistoryViewModel> Appointments { get; set; }
    }

    public class AppointmentHistoryViewModel
    {
        public int Id { get; set; }

        public string Symptoms { get; set; }

        public int AppointmentType { get; set; }

        public int Status { get; set; }

        public string Purpose { get; set; }

        public bool IsFever { get; set; }

        public string Temperature { get; set; }

        public DateTime AppointmentDateTime { get; set; }

        public string Diagnosis { get; set; }

        public UserViewModel User { get; set; }

        public DALFamilyMemberViewModel FamilyMember { get; set; }

        public List<AppointmentImagesViewModel> AppointmentImages { get; set; }

        public List<DoctorPrescriptionViewModel> DoctorPrescription { get; set; }

        public List<AllergyViewModel> AllergyHistory { get; set; }

        public List<VaccinationViewModel> VaccinationHistory { get; set; }

        public List<MedicationsViewModel> MedicationHistory { get; set; }

        public List<MedicalConditionViewModel> MedicalConditionHistory { get; set; }

        public LifeStyleViewModel LifeStyleHistory { get; set; }

        public DoctorViewModels Doctor { get; set; }

    }

    public class DALFamilyMemberViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string ImageUrl { get; set; }

        public string Relation { get; set; }

        public int Age { get; set; }

        public int User_Id { get; set; }

        public string Gender { get; set; }

        public DateTime? DateofBirth { get; set; }

        public string Weight { get; set; }

        public string Height { get; set; }

        public string BMI { get; set; }
    }
    public class DoctorPrescriptionViewModel
    {
        public int Id { get; set; }

        public string PrescriptionNote { get; set; }

        public virtual List<DoctorPrescriptionImagesViewModel> DoctorPrescriptionImages { get; set; }
    }

    public class AllergyViewModel
    {
        public int Id { get; set; }

        public string AllergyName { get; set; }

        public int Appointment_Id { get; set; }
    }
    public class VaccinationViewModel
    {
        public int Id { get; set; }

        public string Vaccination_Name { get; set; }

        public int Appointment_Id { get; set; }
    }
    public class MedicationsViewModel
    {
        public int Id { get; set; }

        public string Medicine_Name { get; set; }

        public string TimePeriod { get; set; }
    }
    public class MedicalConditionViewModel
    {
        public int Id { get; set; }

        public string Condition { get; set; }

        public int Appointment_Id { get; set; }
    }
    public class LifeStyleViewModel
    {
        public int Id { get; set; }

        public string DietryRestrictions { get; set; }

        public string Alcohol { get; set; }

        public string Smoking { get; set; }

        public string SexuallyActive { get; set; }

        public string RecreationalDrugs { get; set; }
    }
    public class AppointmentImagesViewModel
    {
        public int Id { get; set; }

        public string ImageUrl { get; set; }

        public int Appointment_Id { get; set; }
    }

    public class UserViewModel
    {
        public int Id { get; set; }

        public string Gender { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string Password { get; set; }

        public DateTime? DateofBirth { get; set; }

        public int? SignInType { get; set; }

        public short? Status { get; set; }

        public bool EmailConfirmed { get; set; }

        public string VerificationCode { get; set; }

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

        public double CompletionPercentage { get; set; }

        public bool IsNotificationsOn { get; set; }

        public bool IsDeleted { get; set; }

        public string ProfilePictureUrl { get; set; }

        public string SurName { get; set; }

        public string FullName { get; set; }

    }

    public class DoctorPrescriptionImagesViewModel
    {
        public int Id { get; set; }

        public string ImageUrl { get; set; }

        public int DoctorPrescription_Id { get; set; }
    }
}
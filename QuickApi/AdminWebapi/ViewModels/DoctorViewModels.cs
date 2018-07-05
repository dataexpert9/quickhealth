using DBAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AdminWebapi.ViewModels
{
    public class DoctorViewModels
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

        public double AverageRating { get; set; }

        public string ProfilePictureUrl { get; set; }

    }
    public class RequestInquiries
    {
        public RequestInquiries()
        {
            Inquiries = new List<Appointment>();
        }

        public List<Appointment> Inquiries { get; set; }
    }

    public class DoctorHistory
    {
        public DoctorHistory()
        {
            Inquiries = new List<Appointment>();
        }

        public List<Appointment> Inquiries { get; set; }
    }

}
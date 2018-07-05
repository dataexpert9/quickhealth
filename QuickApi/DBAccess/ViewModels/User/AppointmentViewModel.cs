using DBAccess.Models;
using MultipartDataMediaFormatter.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBAccess.ViewModels.User
{
    public class AppointmentBindingModel
    {
        public AppointmentBindingModel()
        {
            ImageUrls = new List<HttpFile>();
        }

        public int Id { get; set; }

        [Required]
        public int AppointmentType { get; set; }

        public string Purpose { get; set; }

        public List<HttpFile> ImageUrls { get; set; }

        public bool IsFever { get; set; }

        public string Symptoms { get; set; }

        public string Temperature { get; set; }

        [Required]
        public int User_Id { get; set; }

        public int? FamilyMember_Id { get; set; }
    }

    public class AppointmentViewModel
    {
        public AppointmentViewModel()
        {
            Appointment = new Appointment();
        }
        public Appointment Appointment { get; set; }

    }

    public class CancelAppointmentBindingModel
    {
        public int User_Id { get; set; }

        public int Appointment_Id { get; set; }
    }
    public class AppointmentDispatchBindingModel
    {
        public int PharmacyRequest_Id { get; set; }

        public double Total { get; set; }

        public DateTime DeliveryDateTime { get; set; }

        public string DeliveryAddress { get; set; }

    }

}

using MultipartDataMediaFormatter.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBAccess.ViewModels.Doctor
{
    class DoctorBindingModels
    {
    }
    public class AcceptAppointmentBindingModel
    {
        [Required]
        public int Appointment_Id { get; set; }

        [Required]
        public int Doctor_Id { get; set; }
    }
    public class DoctorPrescriptionBindingModfel
    {

        public DoctorPrescriptionBindingModfel()
        {
            PrescriptionImages = new List<HttpFile>();
        }

        public string PrescriptionNote { get; set; }

        public int? Doctor_Id { get; set; } = 0;

        public int Appointment_Id { get; set; }

        public List<HttpFile> PrescriptionImages { get; set; }
    }

    public class DoctorCloseChatViewModel
    {
        public int Doctor_Id { get; set; }

        public int Appointment_Id { get; set; }

        public string Diagnosis { get; set; }

    }
    public class RateDoctorBindingModel
    {
        public int Doctor_Id { get; set; }

        public int User_Id { get; set; }

        public double Rating { get; set; }

        public string Feedback { get; set; }
    }
}

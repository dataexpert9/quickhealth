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
}

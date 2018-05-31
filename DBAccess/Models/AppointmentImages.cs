using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBAccess.Models
{
    public partial class AppointmentImages
    {
        public int Id { get; set; }

        public string ImageUrl { get; set; }

        public int Appointment_Id { get; set; }

        public virtual Appointment Appointment { get; set; }
        
    }
}

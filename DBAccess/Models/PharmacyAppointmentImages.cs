using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBAccess.Models
{
    public partial class PharmacyAppointmentImages
    {
        public int Id { get; set; }

        public string ImageUrl { get; set; }

        public int PharmacyAppointment_Id { get; set; }

        public virtual PharmacyAppointment PharmacyAppointment { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBAccess.Models
{
    public partial class DoctorPrescriptionImages
    {
        public int Id { get; set; }

        public string ImageUrl { get; set; }

        public int DoctorPrescription_Id { get; set; }

        public virtual DoctorPrescription DoctorPrescription { get; set; }

    }

}

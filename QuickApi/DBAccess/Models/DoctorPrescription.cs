using MultipartDataMediaFormatter.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBAccess.Models
{
    public partial class DoctorPrescription
    {
        public int Id { get; set; }

        public string PrescriptionNote { get; set; }
        
        public int? Appointment_Id { get; set; }

        public virtual Appointment Appointment { get; set; }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual List<DoctorPrescriptionImages> DoctorPrescriptionImages { get; set; }
    }
}

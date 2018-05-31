using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBAccess.Models
{
    public partial class PharmacyAppointment
    {
        public int Id { get; set; }

        public string Purpose { get; set; }

        public int User_Id { get; set; }

        public virtual User User { get; set; }

        public int? Pharmacy_Id { get; set; }

        public virtual Pharmacy Pharmacy { get; set; }
        
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PharmacyAppointmentImages> PharmacyAppointmentImages { get; set; }
    }
}

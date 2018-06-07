using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBAccess.Models
{
    public partial class PharmacyRequest
    {
        public int Id { get; set; }

        public int Status { get; set; } // Dispatch, Delivery In Progress , Cancel , Deliveryed 

        public string Purpose { get; set; }

        public string ChannelName { get; set; }


        [NotMapped]
        public double Total { get; set; }

        public int User_Id { get; set; }
        public virtual User User { get; set; }


        public int? FamilyMember_Id { get; set; }
        public virtual FamilyMember FamilyMember { get; set; }

        public int Pharmacy_Id { get; set; }
        public virtual Pharmacy Pharmacy { get; set; }




        public int? Appointment_Id { get; set; }
        [ForeignKey("Appointment_Id")]
        public virtual Appointment Appointment { get; set; }
        


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PharmacyPrescription> PharmacyPrescription { get; set; }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual List<PharmacyRequestImages> PharmacyRequestImages { get; set; }

    }
}

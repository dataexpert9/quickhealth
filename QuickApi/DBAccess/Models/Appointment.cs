using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBAccess.Models
{
    public partial class Appointment
    {

        public int Id { get; set; }

        public string Symptoms { get; set; }

        public int AppointmentType { get; set; }

        public int Status { get; set; }

        public string Purpose { get; set; }

        public bool IsFever { get; set; }

        public string Temperature { get; set; }

        public DateTime AppointmentDateTime { get; set; }

        public string Diagnosis { get; set; }
        
        public int? FamilyMember_Id { get; set; }

        public virtual FamilyMember FamilyMember { get; set; }




        public int User_Id { get; set; }

        public virtual User User { get; set; }




        public int? Doctor_Id { get; set; }

        public virtual Doctor Doctor { get; set; }




        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual List<AppointmentImages> AppointmentImages { get; set; }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DoctorPrescription> DoctorPrescription { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AllergyHistory> AllergyHistory { get; set; }



        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<VaccinationHistory> VaccinationHistory { get; set; }
        
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MedicationHistory> MedicationHistory { get; set; }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MedicalConditionHistory> MedicalConditionHistory { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual LifeStyleHistory LifeStyleHistory { get; set; }
    }
}

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

        public int? FamilyMember_Id { get; set; }

        public virtual FamilyMember FamilyMember { get; set; }

        public int User_Id { get; set; }

        public virtual User User { get; set; }

        public int? Doctor_Id { get; set; }

        public virtual Doctor Doctor { get; set; }


    }
}

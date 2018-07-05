using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBAccess.Models
{
    public partial class Medications
    {
        public int Id { get; set; }

        public string Medicine_Name { get; set; }

        public string TimePeriod { get; set; }

        public int? User_Id { get; set; }

        public virtual User User { get; set; }

        public int? FamilyMember_Id { get; set; }

        public virtual FamilyMember FamilyMember { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBAccess.Models
{
    public partial class Prescriptions
    {
        public int Id { get; set; }

        public string Description { get; set; }

        public int? Medication_Id { get; set; }

        public Medications Medication { get; set; }

    }
}

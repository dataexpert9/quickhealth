using DBAccess.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBAccess
{
    public partial class MedicationHistory
    {
        public int Id { get; set; }

        public string Medicine_Name { get; set; }

        public string TimePeriod { get; set; }
        
        public int Appointment_Id { get; set; }

        [JsonIgnore]
        public virtual Appointment Appointment { get; set; }

    }
}

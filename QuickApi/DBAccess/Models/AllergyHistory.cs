using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBAccess.Models
{
    public partial class AllergyHistory
    {
        public int Id { get; set; }

        public string AllergyName { get; set; }

        public int Appointment_Id { get; set; }

        [JsonIgnore]
        public virtual Appointment Appointment { get; set; }
    }
}

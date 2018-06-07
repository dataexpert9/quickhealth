using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBAccess.Models
{
    public partial class PharmacyPrescription
    {
        public int Id { get; set; }

        public string Dose { get; set; }
        
        public string MedicineName { get; set; }
        
        public double Price { get; set; }

        public int PharmacyRequest_Id { get; set; }

        [JsonIgnore]
        public virtual PharmacyRequest PharmacyRequest { get; set; }
    }
}

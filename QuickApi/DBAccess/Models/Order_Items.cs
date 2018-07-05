using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBAccess.Models
{
    public partial class Order_Items
    {
        public int Id { get; set; }

        [Required]
        public int Qty { get; set; }

        public string ProductSize { get; set; } = "";

        public int? PharmacyPrescription_Id { get; set; }

        public int PharmacyOrder_Id { get; set; }

        public virtual PharmacyOrder PharmacyOrder { get; set; }

        [ForeignKey("PharmacyPrescription_Id")]
        public virtual PharmacyPrescription PharmacyPrescription { get; set; }
 
        public string Name { get; set; }

        public double Price { get; set; }

        public string Dose { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBAccess.Models
{
    public partial class Order
    {
        public int Id { get; set; }

        [Required]
        public string OrderNo { get; set; }

        [Required]
        public int Status { get; set; }

        [Required]
        //[JsonConverter(typeof(JsonCustomDateTimeConverter))]
        public DateTime OrderDateTime { get; set; }

        //[JsonConverter(typeof(JsonCustomDateTimeConverter))]
        //public DateTime? DeliveryTime_From { get; set; }

        //[JsonConverter(typeof(JsonCustomDateTimeConverter))]
        //public DateTime? DeliveryTime_To { get; set; }

        public int PaymentMethod { get; set; }

        public double Subtotal { get; set; }

        public double DeliveryFee { get; set; }

        public double Total { get; set; }

        public double Tax { get; set; }

        public double Tip { get; set; }

        public int User_Id { get; set; }

        public bool IsDeleted { get; set; }
        
        public short PaymentStatus { get; set; }
        
        public int? OrderPayment_Id { get; set; }

        public int? Pharmacy_Id { get; set; }

        public virtual Pharmacy Pharmacy { get; set; }

        public virtual OrderPayment OrderPayment { get; set; }

        public virtual User User { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PharmacyOrder> PharmacyOrder { get; set; }

    }
}

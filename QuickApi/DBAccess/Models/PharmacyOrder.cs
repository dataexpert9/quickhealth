using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBAccess.Models
{
    public partial class PharmacyOrder
    {

        public int Id { get; set; }
        
        public string OrderNo { get; set; }

        public int Status { get; set; }
        
        public double Subtotal { get; set; }

        public double Total { get; set; }

        public bool IsDeleted { get; set; }

        public double OrderDeliveryFee { get; set; } 

        public DateTime? OrderDeliveryTime { get; set; }

        public int Type_Id { get; set; }

        public int Order_Id { get; set; }

        public virtual Order Order { get; set; }

        public int Pharmacy_Id { get; set; }

        public virtual Pharmacy Pharmacy { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Order_Items> Order_Items { get; set; }

    }
}

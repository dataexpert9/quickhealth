using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBAccess.Models
{
    public partial class OrderPayment
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public OrderPayment()
        {
        }

        //[Key, ForeignKey("Order")]
        public int Id { get; set; }

        [Required]
        public string Amount { get; set; }

        [Required]
        public string PaymentType { get; set; }

        [Required]
        public string CashCollected { get; set; }

        [Required]
        public string Status { get; set; }

        [Required]
        public string Order_Id { get; set; }

        [Required]
        public string AccountNo { get; set; }

        public virtual Order Order { get; set; }
    }
}

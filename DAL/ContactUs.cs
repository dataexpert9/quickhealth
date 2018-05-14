using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class ContactUs
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public short Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public DateTime? DeletedDate { get; set; }
        public Boolean isDeleted { get; set; }
        public int? UserId { get; set; }
        public User User { get; set; }

    }
}

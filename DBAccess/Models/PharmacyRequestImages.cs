using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBAccess.Models
{
    public partial class PharmacyRequestImages
    {
        public int Id { get; set; }

        public string ImageUrl { get; set; }

        public int? PharmacyRequest_Id { get; set; }
        public virtual PharmacyRequest PharmacyRequest { get; set; }
    }
}

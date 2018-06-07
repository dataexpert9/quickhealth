using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBAccess.Models
{
    public partial class Medicine
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string ScientificName { get; set; }

        public string Price { get; set; }

        public int Pharmacy_Id { get; set; }

        public virtual Pharmacy Pharmacy { get; set; }

    }
}

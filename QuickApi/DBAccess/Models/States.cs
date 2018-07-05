using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBAccess.Models
{
    public partial class States
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int? Country_Id { get; set; }

        public virtual Countries Country { get; set; }

        public List<Cities> Cities { get; set; }

    }
}

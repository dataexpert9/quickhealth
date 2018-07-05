using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBAccess.Models
{
    public partial class Cities
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int? State_Id { get; set; }

        public virtual States State { get; set; }

        public int? Country_Id { get; set; }

        public virtual Countries Country { get; set; }

    }
}

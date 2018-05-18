using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBAccess.Models
{
    public partial class Medications
    {
        public int Id { get; set; }

        public string Medicine_Name { get; set; }

        public DateTime TimePeriod { get; set; }

        public int User_Id { get; set; }

        public virtual User User { get; set; }

    }
}

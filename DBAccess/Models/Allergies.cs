using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBAccess.Models
{
    public partial class Allergies
    {
        public int Id { get; set; }

        public string AllergyName { get; set; }

        public bool IsDeleted { get; set; }

        public int User_Id { get; set; }
        
        public virtual User User { get; set; }
    }
}

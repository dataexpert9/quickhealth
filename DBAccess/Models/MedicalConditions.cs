using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBAccess.Models
{
    public partial class MedicalConditions
    {
        public MedicalConditions()
        {
            User = new User();
        }

        public int Id { get; set; }
        
        public string Condition { get; set; }

        public int User_Id { get; set; }

        public virtual User User { get; set; }

    }
}

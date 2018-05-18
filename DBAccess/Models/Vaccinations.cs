using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBAccess.Models
{
    public partial class Vaccinations
    {
        public int Id { get; set; }

        public string Vaccination_Name { get; set; }

        public bool IsDeleted { get; set; }

        public int User_Id { get; set; }

        public virtual User User { get; set; }
        
    }
}

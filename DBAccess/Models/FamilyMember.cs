using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBAccess.Models
{
    public partial class FamilyMember
    {
        public FamilyMember()
        {
            User = new User();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public string ImageUrl { get; set; }

        public string Relation { get; set; }

        public int Age { get; set; }

        public int User_Id { get; set; }

        public virtual User User { get; set; }
        
    }
}

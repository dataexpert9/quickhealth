using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBAccess.Models
{
    public partial class Countries
    {
        public int Id { get; set; }

        public string SortName { get; set; }

        public string Name { get; set; }

        public int PhoneCode { get; set; }

        public List<States> States { get; set; }

        public List<Cities> Cities { get; set; }

    }
}

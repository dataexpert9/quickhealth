using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBAccess.ViewModels.User
{
    class FamilyMemberModels
    {
    }

    public class AddFamilyMemberBindingModel
    {
        public string FullName { get; set; }

        public string Relation { get; set; }

        public int Age { get; set; }

    }
}

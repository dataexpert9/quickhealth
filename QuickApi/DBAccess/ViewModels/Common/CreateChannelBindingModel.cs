using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBAccess.ViewModels.Common
{
    public class CreateChannelBindingModel
    {
        public int? User_Id { get; set; }

        public int? FamilyMember_Id { get; set; }

        public int? Doctor_Id { get; set; }

        public int? Pharmacy_Id { get; set; }

        public string ChannelName { get; set; }

    }

    public class CreateChannelForRequestBindingModel
    {
       public int PharmacyRequest_Id { get; set; }

    }
}

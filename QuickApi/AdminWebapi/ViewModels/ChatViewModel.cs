using DBAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AdminWebapi.ViewModels
{
    public class ChatViewModel
    {
        public Chat Chat { get; set; }
    }

    public class ChatPharmacyRequestViewModel
    {
        public int Id { get; set; }

        public int Status { get; set; } 

        public string Purpose { get; set; }

        public string ChannelName { get; set; }

        public double Total { get; set; }

        public bool IsDeleted { get; set; } = false;

        public int User_Id { get; set; }
        public virtual AdminUserViewModel User { get; set; }


        public int? FamilyMember_Id { get; set; }
        public AdminFamilyMemberViewModel FamilyMember { get; set; }

        public int Pharmacy_Id { get; set; }
        public virtual AdminPharmacyViewModel Pharmacy { get; set; }

        public int? Appointment_Id { get; set; }
        
    }
}
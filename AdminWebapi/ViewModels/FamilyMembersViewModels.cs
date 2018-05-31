using DBAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AdminWebapi.ViewModels
{
    public class FamilyMemberListViewModels
    {
        public FamilyMemberListViewModels()
        {
            FamilyMembers = new List<FamilyMember>();
        }
        public List<FamilyMember> FamilyMembers { get; set; }
    }

    public class FamilyMemberViewModel
    {
        public FamilyMemberViewModel()
        {
            FamilyMember = new FamilyMember();
        }
        public FamilyMember FamilyMember { get; set; }
    }
}
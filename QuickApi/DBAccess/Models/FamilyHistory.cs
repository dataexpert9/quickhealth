﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBAccess.Models
{
    public partial class FamilyHistory
    {
      
        public int Id { get; set; }

        public string Relation { get; set; }

        public string Reason { get; set; }

        public bool IsDeleted { get; set; }

        public int User_Id { get; set; }

        [ForeignKey("User_Id")]
        public virtual User User { get; set; }


        public int? FamilyMember_Id { get; set; }

        [ForeignKey("FamilyMember_Id")]
        public virtual FamilyMember FamilyMember { get; set; }

    }
}

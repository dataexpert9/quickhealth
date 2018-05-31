using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBAccess.Models
{
    public partial class Chat
    {
        public int Id { get; set; }

        public string ChannelName { get; set; }

        public int Status { get; set; }

        public int User_Id { get; set; }

        public virtual User User { get; set; }

        public int? Doctor_Id { get; set; }

        public virtual Doctor Doctor { get; set; }

        public int? Pharmacy_Id { get; set; }

        //public virtual Pharmacy Pharmacy { get; set; }

    }
}

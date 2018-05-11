using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBAccess.Models
{
    public class AppRatings
    {
        public int Id { get; set; }

        public short Rating { get; set; }

        public string Description { get; set; }

        public int? Doctor_Id { get; set; }

        [ForeignKey("Doctor_Id")]
        public Doctor Doctor { get; set; }

        public int? User_Id { get; set; }

        [ForeignKey("User_Id")]
        public User User { get; set; }
    }
}

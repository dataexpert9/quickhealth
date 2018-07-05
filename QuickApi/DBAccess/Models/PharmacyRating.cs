using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBAccess.Models
{
    public partial class PharmacyRating
    {
        public int Id { get; set; }

        public double Rating { get; set; }

        public string Feedback { get; set; }

        public int Pharmacy_Id { get; set; }

        [NotMapped]
        public double AverageRating { get; set; }

        public virtual Pharmacy Pharmacy { get; set; }

        public int User_Id { get; set; }

        public virtual User User { get; set; }

    }
}

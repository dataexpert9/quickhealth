using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBAccess.ViewModels.Common
{
    public class AppRatingModel
    {
        [Required]
        public double Rating { get; set; }

        public string Feedback { get; set; }

        public int? User_Id { get; set; }

        public int? Doctor_Id { get; set; }

        public int? Pharmacy_Id { get; set; }

    }
}

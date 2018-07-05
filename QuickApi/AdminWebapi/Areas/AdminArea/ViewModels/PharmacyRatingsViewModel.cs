using DBAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AdminWebapi.Areas.AdminArea.ViewModels
{
    public class PharmacyRatingsViewModel
    {
        public List<PharmacyRating> Ratings { get; set; }
        public double AverageRating { get; set; }

    }
}
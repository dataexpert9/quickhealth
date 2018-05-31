using MultipartDataMediaFormatter.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBAccess.ViewModels.Pharmacy
{
    class PharmacyViewModels
    {
    }

    public class PharmacyAppointmentBindingModel
    {
        public int? User_Id { get; set; }

        [Required]
        public int Pharmacy_Id { get; set; }

        public string purpose { get; set; }

        public List<HttpFile> ImageUrls { get; set; }
        
    }
}

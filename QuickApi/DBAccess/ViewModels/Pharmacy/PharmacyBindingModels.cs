using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBAccess.ViewModels.Pharmacy
{
    class PharmacyBindingModels
    {
    }

    public class CancelPharmacyAppointmentBindingModel
    {
        public int User_Id { get; set; }

        public int RequestPharmacy_Id { get; set; }
    }
    public class RatePharmacyBindingModel
    {
        public int Pharmacy_Id { get; set; }

        public int User_Id { get; set; }

        public double Rating { get; set; }

        public string Feedback { get; set; }
    }
}

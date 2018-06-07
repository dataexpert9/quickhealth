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
}

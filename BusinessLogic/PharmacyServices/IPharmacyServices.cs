using DBAccess.Models;
using DBAccess.ViewModels.Common;
using DBAccess.ViewModels.Pharmacy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.PharmacyServices
{
    public interface IPharmacyServices
    {
        List<Pharmacy> GetNearByPharmacies(double lat,double lng,string City);
        List<Pharmacy> SearchPharmacies(string Name = "", string PhoneNumber = "", string ZipCode = "");
        //PharmacyAppointment GetPharmacyAppointment(PharmacyAppointmentBindingModel model);



    }
}

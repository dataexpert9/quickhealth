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
        PharmacyRequest PharmacyRequest(PharmacyAppointmentBindingModel model);
        PharmacyRequest UpdatePharmacyRequest(UpdatePharmacyAppointmentBindingModel model);
        List<PharmacyRequest> GetMyPrescriptions(int User_Id,int? FamilyMember_Id);
        bool CancelPharmacyAppointment(CancelPharmacyAppointmentBindingModel model);
        bool RatePharmacy(RatePharmacyBindingModel model);






    }
}

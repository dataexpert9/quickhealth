using AdminWebapi.Areas.AdminArea.ViewModels;
using BusinessLogic.ViewModels;
using DBAccess.Models;
using DBAccess.ViewModels;
using DBAccess.ViewModels.Admin;
using DBAccess.ViewModels.Order;
using DBAccess.ViewModels.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace BusinessLogic.AdminService
{
    public interface IAdminService
    {
        Admin WebPanelLogin(WebLoginBindingModel model);
        WebDashboardStatsViewModel GetAdminDashboardStats(int AdminId);
        List<PharmacyRequest> AdminPharmacyRequests(DateTime? StartDateOfAppointment, DateTime? EndDateOfAppointment, int? Pharmacy_Id = 0, string Purpose = "");
        List<Doctor> GetTopDoctors(); 
        PharmacyRequest GetSinglePharmacyRequests(int Request_Id);
        List<PharmacyPrescription> GetPrescribedMedicines(int Pharmacy_Id,int PharmacyRequest_Id);
        Admin AddAdmin(AddAdminBindingModel model);
        SearchAdminsViewModel SearchAdmins(string FirstName, string LastName, string Email, string Phone, int? PharmacyId=0);
        object GetEntityById(int EntityType, int Id);
        PharmacyPrescription PrescribeMedicine(AddPrescribtionBindingModel model);
        List<PharmacyRequest> GetPharmacyHistory(int Pharmacy_Id);
        PharmacyRequest GetPharmacyRequestDetails(int Request_Id);
        bool UpdateStatus(int PharmacyRequest_Id,int Status);
        string PrescribeMedicine(int PharmacyRequest_Id);
        DoctorPrescription GetSingleDoctorPrescriptions(int Appointment_Id);
        Admin ChangePassword(AdminSetPasswordBindingModel model,string userEmail);
        string ChangeOrderStatus(ChangeOrderStatusListBindingModel model);
        SearchSubAdminNotificationsViewModel GetMyNotifications(int Id,bool Unread);
        List<AdminSubAdminNotifications> AddNewNotification(PubNubNotificationBindingModel model);
        int MarkNotificationAsRead(int Id,int AdminId);
        List<PharmacyRating> GetPharmacyRatings(int Pharmacy_Id);
        List<Pharmacy> GetTopPharmacies();





    }
}

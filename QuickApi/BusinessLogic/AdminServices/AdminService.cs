using BusinessLogic.Components.Helpers;
using BusinessLogic.CustomAuthorization;
using BusinessLogic.HelperServices;
using DBAccess;
using DBAccess.GenericRepository;
using DBAccess.Models;
using DBAccess.ViewModels;
using DBAccess.ViewModels.User;
using Nexmo.Api;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Data.Entity;
using Z.EntityFramework.Plus;
using AdminWebapi.Areas.AdminArea.ViewModels;
using DBAccess.ViewModels.Admin;
using BusinessLogic.ViewModels;
using static BusinessLogic.HelperServices.Utility;
using DBAccess.ViewModels.Order;
using System.Web.Hosting;

namespace BusinessLogic.AdminService
{
    public class AdminService : IAdminService
    {
        private readonly AdminDBContext _DBContext = new AdminDBContext();

        private readonly GenericRepository<Admin> _AdminRepository;
        private readonly GenericRepository<User> _UserRepository;
        private readonly GenericRepository<Appointment> _AppointmentRepository;
        private readonly GenericRepository<Doctor> _DoctorRepository;
        private readonly GenericRepository<DoctorPrescription> _DoctorPrescriptionRepository;
        private readonly GenericRepository<Pharmacy> _PharmacyRepository;
        private readonly GenericRepository<PharmacyRequest> _PharmacyRequestRepository;
        private readonly GenericRepository<PharmacyPrescription> _PharmacyPrescriptionRepository;
        private readonly GenericRepository<PharmacyOrder> _PharmacyOrderRepository;
        private readonly GenericRepository<Order> _OrderRepository;
        private readonly GenericRepository<UserDevice> _UserDeviceRepository;
        private readonly GenericRepository<Notification> _NotificationRepository;
        private readonly GenericRepository<AdminSubAdminNotifications> _AdminSubAdminNotificationsRepository;
        private readonly GenericRepository<PharmacyRating> _PharmacyRatingRepository;
        private readonly GenericRepository<Order_Items> _Order_ItemsRepository;
        private static readonly Object obj = new Object();

        public AdminService()
        {
            _AdminRepository = new GenericRepository<Admin>(_DBContext);
            _UserRepository = new GenericRepository<User>(_DBContext);
            _AppointmentRepository = new GenericRepository<Appointment>(_DBContext);
            _DoctorRepository = new GenericRepository<Doctor>(_DBContext);
            _PharmacyRepository = new GenericRepository<Pharmacy>(_DBContext);
            _PharmacyRequestRepository = new GenericRepository<PharmacyRequest>(_DBContext);
            _PharmacyPrescriptionRepository = new GenericRepository<PharmacyPrescription>(_DBContext);
            _DoctorPrescriptionRepository = new GenericRepository<DoctorPrescription>(_DBContext);
            _PharmacyOrderRepository = new GenericRepository<PharmacyOrder>(_DBContext);
            _OrderRepository = new GenericRepository<Order>(_DBContext);
            _UserDeviceRepository = new GenericRepository<UserDevice>(_DBContext);
            _NotificationRepository = new GenericRepository<Notification>(_DBContext);
            _AdminSubAdminNotificationsRepository = new GenericRepository<AdminSubAdminNotifications>(_DBContext);
            _PharmacyRatingRepository = new GenericRepository<PharmacyRating>(_DBContext);

            _Order_ItemsRepository = new GenericRepository<Order_Items>(_DBContext);
        }

        public Admin WebPanelLogin(WebLoginBindingModel model)
        {

            var hashPass = CryptoHelper.Hash(model.Password);
            var Admin = _AdminRepository.GetFirst(x => x.Email == model.Email && x.Password == hashPass);
            if (Admin == null)
                return null;
            else
                return Admin;
        }

        public WebDashboardStatsViewModel GetAdminDashboardStats(int AdminId)
        {
            AdminDashboardViewModel AdminDashboard = new AdminDashboardViewModel();
            PharmacyDashboardViewModel PharmacyDashboard = new PharmacyDashboardViewModel();

            var AdminModel = _AdminRepository.GetFirst(x => x.Id == AdminId);
            if (AdminModel != null)
            {
                if (AdminModel.Role == (int)Utility.UserTypes.SuperAdmin)
                {
                    AdminDashboard.TotalUsers = _UserRepository.CountWithCondition(x => x.IsDeleted == false);
                    AdminDashboard.TodayAppointments = _AppointmentRepository.CountAllWithoutCondition();
                    AdminDashboard.TotalDoctors = _DoctorRepository.CountWithCondition(x => x.IsDeleted == false);
                    AdminDashboard.TotalPharmacies = _PharmacyRepository.CountWithCondition(x => x.IsDeleted == false);

                }
                else if (AdminModel.Role == (int)Utility.UserTypes.Pharmacy)
                {
                    PharmacyDashboard.CompletedRequests = _PharmacyRequestRepository.CountWithCondition(x => x.Pharmacy_Id == AdminModel.Pharmacy_Id.Value && (x.Status == (int)GlobalUtility.PharmacyRequest.Delivered || x.Status == (int)GlobalUtility.PharmacyRequest.Completed || x.Status == (int)GlobalUtility.PharmacyRequest.ReadyToDispatch));
                    PharmacyDashboard.PendingRequests = _PharmacyRequestRepository.CountWithCondition(x => x.Pharmacy_Id == AdminModel.Pharmacy_Id.Value && x.Status == (int)GlobalUtility.PharmacyRequest.Requested);
                    PharmacyDashboard.TotalRequests = _PharmacyRequestRepository.CountWithCondition(x => x.Pharmacy_Id == AdminModel.Pharmacy_Id.Value);
                }
                return new WebDashboardStatsViewModel { AdminDashboard = AdminDashboard, PharmacyDashboard = PharmacyDashboard };
            }
            else
            {
                return null;
            }
        }

        public List<PharmacyRequest> AdminPharmacyRequests(DateTime? StartDateOfAppointment, DateTime? EndDateOfAppointment, int? Pharmacy_Id = 0, string Purpose = "")
        {
            var PharmacyRequests = new List<PharmacyRequest>();

            if (Pharmacy_Id.HasValue && Pharmacy_Id != 0)
            {
                if (!string.IsNullOrEmpty(Purpose))
                {
                    PharmacyRequests = _PharmacyRequestRepository.GetWithIncludeList(x => x.Status == (int)PharmacyRequestStatuses.Requested && (x.Purpose.Contains(Purpose) || x.Appointment.Purpose.Contains(Purpose)) && x.Pharmacy_Id == Pharmacy_Id, "Pharmacy", "Appointment.Doctor", "User", "FamilyMember", "Appointment.AllergyHistory", "Appointment.VaccinationHistory", "Appointment.MedicationHistory", "Appointment.MedicalConditionHistory", "Appointment.LifeStyleHistory").ToList();
                }
                else
                {
                    PharmacyRequests = _PharmacyRequestRepository.GetWithIncludeList(x => x.Status == (int)PharmacyRequestStatuses.Requested && x.Pharmacy_Id == Pharmacy_Id, "Pharmacy", "Appointment.Doctor", "User", "FamilyMember").ToList();
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(Purpose))
                {
                    PharmacyRequests = _PharmacyRequestRepository.GetWithIncludeList(x => x.Status == (int)PharmacyRequestStatuses.Requested && x.Purpose.Contains(Purpose) || x.Appointment.Purpose.Contains(Purpose), "Pharmacy", "Appointment.Doctor", "User", "FamilyMember", "Appointment.AllergyHistory", "Appointment.VaccinationHistory", "Appointment.MedicationHistory", "Appointment.MedicalConditionHistory", "Appointment.LifeStyleHistory").ToList();
                }
                else
                {
                    PharmacyRequests = _PharmacyRequestRepository.GetWithIncludeList(x => x.Status == (int)PharmacyRequestStatuses.Requested, "Pharmacy", "Appointment.Doctor", "User", "FamilyMember").ToList();
                }
            }


            return PharmacyRequests;
        }

        public List<Doctor> GetTopDoctors()
        {
            var DoctorList = _DoctorRepository.GetWithIncludeList(x => x.IsDeleted == false, "DoctorRating");

            if (DoctorList != null)
            {
                foreach (var doctor in DoctorList)
                {
                    doctor.CalculateAverageRating();
                }
            }
            return DoctorList.OrderByDescending(x => x.AverageRating).Take(5).ToList();
        }

        public List<PharmacyRating> GetPharmacyRatings(int Pharmacy_Id)
        {
            var resp = _PharmacyRatingRepository.GetWithIncludeList(x => x.Pharmacy_Id == Pharmacy_Id, "User").ToList();
            return resp;
        }

        public PharmacyRequest GetSinglePharmacyRequests(int Request_Id)
        {
            var PharmacyRequests = new PharmacyRequest();
            PharmacyRequests = _PharmacyRequestRepository.GetFirstWithInclude(x => x.Id == Request_Id, "Pharmacy", "Appointment.Doctor", "User", "FamilyMember", "PharmacyPrescription", "PharmacyRequestImages");
            return PharmacyRequests;
        }

        public List<PharmacyPrescription> GetPrescribedMedicines(int Pharmacy_Id, int PharmacyRequest_Id)
        {
            var PharmacyPrescription = _PharmacyPrescriptionRepository.GetMany(x => x.PharmacyRequest_Id == PharmacyRequest_Id);

            return PharmacyPrescription;

        }

        public Admin AddAdmin(AddAdminBindingModel model)
        {



            var Admin = _AdminRepository.GetFirst(x => x.Email.Contains(model.Email));

            if (Admin != null)
            {
                Admin.FirstName = model.FirstName;
                Admin.LastName = model.LastName;
                Admin.Email = model.Email;
                Admin.Phone = model.Phone;
                if (model.Image.Buffer != null)
                    Admin.ImageUrl = ImageHelper.SaveFileFromBytes(model.Image, "AdminImages");

                _AdminRepository.Save();
                return Admin;
            }
            else
            {
                var hashPass = CryptoHelper.Hash(model.Password);

                Admin = new Admin
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    Password = hashPass,
                    Status = (int)Utility.AccountStatus.Active,
                    Phone = model.Phone,
                    Role = (int)Utility.UserTypes.Pharmacy,
                    Pharmacy_Id = model.Pharmacy_Id,
                    IsDeleted = false
                };
                _AdminRepository.Insert(Admin);
                _AdminRepository.Save();

                return Admin;

            }


        }

        public SearchAdminsViewModel SearchAdmins(string FirstName, string LastName, string Email, string Phone, int? PharmacyId = 0)
        {
            SearchAdminsViewModel response = new SearchAdminsViewModel();

            var query = @"select * From Admins Where ";

            if (!string.IsNullOrEmpty(FirstName))
                query += "FirstName LIKE '%" + FirstName + "%' AND ";

            if (!string.IsNullOrEmpty(LastName))
                query += "LastName LIKE '%" + LastName + "%' AND";


            if (!string.IsNullOrEmpty(Email))
                query += "Email LIKE '%" + Email + "%' AND";

            if (!string.IsNullOrEmpty(Phone))
                query += "Phone LIKE '%" + Phone + "%' AND";

            if (PharmacyId.HasValue)
                query += "Pharmacy_Id LIKE '%" + PharmacyId + "%' AND";

            query += " IsDeleted='false'";


            response.Admins = _AdminRepository.GetListWithStoreProcedure(query).ToList();

            return response;
        }

        public object GetEntityById(int EntityType, int Id)
        {

            switch (EntityType)
            {
                case (int)BasketEntityTypes.Pharmacist:
                    var Pharmacist = _AdminRepository.GetFirst(x => x.Id == Id);
                    return Pharmacist;
                default:
                    return null;
            }

        }

        public PharmacyPrescription PrescribeMedicine(AddPrescribtionBindingModel model)
        {
            PharmacyPrescription PrescriptionModel = new PharmacyPrescription();
            if (model.Id != 0)
                PrescriptionModel = _PharmacyPrescriptionRepository.GetFirst(x => x.Id == model.Id);

            PrescriptionModel.MedicineName = model.MedicineName;
            PrescriptionModel.Price = model.Price;
            PrescriptionModel.Dose = model.Dose;
            PrescriptionModel.PharmacyRequest_Id = model.PharmacyRequest_Id;
            PrescriptionModel.Dose = model.Dose;


            if (model.Id == 0)
                _PharmacyPrescriptionRepository.Insert(PrescriptionModel);

            _PharmacyPrescriptionRepository.Save();
            return PrescriptionModel;

        }

        public List<PharmacyRequest> GetPharmacyHistory(int Pharmacy_Id)
        {
            var PharmacyRequests = new List<PharmacyRequest>();

            if (Pharmacy_Id != 0)
            {
                PharmacyRequests = _PharmacyRequestRepository.GetWithInclude(x => (x.Status == (int)PharmacyRequestStatuses.Rejected || x.Status == (int)PharmacyRequestStatuses.Completed) && x.Pharmacy_Id == Pharmacy_Id, "Pharmacy", "Appointment.Doctor", "User", "FamilyMember").ToList();
            }
            else
            {
                PharmacyRequests = _PharmacyRequestRepository.GetWithInclude(x => (x.Status == (int)PharmacyRequestStatuses.Rejected || x.Status == (int)PharmacyRequestStatuses.Completed), "Pharmacy", "Appointment.Doctor", "User", "FamilyMember").ToList();
            }

            return PharmacyRequests;

        }

        public PharmacyRequest GetPharmacyRequestDetails(int Request_Id)
        {
            var RequestDetails = _PharmacyRequestRepository.GetFirstWithInclude(x => x.Id == Request_Id, "User", "FamilyMember", "Appointment.Doctor", "PharmacyPrescription");
            return RequestDetails;

        }

        public bool UpdateStatus(int PharmacyRequest_Id, int Status)
        {
            var PharmacyRequest = _PharmacyRequestRepository.GetFirstWithInclude(x => x.Id == PharmacyRequest_Id, "PharmacyPrescription");
            if (Status == (int)Utility.PharmacyRequestStatuses.Completed)
            {
                if (PharmacyRequest.PharmacyPrescription.Count == 0)
                    return false;
            }
            PharmacyRequest.Status = Status;
            _PharmacyRequestRepository.Save();
            return true;
        }

        public string PrescribeMedicine(int PharmacyRequest_Id)
        {

            var PharmacyPrescription = _PharmacyPrescriptionRepository.GetFirst(x => x.Id == PharmacyRequest_Id);
            _PharmacyPrescriptionRepository.Delete(PharmacyPrescription);
            _PharmacyPrescriptionRepository.Save();
            return "Success";
        }

        public DoctorPrescription GetSingleDoctorPrescriptions(int Appointment_Id)
        {
            var DoctorPrescription = _DoctorPrescriptionRepository.GetFirstWithInclude(x => x.Appointment_Id == Appointment_Id, "DoctorPrescriptionImages");
            return DoctorPrescription;

        }


        public Admin ChangePassword(AdminSetPasswordBindingModel model, string userEmail)
        {
            var oldPass = CryptoHelper.Hash(model.OldPassword);
            var Admin = _AdminRepository.GetFirst(x => x.Email == userEmail);

            if (Admin.Password == oldPass)
            {
                var newPass = CryptoHelper.Hash(model.NewPassword);
                Admin.Password = newPass;
                _AdminRepository.Save();
                return Admin;
            }
            else
            {
                return null;
            }





        }

        public string ChangeOrderStatus(ChangeOrderStatusListBindingModel model)
        {

            int User_Id = 0;
            string NotificationMessage = string.Empty;

            foreach (var order in model.Orders)
            {

                var existingStoreOrder = _PharmacyOrderRepository.GetFirst(x => x.Id == order.StoreOrder_Id);
                if (existingStoreOrder != null)
                {
                    existingStoreOrder.Status = order.Status;
                }
            }
            _PharmacyOrderRepository.Save();

            //Mark Statuses for Orders

            foreach (var order in model.Orders)
            {
                var existingOrder = _OrderRepository.GetFirstWithInclude(x => x.Id == order.OrderId, "PharmacyOrder");
                User_Id = existingOrder.User_Id;
                existingOrder.Status = existingOrder.PharmacyOrder.Min(x => x.Status);
                _OrderRepository.Save();
            }

            foreach (var item in model.Orders)
            {


                var updateStatus = _Order_ItemsRepository.GetFirst(x => x.PharmacyOrder_Id == item.StoreOrder_Id);

                var PharmacyPrescription = _PharmacyPrescriptionRepository.GetFirst(x => x.Id == updateStatus.PharmacyPrescription_Id);

                if (PharmacyPrescription != null)
                {
                    var PharmacyRequest = _PharmacyRequestRepository.GetFirst(x => x.Id == PharmacyPrescription.PharmacyRequest_Id);
                    if (PharmacyRequest != null)
                    {
                        PharmacyRequest.Status = (int)Utility.PharmacyRequestStatuses.Deliveryed;
                        _PharmacyRequestRepository.Save();
                    }
                }
                else
                {
                    return null;
                }
                foreach (var order in model.Orders)
                {
                    var MainOrder = _OrderRepository.GetFirst(x => x.Id == order.OrderId);

                    NotificationMessage = "Your order # " + order.OrderId + " status changed to " + Utility.GetPharmacyRequestStatusName(order.Status);

                    _NotificationRepository.Insert(new Notification
                    {
                        CreatedDateTime = DateTime.UtcNow,
                        User_Id = MainOrder.User_Id,
                        Status = (int)Utility.NotificationStatuses.Unread,
                        Text = NotificationMessage,
                        Title = "Order Status Changed"
                    });
                    _NotificationRepository.Save();

                    HostingEnvironment.QueueBackgroundWorkItem(cancellationToken =>
                    {
                        GlobalUtility.objPushNotifications.SendIOSPushNotification(_UserDeviceRepository.GetMany(x1 => x1.Platform == false && x1.User_Id == MainOrder.User_Id).ToList(), null, new Notification { Title = "Order Status Changed", Text = NotificationMessage });
                        GlobalUtility.objPushNotifications.SendAndroidPushNotification(_UserDeviceRepository.GetMany(x1 => x1.Platform == true && x1.User_Id == MainOrder.User_Id).ToList(), null, new Notification { Title = "Order Status Changed", Text = NotificationMessage });
                    });

                }

            }



            return "success";

        }

        public SearchSubAdminNotificationsViewModel GetMyNotifications(int Id, bool Unread)
        {
            return new SearchSubAdminNotificationsViewModel { Notifications = Unread ? _AdminSubAdminNotificationsRepository.GetMany(x => x.AdminId == Id && x.Status == 0).ToList() : _AdminSubAdminNotificationsRepository.GetMany(x => x.AdminId == Id).ToList() };
        }
        public List<AdminSubAdminNotifications> AddNewNotification(PubNubNotificationBindingModel model)
        {

            var User = _UserRepository.GetFirst(x => x.Id == model.User_Id);

            lock(obj)
            {
                var AlreadyNotify = _AdminSubAdminNotificationsRepository.GetFirst(x => x.PharmacyRequest_Id == model.PharmacyRequest_Id);


                if (AlreadyNotify == null)
                {

                    _AdminSubAdminNotificationsRepository.Insert(new AdminSubAdminNotifications
                    {

                        AdminId = model.Admin_Id,
                        CreatedDate = DateTime.UtcNow,
                        Status = (int)GlobalUtility.NotificationStatus.Unread,
                        Text = "New Request",
                        Title = "You have received a new request from " + User.FullName,
                        PharmacyRequest_Id = model.PharmacyRequest_Id
                    });

                    _AdminSubAdminNotificationsRepository.Save();
                }
            }
            return _AdminSubAdminNotificationsRepository.GetMany(x => x.AdminId == model.Admin_Id && x.Status == (int)NotificationStatuses.Unread && x.AdminId == model.Admin_Id);
        }
        public int MarkNotificationAsRead(int Id, int AdminId)
        {
            var AdminNotification = _AdminSubAdminNotificationsRepository.GetFirst(x => x.Id == Id && x.AdminId == AdminId);
            AdminNotification.Status = (int)NotificationStatuses.Read;
            _AdminSubAdminNotificationsRepository.Save();
            return _AdminSubAdminNotificationsRepository.CountWithCondition(x => x.AdminId == AdminId && x.Status == (int)NotificationStatuses.Unread);
        }
        public List<Pharmacy> GetTopPharmacies()
        {

            var TopPharmaciesRating = _PharmacyRepository.GetWithInclude(x => x.IsDeleted == false, "PharmacyRating").ToList();

            if (TopPharmaciesRating != null)
            {
                foreach (var pharmacyrating in TopPharmaciesRating)
                {
                    pharmacyrating.CalculateAverageRating();
                }
            }
            return TopPharmaciesRating.OrderByDescending(x => x.AverageRating).Take(5).ToList();

        }



    }
}

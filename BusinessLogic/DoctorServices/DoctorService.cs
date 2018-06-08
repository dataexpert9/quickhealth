using BusinessLogic.Components.Helpers;
using BusinessLogic.CustomAuthorization;
using BusinessLogic.HelperServices;
using DBAccess;
using DBAccess.GenericRepository;
using DBAccess.Models;
using DBAccess.ViewModels;
using DBAccess.ViewModels.Doctor;
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

namespace BusinessLogic.UserServices
{
    public class DoctorService : IDoctorService
    {
        private readonly AdminDBContext _DBContext = new AdminDBContext();
        private readonly GenericRepository<Doctor> _DoctorRepository;
        private readonly GenericRepository<ContactUs> _ContactUsRepository;
        private readonly GenericRepository<UserDevice> _UserDeviceRepository;
        private readonly GenericRepository<Appointment> _AppointmentRepository;
        private readonly GenericRepository<Admin> _AdminRepository;

        public DoctorService()
        {
            _DoctorRepository = new GenericRepository<Doctor>(_DBContext);
            _ContactUsRepository = new GenericRepository<ContactUs>(_DBContext);
            _UserDeviceRepository = new GenericRepository<UserDevice>(_DBContext);
            _AdminRepository = new GenericRepository<Admin>(_DBContext);
            _AppointmentRepository = new GenericRepository<Appointment>(_DBContext);
        }


        public Doctor RegisterAsDoctor(RegisterDoctorBindingModel model)
        {
            if (_DoctorRepository.Exists(x => x.Email == model.Email))
            {
                return null;
            }
            else
            {
                Doctor doctorModel;
                doctorModel = new Doctor
                {
                    FullName = model.FullName,
                    Phone = model.Phone,
                    Email = model.Email,
                    Password = CryptoHelper.Hash(model.Password),
                    Status = (int)GlobalUtility.StatusCode.NotVerified,
                    SignInType = (int)RoleTypes.Doctor,
                    IsNotificationsOn = true,
                    DateofBirth = model.DateOfBirth.ToShortDateString(),
                    ProfilePictureUrl = ImageHelper.SaveFileFromBytes(model.ProfilePictureUrl, "ProfileImages"),
                    SurName = model.SurName,
                    Country = model.Country,
                    City = model.City,
                    ProviderType = model.ProviderType,
                    Specialization = model.Specialization,
                    Department = model.Department,
                    LatestQualification = model.LatestQualification
                };

                if (model.EductionCertificate != null)
                {
                    foreach (var item in model.EductionCertificate)
                    {
                        doctorModel.DoctorDocuments.Add(new DoctorDocument()
                        {
                            DocumentType = (int)DoctorDocumentType.EductionCertificate,
                            FilePath = ImageHelper.SaveFileFromBytes(item, "DoctorDocuments"),
                            UploadDate = DateTime.Now
                        });
                    }
                }
                if (model.ProfessionalCertificate != null)
                {
                    foreach (var item in model.ProfessionalCertificate)
                    {
                        doctorModel.DoctorDocuments.Add(new DoctorDocument()
                        {
                            DocumentType = (int)DoctorDocumentType.ProfessionalCertificate,
                            FilePath = ImageHelper.SaveFileFromBytes(item, "DoctorDocuments"),
                            UploadDate = DateTime.Now
                        });
                    }
                }
                _DoctorRepository.Insert(doctorModel);
                _DoctorRepository.Save();
                SettingsModel.LoadSettings();
                doctorModel.AppSettings = new Settings { ContactNo = SettingsModel.ContactNo, AboutUs = SettingsModel.AboutUs, PrivacyPolicy = SettingsModel.PrivacyPolicy, TermsConditions = SettingsModel.TermsConditions, Tax = SettingsModel.Tax, Currency = SettingsModel.Currency };
                return doctorModel;
            }
        }


        ///* User Related Servies  */
        public Doctor ValidateDoctor(LoginBindingModel loginModel)
        {
            var hashPass = CryptoHelper.Hash(loginModel.Password);
            //return _UserRepository.GetWithInclude(x => x.Email == loginModel.Email && x.Password == hashPass, "UserAddresses", "PaymentCards").FirstOrDefault();
            return _DoctorRepository.GetWithInclude(x => x.Email == loginModel.Email && x.Password == hashPass).FirstOrDefault();

        }


        public Doctor UpdateAvailabilityStatus(int Doctor_Id, bool IsAvailable)
        {

            var Doctor = _DoctorRepository.GetFirst(x => x.Id == Doctor_Id);
            if (Doctor != null)
            {
                Doctor.IsAvailable = IsAvailable;
                _DoctorRepository.Save();
                return Doctor;
            }
            else
                return null;

        }


        public Doctor UpdateDoctorProfile(EditDoctorProfileBindingModel model)
        {
            var doctorModel = _DoctorRepository.GetFirst(x => x.Id == model.Id);

            if (doctorModel != null)
            {
                doctorModel.FullName = model.FullName;
                doctorModel.SurName = model.SurName;
                doctorModel.Country = model.Country;
                doctorModel.City = model.City;
                doctorModel.ProviderType = model.ProviderType;
                doctorModel.Specialization = model.Specialization;
                doctorModel.Department = model.Department;
                doctorModel.LatestQualification = model.LatestQualification;
                doctorModel.Bio = model.Bio;
                doctorModel.Address = model.Address;

                if (model.ProfilePicture != null)
                {
                    doctorModel.ProfilePictureUrl = ImageHelper.SaveFileFromBytes(model.ProfilePicture, "ProfileImages");
                }

                _DoctorRepository.Save();
                return doctorModel;
            }
            else
                return null;



        }



        public List<Appointment> GetRequestQueries()
        {

            var AppointmentList = _AppointmentRepository.GetWithIncludeList(x => x.Status == (int)Utility.AppointmentStatus.Pending, "User", "FamilyMember", "User.Medications").ToList();
            return AppointmentList;
        }

        public List<Appointment> GetHistory(int Doctor_Id)
        {

            return new List<Appointment>();
        }


        public Appointment AcceptAppointment(AcceptAppointmentBindingModel model)
        {

            var Appointment = _AppointmentRepository.GetFirst(x => x.Id == model.Appointment_Id);

            if (Appointment.Doctor_Id == null)
            {
                Appointment.Doctor_Id = model.Doctor_Id;
                _AppointmentRepository.Save();
                return Appointment;
            }
            else
                return null;


        }

        //public Admin ValidateAdmin(LoginBindingModel loginModel)
        //{
        //    var hashPass = CryptoHelper.Hash(loginModel.Password);
        //    return _AdminRepository.GetFirst(x => x.Email == loginModel.Email && x.Password == hashPass && x.IsDeleted == false);
        //}



        ///* User Related Servies  */
        //public User ValidateUser(LoginBindingModel loginModel)
        //{
        //    var hashPass = CryptoHelper.Hash(loginModel.Password);
        //    //return _UserRepository.GetWithInclude(x => x.Email == loginModel.Email && x.Password == hashPass, "UserAddresses", "PaymentCards").FirstOrDefault();
        //    return _UserRepository.GetWithInclude(x => x.Email == loginModel.Email && x.Password == hashPass).FirstOrDefault();

        //}
        //public User RegisterAsUser(RegisterUserBindingModel model)
        //{
        //    if (_UserRepository.Exists(x => x.Email == model.Email))
        //    {
        //        return null;
        //    }
        //    else
        //    {

        //        //User userModel;
        //        var imageUrl = "";
        //        if (model.UserImage != null)
        //        {
        //            imageUrl = ImageHelper.SaveFileFromBytes(model.UserImage, "ProfileImages");
        //        }

        //        User userModel = new User
        //        {
        //            FullName = model.FullName,
        //            Email = model.Email,
        //            Password = CryptoHelper.Hash(model.Password),
        //            Phone = model.PhoneNumber,
        //            Status = (int)GlobalUtility.StatusCode.NotVerified,
        //            SignInType = (int)RoleTypes.User,
        //            IsNotificationsOn = true,
        //            ProfilePictureUrl = imageUrl,
        //            PhoneConfirmed = false,
        //            EmailConfirmed = false
        //        };
        //        _UserRepository.Insert(userModel);
        //        _UserRepository.Save();
        //        SettingsModel.LoadSettings();
        //        userModel.AppSettings = new Settings { ContactNo = SettingsModel.ContactNo, AboutUs = SettingsModel.AboutUs, PrivacyPolicy = SettingsModel.PrivacyPolicy, TermsConditions = SettingsModel.TermsConditions, Tax = SettingsModel.Tax, Currency = SettingsModel.Currency };
        //        return userModel;
        //    }
        //}
        //public bool UserVerificationSMS(NexmoBindingModel model)
        //{
        //    var user = _UserRepository.GetFirst(x => x.Phone == model.PhoneNumber && x.Id == model.User_Id);
        //    if (user != null)
        //    {
        //        var nexmoVerifyResponse = NumberVerify.Verify(new NumberVerify.VerifyRequest { brand = "INGIC", number = user.Phone });

        //        if (nexmoVerifyResponse.status == "0")
        //        {
        //            return true;
        //        }
        //        else
        //        {
        //            return false;
        //        }
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}
        //public User UpdateNotificationStatus(bool Status, string Email)
        //{
        //    var user = _UserRepository.GetFirst(x => x.Email == Email && x.Status == (int)GlobalUtility.StatusCode.Verified);
        //    if (user != null)
        //    {
        //        user.IsNotificationsOn = Status;
        //        _UserRepository.Save();
        //        return user;
        //    }
        //    else
        //    {
        //        return null;
        //    }
        //}
        //public User SendVerificationSms(PhoneBindingModel model)
        //{
        //    var user = _UserRepository.GetFirst(x => x.Phone == model.PhoneNumber);
        //    if (user != null)
        //    {
        //        var codeInt = new Random().Next(111111, 999999);

        //        var results = SMS.Send(new SMS.SMSRequest
        //        {
        //            from = "Skribl",
        //            title = "Skribl",
        //            to = model.PhoneNumber,
        //            text = "Use this code to reset your password " + codeInt
        //        });
        //        if (results.messages.First().status == "0")
        //        {
        //            //user.ForgotPasswordTokens.Add(new ForgotPasswordToken { CreatedAt = DateTime.Now, IsDeleted = false, User_ID = user.Id, Code = Convert.ToString(codeInt) });
        //            _UserRepository.Update(user);
        //            _UserRepository.Save();
        //            return user;
        //        }
        //        else
        //        {
        //            return new User();
        //        }
        //    }
        //    else
        //    {
        //        return null;
        //    }
        //}
        //public User VerifyUserCode(int userId, int code)
        //{
        //    var user = _UserRepository.GetWithInclude(x => x.Id == userId, "ForgotPasswordTokens").FirstOrDefault();
        //    if (user != null && user.ForgotPasswordTokens.Count > 0)
        //    {
        //        var token = user.ForgotPasswordTokens.FirstOrDefault(x => x.Code == Convert.ToString(code) && x.IsDeleted == false && DateTime.Now.Subtract(x.CreatedAt).Minutes < 11);
        //        user.ForgotPasswordTokens = null;
        //        user.ForgotPasswordTokens.Add(token);
        //    }
        //    return user;
        //}
        ///* END User Related Services */




        ///*Other Services or not verified section */
        //public bool MarkDeviceAsInActive(int UserId, int DeviceId)
        //{
        //    var device = _UserDeviceRepository.GetFirst(x => x.Id == DeviceId && x.User_Id == UserId);
        //    if (device != null)
        //    {
        //        device.IsActive = false;
        //        _UserDeviceRepository.Update(device);
        //        _UserDeviceRepository.Save();
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}
        //public User ResetPasswordThroughEmail(string Email)
        //{
        //    var user = _UserRepository.GetFirst(x => x.Email == Email);
        //    if (user != null)
        //    {
        //        var codeInt = new Random().Next(111111, 999999);
        //        string subject = "Reset your password - " + EmailUtil.FromName;
        //        string body = "Your new password is :";
        //        body = body + " " + codeInt;
        //        user.Password = codeInt.ToString();
        //        EmailUtil.sendEmail(subject, body, Email);
        //        _UserRepository.Update(user);
        //        _UserRepository.Save();
        //    }
        //    return user;
        //}
        //public Admin CreateUpdateAdmin(Admin model)
        //{
        //    var admin = _AdminRepository.GetByID(model.Id);
        //    if (admin != null)
        //    {
        //        admin.FirstName = model.FirstName;
        //        admin.LastName = model.LastName;
        //        admin.Password = model.Password;
        //        admin.AccountNo = model.AccountNo;
        //        admin.Email = model.Email;
        //        admin.ImageUrl = model.ImageUrl;
        //        admin.IsDeleted = model.IsDeleted;
        //        admin.Phone = model.Phone;
        //        admin.Role = model.Role;
        //        admin.Status = model.Status;
        //        _AdminRepository.Update(admin);
        //        _AdminRepository.Save();
        //        return admin;
        //    }
        //    else
        //    {
        //        model.IsDeleted = false;
        //        _AdminRepository.Insert(model);
        //        _AdminRepository.Save();
        //        return model;
        //    }
        //}
        //public bool ChangeForgetPassword(SetForgotPasswordBindingModel model, string userEmail)
        //{
        //    var user = _UserRepository.GetFirst(x => x.Email == userEmail && x.IsDeleted == false);
        //    if (user != null)
        //    {
        //        user.Password = CryptoHelper.Hash(model.NewPassword);
        //        _UserRepository.Update(user);
        //        _UserRepository.Save();
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}
        //public bool ChangePassword(SetPasswordBindingModel model, string UserEmail)
        //{
        //    var hashedPassword = CryptoHelper.Hash(model.OldPassword);
        //    var user = _UserRepository.GetFirst(x => x.Email == UserEmail && x.Password == hashedPassword);
        //    if (user != null)
        //    {
        //        user.Password = CryptoHelper.Hash(model.NewPassword);
        //        _UserRepository.Update(user);
        //        _UserRepository.Save();
        //        return true;
        //    }
        //    else
        //        return false;
        //}
        //public bool MarkUserAccountAsVerified(UserModel model)
        //{
        //    var user = _UserRepository.GetFirst(x => x.Email == model.Email);
        //    if (user != null)
        //    {
        //        user.Status = (int)GlobalUtility.StatusCode.Verified;
        //        user.PhoneConfirmed = true;
        //        _UserRepository.Update(user);
        //        _UserRepository.Save();
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}
        //public List<Admin> GetAllAdmins()
        //{
        //    //  return _UserRepository.GetMany(x => x.SignInType == (int)RoleTypes.SubAdmin || x.SignInType == (int)RoleTypes.SuperAdmin);
        //    return _AdminRepository.GetMany(x => x.IsDeleted == false);
        //}
        //public void updateProfileImage(string email, string imageUrl)
        //{
        //    var user = _UserRepository.GetFirst(x => x.Email == email);
        //    if (user != null)
        //    {
        //        user.ProfilePictureUrl = imageUrl;
        //        _UserRepository.Update(user);
        //        _UserRepository.Save();
        //    }
        //}
        //public User ContactUs(int userId, string description)
        //{
        //    var user = _UserRepository.GetByID(userId);
        //    if (user != null)
        //    {
        //        _ContactUsRepository.Insert(new ContactUs { UserId = userId, Description = description });
        //        _ContactUsRepository.Save();
        //    }
        //    return user;
        //}
        //#region UserAddressRelatedAPIs
        ////public User AddUserAddress(AddUserAddressBindingModel model, ref bool AddressAlreadyExist)
        ////{
        ////    User user;
        ////    user = _UserRepository.GetWithInclude(x => x.Id == model.UserId, "UserAddresses", "PaymentCards").FirstOrDefault();
        ////    if (user != null)
        ////    {
        ////        if (!user.UserAddresses.Any(
        ////            x => x.Apartment == model.Apartment
        ////            && x.City == model.City
        ////            && x.Country == model.Country
        ////            && x.Floor == model.Floor
        ////            && x.NearestLandmark == model.NearestLandmark
        ////            && x.BuildingName == model.BuildingName
        ////            && x.StreetName == model.StreetName
        ////            && x.Type == model.AddressType
        ////            && x.IsDeleted == false)
        ////            )
        ////        {
        ////            foreach (var address in user.UserAddresses)
        ////                address.IsPrimary = false;
        ////            user.UserAddresses.Add(new UserAddress
        ////            {
        ////                Apartment = model.Apartment,
        ////                City = model.City,
        ////                Country = model.Country,
        ////                Floor = model.Floor,
        ////                NearestLandmark = model.NearestLandmark,
        ////                BuildingName = model.BuildingName,
        ////                StreetName = model.StreetName,
        ////                Type = model.AddressType,
        ////                IsPrimary = true
        ////            });
        ////            _UserRepository.Update(user);
        ////            _UserRepository.Save();
        ////        }
        ////        else
        ////        {
        ////            AddressAlreadyExist = true;
        ////        }
        ////    }
        ////    return user;
        ////} 

        ////public User EditUserAddress(EditUserAddressBindingModel model, ref bool AddressNotExist)
        ////{
        ////    User user;
        ////    user = _UserRepository.GetWithInclude(x => x.Id == model.UserId, "UserAddresses", "PaymentCards").FirstOrDefault();
        ////    if (user != null)
        ////    {
        ////        var address = user.UserAddresses.FirstOrDefault(
        ////                       x => x.Id == model.AddressId && x.IsDeleted == false
        ////                       );
        ////        if (address != null)
        ////        {
        ////            address.Apartment = model.Apartment;
        ////            address.City = model.City;
        ////            address.Country = model.Country;
        ////            address.Floor = model.Floor;
        ////            address.NearestLandmark = model.NearestLandmark;
        ////            address.BuildingName = model.BuildingName;
        ////            address.StreetName = model.StreetName;
        ////            address.Type = model.AddressType;
        ////            address.IsPrimary = model.IsPrimary;
        ////            _UserRepository.Update(user);
        ////            _UserRepository.Save();
        ////        }
        ////        else
        ////        {
        ////            AddressNotExist = true;
        ////        }
        ////    }
        ////    return user;
        ////}
        ////public User DeleteUserAddress(int UserId, int AddressId, ref bool AddressNotExist)
        ////{
        ////    User user;
        ////    user = _UserRepository.GetWithInclude(x => x.Id == UserId, "UserAddresses", "PaymentCards").FirstOrDefault();
        ////    if (user != null)
        ////    {
        ////        var address = user.UserAddresses.FirstOrDefault(
        ////                       x => x.Id == AddressId && x.IsDeleted == false
        ////                       );
        ////        if (address != null)
        ////        {
        ////            address.IsDeleted = true;
        ////            _UserRepository.Update(user);
        ////            _UserRepository.Save();
        ////        }
        ////        else
        ////        {
        ////            AddressNotExist = true;
        ////        }
        ////    }
        ////    return user;

        ////}
        //#endregion
        //#region DeletePaymentCard
        ////public User DeletePaymentCard(int UserId, int CardId, ref bool AddressNotExist)
        ////{
        ////    User user;
        ////    user = _UserRepository.GetWithInclude(x => x.Id == UserId, "PaymentCards").FirstOrDefault();
        ////    if (user != null)
        ////    {
        ////        var card = user.PaymentCards.FirstOrDefault(
        ////                       x => x.Id == CardId && x.IsDeleted == false
        ////                       );
        ////        if (card != null)
        ////        {
        ////            card.IsDeleted = true;
        ////            _UserRepository.Update(user);
        ////            _UserRepository.Save();
        ////        }
        ////        else
        ////        {
        ////            AddressNotExist = true;
        ////        }
        ////    }
        ////    return user;

        ////} 
        //#endregion
        //public User GetUserById(int userId)
        //{
        //    return _UserRepository.GetWithInclude(x => x.Id == userId && x.IsDeleted == false, "UserAddresses", "PaymentCards").FirstOrDefault();
        //}
        //public User UpdateUserProfileWithImage(EditUserProfileBindingModel model, HttpRequest httpRequest, HttpPostedFile postedFile)
        //{
        //    string newFullPath = string.Empty;
        //    string fileNameOnly = string.Empty;

        //    var userModel = _UserRepository.GetWithInclude(x => x.Email == model.Email, "UserAddresses", "PaymentCards").FirstOrDefault();
        //    //userModel.FirstName = model.FirstName;
        //    //userModel.LastName = model.LastName;
        //    userModel.SurName = model.SurName;
        //    userModel.Gender = model.Gender;
        //    //userModel.Bio = model.Bio;

        //    userModel.DateofBirth = model.DateofBirth;
        //    //userModel.PassportNo = model.PassportNo;
        //    //userModel.PassportCountryIssued = model.PassportCountryIssued;
        //    //userModel.PassportExpiryDate = model.PassportExpiryDate;
        //    userModel.Phone = model.PhoneNumber;

        //    if (httpRequest.Files.Count > 0)
        //    {
        //        var ext = Path.GetExtension(postedFile.FileName);
        //        ext = ext.ToLower();
        //        newFullPath = HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings["UserImageFolderPath"] + userModel.Id + ext);
        //        postedFile.SaveAs(newFullPath);
        //        userModel.ProfilePictureUrl = ConfigurationManager.AppSettings["UserImageFolderPath"] + userModel.Id + ext;
        //    }
        //    return userModel;

        //}
        //public bool CheckPhoneAlreadyRegister(string phoneNumber, string exceptUserEmail)
        //{
        //    return _UserRepository.GetFirst(x => x.Phone == phoneNumber && x.Email != exceptUserEmail) != null;
        //}
        /* END Other Services or not verified section */
    }
}

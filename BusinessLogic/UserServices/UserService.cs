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

namespace BusinessLogic.UserServices
{
    public class UserService : IUserService
    {
        private readonly AdminDBContext _DBContext = new AdminDBContext();
        private readonly GenericRepository<User> _UserRepository;
        private readonly GenericRepository<Doctor> _DoctorRepository;
        private readonly GenericRepository<ContactUs> _ContactUsRepository;
        private readonly GenericRepository<UserDevice> _UserDeviceRepository;
        private readonly GenericRepository<Admin> _AdminRepository;
        private readonly GenericRepository<FamilyHistory> _FamilyHistory;
        private readonly GenericRepository<FamilyMember> _FamilyMember;
        private readonly GenericRepository<Allergies> _Allergies;
        private readonly GenericRepository<LifeStyle> _LifeStyle;
        private readonly GenericRepository<MedicalConditions> _MedicalConditions;
        private readonly GenericRepository<Vaccinations> _Vaccinations;
        private readonly GenericRepository<Medications> _Medications;
        private readonly GenericRepository<Appointment> _Appointment;






        public UserService()
        {
            _UserRepository = new GenericRepository<User>(_DBContext);
            _ContactUsRepository = new GenericRepository<ContactUs>(_DBContext);
            _UserDeviceRepository = new GenericRepository<UserDevice>(_DBContext);
            _AdminRepository = new GenericRepository<Admin>(_DBContext);
            _DoctorRepository = new GenericRepository<Doctor>(_DBContext);
            _FamilyHistory = new GenericRepository<FamilyHistory>(_DBContext);
            _FamilyMember = new GenericRepository<FamilyMember>(_DBContext);
            _Allergies = new GenericRepository<Allergies>(_DBContext);
            _LifeStyle = new GenericRepository<LifeStyle>(_DBContext);
            _MedicalConditions = new GenericRepository<MedicalConditions>(_DBContext);
            _Vaccinations = new GenericRepository<Vaccinations>(_DBContext);
            _Medications = new GenericRepository<Medications>(_DBContext);
            _Appointment = new GenericRepository<Appointment>(_DBContext);





        }

        public Admin ValidateAdmin(LoginBindingModel loginModel)
        {
            var hashPass = CryptoHelper.Hash(loginModel.Password);
            return _AdminRepository.GetFirst(x => x.Email == loginModel.Email && x.Password == hashPass && x.IsDeleted == false);
        }



        /* User Related Servies  */
        public User ValidateUser(LoginBindingModel loginModel)
        {
            var hashPass = CryptoHelper.Hash(loginModel.Password);
            //return _UserRepository.GetWithInclude(x => x.Email == loginModel.Email && x.Password == hashPass, "UserAddresses", "PaymentCards").FirstOrDefault();
            return _UserRepository.GetWithInclude(x => x.Email == loginModel.Email && x.Password == hashPass, "FamilyMembers", "FamilyHistory", "MedicalConditions", "Allergies", "Vaccinations", "Medications", "LifeStyle").FirstOrDefault();

        }
        public User RegisterAsUser(RegisterUserBindingModel model)
        {
            if (_UserRepository.Exists(x => x.Email == model.Email))
            {
                return null;
            }
            else
            {

                //User userModel;
                var imageUrl = "";
                if (model.UserImage != null)
                {
                    imageUrl = ImageHelper.SaveFileFromBytes(model.UserImage, "ProfileImages");
                }
                
                User userModel = new User
                {
                    FullName = model.FullName,
                    SurName = model.Surname,
                    Email = model.Email,
                    Password = CryptoHelper.Hash(model.Password),
                    Phone = model.PhoneNumber,
                    Status = (int)GlobalUtility.StatusCode.Verified,
                    SignInType = (int)RoleTypes.User,
                    IsNotificationsOn = true,
                    ProfilePictureUrl = imageUrl,
                    PhoneConfirmed = true,
                    EmailConfirmed = true,
                    DateofBirth = model.DOB,
                    City = model.City,
                    Country = model.Country
                };
                _UserRepository.Insert(userModel);
                _UserRepository.Save();

                _UserRepository.Delete(userModel.Id);
                SettingsModel.LoadSettings();
                userModel.AppSettings = new Settings { ContactNo = SettingsModel.ContactNo, AboutUs = SettingsModel.AboutUs, PrivacyPolicy = SettingsModel.PrivacyPolicy, TermsConditions = SettingsModel.TermsConditions, Tax = SettingsModel.Tax, Currency = SettingsModel.Currency };
                return userModel;
            }
        }
        public bool UserVerificationSMS(NexmoBindingModel model)
        {
            var user = _UserRepository.GetFirst(x => x.Phone == model.PhoneNumber && x.Id == model.User_Id);
            if (user != null)
            {
                var nexmoVerifyResponse = NumberVerify.Verify(new NumberVerify.VerifyRequest { brand = "INGIC", number = user.Phone });

                if (nexmoVerifyResponse.status == "0")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        public User UpdateNotificationStatus(bool Status, string Email)
        {
            var user = _UserRepository.GetFirst(x => x.Email == Email && x.Status == (int)GlobalUtility.StatusCode.Verified);
            if (user != null)
            {
                user.IsNotificationsOn = Status;
                _UserRepository.Save();
                return user;
            }
            else
            {
                return null;
            }
        }
        public User SendVerificationSms(PhoneBindingModel model)
        {
            var user = _UserRepository.GetFirst(x => x.Phone == model.PhoneNumber);
            if (user != null)
            {
                var codeInt = new Random().Next(111111, 999999);

                var results = SMS.Send(new SMS.SMSRequest
                {
                    from = "Skribl",
                    title = "Skribl",
                    to = model.PhoneNumber,
                    text = "Use this code to reset your password " + codeInt
                });
                if (results.messages.First().status == "0")
                {
                    //user.ForgotPasswordTokens.Add(new ForgotPasswordToken { CreatedAt = DateTime.Now, IsDeleted = false, User_ID = user.Id, Code = Convert.ToString(codeInt) });
                    _UserRepository.Update(user);
                    _UserRepository.Save();
                    return user;
                }
                else
                {
                    return new User();
                }
            }
            else
            {
                return null;
            }
        }
        public User VerifyUserCode(int userId, int code)
        {
            var user = _UserRepository.GetWithInclude(x => x.Id == userId, "ForgotPasswordTokens").FirstOrDefault();
            if (user != null && user.ForgotPasswordTokens.Count > 0)
            {
                var token = user.ForgotPasswordTokens.FirstOrDefault(x => x.Code == Convert.ToString(code) && x.IsDeleted == false && DateTime.Now.Subtract(x.CreatedAt).Minutes < 11);
                user.ForgotPasswordTokens = null;
                user.ForgotPasswordTokens.Add(token);
            }
            return user;
        }
        /* END User Related Services */

        /*Dcotor Related Services */
        public User RegisterAsDoctor(RegisterDoctorBindingModel model)
        {
            if (_UserRepository.Exists(x => x.Email == model.Email))
            {
                return null;
            }
            else
            {
                User userModel;
                // change statuses later
                userModel = new User
                {
                    FullName = model.FullName,
                    Phone = model.Phone,
                    Email = model.Email,
                    Password = CryptoHelper.Hash(model.Password),
                    Status = (int)GlobalUtility.StatusCode.Verified,
                    SignInType = (int)RoleTypes.Doctor,
                    IsNotificationsOn = true,
                    DateofBirth = model.DateOfBirth,
                    ProfilePictureUrl = ImageHelper.SaveFileFromBytes(model.ProfilePictureUrl, "ProfileImages"),
                    SurName = model.SurName,
                    Country = model.Country,
                    City = model.City,
                    ProviderType = model.ProviderType
                };
                //if (model.Specialization != null)
                //{
                //    foreach (var item in model.Specialization)
                //    {
                //        userModel.DoctorDocuments.Add(new DoctorDocument()
                //        {
                //            DocumentType = (int)DoctorDocumentType.Specialization,
                //            FilePath = ImageHelper.SaveFileFromBytes(item, "DoctorDocuments"),
                //            UploadDate = DateTime.Now
                //        });
                //    }
                //}
                //if (model.Department != null)
                //{
                //    foreach (var item in model.Department)
                //    {
                //        userModel.DoctorDocuments.Add(new DoctorDocument()
                //        {
                //            DocumentType = (int)DoctorDocumentType.Department,
                //            FilePath = ImageHelper.SaveFileFromBytes(item, "DoctorDocuments"),
                //            UploadDate = DateTime.Now
                //        });
                //    }
                //}
                //if (model.LatestQualification != null)
                //{
                //    foreach (var item in model.LatestQualification)
                //    {
                //        userModel.DoctorDocuments.Add(new DoctorDocument()
                //        {
                //            DocumentType = (int)DoctorDocumentType.LatestQualification,
                //            FilePath = ImageHelper.SaveFileFromBytes(item, "DoctorDocuments"),
                //            UploadDate = DateTime.Now
                //        });
                //    }
                //}
                if (model.EductionCertificate != null)
                {
                    foreach (var item in model.EductionCertificate)
                    {
                        //userModel.DoctorDocuments.Add(new DoctorDocument()
                        //{
                        //    DocumentType = (int)DoctorDocumentType.EductionCertificate,
                        //    FilePath = ImageHelper.SaveFileFromBytes(item, "DoctorDocuments"),
                        //    UploadDate = DateTime.Now
                        //});
                    }
                }
                if (model.ProfessionalCertificate != null)
                {
                    foreach (var item in model.ProfessionalCertificate)
                    {
                        //userModel.DoctorDocuments.Add(new DoctorDocument()
                        //{
                        //    DocumentType = (int)DoctorDocumentType.ProfessionalCertificate,
                        //    FilePath = ImageHelper.SaveFileFromBytes(item, "DoctorDocuments"),
                        //    UploadDate = DateTime.Now
                        //});
                    }
                }
                _UserRepository.Insert(userModel);
                _UserRepository.Save();
                SettingsModel.LoadSettings();
                userModel.AppSettings = new Settings { ContactNo = SettingsModel.ContactNo, AboutUs = SettingsModel.AboutUs, PrivacyPolicy = SettingsModel.PrivacyPolicy, TermsConditions = SettingsModel.TermsConditions, Tax = SettingsModel.Tax, Currency = SettingsModel.Currency };
                return userModel;
            }
        }
        /* END Doctor Related Services */


        /*Other Services or not verified section */
        public bool MarkDeviceAsInActive(int UserId, int DeviceId)
        {
            var device = _UserDeviceRepository.GetFirst(x => x.Id == DeviceId && x.User_Id == UserId);
            if (device != null)
            {
                device.IsActive = false;
                _UserDeviceRepository.Update(device);
                _UserDeviceRepository.Save();
                return true;
            }
            else
            {
                return false;
            }
        }
        public User ResetPasswordThroughEmail(string Email)
        {
            var user = _UserRepository.GetFirst(x => x.Email == Email);

            if (user == null)
            {
                var doctor = _DoctorRepository.GetFirst(x => x.Email == Email);
                if (doctor == null)
                {
                    return null;
                }

                var codeInt = new Random().Next(111111, 999999);
                string subject = "Reset your password - " + EmailUtil.FromName;
                string body = "Your new password is :";
                body = body + " " + codeInt;

                var encryptedPass = CryptoHelper.Hash(codeInt.ToString());
                doctor.Password = encryptedPass;
                EmailUtil.sendEmail(subject, body, Email);
                _DoctorRepository.Update(doctor);
                _DoctorRepository.Save();
                return new User();
            }
            else
            {
                var codeInt = new Random().Next(111111, 999999);
                string subject = "Reset your password - " + EmailUtil.FromName;
                string body = "Your new password is :";
                body = body + " " + codeInt;

                var encryptedPass = CryptoHelper.Hash(codeInt.ToString());
                user.Password = encryptedPass;
                EmailUtil.sendEmail(subject, body, Email);
                _UserRepository.Update(user);
                _UserRepository.Save();
                return new User();
            }
            return null;
        }
        public Admin CreateUpdateAdmin(Admin model)
        {
            var admin = _AdminRepository.GetByID(model.Id);
            if (admin != null)
            {
                admin.FirstName = model.FirstName;
                admin.LastName = model.LastName;
                admin.Password = model.Password;
                admin.AccountNo = model.AccountNo;
                admin.Email = model.Email;
                admin.ImageUrl = model.ImageUrl;
                admin.IsDeleted = model.IsDeleted;
                admin.Phone = model.Phone;
                admin.Role = model.Role;
                admin.Status = model.Status;
                _AdminRepository.Update(admin);
                _AdminRepository.Save();
                return admin;
            }
            else
            {
                model.IsDeleted = false;
                _AdminRepository.Insert(model);
                _AdminRepository.Save();
                return model;
            }
        }
        public bool ChangeForgetPassword(SetForgotPasswordBindingModel model, string userEmail)
        {
            var user = _UserRepository.GetFirst(x => x.Email == userEmail && x.IsDeleted == false);
            if (user != null)
            {
                user.Password = CryptoHelper.Hash(model.NewPassword);
                _UserRepository.Update(user);
                _UserRepository.Save();
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool ChangePassword(SetPasswordBindingModel model, string UserEmail)
        {
            var hashedPassword = CryptoHelper.Hash(model.OldPassword);
            var user = _UserRepository.GetFirst(x => x.Email == UserEmail && x.Password == hashedPassword);
            if (user != null)
            {
                user.Password = CryptoHelper.Hash(model.NewPassword);
                _UserRepository.Update(user);
                _UserRepository.Save();
                return true;
            }
            else
                return false;
        }
        public bool MarkUserAccountAsVerified(UserModel model)
        {
            var user = _UserRepository.GetFirst(x => x.Email == model.Email);
            if (user != null)
            {
                user.Status = (int)GlobalUtility.StatusCode.Verified;
                user.PhoneConfirmed = true;
                _UserRepository.Update(user);
                _UserRepository.Save();
                return true;
            }
            else
            {
                return false;
            }
        }
        public List<Admin> GetAllAdmins()
        {
            //  return _UserRepository.GetMany(x => x.SignInType == (int)RoleTypes.SubAdmin || x.SignInType == (int)RoleTypes.SuperAdmin);
            return _AdminRepository.GetMany(x => x.IsDeleted == false);
        }
        public void updateProfileImage(string email, string imageUrl)
        {
            var user = _UserRepository.GetFirst(x => x.Email == email);
            if (user != null)
            {
                user.ProfilePictureUrl = imageUrl;
                _UserRepository.Update(user);
                _UserRepository.Save();
            }
        }
        public User ContactUs(int userId, string description)
        {
            var user = _UserRepository.GetByID(userId);
            if (user != null)
            {
                _ContactUsRepository.Insert(new ContactUs { UserId = userId, Description = description });
                _ContactUsRepository.Save();
            }
            return user;
        }
        #region UserAddressRelatedAPIs
        //public User AddUserAddress(AddUserAddressBindingModel model, ref bool AddressAlreadyExist)
        //{
        //    User user;
        //    user = _UserRepository.GetWithInclude(x => x.Id == model.UserId, "UserAddresses", "PaymentCards").FirstOrDefault();
        //    if (user != null)
        //    {
        //        if (!user.UserAddresses.Any(
        //            x => x.Apartment == model.Apartment
        //            && x.City == model.City
        //            && x.Country == model.Country
        //            && x.Floor == model.Floor
        //            && x.NearestLandmark == model.NearestLandmark
        //            && x.BuildingName == model.BuildingName
        //            && x.StreetName == model.StreetName
        //            && x.Type == model.AddressType
        //            && x.IsDeleted == false)
        //            )
        //        {
        //            foreach (var address in user.UserAddresses)
        //                address.IsPrimary = false;
        //            user.UserAddresses.Add(new UserAddress
        //            {
        //                Apartment = model.Apartment,
        //                City = model.City,
        //                Country = model.Country,
        //                Floor = model.Floor,
        //                NearestLandmark = model.NearestLandmark,
        //                BuildingName = model.BuildingName,
        //                StreetName = model.StreetName,
        //                Type = model.AddressType,
        //                IsPrimary = true
        //            });
        //            _UserRepository.Update(user);
        //            _UserRepository.Save();
        //        }
        //        else
        //        {
        //            AddressAlreadyExist = true;
        //        }
        //    }
        //    return user;
        //} 

        //public User EditUserAddress(EditUserAddressBindingModel model, ref bool AddressNotExist)
        //{
        //    User user;
        //    user = _UserRepository.GetWithInclude(x => x.Id == model.UserId, "UserAddresses", "PaymentCards").FirstOrDefault();
        //    if (user != null)
        //    {
        //        var address = user.UserAddresses.FirstOrDefault(
        //                       x => x.Id == model.AddressId && x.IsDeleted == false
        //                       );
        //        if (address != null)
        //        {
        //            address.Apartment = model.Apartment;
        //            address.City = model.City;
        //            address.Country = model.Country;
        //            address.Floor = model.Floor;
        //            address.NearestLandmark = model.NearestLandmark;
        //            address.BuildingName = model.BuildingName;
        //            address.StreetName = model.StreetName;
        //            address.Type = model.AddressType;
        //            address.IsPrimary = model.IsPrimary;
        //            _UserRepository.Update(user);
        //            _UserRepository.Save();
        //        }
        //        else
        //        {
        //            AddressNotExist = true;
        //        }
        //    }
        //    return user;
        //}
        //public User DeleteUserAddress(int UserId, int AddressId, ref bool AddressNotExist)
        //{
        //    User user;
        //    user = _UserRepository.GetWithInclude(x => x.Id == UserId, "UserAddresses", "PaymentCards").FirstOrDefault();
        //    if (user != null)
        //    {
        //        var address = user.UserAddresses.FirstOrDefault(
        //                       x => x.Id == AddressId && x.IsDeleted == false
        //                       );
        //        if (address != null)
        //        {
        //            address.IsDeleted = true;
        //            _UserRepository.Update(user);
        //            _UserRepository.Save();
        //        }
        //        else
        //        {
        //            AddressNotExist = true;
        //        }
        //    }
        //    return user;

        //}
        #endregion
        #region DeletePaymentCard
        //public User DeletePaymentCard(int UserId, int CardId, ref bool AddressNotExist)
        //{
        //    User user;
        //    user = _UserRepository.GetWithInclude(x => x.Id == UserId, "PaymentCards").FirstOrDefault();
        //    if (user != null)
        //    {
        //        var card = user.PaymentCards.FirstOrDefault(
        //                       x => x.Id == CardId && x.IsDeleted == false
        //                       );
        //        if (card != null)
        //        {
        //            card.IsDeleted = true;
        //            _UserRepository.Update(user);
        //            _UserRepository.Save();
        //        }
        //        else
        //        {
        //            AddressNotExist = true;
        //        }
        //    }
        //    return user;

        //} 
        #endregion
        public User GetUserById(int userId)
        {
            //return _UserRepository.GetWithInclude(x => x.Id == userId && x.IsDeleted == false, "UserAddresses", "PaymentCards").FirstOrDefault();
            return _UserRepository.GetWithInclude(x => x.Id == userId && x.IsDeleted == false).FirstOrDefault();

        }


        public User UpdateUserProfile(EditUserProfileBindingModel model)
        {
            string newFullPath = string.Empty;
            string fileNameOnly = string.Empty;

            //var userModel = _UserRepository.GetWithInclude(x => x.Email == model.Email, "UserAddresses", "PaymentCards").FirstOrDefault();
            var userModel = _UserRepository.GetWithInclude(x => x.Email == model.Email, "FamilyMembers", "FamilyHistory", "MedicalConditions", "Allergies", "Vaccinations", "Medications", "LifeStyle").FirstOrDefault();

            userModel.FullName = model.About.FullName;
            userModel.Gender = model.About.Gender;
            userModel.Location_Name = model.About.Location;
            userModel.Phone = model.About.PhoneNumber;
            userModel.DateofBirth = model.About.DOB;
            userModel.Address = model.About.Address;
            userModel.Weight = model.About.Weight;
            userModel.Height = model.About.Height;
            userModel.BMI = model.About.BMI;


            if (model.FamilyHistory.Count > 0)
            {
                List<FamilyHistory> FamilyHistories = new List<FamilyHistory>();
                if (userModel.FamilyHistory.Count != 0)
                {
                    _FamilyHistory.DeleteMany(userModel.FamilyHistory);
                }

                foreach (var relative in model.FamilyHistory)
                {
                    FamilyHistories.Add(new FamilyHistory
                    {
                        FamilyMember_Id = relative.FamilyMember_Id,
                        Reason = relative.Reason,
                        Relation = relative.Relation,
                        IsDeleted = false,
                        User_Id = model.User_Id
                    });
                }
                _FamilyHistory.InsertMany(FamilyHistories);
                _FamilyHistory.Save();

            }




            if (model.Allergies.Count > 0)
            {
                List<Allergies> Allergies = new List<Allergies>();
                if (userModel.Allergies.Count != 0)
                {
                    _Allergies.DeleteMany(userModel.Allergies);
                }
                foreach (var allergy in model.Allergies)
                {
                    Allergies.Add(new Allergies
                    {
                        AllergyName = allergy.AllergyName,
                        User_Id = model.User_Id,
                        IsDeleted = false

                    });
                }
                _Allergies.InsertMany(Allergies);
                _Allergies.Save();
            }


            if (model.LifeStyle != null)
            {
                LifeStyle LifeStyle = new LifeStyle();

                if (userModel.LifeStyle != null)
                {
                    _LifeStyle.Delete(userModel.LifeStyle);

                }

                LifeStyle.Alcohol = model.LifeStyle.Alcohol;
                LifeStyle.DietryRestrictions = model.LifeStyle.DietryRestrictions;
                LifeStyle.RecreationalDrugs = model.LifeStyle.RecreationalDrugs;
                LifeStyle.SexuallyActive = model.LifeStyle.SexuallyActive;
                LifeStyle.Smoking = model.LifeStyle.Smoking;
                LifeStyle.Id = model.User_Id;


                _LifeStyle.Insert(LifeStyle);
                _LifeStyle.Save();

            }



            if (model.MedicalConditions.Count > 0)
            {
                List<MedicalConditions> MedicalConditions = new List<MedicalConditions>();
                if (userModel.MedicalConditions.Count != 0)
                {
                    _MedicalConditions.DeleteMany(userModel.MedicalConditions);
                }

                foreach (var condition in model.MedicalConditions)
                {
                    MedicalConditions.Add(new MedicalConditions
                    {
                        Condition = condition.Condition,
                        User_Id = model.User_Id

                    });
                }
                _MedicalConditions.InsertMany(MedicalConditions);
                _MedicalConditions.Save();
            }



            if (model.Vaccinations.Count > 0)
            {
                List<Vaccinations> Vaccinations = new List<Vaccinations>();
                if (userModel.Vaccinations.Count != 0)
                {
                    _Vaccinations.DeleteMany(userModel.Vaccinations);
                }
                foreach (var vaccine in model.Vaccinations)
                {
                    Vaccinations.Add(new Vaccinations
                    {
                        Vaccination_Name = vaccine.Vaccination_Name,
                        User_Id = model.User_Id,
                        IsDeleted = false

                    });
                }
                _Vaccinations.InsertMany(Vaccinations);
                _Vaccinations.Save();
            }


            if (model.Medications.Count > 0)
            {
                List<Medications> Medications = new List<Medications>();
                if (userModel.Medications.Count != 0)
                {
                    _Medications.DeleteMany(userModel.Medications);
                }

                foreach (var medicine in model.Medications)
                {
                    Medications.Add(new Medications
                    {
                        Medicine_Name = medicine.Medicine_Name,
                        TimePeriod = medicine.TimePeriod,
                        User_Id=model.User_Id

                    });
                }
                _Medications.InsertMany(Medications);
                _Medications.Save();
            }


            if (model.ProfileImage != null)
            {
                userModel.ProfilePictureUrl = ImageHelper.SaveFileFromBytes(model.ProfileImage, "ProfileImages");
            }
            _UserRepository.Save();
            return userModel;

        }

        public User UpdateUserProfileWithImage(EditUserProfileBindingModel model, HttpRequest httpRequest, HttpPostedFile postedFile)
        {
            string newFullPath = string.Empty;
            string fileNameOnly = string.Empty;

            //var userModel = _UserRepository.GetWithInclude(x => x.Email == model.Email, "UserAddresses", "PaymentCards").FirstOrDefault();


            var userModel = _UserRepository.GetWithInclude(x => x.Email == model.Email).FirstOrDefault();

            userModel.FullName = model.About.FullName;
            userModel.Gender = model.About.Gender;
            userModel.Location_Name = model.About.Location;
            userModel.Phone = model.About.PhoneNumber;
            userModel.DateofBirth = model.About.DOB;
            userModel.Address = model.About.Address;
            userModel.Weight = model.About.Weight;
            userModel.Height = model.About.Height;
            userModel.BMI = model.About.BMI;

            if (httpRequest.Files.Count > 0)
            {
                var ext = Path.GetExtension(postedFile.FileName);
                ext = ext.ToLower();
                newFullPath = HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings["UserImageFolderPath"] + userModel.Id + ext);
                postedFile.SaveAs(newFullPath);
                userModel.ProfilePictureUrl = ConfigurationManager.AppSettings["UserImageFolderPath"] + userModel.Id + ext;
            }
            return userModel;

        }
        public bool CheckPhoneAlreadyRegister(string phoneNumber, string exceptUserEmail)
        {
            return _UserRepository.GetFirst(x => x.Phone == phoneNumber && x.Email != exceptUserEmail) != null;
        }

        /* END Other Services or not verified section */

        public FamilyMember AddNewFamilyMember(AddFamilyMemberBindingModel model)
        {


            string newFullPath = string.Empty;
            FamilyMember MemberToAdd = new FamilyMember();

            if (model.Id == 0)
                MemberToAdd = _FamilyMember.GetFirst(x => x.Name.Contains(model.FullName) && x.User_Id == model.User_Id);
            else
                MemberToAdd = _FamilyMember.GetFirst(x => x.Id == model.Id);

            if (model.Id == 0 && MemberToAdd != null)
            {
                return null;
            }
            if (MemberToAdd == null)
            {
                MemberToAdd = new FamilyMember();
            }


            MemberToAdd.Name = model.FullName;
            MemberToAdd.Age = model.Age;
            MemberToAdd.Relation = model.Relation;
            MemberToAdd.User_Id = model.User_Id;

            if (model.ImageUrl != null)
            {
                MemberToAdd.ImageUrl = ImageHelper.SaveFileFromBytes(model.ImageUrl, "FamilyMembers");
            }

            if (model.Id == 0)
                _FamilyMember.Insert(MemberToAdd);

            _FamilyMember.Save();

            return MemberToAdd;
        }

        public List<FamilyMember> GetFamilyMembers(int userId)
        {
            //return _UserRepository.GetWithInclude(x => x.Id == userId && x.IsDeleted == false, "UserAddresses", "PaymentCards").FirstOrDefault();
            return _FamilyMember.GetMany(x => x.User_Id == userId);

        }

        public bool DeleteFamilyMember(int FamilyMember_Id, int User_Id)
        {
            var FamilyMember = _FamilyMember.GetFirst(x => x.Id == FamilyMember_Id && x.User_Id == User_Id);
            if (FamilyMember == null)
            {
                return false;
            }

            _FamilyMember.Delete(FamilyMember);
            _FamilyMember.Save();
            return true;
        }


        public Appointment GetAppointment(AppointmentBindingModel model)
        {

            Appointment AppointmentModel;
            List<AppointmentImages> Images = new List<AppointmentImages>();


            //var UserModel = _UserRepository.GetFirstWithInclude(x=>x.Id==model.User_Id,"Appointment");

            //UserModel.Appointment.Add(new Appointment {
            //    AppointmentType = model.AppointmentType,
            //    Purpose = model.Purpose,
            //    IsFever = model.IsFever,
            //    Temperature = model.Temperature,
            //    Symptoms = model.Symptoms,
            //    Status = (int)Utility.AppointmentStatus.Pending,
            //    AppointmentDateTime = DateTime.UtcNow,
            //    FamilyMember_Id = model.FamilyMember_Id,
            //    User_Id = model.User_Id,
            //    Doctor_Id = 4
            //});

            //_UserRepository.Save();



            if (model.FamilyMember_Id != 0)
                AppointmentModel = _Appointment.GetFirst(x => x.User_Id == model.User_Id && x.FamilyMember_Id == model.FamilyMember_Id.Value && x.Status != (int)Utility.AppointmentStatus.Cancel);
            else
                AppointmentModel = _Appointment.GetFirst(x => x.User_Id == model.User_Id && x.Status != (int)Utility.AppointmentStatus.Cancel);



            if (AppointmentModel != null && AppointmentModel.AppointmentDateTime.Date == DateTime.Today)
                return null;
            else
            {



                AppointmentModel = new Appointment
                {
                    AppointmentType = model.AppointmentType,
                    Purpose = model.Purpose,
                    IsFever = model.IsFever,
                    Temperature = model.Temperature,
                    Allergies=model.Allergies,
                    MedicalConditions=model.MedicalCondition,
                    Status = (int)Utility.AppointmentStatus.Pending,
                    AppointmentDateTime = DateTime.UtcNow,
                    User_Id = model.User_Id
                };

                if (model.FamilyMember_Id != 0)
                    AppointmentModel.FamilyMember_Id = model.FamilyMember_Id;


                _Appointment.Insert(AppointmentModel);
                _Appointment.Save();

                foreach (var file in model.ImageUrls)
                {
                    if (AppointmentModel.AppointmentImages == null)
                    {
                        AppointmentModel.AppointmentImages = new List<AppointmentImages>();
                    }

                    if (file != null)
                    {
                        Images.Add(new AppointmentImages
                        {
                            Appointment_Id = AppointmentModel.Id,
                            ImageUrl = ImageHelper.SaveFileFromBytes(file, "AppointmentImages")
                        });
                    }
                    AppointmentModel.AppointmentImages.AddRange(Images);
                }
                _Appointment.Save();

                //AppointmentModel = _Appointment.GetFirstWithInclude(x => x.Id == AppointmentModel.Id, "Doctor", "User", "FamilyMember");
                return AppointmentModel;

            }
        }

        public Appointment CancelAppointment(CancelAppointmentBindingModel model)
        {
            var AppointmentModel = _Appointment.GetFirst(x => x.Id == model.Appointment_Id);

            if (AppointmentModel != null && AppointmentModel.Status == (int)Utility.AppointmentStatus.Pending)
            {
                AppointmentModel.Status = (int)Utility.AppointmentStatus.Cancel;
                _Appointment.Save();
                return AppointmentModel;
            }

            return null;

        }
        
        public List<Appointment> GetMyCases(int User_Id)
        {

            List<Appointment> returnModel = new List<Appointment>();

            using (AdminDBContext ctx = new AdminDBContext())
            {
                returnModel = _Appointment.GetWithInclude(x => x.User_Id == User_Id, "Doctor", "User", "User.Allergies", "User.MedicalConditions", "DoctorPrescription", "DoctorPrescription.DoctorPrescriptionImages").ToList();

                    //ctx.Appointment.Include(x => x.Doctor).Include(x => x.User).Include(x => x.User.Allergies).Include(x => x.User.MedicalConditions).Include(x=>x.).Where(x => x.User_Id == User_Id).ToList();
            }
            return returnModel;

        }


        public User GetUserProfile(int User_Id)
        {

            User returnModel = new User();

            using (AdminDBContext ctx = new AdminDBContext())
            {
                returnModel = _UserRepository.GetFirstWithInclude(x =>x.Id==User_Id && x.IsDeleted == false, "FamilyHistory", "MedicalConditions", "Allergies", "Vaccinations", "Medications", "LifeStyle", "FamilyMembers");

                //ctx.Appointment.Include(x => x.Doctor).Include(x => x.User).Include(x => x.User.Allergies).Include(x => x.User.MedicalConditions).Include(x=>x.).Where(x => x.User_Id == User_Id).ToList();
            }
            return returnModel;
        }

    }
}

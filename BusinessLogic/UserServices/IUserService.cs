using DBAccess.Models;
using DBAccess.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace BusinessLogic.UserServices
{
    public interface IUserService
    {
        User ValidateUser(LoginBindingModel loginModel);
        Admin ValidateAdmin(LoginBindingModel loginModel);

        bool ChangeForgetPassword(SetForgotPasswordBindingModel Model,string userEmail);
        bool ChangePassword(SetPasswordBindingModel model, string UserEmail);

        User RegisterAsUser(RegisterUserBindingModel model);
        User RegisterAsDoctor(RegisterDoctorBindingModel model);
        User UpdateNotificationStatus(bool Status,string Email);



        //User RegisterAsDoctor(NexmoBindingModel model);

        Admin CreateUpdateAdmin(Admin model);
        bool MarkUserAccountAsVerified(UserModel model);

        User SendVerificationSms(PhoneBindingModel model);
        bool UserVerificationSMS(NexmoBindingModel model);

        List<Admin> GetAllAdmins();

        User VerifyUserCode(int userId, int code);
        void updateProfileImage(string email, string imageUrl);
        User ContactUs(int userId, string description);
        //User AddUserAddress(AddUserAddressBindingModel model,ref bool AddressAlreadyExist);

        //User EditUserAddress(EditUserAddressBindingModel model, ref bool AddressNotExist);
        //User DeleteUserAddress(int UserId, int AddressId, ref bool AddressNotExist);
        //User DeletePaymentCard(int UserId, int CardId, ref bool AddressNotExist);
        User ResetPasswordThroughEmail(string Email);
        User UpdateUserProfileWithImage(EditUserProfileBindingModel model, HttpRequest request, HttpPostedFile postedFile);
        User UpdateUserProfile(EditUserProfileBindingModel model);


        User GetUserById(int userId);
        bool MarkDeviceAsInActive(int UserId, int DeviceId);
        bool CheckPhoneAlreadyRegister(string phoneNumber, string exceptUserEmail);
    }
}

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using AdminWebapi.Models;
using AdminWebapi.ViewModels;
using System.IO;
using System.Configuration;
using System.Data.Entity;
using System.Net.Mail;
using DBAccess;
using DBAccess.Models;
//using AdminWebapi.Components.Helpers;
using BusinessLogic.CustomAuthorization;
using BusinessLogic.SocialLogin;
using BusinessLogic.HelperServices;
using BusinessLogic.UserServices;
using DBAccess.ViewModels;
using BusinessLogic.CommonServices;
//using static BasketApi.Global;
//using BasketApi.Components.Helpers;

namespace AdminWebapi.Controllers
{
    [RoutePrefix("api/User")]
    [ExceptionHandlingFilter]
    public class UserController : ApiController
    {
        private ApplicationUserManager _userManager;
        private readonly IUserService _UserService;
        private readonly IDoctorService _DoctorService;
        private readonly IImageUpload _ImageUpload;
        public UserController(IUserService userService,IDoctorService doctorService, IImageUpload imageUpload)
        {
            _UserService = userService;
            _ImageUpload = imageUpload;
            _DoctorService = doctorService;
        }

        /// <summary>
        /// Login
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [Route("Login")]
        [HttpPost]
        public async Task<IHttpActionResult> Login(LoginBindingModel model)
        {
            //try
            //{
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            User userModel = _UserService.ValidateUser(model);
            if (userModel != null)
            {
                if (userModel.Status == (int)GlobalUtility.StatusCode.Verified)
                {
                    await userModel.GenerateToken(Request);
                    SettingsModel.LoadSettings();
                    userModel.AppSettings = new Settings { Id = SettingsModel.Id, ContactNo = SettingsModel.ContactNo, AboutUs = SettingsModel.AboutUs, PrivacyPolicy = SettingsModel.PrivacyPolicy, TermsConditions = SettingsModel.TermsConditions, Tax = SettingsModel.Tax, Currency = SettingsModel.Currency };
                    return Ok(new CustomResponse<User> { Message = GlobalUtility.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = userModel });
                }else
                {
                    return Content(HttpStatusCode.OK, new CustomResponse<Error>
                    {
                        Message = "Forbidden",
                        StatusCode = (int)HttpStatusCode.Forbidden,
                        Result = new Error { ErrorMessage = "Please verify your mobile number to proceed." }
                    });
                }
            }else
            {
                Doctor doctorModel = _DoctorService.ValidateDoctor(model);
                if (doctorModel != null)
                {
                    await doctorModel.GenerateToken(Request);
                    SettingsModel.LoadSettings();
                    doctorModel.AppSettings = new Settings { Id = SettingsModel.Id, ContactNo = SettingsModel.ContactNo, AboutUs = SettingsModel.AboutUs, PrivacyPolicy = SettingsModel.PrivacyPolicy, TermsConditions = SettingsModel.TermsConditions, Tax = SettingsModel.Tax, Currency = SettingsModel.Currency };
                    return Ok(new CustomResponse<Doctor> { Message = GlobalUtility.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = doctorModel });

                }else
                {
                    return Content(HttpStatusCode.OK, new CustomResponse<Error>
                    {
                        Message = "Forbidden",
                        StatusCode = (int)HttpStatusCode.Forbidden,
                        Result = new Error { ErrorMessage = "Invalid email or password." }
                    });
                }
            }

           
            //  }
            //}
            //catch (Exception ex)
            //{
            //    return StatusCode(Utility.LogError(ex));
            //}
        }

        /// <summary>
        /// Login for web admin panel
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        //[AllowAnonymous]
        //[Route("WebPanelLogin")]
        //[HttpPost]
        //public async Task<IHttpActionResult> WebPanelLogin(LoginBindingModel model)
        //{
        //    try
        //    {
        //        if (!ModelState.IsValid)
        //        {
        //            return BadRequest(ModelState);
        //        }
        //        Admin adminModel = _UserService.ValidateAdmin(model);
        //        if (adminModel != null)
        //        {
        //            await adminModel.GenerateToken(Request);
        //            CustomResponse<Admin> response = new CustomResponse<Admin> { Message = GlobalUtility.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = adminModel };
        //            return Ok(response);
        //        }
        //        else
        //            return Content(HttpStatusCode.OK, new CustomResponse<Error>
        //            {
        //                Message = "Forbidden",
        //                StatusCode = (int)HttpStatusCode.Forbidden,
        //                Result = new Error { ErrorMessage = "Invalid Email or Password" }
        //            });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(Utility.LogError(ex));
        //    }
        //}

        private int SavePicture(HttpRequestMessage request, out string PicturePath)
        {
            try
            {
                var httpRequest = HttpContext.Current.Request;
                PicturePath = String.Empty;

                if (httpRequest.Files.Count > 1)
                    return 3;

                foreach (string file in httpRequest.Files)
                {
                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created);

                    var postedFile = httpRequest.Files[file];
                    if (postedFile != null && postedFile.ContentLength > 0)
                    {
                        int MaxContentLength = 1024 * 1024 * 1; //Size = 1 MB  

                        IList<string> AllowedFileExtensions = new List<string> { ".jpg", ".gif", ".png" };
                        var ext = Path.GetExtension(postedFile.FileName);
                        var extension = ext.ToLower();
                        if (!AllowedFileExtensions.Contains(extension))
                        {
                            var message = string.Format("Please Upload image of type .jpg,.gif,.png.");
                            return 1;
                        }
                        else if (postedFile.ContentLength > MaxContentLength)
                        {

                            var message = string.Format("Please Upload a file upto 1 mb.");
                            return 2;
                        }
                        else
                        {
                            int count = 1;
                            string fileNameOnly = Path.GetFileNameWithoutExtension(postedFile.FileName);
                            string newFullPath = HttpContext.Current.Server.MapPath("~/App_Data/" + postedFile.FileName);

                            while (File.Exists(newFullPath))
                            {
                                string tempFileName = string.Format("{0}({1})", fileNameOnly, count++);
                                newFullPath = HttpContext.Current.Server.MapPath("~/App_Data/" + tempFileName + extension);
                            }

                            postedFile.SaveAs(newFullPath);
                            PicturePath = newFullPath;
                        }
                    }
                }
                return 0;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Logout
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [Route("Logout")]
        public IHttpActionResult Logout()
        {
            HttpContext.Current.GetOwinContext().Authentication.SignOut(OAuthDefaults.AuthenticationType);
            return Ok();
        }

        //[Authorize]
        //[Route("MarkVerified")]
        //[HttpPost]
        //public IHttpActionResult MarkUserAccountAsVerified(UserModel model)
        //{
        //    if (!ModelState.IsValid)
        //        return BadRequest(ModelState);

        //    var isDone = _UserService.MarkUserAccountAsVerified(model);

        //    if (!isDone)
        //    {
        //        return BadRequest("User account doesn't exist");
        //    }
        //    else
        //    {
        //        return Ok();
        //    }
        //}

        /// <summary>
        /// Send verification code to user
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>

        //[HttpPost]
        //[Route("SendVerificationSms")]
        //public IHttpActionResult SendVerificationSms(PhoneBindingModel model)
        //{
        //    if (!ModelState.IsValid)
        //        return BadRequest(ModelState);
        //    var user = _UserService.SendVerificationSms(model);
        //    if (user != null)
        //    {
        //        if (user.Id != 0)
        //        {
        //            return Ok(new CustomResponse<User> { Message = GlobalUtility.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = user });
        //        }
        //        else
        //        {
        //            return Content(HttpStatusCode.OK, new CustomResponse<Error> { Message = "InternalServerError", StatusCode = (int)HttpStatusCode.InternalServerError, Result = new Error { ErrorMessage = "SMS failed due to some reason." } });
        //        }
        //    }
        //    else
        //    {
        //        return Ok(new CustomResponse<Error> { Message = "NotFound", StatusCode = (int)HttpStatusCode.NotFound, Result = new Error { ErrorMessage = "User with entered phone number doesn’t exist." } });
        //    }
        //}
        /// <summary>
        /// Change Forget Password
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        //[Authorize]
        //[Route("ChangeForgotPassword")]
        //public async Task<IHttpActionResult> ChangeForgotPassword(SetForgotPasswordBindingModel model)
        //{
        //    var userEmail = User.Identity.Name;
        //    if (string.IsNullOrEmpty(userEmail))
        //    {
        //        throw new Exception("User Email is empty in user.identity.name.");
        //    }
        //    else if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    if (_UserService.ChangeForgetPassword(model, userEmail))
        //    {
        //        return Ok(new CustomResponse<string> { Message = GlobalUtility.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK });
        //    }
        //    else
        //    {
        //        return Ok(new CustomResponse<Error> { Message = "Forbidden", StatusCode = (int)HttpStatusCode.Forbidden, Result = new Error { ErrorMessage = "Invalid old password." } });
        //    }
        //}

        /// <summary>
        /// Verify code sent to user. 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>

        //[HttpPost]
        //[Route("VerifySmsCode")]
        //public IHttpActionResult VerifySmsCode(PhoneVerificationModel model)
        //{
        //    try
        //    {
        //        var userEmail = User.Identity.Name;

        //        if (string.IsNullOrEmpty(userEmail))
        //            throw new Exception("User Email is empty in user.identity.name.");
        //        if (!ModelState.IsValid)
        //            return BadRequest(ModelState);

        //        var nexmoCheckResponse = NumberVerify.Check(new NumberVerify.CheckRequest { request_id = model.request_id, code = model.Code });

        //        if (nexmoCheckResponse.status == "0")
        //        {
        //            //using (BasketContext ctx = new BasketContext())
        //            //{
        //            //    ctx.Users.FirstOrDefault(x => x.Email == userEmail).Status = (int)Global.StatusCode.Verified;
        //            //    ctx.SaveChanges();
        //            //}
        //            return Content(HttpStatusCode.OK, new MessageViewModel { Details = "Account Verified Successfully." });
        //        }
        //        else
        //            return Content(HttpStatusCode.OK, new CustomResponse<NumberVerify.CheckResponse> { Message = "InternalServerError", StatusCode = (int)HttpStatusCode.InternalServerError, Result = nexmoCheckResponse });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(Utility.LogError(ex));
        //    }
        //}

        //[AllowAnonymous]
        //[HttpGet]
        //[Route("VerifyUserCode")]
        //public async Task<IHttpActionResult> VerifyUserCode(int userId, int code)
        //{
        //    var user = _UserService.VerifyUserCode(userId, code);
        //    if (user == null)
        //        return Ok(new CustomResponse<Error> { Message = GlobalUtility.ResponseMessages.BadRequest, StatusCode = (int)HttpStatusCode.BadRequest, Result = new Error { ErrorMessage = "Invalid UserId" } });
        //    if (user.ForgotPasswordTokens.Count > 0)
        //    {
        //        await user.GenerateToken(Request);
        //        return Ok(new CustomResponse<User> { Message = GlobalUtility.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = user });
        //    }
        //    else
        //        return Ok(new CustomResponse<Error> { Message = GlobalUtility.ResponseMessages.BadRequest, StatusCode = (int)HttpStatusCode.BadRequest, Result = new Error { ErrorMessage = "Invalid code" } });
        //}

        [Route("RegisterAsUser")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<IHttpActionResult> RegisterAsUser(RegisterUserBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userModel = _UserService.RegisterAsUser(model);
            if (userModel == null)
            {
                return Content(HttpStatusCode.OK, new CustomResponse<Error>
                {
                    Message = "Conflict",
                    StatusCode = (int)HttpStatusCode.Conflict,
                    Result = new Error { ErrorMessage = "User with entered email already exists." }
                });
            }
            else
            {
                NexmoBindingModel verificationMessageModel = new NexmoBindingModel();

                //var nexmoResponse = _UserService.UserVerificationSMS(new NexmoBindingModel {PhoneNumber=userModel.Phone,User_Id=userModel.Id });

                //if(!nexmoResponse)
                //{
                //    return Content(HttpStatusCode.OK, new CustomResponse<Error>
                //    {
                //        Message = "Conflict",
                //        StatusCode = (int)HttpStatusCode.Conflict,
                //        Result = new Error { ErrorMessage = "Verification SMS failed due to some reason." }

                //    });
                //}

                Random _rdm = new Random();
                userModel.VerificationCode = Convert.ToString(_rdm.Next(1000, 9999));
                await userModel.GenerateToken(Request);
                CustomResponse<User> response = new CustomResponse<User> { Message = GlobalUtility.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = userModel };
                return Ok(response);
            }
        }

        [Route("RegisterAsDoctor")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<IHttpActionResult> RegisterAsDoctor(RegisterDoctorBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var doctor = _DoctorService.RegisterAsDoctor(model);
            if (doctor == null)
            {
                return Content(HttpStatusCode.OK, new CustomResponse<Error>
                {
                    Message = "Conflict",
                    StatusCode = (int)HttpStatusCode.Conflict,
                    Result = new Error { ErrorMessage = "User with entered email already exists." }
                });
            }
            else
            {
                await doctor.GenerateToken(Request);
                CustomResponse<Doctor> response = new CustomResponse<Doctor> { Message = GlobalUtility.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = doctor };
                return Ok(response);
            }
        }




        [Route("UpdateUserProfile")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<IHttpActionResult> UpdateUserProfile(EditUserProfileBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userModel = _UserService.UpdateUserProfile(model);
            if (userModel == null)
            {
                return Content(HttpStatusCode.OK, new CustomResponse<Error>
                {
                    Message = "Conflict",
                    StatusCode = (int)HttpStatusCode.Conflict,
                    Result = new Error { ErrorMessage = "User with entered email already exists." }
                });
            }
            else
            {
                await userModel.GenerateToken(Request);
                CustomResponse<User> response = new CustomResponse<User> { Message = GlobalUtility.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = userModel };
                return Ok(response);
            }
        }



        //[Route("RegisterAsAdmin")]
        //[HttpPost]
        //[Authorize]
        //public IHttpActionResult RegisterAsAdmin(Admin model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }
        //    if(model.Base64EncodedString != "")
        //    model.ImageUrl = _ImageUpload.uploadImage(model.Base64EncodedString, "ProfileImages");
        //    var admin = _UserService.CreateUpdateAdmin(model);
        //    if (admin == null)
        //    {
        //        return Content(HttpStatusCode.OK, new CustomResponse<Error>
        //        {
        //            Message = "Conflict",
        //            StatusCode = (int)HttpStatusCode.Conflict,
        //            Result = new Error { ErrorMessage = "User with entered email already exists." }
        //        });
        //    }
        //    else
        //    {
        //        CustomResponse<Admin> response = new CustomResponse<Admin> { Message = GlobalUtility.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = admin };
        //        return Ok(response);
        //    }
        //}

        [Route("UploadUserImage")]
        [Authorize]
        public async Task<IHttpActionResult> UploadUserImage()
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
                var httpRequest = HttpContext.Current.Request;
                string newFullPath = string.Empty;
                string fileNameOnly = string.Empty;

                #region Validations
                var userEmail = User.Identity.Name;
                if (string.IsNullOrEmpty(userEmail))
                {
                    throw new Exception("User Email is empty in user.identity.name.");
                }
                else if (!Request.Content.IsMimeMultipartContent())
                {
                    return Content(HttpStatusCode.OK, new CustomResponse<Error>
                    {
                        Message = "UnsupportedMediaType",
                        StatusCode = (int)HttpStatusCode.UnsupportedMediaType,
                        Result = new Error { ErrorMessage = "Multipart data is not included in request." }
                    });
                }
                else if (httpRequest.Files.Count == 0)
                {
                    return Content(HttpStatusCode.OK, new CustomResponse<Error>
                    {
                        Message = "NotFound",
                        StatusCode = (int)HttpStatusCode.NotFound,
                        Result = new Error { ErrorMessage = "Image not found, please upload an image." }
                    });
                }
                else if (httpRequest.Files.Count > 1)
                {
                    return Content(HttpStatusCode.OK, new CustomResponse<Error>
                    {
                        Message = "UnsupportedMediaType",
                        StatusCode = (int)HttpStatusCode.UnsupportedMediaType,
                        Result = new Error { ErrorMessage = "Multiple images are not allowed. Please upload 1 image." }
                    });
                }
                #endregion

                var postedFile = httpRequest.Files[0];

                if (postedFile != null && postedFile.ContentLength > 0)
                {

                    int MaxContentLength = 1024 * 1024 * 10; //Size = 1 MB  

                    IList<string> AllowedFileExtensions = new List<string> { ".jpg", ".gif", ".png" };
                    var ext = postedFile.FileName.Substring(postedFile.FileName.LastIndexOf('.'));
                    var extension = ext.ToLower();
                    if (!AllowedFileExtensions.Contains(extension))
                    {
                        return Content(HttpStatusCode.OK, new CustomResponse<Error>
                        {
                            Message = "UnsupportedMediaType",
                            StatusCode = (int)HttpStatusCode.UnsupportedMediaType,
                            Result = new Error { ErrorMessage = "Please Upload image of type .jpg, .gif, .png." }
                        });
                    }
                    else if (postedFile.ContentLength > MaxContentLength)
                    {

                        return Content(HttpStatusCode.OK, new CustomResponse<Error>
                        {
                            Message = "UnsupportedMediaType",
                            StatusCode = (int)HttpStatusCode.UnsupportedMediaType,
                            Result = new Error { ErrorMessage = "Please Upload a file upto 1 mb." }
                        });
                    }
                    else
                    {
                        int count = 1;
                        fileNameOnly = Path.GetFileNameWithoutExtension(postedFile.FileName);
                        newFullPath = HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings["UserImageFolderPath"] + postedFile.FileName);

                        while (File.Exists(newFullPath))
                        {
                            string tempFileName = string.Format("{0}({1})", fileNameOnly, count++);
                            newFullPath = HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings["UserImageFolderPath"] + tempFileName + extension);
                        }
                        postedFile.SaveAs(newFullPath);
                    }
                }

                MessageViewModel successResponse = new MessageViewModel { StatusCode = "200 OK", Details = "Image Updated Successfully." };
                var filePath = Utility.BaseUrl + ConfigurationManager.AppSettings["UserImageFolderPath"] + Path.GetFileName(newFullPath);
                ImagePathViewModel model = new ImagePathViewModel { Path = filePath };
                
                _UserService.updateProfileImage(userEmail, filePath);
                return Content(HttpStatusCode.OK, model);
        }

        /// <summary>
        /// Change user password
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize]
        [Route("ChangePassword")]
        public async Task<IHttpActionResult> ChangePassword(SetPasswordBindingModel model)
        {
                var userEmail = User.Identity.Name;
                if (string.IsNullOrEmpty(userEmail))
                {
                    throw new Exception("User Email is empty in user.identity.name.");
                }
                else if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                if (_UserService.ChangePassword(model,userEmail))
                {
                    return Ok(new CustomResponse<string> { Message = GlobalUtility.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK });
                }
                else
                    return Ok(new CustomResponse<Error> { Message = "Forbidden", StatusCode = (int)HttpStatusCode.Forbidden, Result = new Error { ErrorMessage = "Invalid old password." } });
        }

        [HttpGet]
        [Route("UpdateNotificationStatus")]
        public async Task<IHttpActionResult> UpdateNotificationStatus(bool Status,string Email)
        {
            var user = _UserService.UpdateNotificationStatus(Status, Email);
            if (user != null)
            {
                return Ok(new CustomResponse<User> { Message = GlobalUtility.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = user });
            }
            else
            {
                return Ok(new CustomResponse<Error> { Message = "NotFound", StatusCode = (int)HttpStatusCode.NotFound, Result = new Error { ErrorMessage = "User with entered email doesn’t exist." } });
            }
        }


        //[Route("AddExternalLogin")]
        //public async Task<IHttpActionResult> AddExternalLogin(AddExternalLoginBindingModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);

        //    AuthenticationTicket ticket = AccessTokenFormat.Unprotect(model.ExternalAccessToken);

        //    if (ticket == null || ticket.Identity == null || (ticket.Properties != null
        //        && ticket.Properties.ExpiresUtc.HasValue
        //        && ticket.Properties.ExpiresUtc.Value < DateTimeOffset.UtcNow))
        //    {
        //        return BadRequest("External login failure.");
        //    }

        //    ExternalLoginData externalData = ExternalLoginData.FromIdentity(ticket.Identity);

        //    if (externalData == null)
        //    {
        //        return BadRequest("The external login is already associated with an account.");
        //    }

        //    IdentityResult result = await UserManager.AddLoginAsync(User.Identity.GetUserId(),
        //        new UserLoginInfo(externalData.LoginProvider, externalData.ProviderKey));

        //    if (!result.Succeeded)
        //    {
        //        //return GetErrorResult(result);
        //    }

        //    return Ok();
        //}


        /// <summary>
        /// Update user profile with image. This is multipart request. SignInType 0 for user, 1 for deliverer
        /// </summary>
        /// <returns></returns>
        //[Authorize]
        //[Route("UpdateUserProfile")]
        //public async Task<IHttpActionResult> UpdateUserProfileWithImage()
        //{
        //    var userId = Convert.ToInt32(User.GetClaimValue("userid"));
        //    var httpRequest = HttpContext.Current.Request;
        //    #region InitializingModel
        //    EditUserProfileBindingModel model = new EditUserProfileBindingModel();
        //    model.FirstName = httpRequest.Params["FirstName"];
        //    model.LastName = httpRequest.Params["LastName"];
        //    model.SurName = httpRequest.Params["SurName"];
        //    model.Bio = httpRequest.Params["Bio"];
        //    model.Airline_Id = Convert.ToInt16(httpRequest.Params["Airline_Id"]);
        //    model.JobTitle = httpRequest.Params["JobTitle"];
        //    model.Grade = httpRequest.Params["Grade"];
        //    model.AircraftTrainedFor = httpRequest.Params["AircraftTrainedFor"];
        //    model.Base = httpRequest.Params["Base"];
        //    model.DateofBirth = httpRequest.Params["DateofBirth"];
        //    model.Language_Id = Convert.ToInt16(httpRequest.Params["Language_Id"]);
        //    model.PassportNo = httpRequest.Params["PassportNo"];
        //    model.PassportCountryIssued = httpRequest.Params["CountryIssued"];
        //    model.ExpiryDate = httpRequest.Params["ExpiryDate"];
        //    model.Email = httpRequest.Params["Email"];
        //    model.PhoneNumber = httpRequest.Params["PhoneNumber"];
        //    model.Gender = Convert.ToInt32(httpRequest.Params["Gender"]);
        //    model.JobTitle = httpRequest.Params["JobTitle"];

        //    #endregion
        //    Validate(model);

        //    #region Validations
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    if (!Request.Content.IsMimeMultipartContent())
        //    {
        //        return Content(HttpStatusCode.OK, new CustomResponse<Error>
        //        {
        //            Message = "UnsupportedMediaType",
        //            StatusCode = (int)HttpStatusCode.UnsupportedMediaType,
        //            Result = new Error { ErrorMessage = "Multipart data is not included in request." }
        //        });
        //    }
        //    else if (httpRequest.Files.Count > 1)
        //    {
        //        return Content(HttpStatusCode.OK, new CustomResponse<Error>
        //        {
        //            Message = "UnsupportedMediaType",
        //            StatusCode = (int)HttpStatusCode.UnsupportedMediaType,
        //            Result = new Error { ErrorMessage = "Multiple images are not supported, please upload one image." }
        //        });
        //    }
        //    #endregion

        //    HttpPostedFile postedFile = null;
        //    string fileExtension = string.Empty;

        //    #region ImageSaving
        //    if (httpRequest.Files.Count > 0)
        //    {
        //        postedFile = httpRequest.Files[0];
        //        if (postedFile != null && postedFile.ContentLength > 0)
        //        {
        //            IList<string> AllowedFileExtensions = new List<string> { ".jpg", ".gif", ".png" };
        //            var ext = Path.GetExtension(postedFile.FileName);
        //            fileExtension = ext.ToLower();
        //            if (!AllowedFileExtensions.Contains(fileExtension))
        //            {
        //                return Content(HttpStatusCode.OK, new CustomResponse<Error>
        //                {
        //                    Message = "UnsupportedMediaType",
        //                    StatusCode = (int)HttpStatusCode.UnsupportedMediaType,
        //                    Result = new Error { ErrorMessage = "Please Upload image of type .jpg,.gif,.png." }
        //                });
        //            }
        //            else if (postedFile.ContentLength > GlobalUtility.MaximumImageSize)
        //            {
        //                return Content(HttpStatusCode.OK, new CustomResponse<Error>
        //                {
        //                    Message = "UnsupportedMediaType",
        //                    StatusCode = (int)HttpStatusCode.UnsupportedMediaType,
        //                    Result = new Error { ErrorMessage = "Please Upload a file upto " + GlobalUtility.ImageSize + "." }
        //                });
        //            }
        //        }
        //    }
        //    #endregion

        //    if (_UserService.CheckPhoneAlreadyRegister(model.PhoneNumber, model.Email))
        //    {
        //        return Ok(new CustomResponse<Error>
        //        {
        //            Message = "Conflict",
        //            StatusCode = (int)HttpStatusCode.Conflict,
        //            Result = new Error { ErrorMessage = "User with entered phone number already exists." }
        //        });
        //    }

        //    User userModel = _UserService.UpdateUserProfileWithImage(model, httpRequest, postedFile);
        //    if (userModel == null)
        //    {
        //        return Content(HttpStatusCode.OK, new CustomResponse<Error>
        //        {
        //            Message = "NotFound",
        //            StatusCode = (int)HttpStatusCode.NotFound,
        //            Result = new Error { ErrorMessage = "UserId does not exist." }
        //        });
        //    }
        //    else
        //    {
        //        await userModel.GenerateToken(Request);
        //        SettingsModel.LoadSettings();
        //        userModel.AppSettings = new Settings { ContactNo = SettingsModel.ContactNo, AboutUs = SettingsModel.AboutUs, PrivacyPolicy = SettingsModel.PrivacyPolicy, TermsConditions = SettingsModel.TermsConditions, Tax = SettingsModel.Tax, Currency = SettingsModel.Currency };
        //        CustomResponse<User> response = new CustomResponse<User> { Message = GlobalUtility.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = userModel };
        //        return Ok(response);
        //    }
        //}


        /// <summary>
        /// Get All Admins
        /// </summary>
        /// <returns></returns>
        //[Route("GetAllAdmin")]
        //[Authorize]
        //[HttpGet]
        //public async Task<IHttpActionResult> GetAllAdmins()
        //{
        //    var users = _UserService.GetAllAdmins();
        //    CustomResponse<IEnumerable<Admin>> response = new CustomResponse<IEnumerable<Admin>>
        //    {
        //        Message = GlobalUtility.ResponseMessages.Success,
        //        StatusCode = (int)HttpStatusCode.OK,
        //        Result = users
        //    };

        //    return Ok(response);
        //}


        //[AllowAnonymous]
        //[HttpGet]
        //[Route("ExternalLogin")]
        //public async Task<IHttpActionResult> ExternalLogin(string accessToken, int? socialLoginType)
        //{
        //        if (socialLoginType.HasValue && !string.IsNullOrEmpty(accessToken))
        //        {
        //            SocialLogins socialLogin = new SocialLogins();
        //            // send access token and social login type to GetSocialUserData in return it will give you full name email and profile picture of user 
        //            var socialUser = await socialLogin.GetSocialUserData(accessToken, (SocialLogins.SocialLoginType)socialLoginType.Value);

        //            if (socialUser != null)
        //            {
        //                using (AdminDBContext ctx = new AdminDBContext())
        //                {

        //                    // if user have privacy on his / her email then we will create email address from his user Id which will be send by mobile developer 
        //                    if (string.IsNullOrEmpty(socialUser.email))
        //                    {
        //                        socialUser.email = socialUser.id + "@gmail.com";
        //                    }
        //                    var existingUser = ctx.Users.Include(x => x.UserAddresses).Include(x => x.PaymentCards).FirstOrDefault(x => x.Email == socialUser.email);

        //                    if (existingUser != null)
        //                    {
        //                        // if user already have registered through social login them wee will always check his picture and name just to get updated values of that user from facebook 
        //                        existingUser.ProfilePictureUrl = socialUser.picture;
        //                        ctx.SaveChanges();
        //                        await existingUser.GenerateToken(Request);
        //                        SettingsModel.LoadSettings();
        //                        existingUser.AppSettings = new Settings
        //                        {
        //                            Currency = SettingsModel.Currency,
        //                            Tax = SettingsModel.Tax,
        //                            AboutUs = SettingsModel.AboutUs,
        //                            TermsConditions = SettingsModel.TermsConditions,
        //                            PrivacyPolicy = SettingsModel.PrivacyPolicy
        //                        };
        //                        CustomResponse<User> response = new CustomResponse<User> { Message = GlobalUtility.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = existingUser };
        //                        return Ok(response);
        //                    }
        //                    else
        //                    {
        //                        int SignInType = 0;
        //                        if (socialLoginType.Value == (int)SocialLogins.SocialLoginType.Google)
        //                        {
        //                            SignInType = (int)Utility.SocialLoginType.Google;
        //                        }
        //                        else if (socialLoginType.Value == (int)SocialLogins.SocialLoginType.Facebook)
        //                            SignInType = (int)Utility.SocialLoginType.Facebook;


        //                        var newUser = new User { FirstName = socialUser.name, Email = socialUser.email, ProfilePictureUrl = socialUser.picture, SignInType = SignInType, Status = 1 };
        //                        ctx.Users.Add(newUser);
        //                        ctx.SaveChanges();
        //                        await newUser.GenerateToken(Request);
        //                        SettingsModel.LoadSettings();
        //                        newUser.AppSettings = new Settings
        //                        {
        //                            Currency = SettingsModel.Currency,
        //                            Tax = SettingsModel.Tax,
        //                            AboutUs = SettingsModel.AboutUs,
        //                            TermsConditions = SettingsModel.TermsConditions,
        //                            PrivacyPolicy = SettingsModel.PrivacyPolicy
        //                        };
        //                        return Ok(new CustomResponse<User> { Message = GlobalUtility.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = newUser });
        //                    }
        //                }
        //            }
        //            else
        //                return BadRequest("Unable to get user info");
        //        }
        //        else
        //            return BadRequest("Please provide access token along with social login type");

        //}

        /// <summary>
        /// Contact us
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        //[HttpGet]
        //[Route("ContactUs")]
        //[Authorize]
        //public async Task<IHttpActionResult> ContactUs(string Description)
        //{
        //    if (String.IsNullOrWhiteSpace(Description))
        //    {
        //        return Ok(new CustomResponse<Error> { Message = GlobalUtility.ResponseMessages.BadRequest, StatusCode = (int)HttpStatusCode.BadRequest, Result = new Error { ErrorMessage = GlobalUtility.ResponseMessages.CannotBeEmpty("Description") } });
        //    }
        //    else
        //    {
        //        var userId = Convert.ToInt32(User.GetClaimValue("userid"));
        //        var user = _UserService.ContactUs(userId, Description);
        //        if (user == null)
        //        {
        //            return Ok(new CustomResponse<Error> { Message = GlobalUtility.ResponseMessages.NotFound, StatusCode = (int)HttpStatusCode.BadRequest, Result = new Error { ErrorMessage = "UserId not found." } });
        //        }
        //        else
        //        {
        //            return Ok(new CustomResponse<string> { Message = GlobalUtility.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK });
        //        }
        //    }
        //}


        /// <summary>
        /// Add user address
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>

        //[Authorize]
        //[HttpPost]
        //[Route("AddUserAddress")]
        //public async Task<IHttpActionResult> AddUserAddress(AddUserAddressBindingModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }
        //    bool addressAlreadyExist = false;
        //    var user = _UserService.AddUserAddress(model, ref addressAlreadyExist);
        //    if (user != null)
        //    {
        //        if (addressAlreadyExist)
        //        {
        //            return Ok(new CustomResponse<Error> { Message = GlobalUtility.ResponseMessages.Conflict, StatusCode = (int)HttpStatusCode.Conflict, Result = new Error { ErrorMessage = GlobalUtility.ResponseMessages.GenerateAlreadyExists("Address") } });
        //        }
        //        return Ok(new CustomResponse<User> { Message = GlobalUtility.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = user });
        //    }
        //    else
        //    {
        //        return Ok(new CustomResponse<Error> { Message = GlobalUtility.ResponseMessages.NotFound, StatusCode = (int)HttpStatusCode.NotFound, Result = new Error { ErrorMessage = GlobalUtility.ResponseMessages.GenerateNotFound("User") } });
        //    }
        //}
        //[Authorize]
        //[HttpPost]
        //[Route("EditUserAddress")]
        //public async Task<IHttpActionResult> EditUserAddress(EditUserAddressBindingModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }
        //    bool AddressNotExist = false;
        //    var user = _UserService.EditUserAddress(model, ref AddressNotExist);

        //    if (user != null)
        //    {
        //        if (AddressNotExist)
        //        {
        //            return Ok(new CustomResponse<Error> { Message = GlobalUtility.ResponseMessages.NotFound, StatusCode = (int)HttpStatusCode.NotFound, Result = new Error { ErrorMessage = GlobalUtility.ResponseMessages.GenerateInvalid("AddressId") } });
        //        }
        //        return Ok(new CustomResponse<User> { Message = GlobalUtility.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = user });
        //    }
        //    else
        //        return Ok(new CustomResponse<Error> { Message = GlobalUtility.ResponseMessages.NotFound, StatusCode = (int)HttpStatusCode.NotFound, Result = new Error { ErrorMessage = GlobalUtility.ResponseMessages.GenerateInvalid("UserId") } });
        //}

        /// <summary>
        /// An email will be sent to user containing password reset url. Which will redirect user to password reset page.
        /// </summary>
        /// <param name="Email">Email of user.</param>
        /// <returns></returns>
        [HttpGet]
        [Route("ResetPasswordThroughEmail")]
        public async Task<IHttpActionResult> ResetPasswordThroughEmail(string Email)
        {
            var user = _UserService.ResetPasswordThroughEmail(Email);
            if(user !=null)
            {
                return Ok(new CustomResponse<User> { Message = GlobalUtility.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = user });
            }
            else
            {
                return Ok(new CustomResponse<Error> { Message = "NotFound", StatusCode = (int)HttpStatusCode.NotFound, Result = new Error { ErrorMessage = "User with entered email doesn’t exist." } });
            }
        }


        ///// <summary>
        ///// Register for getting push notifications
        ///// </summary>
        ///// <param name="model"></param>
        ///// <returns></returns>
        //[Authorize]
        //[HttpPost]
        //[Route("RegisterPushNotification")]
        //public async Task<IHttpActionResult> RegisterPushNotification(RegisterPushNotificationBindingModel model)
        //{
        //    try
        //    {
        //        if (!ModelState.IsValid)
        //        {
        //            return BadRequest(ModelState);
        //        }
        //        using (AdminDBContext ctx = new AdminDBContext())
        //        {
        //            var user = ctx.Users.Include(x => x.UserDevices).FirstOrDefault(x => x.Id == model.User_Id);
        //            if (user != null)
        //            {
        //                var existingUserDevice = user.UserDevices.FirstOrDefault(x => x.UDID.Equals(model.UDID));
        //                if (existingUserDevice == null)
        //                {
        //                    //foreach (var userDevice in user.UserDevices)
        //                    //    userDevice.IsActive = false;

        //                    var userDeviceModel = new UserDevice
        //                    {
        //                        Platform = model.IsAndroidPlatform,
        //                        ApplicationType = model.IsPlayStore ? UserDevice.ApplicationTypes.PlayStore : UserDevice.ApplicationTypes.Enterprise,
        //                        EnvironmentType = model.IsProduction ? UserDevice.ApnsEnvironmentTypes.Production : UserDevice.ApnsEnvironmentTypes.Sandbox,
        //                        UDID = model.UDID,
        //                        AuthToken = model.AuthToken,
        //                        IsActive = true
        //                    };

        //                    PushNotificationsUtil.ConfigurePushNotifications(userDeviceModel);

        //                    user.UserDevices.Add(userDeviceModel);
        //                    ctx.SaveChanges();
        //                    return Ok(new CustomResponse<UserDevice> { Message = GlobalUtility.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = userDeviceModel });
        //                }
        //                else
        //                {
        //                    //foreach (var userDevice in user.UserDevices)
        //                    //    userDevice.IsActive = false;

        //                    existingUserDevice.Platform = model.IsAndroidPlatform;
        //                    existingUserDevice.ApplicationType = model.IsPlayStore ? UserDevice.ApplicationTypes.PlayStore : UserDevice.ApplicationTypes.Enterprise;
        //                    existingUserDevice.EnvironmentType = model.IsProduction ? UserDevice.ApnsEnvironmentTypes.Production : UserDevice.ApnsEnvironmentTypes.Sandbox;
        //                    existingUserDevice.UDID = model.UDID;
        //                    existingUserDevice.AuthToken = model.AuthToken;
        //                    existingUserDevice.IsActive = true;
        //                    ctx.SaveChanges();
        //                    PushNotificationsUtil.ConfigurePushNotifications(existingUserDevice);
        //                    return Ok(new CustomResponse<UserDevice> { Message = GlobalUtility.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = existingUserDevice });
        //                }
        //            }
        //            else
        //                return Ok(new CustomResponse<Error> { Message = GlobalUtility.ResponseMessages.NotFound, StatusCode = (int)HttpStatusCode.NotFound, Result = new Error { ErrorMessage = GlobalUtility.ResponseMessages.GenerateNotFound("User") } });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(Utility.LogError(ex));
        //    }
        //}

        //[Authorize]
        //[HttpPost]
        //[Route("AddEditPaymentCard")]
        //public async Task<IHttpActionResult> AddEditPaymentCard(PaymentCardBindingModel model)
        //{
        //    try
        //    {
        //        if (!ModelState.IsValid)
        //        {
        //            return BadRequest(ModelState);
        //        }
        //        using (AdminDBContext ctx = new AdminDBContext())
        //        {
        //            var existingUser = ctx.Users.Include(x => x.UserAddresses).Include(x => x.PaymentCards).FirstOrDefault(x => x.Id == model.UserId && x.IsDeleted == false);

        //            if (existingUser != null)
        //            {
        //                if (model.IsEdit)
        //                {
        //                    var existingCard = existingUser.PaymentCards.FirstOrDefault(x => x.Id == model.Id && x.IsDeleted == false);
        //                    if (existingCard != null)
        //                    {
        //                        ctx.Entry(existingCard).CurrentValues.SetValues(model);
        //                    }
        //                    else
        //                        return Ok(new CustomResponse<Error> { Message = GlobalUtility.ResponseMessages.NotFound, StatusCode = (int)HttpStatusCode.NotFound, Result = new Error { ErrorMessage = GlobalUtility.ResponseMessages.GenerateNotFound("Card") } });
        //                }
        //                else
        //                {
        //                    existingUser.PaymentCards.Add(new PaymentCard
        //                    {
        //                        CardNumber = model.CardNumber,
        //                        CardType = model.CardType,
        //                        CCV = model.CCV,
        //                        ExpiryDate = model.ExpiryDate,
        //                        NameOnCard = model.NameOnCard,
        //                        User_ID = model.UserId
        //                    });
        //                }
        //                ctx.SaveChanges();
        //                return Ok(new CustomResponse<User> { Message = GlobalUtility.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = existingUser });
        //            }
        //            else
        //            {
        //                return Ok(new CustomResponse<Error> { Message = GlobalUtility.ResponseMessages.NotFound, StatusCode = (int)HttpStatusCode.NotFound, Result = new Error { ErrorMessage = GlobalUtility.ResponseMessages.GenerateNotFound("User") } });
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(Utility.LogError(ex));
        //    }
        //}

        //[Authorize]
        //[HttpGet]
        //[Route("DeletePaymentCard")]
        //public async Task<IHttpActionResult> DeletePaymentCard(int UserId, int CardId)
        //{
        //    bool AddressNotExist = false;
        //    var user = _UserService.DeleteUserAddress(UserId, CardId, ref AddressNotExist);
        //    if (user != null)
        //    {
        //        if (AddressNotExist)
        //        {
        //            return Ok(new CustomResponse<Error> { Message = GlobalUtility.ResponseMessages.NotFound, StatusCode = (int)HttpStatusCode.NotFound, Result = new Error { ErrorMessage = GlobalUtility.ResponseMessages.GenerateInvalid("Card") } });
        //        }
        //        return Ok(new CustomResponse<User> { Message = GlobalUtility.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = user });
        //    }
        //    else
        //        return Ok(new CustomResponse<Error> { Message = GlobalUtility.ResponseMessages.NotFound, StatusCode = (int)HttpStatusCode.NotFound, Result = new Error { ErrorMessage = GlobalUtility.ResponseMessages.GenerateInvalid("User") } });
        //}

        //[Authorize]
        //[HttpGet]
        //[Route("DeleteUserAddress")]
        //public async Task<IHttpActionResult> DeleteUserAddress(int UserId, int AddressId)
        //{
        //    bool AddressNotExist = false;
        //    var user = _UserService.DeleteUserAddress(UserId,AddressId, ref AddressNotExist);
        //    if (user != null)
        //    {
        //        if (AddressNotExist)
        //        {
        //            return Ok(new CustomResponse<Error> { Message = GlobalUtility.ResponseMessages.NotFound, StatusCode = (int)HttpStatusCode.NotFound, Result = new Error { ErrorMessage = GlobalUtility.ResponseMessages.GenerateInvalid("Address") } });
        //        }
        //        return Ok(new CustomResponse<User> { Message = GlobalUtility.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = user });
        //    }
        //    else
        //        return Ok(new CustomResponse<Error> { Message = GlobalUtility.ResponseMessages.NotFound, StatusCode = (int)HttpStatusCode.NotFound, Result = new Error { ErrorMessage = GlobalUtility.ResponseMessages.GenerateInvalid("User") } });
        //}

        //[Authorize]
        //[HttpGet]
        //[Route("GetUser")]
        //public async Task<IHttpActionResult> GetUser(int UserId)
        //{
        //    var userModel = _UserService.GetUserById(UserId);
        //    if (userModel != null)
        //    {
        //        SettingsModel.LoadSettings();
        //        await userModel.GenerateToken(Request);
        //        userModel.AppSettings = new Settings { ContactNo = SettingsModel.ContactNo, AboutUs = SettingsModel.AboutUs, PrivacyPolicy = SettingsModel.PrivacyPolicy, TermsConditions = SettingsModel.TermsConditions, Tax = SettingsModel.Tax, Currency = SettingsModel.Currency };
        //        return Ok(new CustomResponse<User> { Message = GlobalUtility.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = userModel });
        //    }
        //    else
        //        return Ok(new CustomResponse<Error> { Message = GlobalUtility.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = new Error { ErrorMessage = "Invalid UserId" } });
        //}

        //[HttpGet]
        //[Route("MarkDeviceAsInActive")]
        //public async Task<IHttpActionResult> MarkDeviceAsInActive(int UserId, int DeviceId)
        //{
        //    bool isDone = _UserService.MarkDeviceAsInActive(UserId, DeviceId);
        //    if (isDone)
        //    {
        //        return Ok(new CustomResponse<string> { Message = GlobalUtility.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK });
        //    }
        //    else
        //    {
        //        return Ok(new CustomResponse<Error> { Message = GlobalUtility.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = new Error { ErrorMessage = "Invalid UserId or DeviceId." } });
        //    }
        //}


        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        private IAuthenticationManager Authentication
        {
            get { return Request.GetOwinContext().Authentication; }
        }

        public ISecureDataFormat<AuthenticationTicket> AccessTokenFormat { get; private set; }

        private class ExternalLoginData
        {
            public string LoginProvider { get; set; }
            public string ProviderKey { get; set; }
            public string UserName { get; set; }

            public IList<Claim> GetClaims()
            {
                IList<Claim> claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.NameIdentifier, ProviderKey, null, LoginProvider));

                if (UserName != null)
                {
                    claims.Add(new Claim(ClaimTypes.Name, UserName, null, LoginProvider));
                }

                return claims;
            }

            public static ExternalLoginData FromIdentity(ClaimsIdentity identity)
            {
                if (identity == null)
                {
                    return null;
                }

                Claim providerKeyClaim = identity.FindFirst(ClaimTypes.NameIdentifier);

                if (providerKeyClaim == null || String.IsNullOrEmpty(providerKeyClaim.Issuer)
                    || String.IsNullOrEmpty(providerKeyClaim.Value))
                {
                    return null;
                }

                if (providerKeyClaim.Issuer == ClaimsIdentity.DefaultIssuer)
                {
                    return null;
                }

                return new ExternalLoginData
                {
                    LoginProvider = providerKeyClaim.Issuer,
                    ProviderKey = providerKeyClaim.Value,
                    UserName = identity.FindFirstValue(ClaimTypes.Name)
                };
            }
        }
        
    }
}

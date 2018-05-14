using DAL;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using Nexmo.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using BasketApi.CustomAuthorization;
using BasketApi.Models;
using BasketApi.ViewModels;
using System.IO;
using System.Configuration;
using System.Data.Entity;
using System.Net.Mail;
using static BasketApi.Global;
using BasketApi.Components.Helpers;

namespace BasketApi.Controllers
{
    [RoutePrefix("api/User")]
    public class UserController : ApiController
    {
        private ApplicationUserManager _userManager;

        [Route("all")]
        public IHttpActionResult Getall()
        {
            try
            {
                //var nexmoVerifyResponse = NumberVerify.Verify(new NumberVerify.VerifyRequest { brand = "INGIC", number = "+923325345126" });
                //var nexmoCheckResponse = NumberVerify.Check(new NumberVerify.CheckRequest { request_id = nexmoVerifyResponse.request_id, code = "6310"});
                return Ok("Hello");
            }
            catch (Exception ex)
            {
                return Ok("Hello");
            }
        }

        //[Route("UploadImage")]
        //[AllowAnonymous]
        //public async Task<HttpResponseMessage> PostFile()
        //{
        //    // Check if the request contains multipart/form-data. 
        //    if (!Request.Content.IsMimeMultipartContent())
        //    {
        //        throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
        //    }

        //    string root = HttpContext.Current.Server.MapPath("~/App_Data");
        //    var provider = new MultipartFormDataStreamProvider(root);

        //    try
        //    {
        //        StringBuilder sb = new StringBuilder(); // Holds the response body 

        //        // Read the form data and return an async task. 
        //        await Request.Content.ReadAsMultipartAsync(provider);

        //        // This illustrates how to get the form data. 
        //        foreach (var key in provider.FormData.AllKeys)
        //        {
        //            foreach (var val in provider.FormData.GetValues(key))
        //            {
        //                sb.Append(string.Format("{0}: {1}\n", key, val));
        //            }
        //        }

        //        // This illustrates how to get the file names for uploaded files. 
        //        foreach (var file in provider.FileData)
        //        {
        //            FileInfo fileInfo = new FileInfo(file.LocalFileName);
        //            sb.Append(string.Format("Uploaded file: {0} ({1} bytes)\n", fileInfo.Name, fileInfo.Length));
        //        }
        //        return new HttpResponseMessage()
        //        {
        //            Content = new StringContent(sb.ToString())
        //        };
        //    }
        //    catch (System.Exception e)
        //    {
        //        return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
        //    }
        //}

        //[Authorize]
        [Route("Register")]
        [AllowAnonymous]
        [HttpPost]
        public async Task<IHttpActionResult> Register(RegisterBindingModel model)
        {
            try
            {
                var httpRequest = HttpContext.Current.Request;
                string newFullPath = string.Empty;
                string fileNameOnly = string.Empty;

                Validate(model);

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }


                using (BasketContext ctx = new BasketContext())
                {
                    if (ctx.Users.Any(x => x.Email == model.Email))
                    {
                        return Content(HttpStatusCode.OK, new CustomResponse<Error>
                        {
                            Message = "Conflict",
                            StatusCode = (int)HttpStatusCode.Conflict,
                            Result = new Error { ErrorMessage = "User with email already exists." }
                        });
                    }

                    if (!model.SignInType.HasValue)
                        model.SignInType = (int)RoleTypes.User;

                    User userModel = new User
                    {
                        FullName = model.FullName,
                        Email = model.Email,
                        Phone = model.PhoneNumber,
                        Address = model.Address,
                        Password = CryptoHelper.Hash(model.Password),
                        Status = (int)Global.StatusCode.Verified,
                        SignInType = model.SignInType,
                        IsNotificationsOn = true,
                        IsDeleted = false
                    };

                    ctx.Users.Add(userModel);
                    ctx.SaveChanges();

                    await userModel.GenerateToken(Request);
                    CustomResponse<User> response = new CustomResponse<User> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = userModel };
                    return Ok(response);


                }
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
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
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                using (BasketContext ctx = new BasketContext())
                {
                    DAL.User userModel;

                    if (model.SignInType == (int)RoleTypes.User)
                    {
                        //userModel = ctx.Users.FirstOrDefault(x => x.Email == model.Email && x.Password == model.Password && x.SignInType == model.SignInType.Value);
                        var hashedPassword = CryptoHelper.Hash(model.Password);
                        userModel = ctx.Users.FirstOrDefault(x => x.Phone == model.PhoneNumber && x.Password == hashedPassword && x.IsDeleted == false);

                        if (userModel != null)
                        {
                            await userModel.GenerateToken(Request);
                            //BasketSettings.LoadSettings();
                            //userModel.BasketSettings = new Settings
                            //{
                            //    Id = BasketSettings.Id,
                            //    MinimumOrderPrice = BasketSettings.MinimumOrderPrice,
                            //    Currency = BasketSettings.Currency,
                            //    DeliveryFee = BasketSettings.DeliveryFee,
                            //    ServiceFee = BasketSettings.ServiceFee,
                            //    AboutUs = BasketSettings.AboutUs,
                            //    NearByRadius = BasketSettings.NearByRadius * 1609.344,
                            //    Help = BasketSettings.Help
                            //};
                        }
                    }
                    else
                    {
                        return Content(HttpStatusCode.OK, new CustomResponse<Error>
                        {
                            Message = "Forbidden",
                            StatusCode = (int)HttpStatusCode.Forbidden,
                            Result = new Error { ErrorMessage = "Invalid Email or Password." }
                        });
                    }
                    return Ok(new CustomResponse<User> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = userModel });

                }
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }

        [Authorize]
        [HttpGet]
        [Route("UpdateNotificationStatus")]
        public async Task<IHttpActionResult> UpdateNotificationStatus(string Email, bool IsNotificationOn)
        {
            try
            {
                using (BasketContext ctx = new BasketContext())
                {
                    var user = ctx.Users.FirstOrDefault(x => x.Email == Email && x.IsDeleted == false);

                    if (user != null)
                    {
                        user.IsNotificationsOn = IsNotificationOn;
                        ctx.SaveChanges();
                        return Ok(new CustomResponse<string> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK });
                    }
                    else
                    {
                        return Ok(new CustomResponse<Error> { Message = "Not Found", StatusCode = (int)HttpStatusCode.NotFound, Result = new Error { ErrorMessage = "User with entered email doesn’t exist." } });
                    }

                }
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }


        /// <summary>
        /// Change user password
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [Route("ChangePassword")]
        public async Task<IHttpActionResult> ChangePassword(SetPasswordBindingModel model)
        {
            try
            {
                var User_Id = User.GetClaimValue("userid");

                if (string.IsNullOrEmpty(User_Id))
                {
                    throw new Exception("User id is empty in user.identity.id.");
                }
                else if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                using (BasketContext ctx = new BasketContext())
                {
                    var userid = Convert.ToInt32(User_Id);
                    var hashedPassword = CryptoHelper.Hash(model.OldPassword);
                    var user = ctx.Users.FirstOrDefault(x => x.Id == userid && x.Password == hashedPassword);
                    if (user != null)
                    {
                        user.Password = CryptoHelper.Hash(model.NewPassword);
                        ctx.SaveChanges();
                        return Ok(new CustomResponse<string> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK });
                    }
                    else
                        return Ok(new CustomResponse<Error> { Message = "Forbidden", StatusCode = (int)HttpStatusCode.Forbidden, Result = new Error { ErrorMessage = "Invalid old password." } });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }

        /// <summary>
        /// Send verification code to user
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("ForgetPasswordThroughEmail")]
        public async Task<IHttpActionResult> ForgetPasswordThroughEmail(string Email)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                using (BasketContext ctx = new BasketContext())
                {
                    var user = ctx.Users.FirstOrDefault(x => x.Email == Email && x.Status != (int)Global.StatusCode.NotVerified && x.IsDeleted == false);

                    if (user != null)
                    {
                        if (!string.IsNullOrEmpty(user.Email))
                        {
                            await ctx.ForgotPasswordTokens.Where(x => x.User_ID == user.Id).ForEachAsync(x => x.IsDeleted = true);
                            var code = Guid.NewGuid().ToString().Substring(0, 5);
                            var body = "Your Forget Password Verification Code is :" + code;
                            Utility.SendEmail("Forget Password Code", body, user.Email);


                            user.ForgotPasswordTokens.Add(new ForgotPasswordToken { CreatedAt = DateTime.Now, IsDeleted = false, User_ID = user.Id, Code = code });
                            ctx.SaveChanges();
                            return Ok(new CustomResponse<string> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK });

                        }
                        else
                        {
                            return Ok(new CustomResponse<Error> { Message = "NotFound", StatusCode = (int)HttpStatusCode.NotFound, Result = new Error { ErrorMessage = "User emai lnot found." } });
                        }
                    }
                    else
                    {
                        return Ok(new CustomResponse<Error> { Message = "NotFound", StatusCode = (int)HttpStatusCode.NotFound, Result = new Error { ErrorMessage = "User with email does not exists." } });
                    }

                }
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }

        /// <summary>
        /// Verify code sent to user. 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("VerifyForgetPasswordCode")]
        public IHttpActionResult VerifyForgetPasswordCode(PhoneVerificationModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                using (BasketContext ctx = new BasketContext())
                {
                    var user = ctx.Users.FirstOrDefault(x => x.Email == model.Email);
                    if (user != null)
                    {
                        var verificationCode = ctx.ForgotPasswordTokens.FirstOrDefault(x => x.User_ID == user.Id && x.Code == model.Code && x.IsDeleted == false);
                        if (verificationCode != null)
                        {
                            if(DateTime.Now.Subtract(verificationCode.CreatedAt).Minutes < 60)
                            {
                                verificationCode.IsDeleted = true;
                                return Content(HttpStatusCode.OK, new CustomResponse<string> { Message = "Success", StatusCode = (int)HttpStatusCode.Accepted });
                            }else
                            {
                                return Ok(new CustomResponse<Error> { Message = "Code Expired", StatusCode = (int)HttpStatusCode.Conflict, Result = new Error { ErrorMessage = "Verification code expired." } });
                            }
                        }
                    }
                    else
                        return Ok(new CustomResponse<Error> { Message = "NotFound", StatusCode = (int)HttpStatusCode.NotFound, Result = new Error { ErrorMessage = "Invalid verification code." } });
                }
                return Ok(new CustomResponse<Error> { Message = "InternetServerError", StatusCode = (int)HttpStatusCode.InternalServerError, Result = new Error { ErrorMessage = "Oops! Something went wrong." } });

            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }

        [HttpPost]
        [Route("ChangePasswordOnForget")]
        public IHttpActionResult ChangePasswordOnForget(ChangePasswordOnForgetBindingModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                using (BasketContext ctx = new BasketContext())
                {
                    var user = ctx.Users.FirstOrDefault(x => x.Email == model.Email);
                    if (user != null)
                    {
                        var hashedPassword = CryptoHelper.Hash(model.NewPassword);
                        user.Password = hashedPassword;
                        ctx.SaveChanges();
                        user.GenerateToken(Request);
                        return Content(HttpStatusCode.OK, new CustomResponse<User> { Message = "Success", StatusCode = (int)HttpStatusCode.Accepted, Result = user });

                    }
                    return Ok(new CustomResponse<Error> { Message = "NotFound", StatusCode = (int)HttpStatusCode.NotFound, Result = new Error { ErrorMessage = "User with email already exists." } });

                }
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }

        ///// <summary>
        ///// Login for web admin panel
        ///// </summary>
        ///// <param name="model"></param>
        ///// <returns></returns>
        //[AllowAnonymous]
        //[Route("WebPanelLogin")]
        //[HttpPost]
        //public async Task<IHttpActionResult> WebPanelLogin(WebLoginBindingModel model)
        //{
        //    try
        //    {
        //        if (!ModelState.IsValid)
        //        {
        //            return BadRequest(ModelState);
        //        }

        //        using (BasketContext ctx = new BasketContext())
        //        {
        //            DAL.Admin adminModel;

        //            var hashedPassword = CryptoHelper.Hash(model.Password);

        //            adminModel = ctx.Admins.Include(x => x.ReceivedNotifications).FirstOrDefault(x => x.Email == model.Email && x.Password == hashedPassword && x.IsDeleted == false);

        //            if (adminModel != null)
        //            {
        //                await adminModel.GenerateToken(Request);
        //                CustomResponse<Admin> response = new CustomResponse<Admin> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = adminModel };
        //                return Ok(response);
        //            }
        //            else
        //                return Content(HttpStatusCode.OK, new CustomResponse<Error>
        //                {
        //                    Message = "Forbidden",
        //                    StatusCode = (int)HttpStatusCode.Forbidden,
        //                    Result = new Error { ErrorMessage = "Invalid Email or Password" }
        //                });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(Utility.LogError(ex));
        //    }
        //}

        //private int SavePicture(HttpRequestMessage request, out string PicturePath)
        //{
        //    try
        //    {
        //        var httpRequest = HttpContext.Current.Request;
        //        PicturePath = String.Empty;

        //        if (httpRequest.Files.Count > 1)
        //            return 3;

        //        foreach (string file in httpRequest.Files)
        //        {
        //            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created);

        //            var postedFile = httpRequest.Files[file];
        //            if (postedFile != null && postedFile.ContentLength > 0)
        //            {
        //                int MaxContentLength = 1024 * 1024 * 1; //Size = 1 MB  

        //                IList<string> AllowedFileExtensions = new List<string> { ".jpg", ".gif", ".png" };
        //                var ext = Path.GetExtension(postedFile.FileName);
        //                var extension = ext.ToLower();
        //                if (!AllowedFileExtensions.Contains(extension))
        //                {
        //                    var message = string.Format("Please Upload image of type .jpg,.gif,.png.");
        //                    return 1;
        //                }
        //                else if (postedFile.ContentLength > MaxContentLength)
        //                {

        //                    var message = string.Format("Please Upload a file upto 1 mb.");
        //                    return 2;
        //                }
        //                else
        //                {
        //                    int count = 1;
        //                    string fileNameOnly = Path.GetFileNameWithoutExtension(postedFile.FileName);
        //                    string newFullPath = HttpContext.Current.Server.MapPath("~/App_Data/" + postedFile.FileName);

        //                    while (File.Exists(newFullPath))
        //                    {
        //                        string tempFileName = string.Format("{0}({1})", fileNameOnly, count++);
        //                        newFullPath = HttpContext.Current.Server.MapPath("~/App_Data/" + tempFileName + extension);
        //                    }

        //                    postedFile.SaveAs(newFullPath);
        //                    PicturePath = newFullPath;
        //                }
        //            }
        //        }
        //        return 0;
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        ///// <summary>
        ///// Logout
        ///// </summary>
        ///// <returns></returns>
        //[Authorize]
        //[Route("Logout")]
        //public IHttpActionResult Logout()
        //{
        //    HttpContext.Current.GetOwinContext().Authentication.SignOut(OAuthDefaults.AuthenticationType);
        //    return Ok();
        //}

        //[Authorize]
        //[Route("MarkVerified")]
        //[HttpPost]
        //public IHttpActionResult MarkUserAccountAsVerified(UserModel model)
        //{
        //    try
        //    {
        //        if (!ModelState.IsValid)
        //            return BadRequest(ModelState);

        //        using (BasketContext ctx = new BasketContext())
        //        {
        //            var userModel = ctx.Users.FirstOrDefault(x => x.Email == model.Email);
        //            if (userModel == null)
        //                return BadRequest("User account doesn't exist");

        //            userModel.Status = (int)Global.StatusCode.Verified;
        //            ctx.SaveChanges();
        //            return Ok();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(Utility.LogError(ex));
        //    }
        //}

        ///// <summary>
        ///// Send verification code to user
        ///// </summary>
        ///// <param name="model"></param>
        ///// <returns></returns>
        //[Authorize]
        //[HttpPost]
        //[Route("SendVerificationSms")]
        //public async Task<IHttpActionResult> SendVerificationSms(PhoneBindingModel model)
        //{
        //    try
        //    {
        //        if (!ModelState.IsValid)
        //            return BadRequest(ModelState);

        //        using (BasketContext ctx = new BasketContext())
        //        {
        //            var user = ctx.Users.FirstOrDefault(x => x.Id == model.UserId);

        //            if (user != null)
        //            {
        //                int code = new Random().Next(11111, 99999);

        //                await ctx.VerifyNumberCodes.Where(x => x.User_Id == user.Id).ForEachAsync(x => x.IsDeleted = true);
        //                await ctx.SaveChangesAsync();

        //                user.VerifyNumberCodes.Add(new VerifyNumberCodes { CreatedAt = DateTime.Now, IsDeleted = false, User_Id = user.Id, Code = code });
        //                ctx.SaveChanges();
        //                using (HttpClient client = new HttpClient())
        //                {
        //                    var smsGlobalUrl = "https://api.smsglobal.com/http-api.php?action=sendsms&user=" + ConfigurationManager.AppSettings["SmsGlobalUserName"] +
        //                        "&password=" + ConfigurationManager.AppSettings["SmsGlobalPassword"] + "&&from=BasketApp&to=" + model.PhoneNumber.Replace("+", "") +
        //                        "&text=Verification Code : " + code;

        //                    var response = await client.GetAsync(smsGlobalUrl);

        //                    if (response.StatusCode == HttpStatusCode.OK)
        //                    {
        //                        user.Phone = model.PhoneNumber.Replace("+", "");
        //                        ctx.SaveChanges();
        //                        return Ok(new CustomResponse<User> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = user });
        //                    }
        //                    else
        //                    {
        //                        Utility.LogError(new Exception("SmsGlobalError! = " + response.ToString()));
        //                        return Ok(new CustomResponse<Error> { Message = Global.ResponseMessages.BadRequest, StatusCode = (int)HttpStatusCode.BadRequest, Result = new Error { ErrorMessage = "An error occurred while sending the message." } });
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                return Ok(new CustomResponse<Error> { Message = "NotFound", StatusCode = (int)HttpStatusCode.NotFound, Result = new Error { ErrorMessage = "User not found" } });
        //            }

        //        }
        //        var nexmoVerifyResponse = NumberVerify.Verify(new NumberVerify.VerifyRequest { brand = "INGIC", number = model.PhoneNumber });

        //        if (nexmoVerifyResponse.status == "0")
        //            return Content(HttpStatusCode.OK, new CustomResponse<NumberVerify.VerifyResponse> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = nexmoVerifyResponse });
        //        else
        //        {
        //            return Content(HttpStatusCode.OK, new CustomResponse<Error> { Message = "InternalServerError", StatusCode = (int)HttpStatusCode.InternalServerError, Result = new Error { ErrorMessage = "Verification SMS failed due to some reason." } });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(Utility.LogError(ex));
        //    }
        //}

        ///// <summary>
        ///// Verify code sent to user. 
        ///// </summary>
        ///// <param name="model"></param>
        ///// <returns></returns>
        //[Authorize]
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

        //        using (BasketContext ctx = new BasketContext())
        //        {
        //            var user = ctx.Users.Include(x => x.VerifyNumberCodes).FirstOrDefault(x => x.Id == model.UserId);
        //            var codeEntry = user.VerifyNumberCodes.FirstOrDefault(x => x.Code == model.Code && x.IsDeleted == false && DateTime.Now.Subtract(x.CreatedAt).Minutes < 60);
        //            if (codeEntry != null)
        //            {
        //                user.PhoneConfirmed = true;
        //                codeEntry.IsDeleted = true;
        //                ctx.SaveChanges();
        //                return Ok(new CustomResponse<User> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = user });
        //            }
        //            else
        //                return Ok(new CustomResponse<Error> { Message = Global.ResponseMessages.NotFound, StatusCode = (int)HttpStatusCode.Forbidden, Result = new Error { ErrorMessage = "Invalid code" } });
        //        }

        //        //var nexmoCheckResponse = NumberVerify.Check(new NumberVerify.CheckRequest { request_id = model.request_id, code = model.Code });

        //        //if (nexmoCheckResponse.status == "0")
        //        //    return Content(HttpStatusCode.OK, new MessageViewModel { Details = "Account Verified Successfully." });
        //        //else
        //        //    return Content(HttpStatusCode.OK, new CustomResponse<NumberVerify.CheckResponse> { Message = "InternalServerError", StatusCode = (int)HttpStatusCode.InternalServerError, Result = nexmoCheckResponse });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(Utility.LogError(ex));
        //    }
        //}


        //[Route("UploadUserImage")]
        //[Authorize]
        //public async Task<IHttpActionResult> UploadUserImage()
        //{
        //    Dictionary<string, object> dict = new Dictionary<string, object>();
        //    try
        //    {
        //        var httpRequest = HttpContext.Current.Request;
        //        string newFullPath = string.Empty;
        //        string fileNameOnly = string.Empty;

        //        #region Validations
        //        var userEmail = User.Identity.Name;
        //        if (string.IsNullOrEmpty(userEmail))
        //        {
        //            throw new Exception("User Email is empty in user.identity.name.");
        //        }
        //        else if (!Request.Content.IsMimeMultipartContent())
        //        {
        //            return Content(HttpStatusCode.OK, new CustomResponse<Error>
        //            {
        //                Message = "UnsupportedMediaType",
        //                StatusCode = (int)HttpStatusCode.UnsupportedMediaType,
        //                Result = new Error { ErrorMessage = "Multipart data is not included in request." }
        //            });
        //        }
        //        else if (httpRequest.Files.Count == 0)
        //        {
        //            return Content(HttpStatusCode.OK, new CustomResponse<Error>
        //            {
        //                Message = "NotFound",
        //                StatusCode = (int)HttpStatusCode.NotFound,
        //                Result = new Error { ErrorMessage = "Image not found, please upload an image." }
        //            });
        //        }
        //        else if (httpRequest.Files.Count > 1)
        //        {
        //            return Content(HttpStatusCode.OK, new CustomResponse<Error>
        //            {
        //                Message = "UnsupportedMediaType",
        //                StatusCode = (int)HttpStatusCode.UnsupportedMediaType,
        //                Result = new Error { ErrorMessage = "Multiple images are not allowed. Please upload 1 image." }
        //            });
        //        }
        //        #endregion

        //        var postedFile = httpRequest.Files[0];

        //        if (postedFile != null && postedFile.ContentLength > 0)
        //        {

        //            int MaxContentLength = 1024 * 1024 * 10; //Size = 1 MB  

        //            IList<string> AllowedFileExtensions = new List<string> { ".jpg", ".gif", ".png" };
        //            var ext = postedFile.FileName.Substring(postedFile.FileName.LastIndexOf('.'));
        //            var extension = ext.ToLower();
        //            if (!AllowedFileExtensions.Contains(extension))
        //            {
        //                return Content(HttpStatusCode.OK, new CustomResponse<Error>
        //                {
        //                    Message = "UnsupportedMediaType",
        //                    StatusCode = (int)HttpStatusCode.UnsupportedMediaType,
        //                    Result = new Error { ErrorMessage = "Please Upload image of type .jpg, .gif, .png." }
        //                });
        //            }
        //            else if (postedFile.ContentLength > MaxContentLength)
        //            {

        //                return Content(HttpStatusCode.OK, new CustomResponse<Error>
        //                {
        //                    Message = "UnsupportedMediaType",
        //                    StatusCode = (int)HttpStatusCode.UnsupportedMediaType,
        //                    Result = new Error { ErrorMessage = "Please Upload a file upto 1 mb." }
        //                });
        //            }
        //            else
        //            {
        //                int count = 1;
        //                fileNameOnly = Path.GetFileNameWithoutExtension(postedFile.FileName);
        //                newFullPath = HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings["UserImageFolderPath"] + postedFile.FileName);

        //                while (File.Exists(newFullPath))
        //                {
        //                    string tempFileName = string.Format("{0}({1})", fileNameOnly, count++);
        //                    newFullPath = HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings["UserImageFolderPath"] + tempFileName + extension);
        //                }
        //                postedFile.SaveAs(newFullPath);
        //            }
        //        }

        //        MessageViewModel successResponse = new MessageViewModel { StatusCode = "200 OK", Details = "Image Updated Successfully." };
        //        var filePath = Utility.BaseUrl + ConfigurationManager.AppSettings["UserImageFolderPath"] + Path.GetFileName(newFullPath);
        //        ImagePathViewModel model = new ImagePathViewModel { Path = filePath };

        //        using (BasketContext ctx = new BasketContext())
        //        {
        //            ctx.Users.FirstOrDefault(x => x.Email == userEmail).ProfilePictureUrl = filePath;
        //            ctx.SaveChanges();
        //        }

        //        return Content(HttpStatusCode.OK, model);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(Utility.LogError(ex));
        //    }
        //}

        ///// <summary>
        ///// Change user password
        ///// </summary>
        ///// <param name="model"></param>
        ///// <returns></returns>
        //[Authorize]
        //[Route("ChangePassword")]
        //public async Task<IHttpActionResult> ChangePassword(SetPasswordBindingModel model)
        //{
        //    try
        //    {
        //        var userEmail = User.Identity.Name;
        //        if (string.IsNullOrEmpty(userEmail))
        //        {
        //            throw new Exception("User Email is empty in user.identity.name.");
        //        }
        //        else if (!ModelState.IsValid)
        //        {
        //            return BadRequest(ModelState);
        //        }

        //        using (BasketContext ctx = new BasketContext())
        //        {
        //            if (model.SignInType == (int)RoleTypes.User)
        //            {
        //                var hashedPassword = CryptoHelper.Hash(model.OldPassword);
        //                var user = ctx.Users.FirstOrDefault(x => x.Email == userEmail && x.Password == hashedPassword);
        //                if (user != null)
        //                {
        //                    user.Password = CryptoHelper.Hash(model.NewPassword);
        //                    ctx.SaveChanges();
        //                    return Ok(new CustomResponse<string> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK });
        //                }
        //                else
        //                    return Ok(new CustomResponse<Error> { Message = "Forbidden", StatusCode = (int)HttpStatusCode.Forbidden, Result = new Error { ErrorMessage = "Invalid old password." } });

        //            }
        //            else if (model.SignInType == (int)RoleTypes.Deliverer)
        //            {
        //                var hashedPassword = CryptoHelper.Hash(model.OldPassword);
        //                var deliveryMan = ctx.DeliveryMen.FirstOrDefault(x => x.Email == userEmail && x.Password == hashedPassword);
        //                if (deliveryMan != null)
        //                {
        //                    deliveryMan.Password = CryptoHelper.Hash(model.NewPassword);
        //                    ctx.SaveChanges();
        //                    return Ok(new CustomResponse<string> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK });
        //                }
        //                else
        //                    return Ok(new CustomResponse<Error> { Message = "Forbidden", StatusCode = (int)HttpStatusCode.Forbidden, Result = new Error { ErrorMessage = "Invalid old password" } });
        //            }
        //            else
        //                return Ok(new CustomResponse<Error> { Message = Global.ResponseMessages.BadRequest, StatusCode = (int)HttpStatusCode.BadRequest, Result = new Error { ErrorMessage = Global.ResponseMessages.GenerateInvalid("SignInType") } });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(Utility.LogError(ex));
        //    }
        //}

        //[Route("AddExternalLogin")]
        //public async Task<IHttpActionResult> AddExternalLogin(AddExternalLoginBindingModel model)
        //{
        //    try
        //    {
        //        if (!ModelState.IsValid)
        //        {
        //            return BadRequest(ModelState);
        //        }

        //        Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);

        //        AuthenticationTicket ticket = AccessTokenFormat.Unprotect(model.ExternalAccessToken);

        //        if (ticket == null || ticket.Identity == null || (ticket.Properties != null
        //            && ticket.Properties.ExpiresUtc.HasValue
        //            && ticket.Properties.ExpiresUtc.Value < DateTimeOffset.UtcNow))
        //        {
        //            return BadRequest("External login failure.");
        //        }

        //        ExternalLoginData externalData = ExternalLoginData.FromIdentity(ticket.Identity);

        //        if (externalData == null)
        //        {
        //            return BadRequest("The external login is already associated with an account.");
        //        }

        //        IdentityResult result = await UserManager.AddLoginAsync(User.Identity.GetUserId(),
        //            new UserLoginInfo(externalData.LoginProvider, externalData.ProviderKey));

        //        if (!result.Succeeded)
        //        {
        //            //return GetErrorResult(result);
        //        }

        //        return Ok();
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(Utility.LogError(ex));
        //    }
        //}


        ///// <summary>
        ///// Update user profile with image. This is multipart request. SignInType 0 for user, 1 for deliverer
        ///// </summary>
        ///// <returns></returns>
        //[Authorize]
        //[Route("UpdateUserProfile")]
        //public async Task<IHttpActionResult> UpdateUserProfileWithImage()
        //{
        //    try
        //    {
        //        var httpRequest = HttpContext.Current.Request;
        //        string newFullPath = string.Empty;
        //        string fileNameOnly = string.Empty;

        //        #region InitializingModel
        //        EditUserProfileBindingModel model = new EditUserProfileBindingModel();
        //        model.FullName = httpRequest.Params["fullname"];
        //        model.Email = httpRequest.Params["email"];
        //        model.PhoneNumber = httpRequest.Params["phonenumber"];

        //        if (httpRequest.Params["signintype"] != null)
        //            model.SignInType = Convert.ToInt16(httpRequest.Params["signintype"]);

        //        if (httpRequest.Params["ID"] != null)
        //            model.ID = Convert.ToInt32(httpRequest.Params["ID"]);

        //        if (httpRequest.Params["Status"] != null)
        //            model.Status = Convert.ToInt16(httpRequest.Params["Status"]);

        //        if (httpRequest.Params["EmailConfirmed"] != null)
        //            model.EmailConfirmed = Convert.ToBoolean(httpRequest.Params["EmailConfirmed"]);

        //        if (httpRequest.Params["PhoneConfirmed"] != null)
        //            model.PhoneConfirmed = Convert.ToBoolean(httpRequest.Params["PhoneConfirmed"]);

        //        model.FirstName = httpRequest.Params["FirstName"];
        //        model.LastName = httpRequest.Params["LastName"];
        //        model.ZipCode = httpRequest.Params["ZipCode"];
        //        model.DateofBirth = httpRequest.Params["DateofBirth"];
        //        model.UserName = httpRequest.Params["UserName"];
        //        model.Address = httpRequest.Params["Address"];

        //        #endregion
        //        Validate(model);

        //        #region Validations
        //        if (!ModelState.IsValid)
        //        {
        //            return BadRequest(ModelState);
        //        }

        //        if (!Request.Content.IsMimeMultipartContent())
        //        {
        //            return Content(HttpStatusCode.OK, new CustomResponse<Error>
        //            {
        //                Message = "UnsupportedMediaType",
        //                StatusCode = (int)HttpStatusCode.UnsupportedMediaType,
        //                Result = new Error { ErrorMessage = "Multipart data is not included in request." }
        //            });
        //        }
        //        else if (httpRequest.Files.Count > 1)
        //        {
        //            return Content(HttpStatusCode.OK, new CustomResponse<Error>
        //            {
        //                Message = "UnsupportedMediaType",
        //                StatusCode = (int)HttpStatusCode.UnsupportedMediaType,
        //                Result = new Error { ErrorMessage = "Multiple images are not supported, please upload one image." }
        //            });
        //        }
        //        #endregion

        //        using (BasketContext ctx = new BasketContext())
        //        {
        //            User userModel;
        //            DeliveryMan delivererModel;

        //            HttpPostedFile postedFile = null;
        //            string fileExtension = string.Empty;

        //            #region ImageSaving
        //            if (httpRequest.Files.Count > 0)
        //            {
        //                postedFile = httpRequest.Files[0];
        //                if (postedFile != null && postedFile.ContentLength > 0)
        //                {

        //                    IList<string> AllowedFileExtensions = new List<string> { ".jpg", ".gif", ".png" };
        //                    //var ext = postedFile.FileName.Substring(postedFile.FileName.LastIndexOf('.'));
        //                    var ext = Path.GetExtension(postedFile.FileName);
        //                    fileExtension = ext.ToLower();
        //                    if (!AllowedFileExtensions.Contains(fileExtension))
        //                    {
        //                        return Content(HttpStatusCode.OK, new CustomResponse<Error>
        //                        {
        //                            Message = "UnsupportedMediaType",
        //                            StatusCode = (int)HttpStatusCode.UnsupportedMediaType,
        //                            Result = new Error { ErrorMessage = "Please Upload image of type .jpg,.gif,.png." }
        //                        });
        //                    }
        //                    else if (postedFile.ContentLength > Global.MaximumImageSize)
        //                    {
        //                        return Content(HttpStatusCode.OK, new CustomResponse<Error>
        //                        {
        //                            Message = "UnsupportedMediaType",
        //                            StatusCode = (int)HttpStatusCode.UnsupportedMediaType,
        //                            Result = new Error { ErrorMessage = "Please Upload a file upto " + Global.ImageSize + "." }
        //                        });
        //                    }
        //                    else
        //                    {
        //                        //int count = 1;
        //                        //fileNameOnly = Path.GetFileNameWithoutExtension(postedFile.FileName);
        //                        //newFullPath = HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings["UserImageFolderPath"] + postedFile.FileName);

        //                        //while (File.Exists(newFullPath))
        //                        //{
        //                        //    string tempFileName = string.Format("{0}({1})", fileNameOnly, count++);
        //                        //    newFullPath = HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings["UserImageFolderPath"] + tempFileName + fileExtension);
        //                        //}

        //                        //postedFile.SaveAs(newFullPath);
        //                        //model.ProfilePictureUrl = Utility.BaseUrl + ConfigurationManager.AppSettings["UserImageFolderPath"] + Path.GetFileName(newFullPath);
        //                    }
        //                }
        //            }
        //            #endregion

        //            if (model.SignInType == (int)RoleTypes.User)
        //            {
        //                userModel = ctx.Users.Include(x => x.UserAddresses).Include(x => x.PaymentCards).FirstOrDefault(x => x.Id == model.ID);

        //                if (userModel == null)
        //                {
        //                    return Content(HttpStatusCode.OK, new CustomResponse<Error>
        //                    {
        //                        Message = "NotFound",
        //                        StatusCode = (int)HttpStatusCode.NotFound,
        //                        Result = new Error { ErrorMessage = "UserId does not exist." }
        //                    });
        //                }
        //                else
        //                {
        //                    userModel.FullName = model.FullName;
        //                    userModel.Email = model.Email;
        //                    userModel.Phone = model.PhoneNumber;

        //                    if (httpRequest.Files.Count > 0)
        //                    {
        //                        newFullPath = HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings["UserImageFolderPath"] + userModel.Id + fileExtension);
        //                        postedFile.SaveAs(newFullPath);
        //                        userModel.ProfilePictureUrl = ConfigurationManager.AppSettings["UserImageFolderPath"] + userModel.Id + fileExtension;
        //                    }
        //                    //userModel.ProfilePictureUrl = model.ProfilePictureUrl;

        //                    //ctx.Entry(userModel).CurrentValues.SetValues(model);

        //                    ctx.SaveChanges();
        //                    CustomResponse<User> response = new CustomResponse<User> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = userModel };
        //                    return Ok(response);
        //                }
        //            }
        //            else
        //            {
        //                delivererModel = ctx.DeliveryMen.FirstOrDefault(x => x.Id == model.ID);

        //                if (delivererModel == null)
        //                {
        //                    return Content(HttpStatusCode.OK, new CustomResponse<Error>
        //                    {
        //                        Message = "NotFound",
        //                        StatusCode = (int)HttpStatusCode.NotFound,
        //                        Result = new Error { ErrorMessage = "UserId does not exist." }
        //                    });
        //                }
        //                else
        //                {
        //                    delivererModel.FullName = model.FullName;
        //                    delivererModel.Email = model.Email;
        //                    delivererModel.Phone = model.PhoneNumber;
        //                    delivererModel.SignInType = (int)RoleTypes.Deliverer;
        //                    delivererModel.Address = model.Address;

        //                    if (httpRequest.Files.Count > 0)
        //                    {
        //                        newFullPath = HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings["DelivererImageFolderPath"] + delivererModel.Id + fileExtension);
        //                        postedFile.SaveAs(newFullPath);
        //                        delivererModel.ProfilePictureUrl = ConfigurationManager.AppSettings["DelivererImageFolderPath"] + delivererModel.Id + fileExtension;
        //                    }
        //                    //delivererModel.ProfilePictureUrl = model.ProfilePictureUrl;
        //                    ctx.SaveChanges();

        //                    CustomResponse<DeliveryMan> response = new CustomResponse<DeliveryMan> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = delivererModel };
        //                    return Ok(response);
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(Utility.LogError(ex));
        //    }
        //}

        ///// <summary>
        ///// Get All Admins
        ///// </summary>
        ///// <returns></returns>
        //[Route("GetAllAdmin")]
        //public async Task<IHttpActionResult> GetAllAdmins()
        //{
        //    try
        //    {
        //        using (BasketContext ctx = new BasketContext())
        //        {
        //            var users = ctx.Users.Where(x => x.SignInType == (int)RoleTypes.SubAdmin || x.SignInType == (int)RoleTypes.SuperAdmin).ToList();

        //            CustomResponse<IEnumerable<User>> response = new CustomResponse<IEnumerable<User>>
        //            {
        //                Message = Global.ResponseMessages.Success,
        //                StatusCode = (int)HttpStatusCode.OK,
        //                Result = users
        //            };

        //            return Ok(response);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(Utility.LogError(ex));
        //    }
        //}


        //[AllowAnonymous]
        //public async Task<IHttpActionResult> ExternalLogin(string userId, string accessToken, int? socialLoginType)
        //{
        //    try
        //    {
        //        if (socialLoginType.HasValue && !string.IsNullOrEmpty(accessToken))
        //        {
        //            SocialLogins socialLogin = new SocialLogins();
        //            var socialUser = await socialLogin.GetSocialUserData(accessToken, (SocialLogins.SocialLoginType)socialLoginType.Value);
        //            if (socialUser != null)
        //            {
        //                using (BasketContext ctx = new BasketContext())
        //                {
        //                    var existingUser = ctx.Users.Include(x => x.UserAddresses).Include(x => x.PaymentCards).FirstOrDefault(x => x.Email == socialUser.email && x.SignInType.Value == (Int16)RoleTypes.User);

        //                    if (existingUser != null)
        //                    {
        //                        await existingUser.GenerateToken(Request);
        //                        CustomResponse<User> response = new CustomResponse<User> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = existingUser };
        //                        return Ok(response);
        //                    }
        //                    else
        //                    {
        //                        var newUser = new User { FullName = socialUser.name, EmailConfirmed = false, PhoneConfirmed = false };
        //                        ctx.Users.Add(newUser);
        //                        ctx.SaveChanges();
        //                        await newUser.GenerateToken(Request);
        //                        return Ok(new CustomResponse<User> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = newUser });
        //                    }
        //                }
        //            }
        //            else
        //                return BadRequest("Unable to get user info");
        //        }
        //        else
        //            return BadRequest("Please provide access token along with social login type");
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(Utility.LogError(ex));
        //    }
        //}

        ///// <summary>
        ///// Contact us
        ///// </summary>
        ///// <param name="model"></param>
        ///// <returns></returns>
        //[HttpPost]
        //[Route("ContactUs")]
        //[AllowAnonymous]
        //public async Task<IHttpActionResult> ContactUs(ContactUsBindingModel model)
        //{
        //    try
        //    {
        //        using (BasketContext ctx = new BasketContext())
        //        {
        //            if (!String.IsNullOrEmpty(model.Description))
        //            {
        //                if (model.UserId.HasValue)
        //                {
        //                    if (model.SignInType == (int)RoleTypes.User)
        //                    {
        //                        //Notification
        //                    }
        //                    else if (model.SignInType == (int)RoleTypes.Deliverer)
        //                    {
        //                        //Notification
        //                    }
        //                    ctx.ContactUs.Add(new ContactUs { UserId = model.UserId.Value, Description = model.Description, CreatedDate = DateTime.Now });
        //                }
        //                else
        //                    ctx.ContactUs.Add(new ContactUs { Description = model.Description, CreatedDate = DateTime.Now });
        //                ctx.SaveChanges();
        //                return Ok(new CustomResponse<string> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK });
        //            }
        //            else
        //                return Ok(new CustomResponse<Error> { Message = Global.ResponseMessages.BadRequest, StatusCode = (int)HttpStatusCode.BadRequest, Result = new Error { ErrorMessage = Global.ResponseMessages.CannotBeEmpty("Description") } });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(Utility.LogError(ex));
        //    }
        //}



        //[Authorize]
        //[HttpPost]
        //[Route("EditUserAddress")]
        //public async Task<IHttpActionResult> EditUserAddress(EditUserAddressBindingModel model)
        //{
        //    try
        //    {
        //        if (!ModelState.IsValid)
        //        {
        //            return BadRequest(ModelState);
        //        }
        //        using (BasketContext ctx = new BasketContext())
        //        {
        //            User user;
        //            DeliveryMan deliverer;

        //            if (model.SignInType == (int)RoleTypes.User)
        //            {
        //                user = ctx.Users.Include(x => x.UserAddresses).Include(x => x.PaymentCards).FirstOrDefault(x => x.Id == model.UserId);

        //                if (user != null)
        //                {
        //                    var address = user.UserAddresses.FirstOrDefault(
        //                        x => x.Id == model.AddressId && x.IsDeleted == false
        //                        );
        //                    if (address != null)
        //                    {
        //                        address.Apartment = model.Apartment;
        //                        address.City = model.City;
        //                        address.Country = model.Country;
        //                        address.Floor = model.Floor;
        //                        address.NearestLandmark = model.NearestLandmark;
        //                        address.BuildingName = model.BuildingName;
        //                        address.StreetName = model.StreetName;
        //                        address.Type = model.AddressType;
        //                        address.IsPrimary = model.IsPrimary;

        //                        ctx.SaveChanges();

        //                        return Ok(new CustomResponse<User> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = user });
        //                    }
        //                    else
        //                        return Ok(new CustomResponse<Error> { Message = Global.ResponseMessages.NotFound, StatusCode = (int)HttpStatusCode.NotFound, Result = new Error { ErrorMessage = Global.ResponseMessages.GenerateInvalid("AddressId") } });
        //                }
        //                else
        //                    return Ok(new CustomResponse<Error> { Message = Global.ResponseMessages.NotFound, StatusCode = (int)HttpStatusCode.NotFound, Result = new Error { ErrorMessage = Global.ResponseMessages.GenerateInvalid("UserId") } });
        //            }
        //            else
        //            {
        //                deliverer = ctx.DeliveryMen.Include(x => x.DelivererAddresses).FirstOrDefault(x => x.Id == model.UserId);

        //                if (deliverer != null)
        //                {
        //                    var address = deliverer.DelivererAddresses.FirstOrDefault(
        //                        x => x.Id == model.AddressId && x.IsDeleted == false
        //                        );
        //                    if (address != null)
        //                    {
        //                        address.Apartment = model.Apartment;
        //                        address.City = model.City;
        //                        address.Country = model.Country;
        //                        address.Floor = model.Floor;
        //                        address.NearestLandmark = model.NearestLandmark;
        //                        address.BuildingName = model.BuildingName;
        //                        address.StreetName = model.StreetName;
        //                        address.Type = model.AddressType;
        //                        address.IsPrimary = model.IsPrimary;

        //                        ctx.SaveChanges();

        //                        return Ok(new CustomResponse<DeliveryMan> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = deliverer });
        //                    }
        //                    else
        //                        return Ok(new CustomResponse<Error> { Message = Global.ResponseMessages.NotFound, StatusCode = (int)HttpStatusCode.NotFound, Result = new Error { ErrorMessage = Global.ResponseMessages.GenerateInvalid("AddressId") } });
        //                }
        //                else
        //                    return Ok(new CustomResponse<Error> { Message = Global.ResponseMessages.NotFound, StatusCode = (int)HttpStatusCode.NotFound, Result = new Error { ErrorMessage = Global.ResponseMessages.GenerateInvalid("UserId") } });
        //            }

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(Utility.LogError(ex));
        //    }
        //}

        ///// <summary>
        ///// An email will be sent to user containing password reset url. Which will redirect user to password reset page.
        ///// </summary>
        ///// <param name="Email">Email of user.</param>
        ///// <returns></returns>
        //[HttpGet]
        //[Route("ResetPasswordThroughEmail")]
        //public async Task<IHttpActionResult> ResetPasswordThroughEmail(string Email)
        //{
        //    try
        //    {
        //        using (BasketContext ctx = new BasketContext())
        //        {
        //            var user = ctx.Users.FirstOrDefault(x => x.Email == Email);

        //            if (user != null)
        //            {
        //                string code = Guid.NewGuid().ToString("N").ToUpper();
        //                var callbackUrl = Url.Link("Default", new { Controller = "ResetPassword", Action = "ResetPassword", code = code });

        //                const string subject = "Reset your password - Basket App";
        //                const string body = "Use this link to reset your password";

        //                var smtp = new SmtpClient
        //                {
        //                    Host = "smtp.gmail.com",
        //                    Port = 587,
        //                    EnableSsl = true,
        //                    DeliveryMethod = SmtpDeliveryMethod.Network,
        //                    UseDefaultCredentials = false,
        //                    Credentials = new NetworkCredential(EmailUtil.FromMailAddress.Address, EmailUtil.FromPassword)
        //                };

        //                var message = new MailMessage(EmailUtil.FromMailAddress, new MailAddress(Email))
        //                {
        //                    Subject = subject,
        //                    Body = body + " " + callbackUrl
        //                };

        //                smtp.Send(message);

        //                user.ForgotPasswordTokens.Add(new ForgotPasswordToken { CreatedAt = DateTime.Now, IsDeleted = false, User_ID = user.Id, Code = code });
        //                ctx.SaveChanges();
        //                return Ok(new CustomResponse<string> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK });
        //            }
        //            else
        //            {
        //                return Ok(new CustomResponse<Error> { Message = "NotFound", StatusCode = (int)HttpStatusCode.NotFound, Result = new Error { ErrorMessage = "User with entered email doesn’t exist." } });
        //            }

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(Utility.LogError(ex));
        //    }
        //}


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

        //        using (BasketContext ctx = new BasketContext())
        //        {
        //            if (model.SignInType == (int)RoleTypes.User)
        //            {
        //                var user = ctx.Users.Include(x => x.UserDevices).FirstOrDefault(x => x.Id == model.User_Id);
        //                if (user != null)
        //                {
        //                    var existingUserDevice = user.UserDevices.FirstOrDefault(x => x.UDID.Equals(model.UDID));
        //                    if (existingUserDevice == null)
        //                    {
        //                        //foreach (var userDevice in user.UserDevices)
        //                        //    userDevice.IsActive = false;

        //                        var userDeviceModel = new UserDevice
        //                        {
        //                            Platform = model.IsAndroidPlatform,
        //                            ApplicationType = model.IsPlayStore ? UserDevice.ApplicationTypes.PlayStore : UserDevice.ApplicationTypes.Enterprise,
        //                            EnvironmentType = model.IsProduction ? UserDevice.ApnsEnvironmentTypes.Production : UserDevice.ApnsEnvironmentTypes.Sandbox,
        //                            UDID = model.UDID,
        //                            AuthToken = model.AuthToken,
        //                            IsActive = true
        //                        };

        //                        PushNotificationsUtil.ConfigurePushNotifications(userDeviceModel);

        //                        user.UserDevices.Add(userDeviceModel);
        //                        ctx.SaveChanges();
        //                        return Ok(new CustomResponse<UserDevice> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = userDeviceModel });
        //                    }
        //                    else
        //                    {
        //                        //foreach (var userDevice in user.UserDevices)
        //                        //    userDevice.IsActive = false;
        //                        await ctx.UserDevices.Where(x => x.UDID == existingUserDevice.UDID).ForEachAsync(x => x.IsActive = false);
        //                        await ctx.SaveChangesAsync();

        //                        existingUserDevice.Platform = model.IsAndroidPlatform;
        //                        existingUserDevice.ApplicationType = model.IsPlayStore ? UserDevice.ApplicationTypes.PlayStore : UserDevice.ApplicationTypes.Enterprise;
        //                        existingUserDevice.EnvironmentType = model.IsProduction ? UserDevice.ApnsEnvironmentTypes.Production : UserDevice.ApnsEnvironmentTypes.Sandbox;
        //                        existingUserDevice.UDID = model.UDID;
        //                        existingUserDevice.AuthToken = model.AuthToken;
        //                        existingUserDevice.IsActive = true;
        //                        existingUserDevice.DeliveryMan_Id = null;
        //                        existingUserDevice.User_Id = user.Id;
        //                        ctx.SaveChanges();
        //                        PushNotificationsUtil.ConfigurePushNotifications(existingUserDevice);
        //                        return Ok(new CustomResponse<UserDevice> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = existingUserDevice });
        //                    }
        //                }

        //                else
        //                    return Ok(new CustomResponse<Error> { Message = Global.ResponseMessages.NotFound, StatusCode = (int)HttpStatusCode.NotFound, Result = new Error { ErrorMessage = Global.ResponseMessages.GenerateNotFound("User") } });
        //            }
        //            else if (model.SignInType == (int)RoleTypes.Deliverer)
        //            {
        //                var deliverer = ctx.DeliveryMen.Include(x => x.DelivererDevices).FirstOrDefault(x => x.Id == model.User_Id);
        //                if (deliverer != null)
        //                {
        //                    var existingDelivererDevice = deliverer.DelivererDevices.FirstOrDefault(x => x.UDID.Equals(model.UDID));
        //                    if (existingDelivererDevice == null)
        //                    {
        //                        //foreach (var userDevice in user.UserDevices)
        //                        //    userDevice.IsActive = false;

        //                        var delivererDeviceModel = new UserDevice
        //                        {
        //                            Platform = model.IsAndroidPlatform,
        //                            ApplicationType = model.IsPlayStore ? UserDevice.ApplicationTypes.PlayStore : UserDevice.ApplicationTypes.Enterprise,
        //                            EnvironmentType = model.IsProduction ? UserDevice.ApnsEnvironmentTypes.Production : UserDevice.ApnsEnvironmentTypes.Sandbox,
        //                            UDID = model.UDID,
        //                            AuthToken = model.AuthToken,
        //                            IsActive = true
        //                        };

        //                        PushNotificationsUtil.ConfigurePushNotifications(delivererDeviceModel);

        //                        deliverer.DelivererDevices.Add(delivererDeviceModel);
        //                        ctx.SaveChanges();
        //                        return Ok(new CustomResponse<UserDevice> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = delivererDeviceModel });
        //                    }
        //                    else
        //                    {
        //                        //foreach (var userDevice in user.UserDevices)
        //                        //    userDevice.IsActive = false;
        //                        await ctx.UserDevices.Where(x => x.UDID == existingDelivererDevice.UDID).ForEachAsync(x => x.IsActive = false);
        //                        await ctx.SaveChangesAsync();

        //                        existingDelivererDevice.Platform = model.IsAndroidPlatform;
        //                        existingDelivererDevice.ApplicationType = model.IsPlayStore ? UserDevice.ApplicationTypes.PlayStore : UserDevice.ApplicationTypes.Enterprise;
        //                        existingDelivererDevice.EnvironmentType = model.IsProduction ? UserDevice.ApnsEnvironmentTypes.Production : UserDevice.ApnsEnvironmentTypes.Sandbox;
        //                        existingDelivererDevice.UDID = model.UDID;
        //                        existingDelivererDevice.AuthToken = model.AuthToken;
        //                        existingDelivererDevice.IsActive = true;
        //                        existingDelivererDevice.DeliveryMan_Id = deliverer.Id;
        //                        existingDelivererDevice.User_Id = null;
        //                        ctx.SaveChanges();
        //                        PushNotificationsUtil.ConfigurePushNotifications(existingDelivererDevice);
        //                        return Ok(new CustomResponse<UserDevice> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = existingDelivererDevice });
        //                    }
        //                }

        //                else
        //                    return Ok(new CustomResponse<Error> { Message = Global.ResponseMessages.NotFound, StatusCode = (int)HttpStatusCode.NotFound, Result = new Error { ErrorMessage = Global.ResponseMessages.GenerateNotFound("Deliverer") } });

        //            }
        //            else
        //                return Ok(new CustomResponse<Error> { Message = Global.ResponseMessages.NotFound, StatusCode = (int)HttpStatusCode.NotFound, Result = new Error { ErrorMessage = Global.ResponseMessages.GenerateInvalid("SignInType") } });
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
        //    try
        //    {
        //        using (BasketContext ctx = new BasketContext())
        //        {
        //            var existingUser = ctx.Users.Include(x => x.UserAddresses).Include(x => x.PaymentCards).FirstOrDefault(x => x.Id == UserId && x.IsDeleted == false);
        //            if (existingUser != null)
        //            {
        //                var existingCard = existingUser.PaymentCards.FirstOrDefault(x => x.Id == CardId && x.IsDeleted == false);
        //                if (existingCard != null)
        //                    existingCard.IsDeleted = true;
        //                else
        //                    return Ok(new CustomResponse<Error> { Message = Global.ResponseMessages.NotFound, StatusCode = (int)HttpStatusCode.NotFound, Result = new Error { ErrorMessage = Global.ResponseMessages.GenerateNotFound("Card") } });
        //                ctx.SaveChanges();
        //                return Ok(new CustomResponse<User> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = existingUser });
        //            }
        //            else
        //                return Ok(new CustomResponse<Error> { Message = Global.ResponseMessages.NotFound, StatusCode = (int)HttpStatusCode.NotFound, Result = new Error { ErrorMessage = Global.ResponseMessages.GenerateNotFound("User") } });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(Utility.LogError(ex));
        //    }
        //}

        //[Authorize]
        //[HttpGet]
        //[Route("DeleteUserAddress")]
        //public async Task<IHttpActionResult> DeleteUserAddress(int UserId, int AddressId)
        //{
        //    try
        //    {
        //        using (BasketContext ctx = new BasketContext())
        //        {
        //            var existingUser = ctx.Users.Include(x => x.UserAddresses).Include(x => x.PaymentCards).FirstOrDefault(x => x.Id == UserId && x.IsDeleted == false);
        //            if (existingUser != null)
        //            {
        //                var existingAddress = existingUser.UserAddresses.FirstOrDefault(x => x.Id == AddressId && x.IsDeleted == false);
        //                if (existingAddress != null)
        //                    existingAddress.IsDeleted = true;
        //                else
        //                    return Ok(new CustomResponse<Error> { Message = Global.ResponseMessages.NotFound, StatusCode = (int)HttpStatusCode.NotFound, Result = new Error { ErrorMessage = Global.ResponseMessages.GenerateNotFound("Address") } });

        //                ctx.SaveChanges();
        //                return Ok(new CustomResponse<User> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = existingUser });
        //            }
        //            else
        //                return Ok(new CustomResponse<Error> { Message = Global.ResponseMessages.NotFound, StatusCode = (int)HttpStatusCode.NotFound, Result = new Error { ErrorMessage = Global.ResponseMessages.GenerateNotFound("User") } });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(Utility.LogError(ex));
        //    }
        //}

        ////[Authorize]
        //[HttpGet]
        //[Route("GetUser")]
        //public async Task<IHttpActionResult> GetUser(int UserId, int SignInType)
        //{
        //    try
        //    {
        //        using (BasketContext ctx = new BasketContext())
        //        {
        //            BasketSettings.LoadSettings();

        //            if (SignInType == (int)RoleTypes.User)
        //            {
        //                var user = ctx.Users.Include(x => x.Orders).Include(x => x.Favourites.Select(y => y.Product)).Include(x => x.StoreRatings.Select(y => y.Store)).Include(x => x.DeliveryManRatings.Select(y => y.DeliveryMan)).Include(x => x.ProductRatings.Select(y => y.Product)).Include(x => x.UserRatings.Select(y => y.User)).Include(x => x.AppRatings).Include(x => x.UserAddresses).Include(x => x.PaymentCards).FirstOrDefault(x => x.Id == UserId && x.IsDeleted == false);
        //                user.BasketSettings = new Settings
        //                {
        //                    Id = BasketSettings.Id,
        //                    Currency = BasketSettings.Currency,
        //                    DeliveryFee = BasketSettings.DeliveryFee,
        //                    ServiceFee = BasketSettings.ServiceFee,
        //                    MinimumOrderPrice = BasketSettings.MinimumOrderPrice,
        //                    NearByRadius = BasketSettings.NearByRadius * 1609.344,
        //                    AboutUs = BasketSettings.AboutUs,
        //                    Help = BasketSettings.Help
        //                };
        //                return Ok(new CustomResponse<User> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = user });
        //            }
        //            else
        //            {
        //                var Deliverer = ctx.DeliveryMen.Include(x => x.DelivererAddresses).FirstOrDefault(x => x.Id == UserId && x.IsDeleted == false);
        //                Deliverer.SignInType = (int)RoleTypes.Deliverer;
        //                Deliverer.BasketSettings = new Settings
        //                {
        //                    Id = BasketSettings.Id,
        //                    Currency = BasketSettings.Currency,
        //                    DeliveryFee = BasketSettings.DeliveryFee,
        //                    ServiceFee = BasketSettings.ServiceFee,
        //                    MinimumOrderPrice = BasketSettings.MinimumOrderPrice,
        //                    NearByRadius = BasketSettings.NearByRadius * 1609.344,
        //                    AboutUs = BasketSettings.AboutUs,
        //                    Help = BasketSettings.Help
        //                };
        //                return Ok(new CustomResponse<DeliveryMan> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = Deliverer });
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(Utility.LogError(ex));
        //    }
        //}

        //[HttpGet]
        //[Route("MarkDeviceAsInActive")]
        //public async Task<IHttpActionResult> MarkDeviceAsInActive(int UserId, int DeviceId)
        //{
        //    try
        //    {
        //        using (BasketContext ctx = new BasketContext())
        //        {
        //            var device = ctx.UserDevices.FirstOrDefault(x => x.Id == DeviceId && x.User_Id == UserId);
        //            if (device != null)
        //            {
        //                device.IsActive = false;
        //                ctx.SaveChanges();
        //                return Ok(new CustomResponse<string> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK });
        //            }
        //            else
        //                return Ok(new CustomResponse<Error> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = new Error { ErrorMessage = "Invalid UserId or DeviceId." } });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(Utility.LogError(ex));
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

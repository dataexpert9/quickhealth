using AdminWebapi.Models;
using AdminWebapi.ViewModels;
using BusinessLogic.CommonServices;
using BusinessLogic.HelperServices;
using DBAccess;
using DBAccess.Models;
using DBAccess.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Data.Entity;


namespace AdminWebapi.Controllers
{

    [RoutePrefix("api/Common")]
    [ExceptionHandlingFilter]
    public class CommonController : ApiController
    {
        private readonly IImageUpload _ImageUpload;
        private readonly ICommonServices _CommonServices;

        public CommonController(IImageUpload imageUpload, ICommonServices CommonServices)
        {
            _ImageUpload = imageUpload;
            _CommonServices = CommonServices;
        }
        public CustomResponse<string> uploadImage(ImageUploadRequest request)
        {
            var imagePath = _ImageUpload.uploadImage(request.Base64EncodedString, request.FolderNameWhereToSave);
            return new CustomResponse<string> { Message = GlobalUtility.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = imagePath };
        }


        [HttpGet]
        [Route("GetAppSettings")]
        public async Task<IHttpActionResult> GetAppSettings()
        {
            try
            {
                var AppSettings = _CommonServices.GetAppSettings();
                return Ok(new CustomResponse<SettingsViewModel> { Message = GlobalUtility.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = new SettingsViewModel { AppSettings = AppSettings } });

            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }



        /// <summary>
        /// Create Chat Channel
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>

        [Route("CreateChannel")]
        [HttpPost]
        public async Task<IHttpActionResult> CreateChannel(CreateChannelBindingModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                ChatViewModel returnModel = new ChatViewModel();
                returnModel.Chat = _CommonServices.CreateChannel(model);
                return Ok(new CustomResponse<ChatViewModel> { Message = GlobalUtility.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = returnModel});
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }

        [Route("RateApp")]
        [HttpPost]
        public async Task<IHttpActionResult> RateApp(AppRatingModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var RatingModel = _CommonServices.RateApp(model);
                return Ok(new CustomResponse<string> { Message = GlobalUtility.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = "Rating submitted successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }



        ///// <summary>
        ///// Register for getting push notifications
        ///// </summary>
        ///// <param name="model"></param>
        ///// <returns></returns>
        [HttpPost]
        [Route("RegisterPushNotification")]
        public async Task<IHttpActionResult> RegisterPushNotification(RegisterPushNotificationBindingModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                using (AdminDBContext ctx = new AdminDBContext())
                {
                    var user = ctx.Users.Include(x => x.UserDevices).FirstOrDefault(x => x.Id == model.User_Id);
                    if (user != null)
                    {
                        var existingUserDevice = user.UserDevices.FirstOrDefault(x => x.UDID.Equals(model.UDID));
                        if (existingUserDevice == null)
                        {
                            //foreach (var userDevice in user.UserDevices)
                            //    userDevice.IsActive = false;

                            var userDeviceModel = new UserDevice
                            {
                                Platform = model.IsAndroidPlatform,
                                ApplicationType = model.IsPlayStore ? UserDevice.ApplicationTypes.PlayStore : UserDevice.ApplicationTypes.Enterprise,
                                EnvironmentType = model.IsProduction ? UserDevice.ApnsEnvironmentTypes.Production : UserDevice.ApnsEnvironmentTypes.Sandbox,
                                UDID = model.UDID,
                                AuthToken = model.AuthToken,
                                IsActive = true
                            };

                            PushNotificationsUtil.ConfigurePushNotifications(userDeviceModel);

                            user.UserDevices.Add(userDeviceModel);
                            ctx.SaveChanges();
                            return Ok(new CustomResponse<UserDevice> { Message = GlobalUtility.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = userDeviceModel });
                        }
                        else
                        {
                            //foreach (var userDevice in user.UserDevices)
                            //    userDevice.IsActive = false;

                            existingUserDevice.Platform = model.IsAndroidPlatform;
                            existingUserDevice.ApplicationType = model.IsPlayStore ? UserDevice.ApplicationTypes.PlayStore : UserDevice.ApplicationTypes.Enterprise;
                            existingUserDevice.EnvironmentType = model.IsProduction ? UserDevice.ApnsEnvironmentTypes.Production : UserDevice.ApnsEnvironmentTypes.Sandbox;
                            existingUserDevice.UDID = model.UDID;
                            existingUserDevice.AuthToken = model.AuthToken;
                            existingUserDevice.IsActive = true;
                            ctx.SaveChanges();
                            PushNotificationsUtil.ConfigurePushNotifications(existingUserDevice);
                            return Ok(new CustomResponse<UserDevice> { Message = GlobalUtility.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = existingUserDevice });
                        }
                    }
                    else
                        return Ok(new CustomResponse<Error> { Message = GlobalUtility.ResponseMessages.NotFound, StatusCode = (int)HttpStatusCode.NotFound, Result = new Error { ErrorMessage = GlobalUtility.ResponseMessages.GenerateNotFound("User") } });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }

        [Authorize]
        [HttpGet]
        [Route("GetAllNotifications")]
        public async Task<IHttpActionResult> GetAllNotifications(int? User_Id=0,int? Doctor_Id=0)
        {
            try
            {
                var Notifications = _CommonServices.GetAllNotifications(User_Id,Doctor_Id);
                return Ok(new CustomResponse<NotificationsViewModel> { Message = GlobalUtility.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = new NotificationsViewModel { Notifications= Notifications ,Count= Notifications.Count} });
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }


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

        //        using (BagAdviserContext ctx = new BagAdviserContext())
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

        //                        existingUserDevice.Platform = model.IsAndroidPlatform;
        //                        existingUserDevice.ApplicationType = model.IsPlayStore ? UserDevice.ApplicationTypes.PlayStore : UserDevice.ApplicationTypes.Enterprise;
        //                        existingUserDevice.EnvironmentType = model.IsProduction ? UserDevice.ApnsEnvironmentTypes.Production : UserDevice.ApnsEnvironmentTypes.Sandbox;
        //                        existingUserDevice.UDID = model.UDID;
        //                        existingUserDevice.AuthToken = model.AuthToken;
        //                        existingUserDevice.IsActive = true;
        //                        //existingUserDevice.DeliveryMan_Id = null;
        //                        existingUserDevice.User_Id = user.Id;
        //                        ctx.SaveChanges();
        //                        PushNotificationsUtil.ConfigurePushNotifications(existingUserDevice);
        //                        return Ok(new CustomResponse<UserDevice> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = existingUserDevice });
        //                    }
        //                }

        //                else
        //                    return Ok(new CustomResponse<Error> { Message = Global.ResponseMessages.NotFound, StatusCode = (int)HttpStatusCode.NotFound, Result = new Error { ErrorMessage = Global.ResponseMessages.GenerateNotFound("User") } });
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


    }
}

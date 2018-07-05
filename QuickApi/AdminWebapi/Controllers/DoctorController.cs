using AdminWebapi.Models;
using AdminWebapi.ViewModels;
using AutoMapper;
using BusinessLogic.CommonServices;
using BusinessLogic.HelperServices;
using BusinessLogic.UserServices;
using DBAccess.Models;
using DBAccess.ViewModels;
using DBAccess.ViewModels.Doctor;
using DBAccess.ViewModels.User;
using PubnubApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace AdminWebapi.Controllers
{
    [RoutePrefix("api/Doctor")]
    [ExceptionHandlingFilter]
    public class DoctorController : ApiController
    {
        private ApplicationUserManager _userManager;

        private readonly IDoctorService _DoctorService;
        private readonly IImageUpload _ImageUpload;
        public DoctorController(IDoctorService doctorService, IImageUpload imageUpload)
        {
            _ImageUpload = imageUpload;
            _DoctorService = doctorService;
        }


        [Route("RegisterAsDoctor")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<IHttpActionResult> RegisterAsDoctor(RegisterDoctorBindingModel model)
        {
            try
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
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }

        [HttpGet]
        [Route("UpdateAvailabilityStatus")]
        public async Task<IHttpActionResult> UpdateAvailabilityStatus(bool IsAvailable)
        {
            try
            {
                var Doctor_Id = Convert.ToInt32(User.GetClaimValue("userid"));

                var doctor = _DoctorService.UpdateAvailabilityStatus(Doctor_Id, IsAvailable);

                if (doctor != null)
                    return Ok(new CustomResponse<Doctor> { Message = GlobalUtility.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = doctor });
                else
                    return Ok(new CustomResponse<Error> { Message = "NotFound", StatusCode = (int)HttpStatusCode.NotFound, Result = new Error { ErrorMessage = "Doctor with id doesnt exists." } });
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }

        [Route("UpdateUserProfile")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<IHttpActionResult> UpdateUserProfile(EditDoctorProfileBindingModel model)
        {
            try
            {
                model.Id = Convert.ToInt32(User.GetClaimValue("userid"));

                if (model.Id == 0)
                    throw new Exception("User id is empty in user.identity.");


                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var doctorModel = _DoctorService.UpdateDoctorProfile(model);

                if (doctorModel != null)
                {
                    await doctorModel.GenerateToken(Request);
                    CustomResponse<Doctor> response = new CustomResponse<Doctor> { Message = GlobalUtility.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = doctorModel };
                    return Ok(response);
                }
                else
                    throw new Exception("Doctor id is empty in doctor.identity.");
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }


        [HttpGet]
        [Route("GetRequestQueries")]
        public async Task<IHttpActionResult> GetRequestQueries()
        {
            try
            {
                var Doctor_Id = Convert.ToInt32(User.GetClaimValue("userid"));
                //RequestInquiries returnModel = new RequestInquiries();
                MapperAppointmentViewModel response = new MapperAppointmentViewModel();
                var Inquiries = _DoctorService.GetRequestQueries();

                List<AppointmentHistoryViewModel> returnModel = Mapper.Map<List<Appointment>, List<AppointmentHistoryViewModel>>(Inquiries);
                response.Inquiries = returnModel;

                return Ok(new CustomResponse<MapperAppointmentViewModel> { Message = GlobalUtility.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = response });
                //return Ok(new CustomResponse<RequestInquiries> { Message = GlobalUtility.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = returnModel });
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }

        [HttpGet]
        [Route("GetHistory")]
        public async Task<IHttpActionResult> GetHistory(int? Doctor_Id = 0)
        {
            try
            {
                if (Doctor_Id == 0 || !Doctor_Id.HasValue)
                    Doctor_Id = Convert.ToInt32(User.GetClaimValue("userid"));
                MapperAppointmentViewModel response = new MapperAppointmentViewModel();

                var Inquiries = _DoctorService.GetHistory(Doctor_Id.Value);
                List<AppointmentHistoryViewModel> returnModel = Mapper.Map<List<Appointment>, List<AppointmentHistoryViewModel>>(Inquiries);
                response.Inquiries = returnModel;
                return Ok(new CustomResponse<MapperAppointmentViewModel> { Message = GlobalUtility.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = response });
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }

        [HttpGet]
        [Route("PubNub")]
        public async Task<IHttpActionResult> PubNub()
        {
            try
            {
                //    PNConfiguration pnConfiguration = new PNConfiguration();
                //    pnConfiguration.SubscribeKey = "sub-c-f956c704-63f5-11e8-90b6-8e3ee2a92f04";
                //    pnConfiguration.PublishKey = "pub-c-ee31281d-c11a-4bd8-895f-cca3f587ce5a";
                //    pnConfiguration.SecretKey = "sec-c-ZDMzZWY3MGUtNzExZi00ZDA3LWFiMTktYTFmMzMwOTVjZTc3";
                //    pnConfiguration.LogVerbosity = PNLogVerbosity.BODY;


                //    Dictionary<string, string> message = new Dictionary<string, string>();
                //    message.Add("msg", "hello");

                //    Pubnub pubnub = new Pubnub(pnConfiguration);

                //    SubscribeCallbackExt subscribeCallback = new SubscribeCallbackExt(
                //        (pubnubObj, messageResult) => {
                //            if (messageResult != null)
                //            {
                //                System.Diagnostics.Debug.WriteLine("In Example, SusbcribeCallback received PNMessageResult");
                //                System.Diagnostics.Debug.WriteLine("In Example, SusbcribeCallback messsage channel = " + messageResult.Channel);
                //                string jsonString = messageResult.Message.ToString();
                //                Dictionary<string, string> msg = pubnub.JsonPluggableLibrary.DeserializeToObject<Dictionary<string, string>>(jsonString);
                //                System.Diagnostics.Debug.WriteLine("msg: " + msg["msg"]);
                //            }
                //        },
                //        (pubnubObj, presencResult) => {
                //            if (presencResult != null)
                //            {
                //                System.Diagnostics.Debug.WriteLine("In Example, SusbcribeCallback received PNPresenceEventResult");
                //                System.Diagnostics.Debug.WriteLine(presencResult.Channel + " " + presencResult.Occupancy + " " + presencResult.Event);
                //            }
                //        },
                //        (pubnubObj, statusResult) => {
                //            if (statusResult.Category == PNStatusCategory.PNConnectedCategory)
                //            {
                //                pubnub.Publish()
                //                .Channel("TEST:QUICK_HEALTH")
                //                .Message(message)
                //                .Async(new PNPublishResultExt((publishResult, publishStatus) => {
                //                    if (!publishStatus.Error)
                //                    {
                //                        System.Diagnostics.Debug.WriteLine(string.Format("DateTime {0}, In Publish Example, Timetoken: {1}", DateTime.UtcNow, publishResult.Timetoken));
                //                    }
                //                    else
                //                    {
                //                        System.Diagnostics.Debug.WriteLine(publishStatus.Error);
                //                        System.Diagnostics.Debug.WriteLine(publishStatus.ErrorData.Information);
                //                    }
                //                }));
                //            }
                //        }
                //    );

                //    pubnub.AddListener(subscribeCallback);

                //    pubnub.Subscribe<string>()
                //        .Channels(new string[]{
                //"TEST:QUICK_HEALTH"
                //        }).Execute();


                //    pubnub.History()
                //    .Channel("TEST:QUICK_HEALTH")
                //    .Count(100)
                //    .Async(new PNHistoryResultExt(
                //        (result, status) => {
                //        }
                //    ));
                Global.objPubnub.Publish();
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }


        [Route("AcceptAppointment")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<IHttpActionResult> AcceptAppointment(AcceptAppointmentBindingModel model)
        {
            try
            {

                if (model.Doctor_Id == 0)
                    model.Doctor_Id = Convert.ToInt32(User.GetClaimValue("userid"));

                if (model.Doctor_Id == 0)
                    throw new Exception("User id is empty in user.identity.");

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var Appointment = _DoctorService.AcceptAppointment(model);

                if (Appointment != null)
                {
                    CustomResponse<AppointmentViewModel> response = new CustomResponse<AppointmentViewModel> { Message = GlobalUtility.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = new AppointmentViewModel { Appointment = Appointment } };
                    return Ok(response);
                }
                else
                {
                    return Content(HttpStatusCode.OK, new CustomResponse<Error>
                    {
                        Message = "Conflict",
                        StatusCode = (int)HttpStatusCode.Conflict,
                        Result = new Error { ErrorMessage = "Appointment is already in progress." }
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }



        [Route("DoctorPrescriptions")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<IHttpActionResult> DoctorPrescriptions(DoctorPrescriptionBindingModfel model)
        {
            try
            {
                if (!model.Doctor_Id.HasValue || model.Doctor_Id == 0)
                    model.Doctor_Id = Convert.ToInt32(User.GetClaimValue("userid"));

                if (model.Doctor_Id == 0)
                    throw new Exception("User id is empty in user.identity.");

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var Appointment = _DoctorService.DoctorPrescriptions(model);

                if (Appointment)
                {
                    CustomResponse<string> response = new CustomResponse<string> { Message = GlobalUtility.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = "Appointment updated successfully." };
                    return Ok(response);
                }
                else
                {
                    return Content(HttpStatusCode.OK, new CustomResponse<Error>
                    {
                        Message = "NotFound",
                        StatusCode = (int)HttpStatusCode.NotFound,
                        Result = new Error { ErrorMessage = "Appointment not found." }
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }
        
        [Route("DoctorCloseChat")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<IHttpActionResult> DoctorCloseChat(DoctorCloseChatViewModel model)
        {
            try
            {
                if (model.Doctor_Id == 0)
                    model.Doctor_Id = Convert.ToInt32(User.GetClaimValue("userid"));

                if (model.Doctor_Id == 0)
                    throw new Exception("Doctor id is empty in user.identity.");

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var Appointment = _DoctorService.DoctorCloseChat(model);

                if (Appointment)
                {
                    CustomResponse<string> response = new CustomResponse<string> { Message = GlobalUtility.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = "Appointment updated successfully." };
                    return Ok(response);
                }
                else
                {
                    return Content(HttpStatusCode.OK, new CustomResponse<Error>
                    {
                        Message = "NotFound",
                        StatusCode = (int)HttpStatusCode.NotFound,
                        Result = new Error { ErrorMessage = "Appointment not found." }
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }

        [Route("RateDoctor")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<IHttpActionResult> RateDoctor(RateDoctorBindingModel model)
        {
            try
            {
                if (model.User_Id == 0)
                    model.User_Id= Convert.ToInt32(User.GetClaimValue("userid"));

                if (model.User_Id == 0)
                    throw new Exception("User id is empty in user.identity.");

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var Appointment = _DoctorService.RateDoctor(model);

                if (Appointment)
                {
                    CustomResponse<string> response = new CustomResponse<string> { Message = GlobalUtility.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = "Doctor's rating updated successfully." };
                    return Ok(response);
                }
                else
                {
                    return Content(HttpStatusCode.OK, new CustomResponse<Error>
                    {
                        Message = "NotFound",
                        StatusCode = (int)HttpStatusCode.NotFound,
                        Result = new Error { ErrorMessage = "Doctor not found." }
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }


        [Route("GetDoctorProfile")]
        [HttpGet]
        public async Task<IHttpActionResult> GetDoctorProfile(int? DoctorId = 0)
        {
            try
            {
                GetDoctorProfileModel returnModel = new GetDoctorProfileModel();

                if (DoctorId == 0)
                    DoctorId = Convert.ToInt32(User.GetClaimValue("userid"));

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                returnModel.Doctor = _DoctorService.GetDoctorProfile(DoctorId.Value);
                return Ok(new CustomResponse<GetDoctorProfileModel> { Message = GlobalUtility.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = returnModel });

            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }



    }
}

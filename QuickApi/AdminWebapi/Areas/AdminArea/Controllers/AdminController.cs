using AdminWebapi.Areas.AdminArea.ViewModels;
using BusinessLogic.AdminService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using DBAccess.Models;
using AdminWebapi.Models;
using BusinessLogic.HelperServices;
using AdminWebapi.ViewModels;
using DBAccess.ViewModels.Admin;
using BusinessLogic.ViewModels;
using static BusinessLogic.HelperServices.Utility;
using System.Globalization;
using DBAccess.ViewModels;
using DBAccess.ViewModels.Order;

namespace AdminWebapi.Areas.AdminArea.Controllers
{
    [RoutePrefix("api/Admin")]
    [ExceptionHandlingFilter]
    public class AdminController : ApiController
    {
        private ApplicationUserManager _userManager;
        private readonly IAdminService _AdminService;

        public AdminController(IAdminService AdminService)
        {
            _AdminService = AdminService;
        }

        /// <summary>
        /// Login for web admin panel
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [Route("WebPanelLogin")]
        [HttpPost]
        public async Task<IHttpActionResult> WebPanelLogin(WebLoginBindingModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                Admin AdminModel = _AdminService.WebPanelLogin(model);
                if (AdminModel != null)
                {
                    await AdminModel.GenerateToken(Request);
                    return Ok(new CustomResponse<Admin> { Message = GlobalUtility.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = AdminModel });
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
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }


        /// <summary>
        /// Get Dashboard Stats
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        [Route("GetAdminDashboardStats")]
        public async Task<IHttpActionResult> GetAdminDashboardStats(int? AdminId = 0)
        {
            try
            {

                WebDashboardStatsViewModel model = new WebDashboardStatsViewModel();

                if (AdminId.HasValue)
                    model = _AdminService.GetAdminDashboardStats(AdminId.Value);

                if (AdminId.HasValue)
                {
                    var TopPharmacies = _AdminService.GetTopPharmacies();

                    foreach (var item in TopPharmacies)
                    {
                        
                        model.AdminDashboard.TopPharmacies.Add(new TopPharmaciesViewModel {
                            AverageRating=item.AverageRating,
                            Name=item.Name,
                        });
                    }

                    var DoctorList = _AdminService.GetTopDoctors();

                    foreach (var doctor in DoctorList)
                    {
                        model.AdminDashboard.TopDoctors.Add(new TopDoctorsViewModel
                        {
                            Email = doctor.Email,
                            Name = doctor.FullName,
                            Rating = doctor.AverageRating
                        });
                    }
                }
                if (model != null)
                    return Ok(new CustomResponse<WebDashboardStatsViewModel> { Message = GlobalUtility.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = model });
                else
                    return Content(HttpStatusCode.NotFound, new CustomResponse<Error> { Message = "NotFound", StatusCode = (int)HttpStatusCode.Forbidden, Result = new Error { ErrorMessage = "Admin not found." } });

            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }


        /// <summary>
        /// Get Pharmacy Requests
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        [Route("GetPharmacyRequests")]
        public async Task<IHttpActionResult> GetPharmacyRequests(string StartDate, string EndDate, string Purpose = "", int? Pharmacy_Id = 0, int? Request_Id = 0)
        {
            try
            {
                //DateTime app = new DateTime().Date;

                //if (!string.IsNullOrEmpty(DateOfAppointment))
                //    app = Convert.ToDateTime(DateOfAppointment);
                DateTime StartDateOfAppointment;
                StartDateOfAppointment = DateTime.ParseExact(StartDate, "d/MM/yyyy h:mm:ss tt", CultureInfo.InvariantCulture);

                DateTime EndDateOfAppointment;
                EndDateOfAppointment = DateTime.ParseExact(EndDate, "d/MM/yyyy h:mm:ss tt", CultureInfo.InvariantCulture);


                var PharmacyRequests = _AdminService.AdminPharmacyRequests(StartDateOfAppointment, EndDateOfAppointment, Pharmacy_Id, Purpose);
                List<AdimPharmacyRequestViewModel> returnModel = AutoMapper.Mapper.Map<List<PharmacyRequest>, List<AdimPharmacyRequestViewModel>>(PharmacyRequests);

                return Ok(new CustomResponse<List<AdimPharmacyRequestViewModel>> { Message = GlobalUtility.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = returnModel });

            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }

        /// <summary>
        /// Get Pharmacy Requests
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        [Route("GetSinglePharmacyRequests")]
        public async Task<IHttpActionResult> GetSinglePharmacyRequests(int Request_Id)
        {
            try
            {
                var PharmacyRequests = _AdminService.GetSinglePharmacyRequests(Request_Id);
                AdimPharmacyRequestViewModel returnModel = AutoMapper.Mapper.Map<PharmacyRequest, AdimPharmacyRequestViewModel>(PharmacyRequests);
                return Ok(new CustomResponse<AdimPharmacyRequestViewModel> { Message = GlobalUtility.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = returnModel });
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }


        [HttpGet]
        [Authorize]
        [Route("GetSingleDoctorPrescriptions")]
        public async Task<IHttpActionResult> GetSingleDoctorPrescriptions(int Appointment_Id)
        {
            try
            {
                var DoctorPrescription = _AdminService.GetSingleDoctorPrescriptions(Appointment_Id);
                return Ok(new CustomResponse<DoctorPrescription> { Message = GlobalUtility.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = DoctorPrescription });
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }



        [HttpGet]
        [Authorize]
        [Route("GetTopDoctors")]
        public async Task<IHttpActionResult> GetTopDoctors()
        {
            try
            {

                List<TopDoctorsViewModel> model = new List<TopDoctorsViewModel>();

                var DoctorList = _AdminService.GetTopDoctors();

                foreach (var doctor in DoctorList)
                {
                    model.Add(new TopDoctorsViewModel
                    {
                        Email = doctor.Email,
                        Name = doctor.FullName,
                        Rating = doctor.AverageRating
                    });
                }

                return Ok(new CustomResponse<List<TopDoctorsViewModel>> { Message = GlobalUtility.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = model });

            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }



        /// <summary>
        /// Login for web admin panel
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize]
        [Route("GetPrescribedMedicines")]
        [HttpPost]
        public async Task<IHttpActionResult> GetPrescribedMedicines(AssignPharmacyRequestBindingModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var PharmacyRequest = _AdminService.GetPrescribedMedicines(model.Pharmacy_Id, model.PharmacyRequest_Id);

                if (PharmacyRequest != null)
                {
                    return Ok(new CustomResponse<List<PharmacyPrescription>> { Message = GlobalUtility.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = PharmacyRequest });
                }
                else
                {
                    return Content(HttpStatusCode.OK, new CustomResponse<Error>
                    {
                        Message = "NotFound",
                        StatusCode = (int)HttpStatusCode.NotFound,
                        Result = new Error { ErrorMessage = "Pharmacy request not found." }
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }



        /// <summary>
        /// Add admin
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("AddAdmin")]
        public async Task<IHttpActionResult> AddAdmin(AddAdminBindingModel model)
        {
            try
            {
                var Admin = _AdminService.AddAdmin(model);
                if (Admin != null)
                {
                    return Ok(new CustomResponse<Admin> { Message = GlobalUtility.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = Admin });
                }
                else
                {
                    return Content(HttpStatusCode.OK, new CustomResponse<Error>
                    {
                        Message = "Conflict",
                        StatusCode = (int)HttpStatusCode.Conflict,
                        Result = new Error { ErrorMessage = "Admin with email already exists." }
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }

        [HttpGet]
        [Authorize]
        [Route("SearchAdmins")]
        public async Task<IHttpActionResult> SearchAdmins(string FirstName, string LastName, string Email, string Phone, int? PharmacyId = 0)
        {
            try
            {

                SearchAdminsViewModel model = new SearchAdminsViewModel();

                var AdminList = _AdminService.SearchAdmins(FirstName, LastName, Email, Phone, PharmacyId);
                return Ok(new CustomResponse<SearchAdminsViewModel> { Message = GlobalUtility.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = AdminList });

            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }


        [HttpGet]
        [Authorize]
        [Route("GetEntityById")]
        public async Task<IHttpActionResult> GetEntityById(int EntityType, int Id)
        {
            try
            {

                switch (EntityType)
                {

                    case (int)BasketEntityTypes.Pharmacist:
                        var Pharmacist = _AdminService.GetEntityById(EntityType, Id);
                        return Ok(new CustomResponse<Admin> { Message = GlobalUtility.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = (Admin)Pharmacist });
                    default:
                        return Content(HttpStatusCode.OK, new CustomResponse<Error>
                        {
                            Message = "NotFound",
                            StatusCode = (int)HttpStatusCode.NotFound,
                            Result = new Error { ErrorMessage = "Invalid Request." }
                        });

                }

                //return Ok(new CustomResponse<SearchAdminsViewModel> { Message = GlobalUtility.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = AdminList });

            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }



        [HttpPost]
        [Route("PrescribeMedicine")]
        public async Task<IHttpActionResult> PrescribeMedicine(AddPrescribtionBindingModel model)
        {
            try
            {
                var PharmacyPrescription = _AdminService.PrescribeMedicine(model);
                if (PharmacyPrescription != null)
                {
                    return Ok(new CustomResponse<PharmacyPrescription> { Message = GlobalUtility.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = PharmacyPrescription });
                }
                else
                {
                    return Content(HttpStatusCode.OK, new CustomResponse<Error>
                    {
                        Message = "Conflict",
                        StatusCode = (int)HttpStatusCode.Conflict,
                        Result = new Error { ErrorMessage = "Admin with email already exists." }
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }


        [HttpGet]
        [Authorize]
        [Route("GetPharmacyHistory")]
        public async Task<IHttpActionResult> GetPharmacyHistory(int Pharmacy_Id = 0)
        {
            try
            {

                var PharmacyRequests = _AdminService.GetPharmacyHistory(Pharmacy_Id);
                List<AdimPharmacyRequestViewModel> returnModel = AutoMapper.Mapper.Map<List<PharmacyRequest>, List<AdimPharmacyRequestViewModel>>(PharmacyRequests);
                return Ok(new CustomResponse<List<AdimPharmacyRequestViewModel>> { Message = GlobalUtility.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = returnModel });

            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }

        [HttpGet]
        [Authorize]
        [Route("GetPharmacyRequestDetails")]
        public async Task<IHttpActionResult> GetPharmacyRequestDetails(int Request_Id = 0)
        {
            try
            {

                var PharmacyRequests = _AdminService.GetPharmacyRequestDetails(Request_Id);
                AdimPharmacyRequestViewModel returnModel = AutoMapper.Mapper.Map<PharmacyRequest, AdimPharmacyRequestViewModel>(PharmacyRequests);
                return Ok(new CustomResponse<AdimPharmacyRequestViewModel> { Message = GlobalUtility.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = returnModel });

            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }



        //[HttpGet]
        //[Authorize]
        //[Route("GetSuperAdminDashboardData")]
        //public async Task<IHttpActionResult> GetSuperAdminDashboardData()
        //{
        //    try
        //    {

        //        var PharmacyRequests = _AdminService.GetSuperAdminDashboardData();
        //        AdimPharmacyRequestViewModel returnModel = AutoMapper.Mapper.Map<PharmacyRequest, AdimPharmacyRequestViewModel>(PharmacyRequests);
        //        return Ok(new CustomResponse<AdimPharmacyRequestViewModel> { Message = GlobalUtility.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = returnModel });

        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(Utility.LogError(ex));
        //    }
        //}

        [HttpGet]
        [Authorize]
        [Route("UpdateStatus")]
        public async Task<IHttpActionResult> UpdateStatus(int PharmacyRequest_Id, int Status)
        {
            try
            {

                var PharmacyRequests = _AdminService.UpdateStatus(PharmacyRequest_Id, Status);
                if (PharmacyRequests)
                    return Ok(new CustomResponse<string> { Message = GlobalUtility.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = "Updated Successfully" });
                else
                    return Ok(new CustomResponse<Error> { Message = "Forbidden", StatusCode = (int)HttpStatusCode.Forbidden, Result = new Error { ErrorMessage = "Prescribe at least one medicine to continue." } });
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }



        [HttpGet]
        [Route("DeletePrescribedMedicine")]
        public async Task<IHttpActionResult> DeletePrescribedMedicine(int PharmacyRequest_Id)
        {
            try
            {
                var PharmacyPrescription = _AdminService.PrescribeMedicine(PharmacyRequest_Id);
                return Ok(new CustomResponse<string> { Message = GlobalUtility.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = "Medicine deleted successfully." });

            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }


        [Authorize]
        [Route("ChangePassword")]
        public async Task<IHttpActionResult> ChangePassword(AdminSetPasswordBindingModel model)
        {
            try
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
                var AdminModel = _AdminService.ChangePassword(model, userEmail);

                if (AdminModel != null)
                {
                    return Ok(new CustomResponse<Admin> { Message = GlobalUtility.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = AdminModel });
                }
                else
                    return Ok(new CustomResponse<Error> { Message = "Forbidden", StatusCode = (int)HttpStatusCode.Forbidden, Result = new Error { ErrorMessage = "Invalid old password." } });

            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }



        [HttpPost]
        [Route("ChangeOrderStatus")]
        public async Task<IHttpActionResult> ChangeOrderStatus(ChangeOrderStatusListBindingModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var orderStatusMessage = _AdminService.ChangeOrderStatus(model);
                return Ok(new CustomResponse<string> { Message = "Success", StatusCode = (int)HttpStatusCode.OK, Result = "Order status updated successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }


        [HttpGet]
        [Route("GetMyNotifications")]
        public async Task<IHttpActionResult> GetMyNotifications(int Id, bool Unread = false)
        {
            try
            {
                var Notifications = _AdminService.GetMyNotifications(Id, Unread);
                return Ok(new CustomResponse<SearchSubAdminNotificationsViewModel>
                {
                    Message = GlobalUtility.ResponseMessages.Success,
                    StatusCode = (int)HttpStatusCode.OK,
                    Result = Notifications
                });
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }

        [HttpPost]
        [Route("AddNewNotification")]
        public IHttpActionResult AddNewNotification(PubNubNotificationBindingModel model)
        {
            try
            {
                var AdminNotification = _AdminService.AddNewNotification(model);
                return Ok(new CustomResponse<List<AdminSubAdminNotifications>>
                {
                    Message = GlobalUtility.ResponseMessages.Success,
                    StatusCode = (int)HttpStatusCode.OK,
                    Result = AdminNotification
                });

            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }

        [HttpGet]
        [Route("MarkNotificationAsRead")]
        public async Task<IHttpActionResult> MarkNotificationAsRead(int Id, int AdminId)
        {
            try
            {
                var RemainingCount = _AdminService.MarkNotificationAsRead(Id, AdminId);
                return Ok(new CustomResponse<int>
                {
                    Message = GlobalUtility.ResponseMessages.Success,
                    StatusCode = (int)HttpStatusCode.OK,
                    Result = RemainingCount
                });
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }


        [HttpGet]
        [Route("GetPharmacyRatings")]
        public async Task<IHttpActionResult> GetPharmacyRatings(int Pharmacy_Id)
        {
            try
            {
                PharmacyRatingsViewModel returnModel = new PharmacyRatingsViewModel();
                var Ratings = _AdminService.GetPharmacyRatings(Pharmacy_Id);
                if (Ratings == null)
                    Ratings = new List<PharmacyRating>();

                returnModel.Ratings = Ratings;
                returnModel.AverageRating = Ratings.CalculateAverageRating();

                return Ok(new CustomResponse<PharmacyRatingsViewModel>
                {
                    Message = GlobalUtility.ResponseMessages.Success,
                    StatusCode = (int)HttpStatusCode.OK,
                    Result = returnModel
                });
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }


    }
}

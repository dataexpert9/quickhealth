using AdminWebapi.Models;
using AdminWebapi.ViewModels;
using BusinessLogic.HelperServices;
using BusinessLogic.PharmacyServices;
using DBAccess.ViewModels.Pharmacy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace AdminWebapi.Controllers
{
    [RoutePrefix("api/User")]
    [ExceptionHandlingFilter]
    public class PharmacyController : ApiController
    {

        private readonly IPharmacyServices _pharmacyService;


        public PharmacyController(IPharmacyServices pharmacyService)
        {
            _pharmacyService = pharmacyService;
   
        }

        [Authorize]
        [HttpGet]
        [Route("GetNearByPharmacies")]
        public async Task<IHttpActionResult> GetNearByPharmacies(double? Lat=0,double? Lng=0,string City="")
        {
            try
            {

                PharmacyListViewModel returnModel = new PharmacyListViewModel();
                returnModel.Pharmacies = _pharmacyService.GetNearByPharmacies(Lat.Value,Lng.Value,City);
                return Ok(new CustomResponse<PharmacyListViewModel> { Message = GlobalUtility.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = returnModel });


            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }

        [Authorize]
        [HttpGet]
        [Route("SearchPharmacies")]
        public async Task<IHttpActionResult> SearchPharmacies(string Name="",string PhoneNumber="",string ZipCode="")
        {
            try
            {

                PharmacyListViewModel returnModel = new PharmacyListViewModel();
                returnModel.Pharmacies = _pharmacyService.SearchPharmacies(Name,PhoneNumber,ZipCode);
                return Ok(new CustomResponse<PharmacyListViewModel> { Message = GlobalUtility.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = returnModel });


            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }
        

        [Authorize]
        [Route("PharmacyRequest")]
        [HttpPost]
        public async Task<IHttpActionResult> PharmacyRequest(PharmacyAppointmentBindingModel model)
        {
            try
            {


                if (model.User_Id == 0)
                    model.User_Id = Convert.ToInt32(User.GetClaimValue("userid"));

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var request = _pharmacyService.PharmacyRequest(model);
                if (request != null)
                {
                    PharmacyRequestViewModel returnModel = new PharmacyRequestViewModel();
                    returnModel.PharmacyRequest = request;
                    return Ok(new CustomResponse<PharmacyRequestViewModel> { Message = GlobalUtility.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = returnModel });
                }
                else
                {
                    return Content(HttpStatusCode.OK, new CustomResponse<Error>
                    {
                        Message = "Conflict",
                        StatusCode = (int)HttpStatusCode.Conflict,
                        Result = new Error { ErrorMessage = "You already have appointment today." }
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }

        [Authorize]
        [Route("UpdatePharmacyRequest")]
        [HttpPost]
        public async Task<IHttpActionResult> UpdatePharmacyRequest(UpdatePharmacyAppointmentBindingModel model)
        {
            try
            {


                //if (model.User_Id == 0 || model.User_Id==null)
                //    model.User_Id = Convert.ToInt32(User.GetClaimValue("userid"));

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var requestUpdate = _pharmacyService.UpdatePharmacyRequest(model);
                if (requestUpdate != null)
                {
                    return Ok(new CustomResponse<string> { Message = GlobalUtility.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = "Request updated successfully." });
                }
                else
                {
                    return Content(HttpStatusCode.OK, new CustomResponse<Error>
                    {
                        Message = "Conflict",
                        StatusCode = (int)HttpStatusCode.Conflict,
                        Result = new Error { ErrorMessage = "Request not found." }
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }


        [Authorize]
        [HttpGet]
        [Route("GetMyPrescriptions")]
        public async Task<IHttpActionResult> GetMyPrescriptions(int User_Id, int? FamilyMember_Id=0)
        {
            try
            {

                PharmacyRequestsViewModel returnModel = new PharmacyRequestsViewModel();
                returnModel.MyPrescriptions = _pharmacyService.GetMyPrescriptions(User_Id,FamilyMember_Id);
                return Ok(new CustomResponse<PharmacyRequestsViewModel> { Message = GlobalUtility.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = returnModel });


            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }


        [Route("CancelPharmacyAppointment")]
        [HttpPost]
        public async Task<IHttpActionResult> CancelPharmacyAppointment(CancelPharmacyAppointmentBindingModel model)
        {
            try
            {
                if (model.User_Id == 0)
                    model.User_Id = Convert.ToInt32(User.GetClaimValue("userid"));

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var PharmacyStatus= _pharmacyService.CancelPharmacyAppointment(model);
                if (PharmacyStatus)
                {
                    return Ok(new CustomResponse<string> { Message = GlobalUtility.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = "Request canceled successfully." });
                }
                else
                {
                    return Content(HttpStatusCode.OK, new CustomResponse<Error>
                    {
                        Message = "Not Found",
                        StatusCode = (int)HttpStatusCode.NotFound,
                        Result = new Error { ErrorMessage = "You have no pending request." }
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }


        [Route("RatePharmacy")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<IHttpActionResult> RateDoctor(RatePharmacyBindingModel model)
        {
            try
            {
                if (model.User_Id == 0)
                    model.User_Id = Convert.ToInt32(User.GetClaimValue("userid"));

                if (model.User_Id == 0)
                    throw new Exception("Doctor id is empty in user.identity.");

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var Appointment = _pharmacyService.RatePharmacy(model);

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
    }
}

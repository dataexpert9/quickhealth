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





        //[Route("GetPharmacyAppointment")]
        //[HttpPost]
        //public async Task<IHttpActionResult> GetPharmacyAppointment(PharmacyAppointmentBindingModel model)
        //{
        //    try
        //    {


        //        if (model.User_Id == 0)
        //            model.User_Id = Convert.ToInt32(User.GetClaimValue("userid"));

        //        if (!ModelState.IsValid)
        //        {
        //            return BadRequest(ModelState);
        //        }
        //        var appointment = _pharmacyService.GetPharmacyAppointment(model);
        //        if (appointment != null)
        //        {
        //            return Ok(new CustomResponse<string> { Message = GlobalUtility.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = "Request for appointment submitted successfully." });
        //        }
        //        else
        //        {
        //            return Content(HttpStatusCode.OK, new CustomResponse<Error>
        //            {
        //                Message = "Conflict",
        //                StatusCode = (int)HttpStatusCode.Conflict,
        //                Result = new Error { ErrorMessage = "You already have appointment today." }
        //            });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(Utility.LogError(ex));
        //    }
        //}






    }
}

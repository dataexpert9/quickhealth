using AdminWebapi.Models;
using AdminWebapi.ViewModels;
using BusinessLogic.CommonServices;
using BusinessLogic.HelperServices;
using BusinessLogic.UserServices;
using DBAccess.Models;
using DBAccess.ViewModels;
using DBAccess.ViewModels.Doctor;
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
        //[Authorize]
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



    }
}

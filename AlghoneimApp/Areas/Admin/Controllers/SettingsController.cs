using BasketApi;
using DAL;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace WebApplication1.Areas.Admin.Controllers
{
    [RoutePrefix("api/Settings")]
    public class SettingsController : ApiController
    {


        [HttpGet]
        [Route("GetSettings")]
        public async Task<IHttpActionResult> GetSettings()
        {
            try
            {
                using (BasketContext ctx = new BasketContext())
                {
                    var settings = ctx.Settings.FirstOrDefault();
                    if (settings == null)
                    {
                        return Ok(new CustomResponse<Settings>
                        {
                            Message = Global.ResponseMessages.Success,
                            StatusCode = (int)HttpStatusCode.OK,
                            Result = new Settings { DeliveryFee = 0, AboutUs = "", Help = "", Currency = "AED", MinimumOrderPrice = 0 }
                        });
                    }
                    else
                    {
                        return Ok(new CustomResponse<Settings>
                        {
                            Message = Global.ResponseMessages.Success,
                            StatusCode = (int)HttpStatusCode.OK,
                            Result = ctx.Settings.FirstOrDefault()
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }


        [HttpPost]
        [Route("SetSettings")]
        public async Task<IHttpActionResult> SetSettings()
        {
            try
            {
                Settings model = new Settings();
                var httpRequest = HttpContext.Current.Request;
                string newFullPath = string.Empty;
                string fileNameOnly = string.Empty;



                if (httpRequest.Params["Id"] != null)
                    model.Id = Convert.ToInt32(httpRequest.Params["Id"]);

                //if (httpRequest.Params["ImageDeletedOnEdit"] != null)
                //    model.BannerImage = Convert.ToBoolean(httpRequest.Params["ImageDeletedOnEdit"]);
                if (httpRequest.Params["Currency"] != null)
                    model.Currency = httpRequest.Params["Currency"];
                //if (httpRequest.Params["DeliveryFee"] != null)
                //    model.DeliveryFee = Convert.ToDouble(httpRequest.Params["DeliveryFee"]);
                if (httpRequest.Params["ServiceFee"] != null)
                    model.ServiceFee = Convert.ToDouble(httpRequest.Params["ServiceFee"]);
                if (httpRequest.Params["AboutUs"] != null)
                    model.AboutUs = httpRequest.Params["AboutUs"];
                if (httpRequest.Params["Help"] != null)
                    model.Help = httpRequest.Params["Help"];
                if (httpRequest.Params["NearByRadius"] != null)
                    model.NearByRadius = Convert.ToDouble(Convert.ToInt32(httpRequest.Params["NearByRadius"]) * 1609.344);
                
                //if (httpRequest.Params["MinimumOrderPrice"] != null)
                //    model.MinimumOrderPrice = Convert.ToDouble(httpRequest.Params["MinimumOrderPrice"]);

                Validate(model);



                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                using (BasketContext ctx = new BasketContext())
                {
                    Settings returnModel = new Settings();
                    returnModel = ctx.Settings.FirstOrDefault();
                    // in case on showing subscription table 
                    if (string.IsNullOrEmpty(model.Currency) && model.DeliveryFee == 0)
                    {
                        return Ok(new CustomResponse<Settings>
                        {
                            Message = Global.ResponseMessages.Success,
                            StatusCode = (int)HttpStatusCode.OK,
                            Result = returnModel
                        });

                    }

                    if (returnModel != null)
                    {
                        returnModel.Currency = model.Currency;
                        //returnModel.DeliveryFee = model.DeliveryFee;
                        returnModel.ServiceFee = model.ServiceFee;
                        returnModel.AboutUs = model.AboutUs;
                        returnModel.Help = model.Help;
                        returnModel.NearByRadius = model.NearByRadius;
                        //returnModel.MinimumOrderPrice = model.MinimumOrderPrice;
                        ctx.SaveChanges();

                    }
                    else
                    {
                        ctx.Settings.Add(model);
                        ctx.SaveChanges();
                    }

                    BasketSettings.LoadSettings();

                    return Ok(new CustomResponse<Settings>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = returnModel
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

using BasketApi;
using DAL;
using System;
using System.Linq;
using System.Net;
using System.Web.Http;

namespace WebApplication1.Controllers
{
    [RoutePrefix("api/Store")]
    public class StoreController : ApiController
    {
        //[HttpGet]
        //[Route("GetAllStores")]
        //public IHttpActionResult GetStoreCategories(double Lat, double Lng)
        //{
        //    try
        //    {
        //        BasketContext ctx = new BasketContext();
        //        var res = ctx.Stores.OrderBy(x => x.Name).ToList();

        //        CustomResponse<StoreViewModel> response = new CustomResponse<StoreViewModel>
        //        {
        //            Message = "Success",
        //            StatusCode = (int)HttpStatusCode.OK,
        //            Result = new StoreViewModel { Stores = res }
        //        };
        //        return Ok(response);



        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(Utility.LogError(ex));
        //    }

        //}
    }
}

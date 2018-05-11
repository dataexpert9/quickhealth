//using BasketApi;
//using BasketApi.ViewModels;
//using DAL;
//using System;
//using System.Linq;
//using System.Net;
//using System.Web.Http;

//namespace WebApplication1.Controllers
//{
//    [RoutePrefix("api/Category")]
//    public class CategoriesController : ApiController
//    {

//        //[Authorize]
//        [HttpGet]
//        [Route("GetAllCategories")]
//        public IHttpActionResult GetStoreCategories()
//        {
//            try
//            {
//                BasketContext ctx = new BasketContext();
//                var res = ctx.Categories.Where(x=>x.IsDeleted == false).OrderBy(x => x.Name).ToList();

//                CustomResponse<CategoriesViewModel> response = new CustomResponse<CategoriesViewModel>
//                {
//                    Message = "Success",
//                    StatusCode = (int)HttpStatusCode.OK,
//                    Result = new CategoriesViewModel { Categories = res }
//                };
//                return Ok(response);



//            }
//            catch (Exception ex)
//            {
//                return StatusCode(Utility.LogError(ex));
//            }

//        }

//    }
//}

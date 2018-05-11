using BasketApi;
using BasketApi.Areas.SubAdmin.Models;
using BasketApi.ViewModels;
using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace WebApplication1.Areas.Admin.Controllers
{
    [BasketApi.Authorize("SubAdmin", "SuperAdmin", "ApplicationAdmin", "User", "Guest")]
    [RoutePrefix("api")]
    public class CategoryController : ApiController
    {
        [Route("GetAllCategoriesByStoreId")]
        public async Task<IHttpActionResult> GetAllCategoriesByStoreId(int StoreId)
        {
            try
            {
                using (BasketContext ctx = new BasketContext())
                {
                    CustomResponse<IEnumerable<Category>> response = new CustomResponse<IEnumerable<Category>>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = ctx.Categories.Where(x => x.Store_Id == StoreId && x.IsDeleted == false).OrderBy(x => x.Name).ToList()
                    };
                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }

        [Route("GetCategoriesByStoreId")]
        public async Task<IHttpActionResult> GetAllCategoriesByStoreIdUser(int StoreId)
        {
            try
            {
                using (BasketContext ctx = new BasketContext())
                {
                    CustomResponse<CategoriesViewModel> response = new CustomResponse<CategoriesViewModel>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = new CategoriesViewModel { Categories = ctx.Categories.Where(x => x.Store_Id == StoreId && x.ParentCategoryId == 0 && x.IsDeleted == false).OrderBy(x => x.Name).ToList() }
                    };
                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }


        [Route("GetSubCategoriesByCatId")]
        public async Task<IHttpActionResult> GetSubCategoriesByCatId(int CatId)
        {
            try
            {
                using (BasketContext ctx = new BasketContext())
                {
                    var categories = ctx.Categories.Where(x => x.ParentCategoryId == CatId && x.IsDeleted == false).OrderBy(x => x.Name).ToList();
                    categories.Insert(0, new Category { Name = "All", Id = CatId });
                    CustomResponse<CategoriesViewModel> response = new CustomResponse<CategoriesViewModel>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = new CategoriesViewModel { Categories = categories }
                    };
                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }


        [Route("GetAllCategories")]
        public async Task<IHttpActionResult> GetAllCategories()
        {
            try
            {
                using (BasketContext ctx = new BasketContext())
                {
                    var categories = ctx.Categories.Where(x=>x.IsDeleted == false).OrderBy(x => x.Name).ToList();
                    CustomResponse<CategoriesViewModel> response = new CustomResponse<CategoriesViewModel>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = new CategoriesViewModel { Categories = categories }
                    };
                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }

    }
}

using BasketApi.Areas.Admin.ViewModels;
using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using WebApplication1.Areas.Admin.ViewModels;

namespace BasketApi.Areas.SubAdmin.Controllers
{
    [BasketApi.Authorize("SubAdmin", "SuperAdmin", "ApplicationAdmin", "User", "Guest")]
    [RoutePrefix("api")]
    public class ProductController : ApiController
    {
        [HttpGet]
        [Route("GetProductsByCategoryId")]
        public async Task<IHttpActionResult> GetProductsByCategoryId(int CatId, int UserId, int PageSize, int PageNo, string filterTypes = "", bool IsAll = false)
        {
            try
            {
                List<int> lstFilterTypes = new List<int>();

                if (filterTypes != null && filterTypes != "")
                    lstFilterTypes = filterTypes.Split(',').Select(int.Parse).ToList();

                using (BasketContext ctx = new BasketContext())
                {
                    var userFavourites = ctx.Favourites.Where(x => x.User_ID == UserId && x.IsDeleted == false).ToList();
                    List<Product> products;
                    int TotalCount;

                    if (IsAll)
                    {
                        var CatIds = ctx.Categories.Where(x => x.Id == CatId || x.ParentCategoryId == CatId).Select(x => x.Id).ToList();
                        TotalCount = ctx.Products.Count(x => CatIds.Contains(x.Category_Id) && x.IsDeleted == false);
                        if (lstFilterTypes.Count > 0)
                        {
                            var productsTemp = ctx.Products.Where(x => CatIds.Contains(x.Category_Id) && x.IsDeleted == false);

                            foreach (var filter in lstFilterTypes)
                            {
                                switch (filter)
                                {
                                    case 1:
                                        productsTemp = productsTemp.OrderByDescending(x => x.OrderedCount);
                                        break;
                                    case 2:
                                        productsTemp = productsTemp.OrderByDescending(x => x.AverageRating);
                                        break;
                                }
                            }
                            products = productsTemp.Page(PageSize, PageNo).ToList();
                        }
                        else
                            products = ctx.Products.Where(x => CatIds.Contains(x.Category_Id) && x.IsDeleted == false).OrderByDescending(x => x.Id).Page(PageSize, PageNo).ToList();
                    }
                    else
                    {
                        TotalCount = ctx.Products.Count(x => x.Category_Id == CatId && x.IsDeleted == false);
                        if (lstFilterTypes.Count > 0)
                        {
                            var productsTemp = ctx.Products.Where(x => x.Category_Id == CatId && x.IsDeleted == false);

                            foreach (var filter in lstFilterTypes)
                            {
                                switch (filter)
                                {
                                    case 1:
                                        productsTemp = productsTemp.OrderByDescending(x => x.OrderedCount);
                                        break;
                                    case 2:
                                        productsTemp = productsTemp.OrderByDescending(x => x.AverageRating);
                                        break;
                                }
                            }
                            products = productsTemp.Page(PageSize, PageNo).ToList();
                        }
                        else
                            products = ctx.Products.Where(x => x.Category_Id == CatId && x.IsDeleted == false).OrderByDescending(x => x.Id).Page(PageSize, PageNo).ToList();
                    }

                    foreach (var product in products)
                    {
                        switch (product.WeightUnit)
                        {
                            case 1:
                                product.Weight = Convert.ToString(product.WeightInGrams) + " gm";
                                break;
                            case 2:
                                product.Weight = Convert.ToString(product.WeightInKiloGrams) + " kg";
                                break;
                            case 3:
                                product.Weight = Convert.ToString(product.WeightInMeters) + " m";
                                break;
                            case 4:
                                product.Weight = Convert.ToString(product.WeightInLiter) + " L";
                                break;
                            case 5:
                                product.Weight = Convert.ToString(product.WeightInMilliLiter) + " mL";
                                break;
                        }


                        if (userFavourites.Any(x => x.Product_Id == product.Id))
                            product.IsFavourite = true;
                        else
                            product.IsFavourite = false;
                    }

                    return Ok(new CustomResponse<ProductsViewModel>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = new ProductsViewModel
                        {
                            Count = TotalCount,
                            Products = products
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }

        [HttpGet]
        [Route("GetProductCount")]
        public async Task<IHttpActionResult> GetProductCount()
        {
            try
            {
                using (BasketContext ctx = new BasketContext())
                {
                    ProductCountViewModel model = new ProductCountViewModel { TotalProducts = ctx.Products.Count(x => x.IsDeleted == false) };
                    CustomResponse<ProductCountViewModel> response = new CustomResponse<ProductCountViewModel>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = model
                    };
                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }

        [HttpGet]
        [Route("GetAllProductsByStoreId")]
        public async Task<IHttpActionResult> GetAllProductsByStoreId(int StoreId, int UserId, int PageSize, int PageNo, string filterTypes = "")
        {
            try
            {
                List<int> lstFilterTypes = new List<int>();

                if (filterTypes != null && filterTypes != "")
                    lstFilterTypes = filterTypes.Split(',').Select(int.Parse).ToList();

                List<Product> products;

                using (BasketContext ctx = new BasketContext())
                {
                    var userFavourites = ctx.Favourites.Where(x => x.User_ID == UserId && x.IsDeleted == false).ToList();

                    if (lstFilterTypes.Count > 0)
                    {
                        var productsTemp = ctx.Products.Where(x => x.Store_Id == StoreId && x.IsDeleted == false);

                        foreach (var filter in lstFilterTypes)
                        {
                            switch (filter)
                            {
                                case 1:
                                    productsTemp = productsTemp.OrderByDescending(x => x.OrderedCount);
                                    break;
                                case 2:
                                    productsTemp = productsTemp.OrderByDescending(x => x.AverageRating);
                                    break;
                            }
                        }
                        products = productsTemp.Page(PageSize, PageNo).ToList();
                    }
                    else
                        products = ctx.Products.Where(x => x.Store_Id == StoreId && x.IsDeleted == false).OrderByDescending(x => x.Id).Page(PageSize, PageNo).ToList();

                    return Ok(new CustomResponse<ProductsViewModel>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = new ProductsViewModel
                        {
                            Count = ctx.Products.Count(x => x.Store_Id == StoreId && x.IsDeleted == false),
                            Products = products
                        }
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

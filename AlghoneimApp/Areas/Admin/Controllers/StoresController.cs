﻿using BasketApi;
using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using WebApplication1.Areas.Admin.ViewModels;
using System.Data.Entity;
using BasketApi.ViewModels;

namespace BasketApi.Areas.Admin.Controllers
{
    [BasketApi.Authorize("SubAdmin", "SuperAdmin", "ApplicationAdmin", "User")]
    [RoutePrefix("api")]
    public class StoresController : ApiController
    {
        /// <summary>
        /// Get All Stores
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAllStores")]
        public async Task<IHttpActionResult> GetAllStores()
        {
            try
            {
                using (BasketContext ctx = new BasketContext())
                {
                    var stores = ctx.Stores.Include(x => x.StoreDeliveryHours).Include(x => x.StoreRatings).Where(x => x.IsDeleted == false).ToList();

                    foreach (var store in stores)
                    {
                        store.CalculateAverageRating();
                    }
                    CustomResponse<IEnumerable<Store>> response = new CustomResponse<IEnumerable<Store>>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = stores
                    };
                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }

        /// <summary>
        /// Get Stores by Franchisor Id
        /// </summary>
        /// <param name="FranchisorId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAllStoresByFranchisorId")]
        public async Task<IHttpActionResult> GetAllStoresByFranchisorId(int FranchisorId)
        {
            try
            {
                using (BasketContext ctx = new BasketContext())
                {
                    CustomResponse<IEnumerable<Store>> response = new CustomResponse<IEnumerable<Store>>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = ctx.Stores.ToList()
                    };
                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }

        /// <summary>
        /// Get Stores Count
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetStoresCount")]
        public async Task<IHttpActionResult> GetStoresCount()
        {
            try
            {
                using (BasketContext ctx = new BasketContext())
                {
                    StoreCountViewModel model = new StoreCountViewModel { TotalStores = ctx.Stores.Count() };
                    CustomResponse<StoreCountViewModel> response = new CustomResponse<StoreCountViewModel>
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

        /// <summary>
        /// Get Nearby Stores, Default radius is 50 miles. To get all stores, give long and lat = 0
        /// </summary>
        /// <param name="longitude"></param>
        /// <param name="latitude"></param>
        /// <returns></returns>
        [Route("GetNearbyStores")]
        public async Task<IHttpActionResult> GetNearbyStores(double longitude, double latitude, int PageSize, int PageNo, string filterTypes)
        {
            try
            {
                List<int> lstFilterTypes = new List<int>();

                if (filterTypes != null && filterTypes != "")
                    lstFilterTypes = filterTypes.Split(',').Select(int.Parse).ToList();

                var point = Utility.CreatePoint(latitude, longitude);

                using (BasketContext ctx = new BasketContext())
                {
                    StoresViewModel storesViewModel = new StoresViewModel();
                    if (longitude == 0 && latitude == 0)
                    {
                        storesViewModel.Count = ctx.Stores.Count(x => x.IsDeleted == false);
                        if (lstFilterTypes.Count > 0)
                        {
                            var storesTemp = ctx.Stores.Include(x => x.StoreDeliveryHours).Include(x => x.StoreRatings).Where(x => x.IsDeleted == false);

                            foreach (var filter in lstFilterTypes)
                            {
                                switch (filter)
                                {
                                    case 1:
                                        storesTemp = storesTemp.OrderByDescending(x => x.OrderedCount);
                                        break;
                                    case 2:
                                        storesTemp = storesTemp.OrderByDescending(x => x.AverageRating);
                                        break;
                                }
                            }
                            storesViewModel.Stores = storesTemp.Page(PageSize, PageNo).ToList();
                        }
                        else
                            storesViewModel.Stores = ctx.Stores.Include(x => x.StoreDeliveryHours).Include(x => x.StoreRatings).Where(x => x.IsDeleted == false).OrderByDescending(x => x.Id).Page(PageSize, PageNo).ToList();
                    }
                    else
                    {
                        storesViewModel.Count = ctx.Stores.Count(x => x.Location.Distance(point) < BasketSettings.NearByRadius && x.IsDeleted == false);

                        if (lstFilterTypes.Count > 0)
                        {
                            var storesTemp = ctx.Stores.Include(x => x.StoreDeliveryHours).Include(x => x.StoreRatings).Where(x => x.Location.Distance(point) < BasketSettings.NearByRadius && x.IsDeleted == false);

                            foreach (var filter in lstFilterTypes)
                            {
                                switch (filter)
                                {
                                    case 1:
                                        storesTemp = storesTemp.OrderByDescending(x => x.OrderedCount);
                                        break;
                                    case 2:
                                        storesTemp = storesTemp.OrderByDescending(x => x.AverageRating);
                                        break;
                                }
                            }
                            storesViewModel.Stores = storesTemp.Page(PageSize, PageNo).ToList();
                        }
                        else
                            storesViewModel.Stores = ctx.Stores.Include(x => x.StoreDeliveryHours).Include(x => x.StoreRatings).Where(x => x.Location.Distance(point) < BasketSettings.NearByRadius && x.IsDeleted == false).OrderByDescending(x => x.Id).Page(PageSize, PageNo).ToList();
                    }

                    foreach (var store in storesViewModel.Stores)
                    {
                        store.CalculateAverageRating();
                    }

                    CustomResponse<StoresViewModel> reponse = new CustomResponse<StoresViewModel>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = storesViewModel
                    };

                    return Ok(reponse);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }
    }
}
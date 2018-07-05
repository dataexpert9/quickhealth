using AdminWebapi.Models;
using BusinessLogic.HelperServices;
using BusinessLogic.OrderServices;
using DBAccess.Models;
using DBAccess.ViewModels.Order;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace AdminWebapi.Controllers
{

    [RoutePrefix("api/Order")]
    [ExceptionHandlingFilter]
    public class OrderController : ApiController
    {

        private readonly IOrderServices _OrderService;

        public OrderController(IOrderServices orderService)
        {
            _OrderService = orderService;
        }

        [HttpPost]
        [Route("InsertOrder")]
        public async Task<IHttpActionResult> InsertOrder(OrderViewModel model)
        {
            try
            {
                Order order;
                if (System.Web.HttpContext.Current.Request.Params["OBJ"] != null)
                    model = JsonConvert.DeserializeObject<OrderViewModel>(System.Web.HttpContext.Current.Request.Params["OBJ"]);

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                order =_OrderService.InsertOrder(model);
                if (order != null)
                {
                    return Ok(new CustomResponse<Order> { Message = GlobalUtility.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = order });

                }
                else
                {
                    return Content(HttpStatusCode.OK, new CustomResponse<Error>
                    {
                        Message = "Conflict",
                        StatusCode = (int)HttpStatusCode.Conflict,
                        Result = new Error { ErrorMessage = "Please select atleast 1 medicine to continue." }
                    });
                }
      
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }


        //[Authorize]
        [AllowAnonymous]
        [HttpGet]
        [Route("SearchOrders")]
        public async Task<IHttpActionResult> SearchOrders(string StartDate, string EndDate, int? OrderStatusId, int? PaymentMethodId, int? PaymentStatusId, int? StoreId)
        {
            try
            {

                var OrderList = _OrderService.SearchOrders(StartDate, EndDate,  OrderStatusId,  PaymentMethodId,  PaymentStatusId,  StoreId);
                return Ok(new CustomResponse<SearchOrdersListViewModel> { Message = GlobalUtility.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = OrderList });

            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }


        [Authorize]
        [HttpGet]
        [Route("GetOrderByOrderId")]
        public async Task<IHttpActionResult> GetOrderByOrderId(int OrderId, int? UserId)
        {
            try
            {
                var resp=_OrderService.GetOrderByOrderId(OrderId, UserId.Value);
                return Ok(new CustomResponse<OrderAdminViewModel> { Message = GlobalUtility.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = resp });

            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }


    }
}

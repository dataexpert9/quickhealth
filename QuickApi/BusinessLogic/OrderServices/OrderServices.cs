using BusinessLogic.HelperServices;

using DBAccess;
using DBAccess.GenericRepository;
using DBAccess.Models;
using DBAccess.ViewModels;
using DBAccess.ViewModels.Common;
using DBAccess.ViewModels.Order;
using DBAccess.ViewModels.Pharmacy;
using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BusinessLogic.HelperServices.Utility;

namespace BusinessLogic.OrderServices
{
    public class OrderServices : IOrderServices
    {
        private readonly AdminDBContext _DBContext = new AdminDBContext();
        private readonly GenericRepository<Pharmacy> _PharmacyRepository;
        private readonly GenericRepository<PharmacyOrder> _PharmacyOrderRepository;
        private readonly GenericRepository<Order_Items> _OrderItemsRepository;
        private readonly GenericRepository<Order> _OrderRepository;
        private readonly GenericRepository<PharmacyPrescription> _PharmacyPrescriptionRepository;
        private readonly GenericRepository<PharmacyRequest> _PharmacyRequestRepository;






        public OrderServices()
        {
            _PharmacyRepository = new GenericRepository<Pharmacy>(_DBContext);
            _PharmacyOrderRepository = new GenericRepository<PharmacyOrder>(_DBContext);
            _OrderItemsRepository = new GenericRepository<Order_Items>(_DBContext);
            _OrderRepository = new GenericRepository<Order>(_DBContext);
            _PharmacyPrescriptionRepository = new GenericRepository<PharmacyPrescription>();
            _PharmacyRequestRepository = new GenericRepository<PharmacyRequest>(_DBContext);

        }

        public Order InsertOrder(OrderViewModel model)
        {

            if (model.Cart.CartItems.Count() > 0)
            {
                List<Order_Items> Medicines = new List<Order_Items>();
                PharmacyOrder PharmacyOrder = new PharmacyOrder();

                var Pharmacy = _PharmacyRepository.GetFirst(x => x.Id == model.Pharmacy_Id);

                model.CalculateOrderSubTotal();
                model.CalculateOrderTotal(Pharmacy.DeliveryFee);


                var order = new Order
                {
                    DeliveryFee = Pharmacy.DeliveryFee,
                    IsDeleted = false,
                    //OrderDateTime = model.OrderDeliveryTime,
                    OrderDateTime=DateTime.UtcNow,
                    OrderNo = Guid.NewGuid().ToString("N").ToUpper(),
                    Status = (int)PharmacyRequestStatuses.ReadyToDispatch,
                    Subtotal = model.SubTotal,
                    Total = model.Total,
                    User_Id = model.User_Id,
                    Pharmacy_Id = model.Pharmacy_Id
                };
                _OrderRepository.Insert(order);
                _OrderRepository.Save();



                PharmacyOrder.Pharmacy_Id = model.Pharmacy_Id;
                PharmacyOrder.Order_Id = order.Id;
                PharmacyOrder.OrderDeliveryTime = model.OrderDeliveryTime;
                PharmacyOrder.OrderDeliveryFee = Pharmacy.DeliveryFee;
                PharmacyOrder.IsDeleted = false;
                PharmacyOrder.Pharmacy_Id = model.Pharmacy_Id;
                PharmacyOrder.Status = (int)PharmacyRequestStatuses.Pending;
                PharmacyOrder.Subtotal = model.SubTotal;
                PharmacyOrder.Total = model.Total;
                PharmacyOrder.OrderNo = Guid.NewGuid().ToString("N").ToUpper();

                _PharmacyOrderRepository.Insert(PharmacyOrder);
                _PharmacyOrderRepository.Save();

                
                foreach (var item in model.Cart.CartItems)
                {
                    Medicines.Add(new Order_Items
                    {
                        Dose = item.Dose,
                        Name = item.Name,
                        PharmacyPrescription_Id = item.PharmacyPrescription_Id,
                        Price = item.Price,
                        Qty = item.Qty,
                        PharmacyOrder_Id = PharmacyOrder.Id
                    });

                }
                _OrderItemsRepository.InsertMany(Medicines);
                _OrderItemsRepository.Save();

                var PharmacyRequestId= model.Cart.CartItems.FirstOrDefault().PharmacyRequest_Id;
                var PharmacyRequest = _PharmacyRequestRepository.GetFirst(x => x.Id == PharmacyRequestId);
                PharmacyRequest.Status = (int)PharmacyRequestStatuses.ReadyToDispatch;
                _PharmacyRequestRepository.Save();

                var Order = _OrderRepository.GetFirstWithInclude(x => x.Id == order.Id, "Pharmacy", "User");
                return Order;
            }
            else
            {
                return null;
            }

        }


//        public SearchOrdersListViewModel SearchOrders(string StartDate, string EndDate, int? OrderStatusId, int? PaymentMethodId, int? PaymentStatusId, int? StoreId)
//        {

//            SearchOrdersListViewModel returnModel = new SearchOrdersListViewModel();

//            DateTime startDateTime;
//            DateTime endDateTime;
//            if (!string.IsNullOrEmpty(StartDate))
//                startDateTime = DateTime.ParseExact(StartDate, "d/MM/yyyy h:mm:ss tt", CultureInfo.InvariantCulture);

//            if (!string.IsNullOrEmpty(EndDate))
//                endDateTime = DateTime.ParseExact(EndDate, "d/MM/yyyy h:mm:ss tt", CultureInfo.InvariantCulture);

//            #region query
//            var query = @"

//select 
//Orders.Id,
//Orders.OrderDateTime as CreatedOn,
//Orders.Total as OrderTotal,
//Case When Orders.PaymentMethod = 0 Then 'Pending' Else 'Paid' End As PaymentStatus,
//Pharmacies.Name as PharmacyName,
//Pharmacies.Id as StoreId,
//Pharmacies.Location as StoreLocation,
//Users.FullName as CustomerName
//from Orders
//join Users on Users.ID = Orders.User_ID
//join Pharmacies on Pharmacies.Id = Orders.Pharmacy_Id
//where 
//Orders.IsDeleted = 0
//";
//            #endregion

//            if (OrderStatusId.HasValue)
//                query += " and orders.Status = " + OrderStatusId.Value;

//            if (PaymentMethodId.HasValue)
//                query += " and orders.PaymentMethod = " + PaymentMethodId.Value;

//            if (PaymentStatusId.HasValue)
//                query += " and orders.PaymentStatus = " + PaymentStatusId.Value;

//            if (StoreId.HasValue)
//                query += " and Pharmacies.Id = " + StoreId.Value;

//            //var orders = _OrderRepository.GetListWithStoreProcedure(query).ToList();

//            returnModel.Orders= _DBContext.Database.SqlQuery<SearchOrdersViewModel>(query).ToList();

//            return returnModel;
//        }



        public SearchOrdersListViewModel SearchOrders(string StartDate, string EndDate, int? OrderStatusId, int? PaymentMethodId, int? PaymentStatusId, int? StoreId)
        {

            SearchOrdersListViewModel returnModel = new SearchOrdersListViewModel();

            DateTime startDateTime;
            DateTime endDateTime;
            startDateTime = DateTime.ParseExact(StartDate, "d/MM/yyyy h:mm:ss tt", CultureInfo.InvariantCulture);
            endDateTime = DateTime.ParseExact(EndDate, "d/MM/yyyy h:mm:ss tt", CultureInfo.InvariantCulture);

            #region query
            var query = @"

select 
Orders.Id,
Orders.OrderDateTime as CreatedOn,
Orders.Status as OrderStatus,
Orders.Total as OrderTotal,
PharmacyOrders.Id as StoreOrder_Id,
Case When Orders.PaymentMethod = 0 Then 'Pending' Else 'Paid' End As PaymentStatus,
Pharmacies.Name as PharmacyName,
Pharmacies.Id as StoreId,
Pharmacies.Location as StoreLocation,
Users.FullName as CustomerName
from Orders
join Users on Users.ID = Orders.User_ID
join Pharmacies on Pharmacies.Id = Orders.Pharmacy_Id
join PharmacyOrders on PharmacyOrders.Order_Id = Orders.Id
where 
Orders.IsDeleted = 0
and 
 CAST(orders.OrderDateTime AS DATE) >= '" + startDateTime.Date + "' and CAST(orders.OrderDateTime as DATE) <= '" + endDateTime.Date + "'";
            #endregion

            if (OrderStatusId.HasValue)
                query += " and orders.Status = " + OrderStatusId.Value;

            if (PaymentMethodId.HasValue)
                query += " and orders.PaymentMethod = " + PaymentMethodId.Value;

            if (PaymentStatusId.HasValue)
                query += " and orders.PaymentStatus = " + PaymentStatusId.Value;

            if (StoreId.HasValue)
                query += " and Pharmacies.Id = " + StoreId.Value;

            //var orders = _OrderRepository.GetListWithStoreProcedure(query).ToList();

            returnModel.Orders = _DBContext.Database.SqlQuery<SearchOrdersViewModel>(query).ToList();
            
            //var time= returnModel.Orders[0].CreatedOn.ToUniversalTime();

            return returnModel;
        }


        public OrderAdminViewModel GetOrderByOrderId(int OrderId, int UserId)
        {

            #region OrderQuery
            var orderQuery = @"
SELECT Orders.Id,Orders.OrderNo,Orders.Status,Orders.PaymentMethod,Orders.OrderDateTime,Orders.Subtotal,Orders.DeliveryFee,Orders.Total,Orders.User_Id,Orders.IsDeleted,Orders.OrderPayment_Id, Users.FullName as UserFullName,Users.ProfilePictureUrl as UserProfilePictureUrl FROM Orders 
join Users on Users.ID = Orders.User_ID
where Orders.Id = " + OrderId + @" and Orders.IsDeleted = 0 ";
            #endregion

            var order = _DBContext.Database.SqlQuery<OrderAdminViewModel>(orderQuery).FirstOrDefault();


                #region storeOrderQuery
                var storeOrderQuery = @"
select
PharmacyOrders.*,
Pharmacies.Name as StoreName,
Pharmacies.ImageUrl from PharmacyOrders 
join Pharmacies on Pharmacies.Id = PharmacyOrders.Pharmacy_Id
where 
Order_Id = " + order.Id + @"
";
                #endregion

                var UserQuery = @"
select Users.Id , 
Users.FullName,
Users.Email,
Users.Phone,
Users.ProfilePictureUrl
from 
Users Where Users.Id=" + order.User_Id + "";

                var user = _DBContext.Database.SqlQuery<UserViewModel>(UserQuery).FirstOrDefault();

                var storeOrders = _DBContext.Database.SqlQuery<StoreOrderViewModel>(storeOrderQuery).ToList();

                var storeOrderIds = string.Join(",", storeOrders.Select(x => x.Id.ToString()));

                #region OrderItemsQuery
                var orderItemsQuery = @"
SELECT
  CASE
    WHEN ISNULL(Order_Items.PharmacyPrescription_Id, 0) <> 0 THEN PharmacyPrescriptions.Id
  END AS ItemId,
  Order_Items.Name AS Name,
  Order_Items.Price AS Price,
  Order_Items.Id,
  Order_Items.Qty,
  Order_Items.PharmacyPrescription_Id,
  Order_Items.PharmacyOrder_Id
FROM Order_Items
LEFT JOIN PharmacyPrescriptions
  ON PharmacyPrescriptions.Id = Order_Items.PharmacyPrescription_Id
WHERE PharmacyOrder_Id IN ('" + storeOrderIds + "')";
                #endregion

                var orderItems = _DBContext.Database.SqlQuery<OrderItemViewModel>(orderItemsQuery).ToList();

                foreach (var orderItem in orderItems)
                {
                    storeOrders.FirstOrDefault(x => x.Id == orderItem.PharmacyOrder_Id).OrderItems.Add(orderItem);
                }

                foreach (var storeOrder in storeOrders)
                {
                    order.StoreOrders.Add(storeOrder);
                }
                order.User = user;

        
            return order;
        }
    }
}

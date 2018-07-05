using DBAccess.Models;
using DBAccess.ViewModels.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.OrderServices
{
    public interface IOrderServices
    {
        Order InsertOrder(OrderViewModel model);
        SearchOrdersListViewModel SearchOrders(string StartDate, string EndDate, int? OrderStatusId, int? PaymentMethodId, int? PaymentStatusId, int? StoreId);
        OrderAdminViewModel GetOrderByOrderId(int OrderId,int UserId);

    }
}

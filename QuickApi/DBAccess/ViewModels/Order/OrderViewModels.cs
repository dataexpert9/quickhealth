using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBAccess.ViewModels.Order
{
   
    public class OrderViewModel
    {
        public OrderViewModel()
        {
            Cart = new CartViewModel();
        }

        public int User_Id { get; set; }

        public DateTime OrderDeliveryTime { get; set; }

        public string AdditionalNote { get; set; }

        public UInt16 PaymentMethodType { get; set; }

        public string DeliveryAddress { get; set; }

        public double Tip { get; set; }

        public int Pharmacy_Id { get; set; }

        public double SubTotal { get; set; }

        public double Total { get; set; }

        public CartViewModel Cart { get; set; }


    }
    public class CartViewModel
    {
        public CartViewModel()
        {
            CartItems = new List<CartItemViewModel>();
        }
        public List<CartItemViewModel> CartItems { get; set; }
    }
    public class CartItemViewModel
    {
        public int PharmacyRequest_Id { get; set; }
        public int PharmacyPrescription_Id { get; set; }
        public string Name { get; set; }
        public string Dose { get; set; }
        public double Price { get; set; }
        public int Qty { get; set; }
        public int Pharmacy_Id { get; set; }
    }

    public class SearchOrdersListViewModel 
    {
        public SearchOrdersListViewModel()
        {
            Orders = new List<SearchOrdersViewModel>();
        }

        public List<SearchOrdersViewModel> Orders { get; set; }
    }
    public class SearchOrdersViewModel
    {
        public SearchOrdersViewModel()
        {

        }
        public int Id { get; set; }

        public bool IsChecked { get; set; }

        public string CustomerName { get; set; }

        public int StoreId { get; set; }

        public int OrderStatus { get; set; }

        public string OrderStatusName { get; set; }

        public Boolean IsDeleted { get; set; }

        public int StoreOrder_Id { get; set; }

        [Required]
        [Range(1, Int32.MaxValue)]
        public int? DeliveryManId { get; set; }

        public string PharmacyName { get; set; }

        public DateTime CreatedOn { get; set; }

        public string PaymentStatus { get; set; }

        public double OrderTotal { get; set; }


       
    }


    public class OrderAdminViewModel
    {
        public OrderAdminViewModel()
        {
            StoreOrders = new HashSet<StoreOrderViewModel>();
        }
        public int Id { get; set; }

        public string OrderNo { get; set; }

        public int Status { get; set; }

        //[JsonConverter(typeof(JsonCustomDateTimeConverter))]
        public DateTime OrderDateTime { get; set; }

        //[JsonConverter(typeof(JsonCustomDateTimeConverter))]
        //public DateTime? DeliveryTime_From { get; set; }

        //[JsonConverter(typeof(JsonCustomDateTimeConverter))]
        //public DateTime? DeliveryTime_To { get; set; }


        public int PaymentMethod { get; set; }

        public double Subtotal { get; set; }

        public double DeliveryFee { get; set; }

        public double Total { get; set; }

        public int User_Id { get; set; }

        public bool IsDeleted { get; set; }

        public int? OrderPayment_Id { get; set; }


        public string UserFullName { get; set; }

        public string UserProfilePictureUrl { get; set; }

        public virtual ICollection<StoreOrderViewModel> StoreOrders { get; set; }

        public virtual UserViewModel User { get; set; }
    }
    public class StoreOrderViewModel
    {
        public StoreOrderViewModel()
        {
            OrderItems = new List<OrderItemViewModel>();
        }
        public int Id { get; set; }

        public string OrderNo { get; set; }

        public int Status { get; set; }

        public int Store_Id { get; set; }

        public double Subtotal { get; set; }

        public double Total { get; set; }

        public bool IsDeleted { get; set; }

        public int Order_Id { get; set; }

        public double DeliveryFee { get; set; }

        public string StoreName { get; set; }

        public string ImageUrl { get; set; }

        //[JsonConverter(typeof(JsonCustomDateTimeConverter))]
        public DateTime? OrderDeliveryTime { get; set; }

        public List<OrderItemViewModel> OrderItems { get; set; }
    }

    public class OrderItemViewModel
    {
        public int Id { get; set; }

        public int ItemId { get; set; }

        public int Qty { get; set; }

        public int PharmacyOrder_Id { get; set; }

        public string Name { get; set; }

        public double Price { get; set; }

        public string Description { get; set; }

        public string ImageUrl { get; set; }
    }



    public class ChangeOrderStatusListBindingModel
    {
        public ChangeOrderStatusListBindingModel()
        {
            Orders = new List<ChangeOrderStatusBindingModel>();
        }
        public List<ChangeOrderStatusBindingModel> Orders { get; set; }
    }

    public class ChangeOrderStatusBindingModel
    {
        public int OrderId { get; set; }
        public int StoreOrder_Id { get; set; }
        public int Status { get; set; }
    }










}

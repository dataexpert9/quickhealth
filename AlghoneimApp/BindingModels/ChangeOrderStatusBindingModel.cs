using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BasketApi.BindingModels
{
    public class ChangeOrderStatusBindingModel
    {
        public int OrderId { get; set; }
        public int StoreOrder_Id { get; set; }
        public int Status { get; set; }
    }

    public class ChangeOrderStatusListBindingModel
    {
        public ChangeOrderStatusListBindingModel()
        {
            Orders = new List<ChangeOrderStatusBindingModel>();
        }
        public List<ChangeOrderStatusBindingModel> Orders { get; set; }
    }

    public class ChangeDelivererStatusBindingModel
    {
        public int DelivererId { get; set; }
        public bool Status { get; set; }
    }
    public class ChangeDelivererStatusListBindingModel
    {
        public ChangeDelivererStatusListBindingModel()
        {
            Deliverers = new List<ChangeDelivererStatusBindingModel>();
        }
        public List<ChangeDelivererStatusBindingModel> Deliverers { get; set; }
    }

    public class ChangeUserStatusBindingModel
    {
        public int UserId { get; set; }
        public bool Status { get; set; }
    }
    public class ChangeUserStatusListBindingModel
    {
        public ChangeUserStatusListBindingModel()
        {
            Users = new List<ChangeUserStatusBindingModel>();
        }
        public List<ChangeUserStatusBindingModel> Users { get; set; }
    }
}
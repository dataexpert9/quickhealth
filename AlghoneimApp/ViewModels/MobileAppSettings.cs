using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BasketApi.ViewModels
{
    public class MobileAppSettings
    {
        public User User { get; set; }
        public DeliveryMan Deliverer { get; set; }
        public Settings Settings { get; set; }

    }
}
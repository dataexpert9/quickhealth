using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BasketApi
{
    public static class BasketSettings
    {
        public static int Id { get; set; }
        public static double DeliveryFee { get; set; }
        public static string Currency { get; set; }
        public static double MinimumOrderPrice { get; set; }
        public static double ServiceFee { get; set; }
        public static string Help { get; set; }
        public static string AboutUs { get; set; }
        public static double NearByRadius { get; set; }

        public static void LoadSettings()
        {
            try
            {
                using (BasketContext ctx = new BasketContext())
                {
                    var setting = ctx.Settings.FirstOrDefault();
                    if (setting != null)
                    {
                        Id = setting.Id;
                        DeliveryFee = setting.DeliveryFee;
                        ServiceFee = setting.ServiceFee;
                        Currency = setting.Currency;
                        MinimumOrderPrice = setting.MinimumOrderPrice;
                        AboutUs = setting.AboutUs;
                        Help = setting.Help;
                        NearByRadius = setting.NearByRadius;
                    }
                }
            }
            catch (Exception ex)
            {
                Utility.LogError(ex);
            }
        }
    }
}
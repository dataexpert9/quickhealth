using DBAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BusinessLogic.HelperServices
{
    public static class SettingsModel
    {
        public static int Id { get; set; }
        public static string Currency { get; set; }
        public static double Tax { get; set; }
        public static string PrivacyPolicy { get; set; }
        public static string AboutUs { get; set; }
        public static string TermsConditions { get; set; }
        public static string ContactNo { get; set; }
        
        public static void LoadSettings()
        {
            try
            {
                using (AdminDBContext ctx = new AdminDBContext())
                {
                    var setting = ctx.Settings.FirstOrDefault();
                    if (setting != null)
                    {
                        Currency = setting.Currency;
                        Tax = setting.Tax;
                        PrivacyPolicy = setting.PrivacyPolicy;
                        AboutUs = setting.AboutUs;
                        TermsConditions = setting.TermsConditions;
                        ContactNo = setting.ContactNo;
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
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBAccess.Models
{
    public class Settings
    {
        public int Id { get; set; }
        public string Currency { get; set; }
        public string AboutUs { get; set; }
        public double Tax { get; set; }
        public string PrivacyPolicy { get; set; }
        public string TermsConditions { get; set; }
        public string ContactNo { get; set; }
        
    }
}

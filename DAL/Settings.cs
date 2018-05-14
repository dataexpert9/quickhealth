using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class Settings
    {
        [JsonIgnore]
        public int Id { get; set; }
        public double DeliveryFee { get; set; }
        public string Currency { get; set; }
        public string AboutUs { get; set; }
        public string Help { get; set; }
        public double MinimumOrderPrice { get; set; }
        public string StandardWorkingHours { get; set; }
        public double ServiceFee { get; set; }
        public double NearByRadius { get; set; }
    }
}

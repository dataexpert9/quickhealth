using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBAccess.Models
{
    public partial class LifeStyle
    {
        [Key, ForeignKey("User")]
        public int Id { get; set; }

        public string DietryRestrictions { get; set; }

        public string Alcohol { get; set; }

        public string Smoking { get; set; }

        public string SexuallyActive { get; set; }

        public string RecreationalDrugs { get; set; }

        //public int User_Id { get; set; }

        [JsonIgnore]
        public virtual User User { get; set; }
    }
}

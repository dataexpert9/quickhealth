namespace DBAccess.Models
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Notification
    {
        public int Id { get; set; }

        [Required]
        public string Text { get; set; }

        public string Title { get; set; }

        [Required]
        public int Status { get; set; }

        public int? User_Id { get; set; }

        public int? Doctor_Id { get; set; }

        public string ImageUrl { get; set; }

        public DateTime? CreatedDateTime { get; set; }
            
        [JsonIgnore]
        public virtual Doctor Doctor { get; set; }

        [JsonIgnore]
        public virtual User User { get; set; }
        
        public virtual AdminNotifications AdminNotification { get; set; }
    }
}

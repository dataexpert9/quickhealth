using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class AdminNotifications
    {
        public AdminNotifications()
        {
            Notifications = new HashSet<Notification>();
            AdminSubAdminNotifications = new HashSet<AdminSubAdminNotifications>();
        }

        public int Id { get; set; }

        public int Admin_Id { get; set; }

        public string Title  { get; set; }

        public string Text  { get; set; }

        public int TargetAudienceType { get; set; }

        public DateTime CreatedDate { get; set; }

        public Admin Admin { get; set; }

        [JsonIgnore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Notification> Notifications { get; set; }

        [JsonIgnore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AdminSubAdminNotifications> AdminSubAdminNotifications { get; set; }
    }
}

using DBAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBAccess.ViewModels.Admin
{
    public class NotificationsViewModel
    {
    }
    public class SearchSubAdminNotificationsViewModel
    {
        public SearchSubAdminNotificationsViewModel()
        {
            Notifications = new List<AdminSubAdminNotifications>();
        }
        public List<AdminSubAdminNotifications> Notifications { get; set; }
    }

    public class PubNubNotificationBindingModel
    {
        public int PharmacyRequest_Id { get; set; }

        public int User_Id { get; set; }

        public int Admin_Id { get; set; }
    }
}

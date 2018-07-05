using DBAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AdminWebapi.ViewModels
{
    public class CommonViewModels
    {
    }

    public class NotificationsViewModel
    {
        public NotificationsViewModel()
        {
            Notifications = new List<Notification>();
        }
        public List<Notification> Notifications { get; set; }
        public int Count { get; set; }
    }

    public class CountiesList
    {
        public List<Countries> Countries { get; set; }
    }

    public class CityList
    {
        public List<Cities> Cities { get; set; }
    }
}
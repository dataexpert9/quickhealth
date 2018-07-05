using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AdminWebapi.Areas.AdminArea.ViewModels
{
    public class WebDashboardStatsViewModel
    {

        public PharmacyDashboardViewModel PharmacyDashboard { get; set; }
        public AdminDashboardViewModel AdminDashboard { get; set; }
        public List<TopDoctorsViewModel> TopDoctors { get; set; }
      


    }
    public class PharmacyDashboardViewModel
    {
        public int TotalRequests { get; set; }
        public int CompletedRequests { get; set; }
        public int PendingRequests { get; set; }

    }
    public class AdminDashboardViewModel
    {
        public AdminDashboardViewModel()
        {
            TopPharmacies = new List<TopPharmaciesViewModel>();
            TopDoctors = new List<TopDoctorsViewModel>();
        }

        public int UnreadNotificationsCount { get; set; }
        public int TotalUsers { get; set; }
        public int TotalDoctors { get; set; }
        public int TotalPharmacies { get; set; }
        public int TodayAppointments { get; set; }
        public List<TopPharmaciesViewModel> TopPharmacies { get; set; }
        public List<TopDoctorsViewModel> TopDoctors { get; set; }

    }
    public class TopDoctorsViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public double Rating { get; set; }

        public string Email { get; set; }
    }
    public class TopPharmaciesViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public double Rating { get; set; }

        public string Email { get; set; }

        public double AverageRating { get; set; }
    }
}
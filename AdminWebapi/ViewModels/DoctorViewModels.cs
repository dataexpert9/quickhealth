using DBAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AdminWebapi.ViewModels
{
    public class DoctorViewModels
    {
    }
    public class RequestInquiries
    {
        public RequestInquiries()
        {
            Inquiries = new List<Appointment>();
        }

        public List<Appointment> Inquiries { get; set; }
    }

    public class DoctorHistory
    {
        public DoctorHistory()
        {
            Inquiries = new List<Appointment>();
        }

        public List<Appointment> Inquiries { get; set; }
    }

}
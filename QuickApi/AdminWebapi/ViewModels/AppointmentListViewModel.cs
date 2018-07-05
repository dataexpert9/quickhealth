﻿using DBAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AdminWebapi.ViewModels
{
    public class AppointmentListViewModel
    {
        public AppointmentListViewModel()
        {
            Appointments = new List<Appointment>();
        }
        public List<Appointment> Appointments { get; set; }

    }

    //public class AppointmentViewModel
    //{
    //    public AppointmentViewModel()
    //    {
    //        Appointment = new Appointment();
    //    }
    //    public Appointment Appointment { get; set; }

    //}

}
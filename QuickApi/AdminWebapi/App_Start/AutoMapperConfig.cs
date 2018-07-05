using AdminWebapi.ViewModels;
using AutoMapper;
using DBAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace AdminWebapi.App_Start
{
    public class AutoMapperConfig
    {
        public static void Register()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<PharmacyRequest, AdimPharmacyRequestViewModel>();
                cfg.CreateMap<Appointment, AppointmentHistoryViewModel>();
                cfg.CreateMap<PharmacyRequest, ChatPharmacyRequestViewModel>();

            });
        }

    }
}
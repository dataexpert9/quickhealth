﻿using System.ComponentModel.Composition;
using Resolver;

namespace BusinessLogic
{
    [Export(typeof(IComponent))]
    public class DependencyResolver : IComponent
    {
        //Latest
        public void SetUp(IRegisterComponent registerComponent)
        {
            // User Services
            registerComponent.RegisterType<UserServices.IUserService, UserServices.UserService>();
            registerComponent.RegisterType<UserServices.IDoctorService, UserServices.AdminService>();

            // Common Services
            registerComponent.RegisterType<CommonServices.IImageUpload, CommonServices.ImageUpload>();
            registerComponent.RegisterType<CommonServices.ICommonServices,CommonServices.CommonServices>();
            // Security Services
            registerComponent.RegisterType<SecurityServices.ISecurityService, SecurityServices.SecurityService>();


            // Pharmacy Services
            registerComponent.RegisterType<PharmacyServices.IPharmacyServices, PharmacyServices.PharmacyServices>();


            // Admin Services
            registerComponent.RegisterType<AdminService.IAdminService, AdminService.AdminService>();

            // Order Services
            registerComponent.RegisterType<OrderServices.IOrderServices, OrderServices.OrderServices>();
      

        }
    }
}
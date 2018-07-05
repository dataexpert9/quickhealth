using DBAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AdminWebapi.ViewModels
{
    public class UserViewModels
    {
      
    }

    public class GetUserProfileModel
    {
        public GetUserProfileModel()
        {
            User = new User();
        }
        public User User { get; set; }
    }

    public class GetDoctorProfileModel
    {
        public GetDoctorProfileModel()
        {
            Doctor = new Doctor();
        }
        public Doctor Doctor { get; set; }
    }

}
using MultipartDataMediaFormatter.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBAccess.ViewModels.Doctor
{
    class DoctorProfile
    {
    }

    public class EditDoctorProfileBindingModel
    {

        public int Id { get; set; }

        public string FullName { get; set; }

        public string SurName { get; set; }

        public string Country { get; set; }

        public string City { get; set; }

        public int ProviderType { get; set; }

        public bool IsAvailable { get; set; }

        public string Specialization { get; set; }

        public string Department { get; set; }

        public string LatestQualification { get; set; }

        public string Bio { get; set; }

        public string Address { get; set; }

        public bool IsNotificationsOn { get; set; }
        
        public HttpFile ProfilePicture { get; set; }


    }
}

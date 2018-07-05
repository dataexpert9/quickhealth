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

        public string Country { get; set; }

        public string City { get; set; }

        public string Bio { get; set; }

        public string Address { get; set; }

        public DateTime DateOfBirth { get; set; }

        public string PhoneNumber { get; set; }

        public int ProviderType { get; set; }

        public HttpFile ProfilePicture { get; set; }


    }
}

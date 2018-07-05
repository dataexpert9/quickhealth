using MultipartDataMediaFormatter.Infrastructure;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBAccess.ViewModels.Admin
{
    class AdminViewModels
    {
    }

    public class AddAdminBindingModel
    {
        public int Id { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string Email { get; set; }

        public string Phone { get; set; }

        [Required]
        public short Role { get; set; }

        public string Password { get; set; }

        public string AccountNo { get; set; }

        public short? Status { get; set; }

        public bool IsDeleted { get; set; }

        public string ImageUrl { get; set; }

        public int? Pharmacy_Id { get; set; }

        public HttpFile Image { get; set; }
    }
    public class AddPrescribtionBindingModel
    {
        public int Id { get; set; }

        public string Dose { get; set; }

        public string MedicineName { get; set; }

        public double Price { get; set; }

        public int PharmacyRequest_Id { get; set; }

        public int Pharmacy_Id { get; set; }
    }
}

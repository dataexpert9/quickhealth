using DBAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AdminWebapi.ViewModels
{
    public class PharmacyListViewModel
    {

        public PharmacyListViewModel()
        {
            Pharmacies = new List<Pharmacy>();
        }
        public List<Pharmacy> Pharmacies { get; set; }
    }

    public class PharmacyRequestsViewModel
    {
        public List<PharmacyRequest> MyPrescriptions { get; set; }
    }
    public class PharmacyRequestViewModel
    {
        public PharmacyRequest PharmacyRequest { get; set; }
    }

 

}
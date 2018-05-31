using BusinessLogic.HelperServices;
using DBAccess;
using DBAccess.GenericRepository;
using DBAccess.Models;
using DBAccess.ViewModels.Common;
using DBAccess.ViewModels.Pharmacy;
using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.PharmacyServices
{
    public class PharmacyServices : IPharmacyServices
    {
        private readonly AdminDBContext _DBContext = new AdminDBContext();
        private readonly GenericRepository<Pharmacy> _PharmacyRepository;



        public PharmacyServices()
        {
            _PharmacyRepository = new GenericRepository<Pharmacy>(_DBContext);

        }

        public List<Pharmacy> GetNearByPharmacies(double lat, double lng, string City)
        {
            DbGeography points;
            List<Pharmacy> Pharmacies = new List<Pharmacy>();
            if (lat != 0 && lng != 0)
            {
                points = Utility.CreatePoint(lat, lng);
                Pharmacies = _PharmacyRepository.GetMany(x => x.Location.Distance(points) <= GlobalUtility.NearbyStoreRadius && x.IsDeleted == false);
            }
            else
            {
                Pharmacies = _PharmacyRepository.GetMany(x => x.Address.Contains(City) && x.IsDeleted == false);
            }
            return Pharmacies;
        }


        public List<Pharmacy> SearchPharmacies(string Name = "", string PhoneNumber = "", string ZipCode = "")
        {
            List<Pharmacy> Pharmacies = new List<Pharmacy>();
            var query = @"select * From Pharmacies Where ";

            if (!string.IsNullOrEmpty(Name))
                query += "Name LIKE '%" + Name + "%' AND ";

            if (!string.IsNullOrEmpty(PhoneNumber))
                query += "PhoneNumber LIKE '%" + PhoneNumber + "%' AND";


            if (!string.IsNullOrEmpty(ZipCode))
                query += "ZipCode LIKE '%" + ZipCode + "%' AND";

            query += " IsDeleted='false'";


            Pharmacies = _PharmacyRepository.GetListWithStoreProcedure(query).ToList();
            //Pharmacies = _PharmacyRepository.GetMany(x => x.Name.Contains(Name) || x.PhoneNumber.Contains(PhoneNumber) || x.ZipCode.Contains(ZipCode));
            return Pharmacies;
        }
        //public PharmacyAppointment GetPharmacyAppointment(PharmacyAppointmentBindingModel model)
        //{

        //    return new PharmacyAppointment();
        //}
    }
}

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
        private readonly GenericRepository<PharmacyRequest> _PharmacyRequestRepository;
        private readonly GenericRepository<Chat> _ChatRepository;
        private readonly GenericRepository<PharmacyRating> _PharmacyRatingRepository;






        public PharmacyServices()
        {
            _PharmacyRepository = new GenericRepository<Pharmacy>(_DBContext);
            _PharmacyRequestRepository = new GenericRepository<PharmacyRequest>(_DBContext);
            _ChatRepository = new GenericRepository<Chat>(_DBContext);
            _PharmacyRatingRepository = new GenericRepository<PharmacyRating>(_DBContext);

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
                query += "PhoneNumber LIKE '%" + PhoneNumber + "%' AND ";


            if (!string.IsNullOrEmpty(ZipCode))
                query += "ZipCode LIKE '%" + ZipCode + "%' AND";

            query += " IsDeleted='false'";


            Pharmacies = _PharmacyRepository.GetListWithStoreProcedure(query).ToList();
            //Pharmacies = _PharmacyRepository.GetMany(x => x.Name.Contains(Name) || x.PhoneNumber.Contains(PhoneNumber) || x.ZipCode.Contains(ZipCode));
            return Pharmacies;
        }

        public PharmacyRequest PharmacyRequest(PharmacyAppointmentBindingModel model)
        {

            //_PharmacyRequestRepository.GetFirst(x=>x.User_Id==model.User_Id);

            var PharmacyModel = new PharmacyRequest
            {
                Purpose = model.purpose,
                Status = (int)GlobalUtility.PharmacyRequest.Requested,
                User_Id = model.User_Id.Value,
                Pharmacy_Id = model.Pharmacy_Id
            };

            _PharmacyRequestRepository.Insert(PharmacyModel);

            var ChannelName = Utility.CreateChatChannel(model.User_Id.Value, model.Pharmacy_Id, "P");
            var AlreadyChat = _ChatRepository.GetFirst(x => x.ChannelName.Contains(ChannelName));




            if (model.ImageUrls != null)
            {
                List<PharmacyRequestImages> Images = new List<PharmacyRequestImages>();
                foreach (var file in model.ImageUrls)
                {
                    if (PharmacyModel.PharmacyRequestImages == null)
                    {
                        PharmacyModel.PharmacyRequestImages = new List<PharmacyRequestImages>();
                    }

                    if (file != null)
                    {
                        Images.Add(new PharmacyRequestImages
                        {
                            PharmacyRequest_Id = PharmacyModel.Id,
                            ImageUrl = ImageHelper.SaveFileFromBytes(file, "PharmacyRequestImages")
                        });
                    }
                    PharmacyModel.PharmacyRequestImages.AddRange(Images);

                }
            }
            _PharmacyRequestRepository.Save();
            if (AlreadyChat != null)
                PharmacyModel.ChannelName = AlreadyChat.ChannelName;
            else
            {
                _ChatRepository.Insert(new Chat
                {
                    ChannelName = ChannelName,
                    User_Id = model.User_Id.Value,
                    Pharmacy_Id = model.Pharmacy_Id,
                    Status = (int)Utility.ChatStatuses.InProgress

                });
                PharmacyModel.ChannelName = ChannelName;
                _ChatRepository.Save();
            }

            var PharmacyReturnModel = _PharmacyRequestRepository.GetFirstWithInclude(x => x.Id == PharmacyModel.Id, "Pharmacy", "User");

            return PharmacyReturnModel;
        }

        public PharmacyRequest UpdatePharmacyRequest(UpdatePharmacyAppointmentBindingModel model)
        {
            var UpdateRequest = _PharmacyRequestRepository.GetFirst(x => x.Id == model.PharmacyRequest_Id);
            if (UpdateRequest != null)
            {
                if (model.FamilyMember_Id.HasValue && model.FamilyMember_Id != 0)
                    UpdateRequest.FamilyMember_Id = model.FamilyMember_Id.Value;

                UpdateRequest.Appointment_Id = model.Appointment_Id;
                if (model.FamilyMember_Id.HasValue && model.FamilyMember_Id != null)
                    UpdateRequest.FamilyMember_Id = model.FamilyMember_Id.Value;

                if (model.User_Id.HasValue && model.User_Id != null)
                    UpdateRequest.User_Id = model.User_Id.Value;

                _PharmacyRequestRepository.Save();
                return UpdateRequest;
            }
            return null;
        }
        public List<PharmacyRequest> GetMyPrescriptions(int User_Id, int? FamilyMember_Id)
        {
            List<PharmacyRequest> PharmacyRequests = new List<PharmacyRequest>();
            if (FamilyMember_Id.HasValue && FamilyMember_Id != 0)
                PharmacyRequests = _PharmacyRequestRepository.GetWithIncludeList(x => x.Appointment.FamilyMember_Id==FamilyMember_Id && (x.Status == (int)Utility.PharmacyRequestStatuses.Completed || x.Status == (int)Utility.PharmacyRequestStatuses.ReadyToDispatch || x.Status == (int)Utility.PharmacyRequestStatuses.Deliveryed), "PharmacyPrescription", "PharmacyRequestImages", "Pharmacy", "Appointment.Doctor", "Appointment.AllergyHistory", "Appointment.VaccinationHistory", "Appointment.MedicationHistory", "Appointment.MedicalConditionHistory", "Appointment.LifeStyleHistory", "Appointment.FamilyMember", "Appointment.AppointmentImages", "Appointment.DoctorPrescription", "Appointment.DoctorPrescription.DoctorPrescriptionImages").OrderByDescending(x => x.Id).ToList();
            else
                PharmacyRequests = _PharmacyRequestRepository.GetWithIncludeList(x => x.User_Id == User_Id && (x.Appointment.FamilyMember_Id == null || !x.Appointment.FamilyMember_Id.HasValue) && (x.Status == (int)Utility.PharmacyRequestStatuses.Completed || x.Status == (int)Utility.PharmacyRequestStatuses.ReadyToDispatch || x.Status == (int)Utility.PharmacyRequestStatuses.Deliveryed), "PharmacyPrescription", "PharmacyRequestImages", "Pharmacy", "Appointment.Doctor", "Appointment.AllergyHistory", "Appointment.VaccinationHistory", "Appointment.MedicationHistory", "Appointment.MedicalConditionHistory", "Appointment.LifeStyleHistory", "Appointment.AppointmentImages", "Appointment.DoctorPrescription", "Appointment.DoctorPrescription.DoctorPrescriptionImages").OrderByDescending(x => x.Id).ToList();

            foreach (var request in PharmacyRequests)
            {
                if (request.PharmacyPrescription.Count > 0)
                {
                    request.Total = request.PharmacyPrescription.Sum(x => x.Price);
                }
            }

            return PharmacyRequests;
        }

        public Boolean CancelPharmacyAppointment(CancelPharmacyAppointmentBindingModel model)
        {
            var PharmacyRequest = _PharmacyRequestRepository.GetFirst(x => x.Id == model.RequestPharmacy_Id && x.User_Id == model.User_Id && x.Status != (int)GlobalUtility.PharmacyRequest.Rejected);

            if (PharmacyRequest != null)
            {
                PharmacyRequest.Status = (int)GlobalUtility.PharmacyRequest.Rejected;
                _PharmacyRequestRepository.Save();
                return true;
            }
            else
                return false;

        }

        public bool RatePharmacy(RatePharmacyBindingModel model)
        {
            PharmacyRating rating = new PharmacyRating();
            rating.Pharmacy_Id = model.Pharmacy_Id;
            rating.User_Id = model.User_Id;
            rating.Rating = model.Rating;
            rating.Feedback = model.Feedback;
            _PharmacyRatingRepository.Insert(rating);
            _PharmacyRatingRepository.Save();
            return true;
        }


    }
}

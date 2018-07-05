using BusinessLogic.HelperServices;
using DBAccess;
using DBAccess.GenericRepository;
using DBAccess.Models;
using DBAccess.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.CommonServices
{
    public class CommonServices : ICommonServices
    {
        private readonly AdminDBContext _DBContext = new AdminDBContext();
        private readonly GenericRepository<Settings> _SettingsRepository;
        private readonly GenericRepository<Chat> _ChatRepository;
        private readonly GenericRepository<AppRating> _RatingRepository;
        private readonly GenericRepository<Notification> _NotificationRepository;
        private readonly GenericRepository<Countries> _CountriesRepository;
        private readonly GenericRepository<Cities> _CitiesRepository;
        private readonly GenericRepository<PharmacyRequest> _PharmacyRequestRepository;





        public CommonServices()
        {
            _SettingsRepository = new GenericRepository<Settings>(_DBContext);
            _ChatRepository = new GenericRepository<Chat>(_DBContext);
            _RatingRepository = new GenericRepository<AppRating>(_DBContext);
            _NotificationRepository = new GenericRepository<Notification>(_DBContext);
            _CountriesRepository = new GenericRepository<Countries>(_DBContext);
            _CitiesRepository = new GenericRepository<Cities>(_DBContext);
            _PharmacyRequestRepository = new GenericRepository<PharmacyRequest>(_DBContext);

        }

        public Settings GetAppSettings()
        {

            var settings = _SettingsRepository.GetFirstRecord();
            if (settings == null)
                return new Settings();
            else
                return settings;



        }

        public Chat CreateChannel(CreateChannelBindingModel model)
        {

            Chat Chat = new Chat();
            if (string.IsNullOrEmpty(model.ChannelName))
            {

                if (model.Doctor_Id.HasValue)
                    model.ChannelName = "DOCTOR-D" + model.Doctor_Id.Value + "-";
                else if (model.Pharmacy_Id.HasValue)
                    model.ChannelName = "PHARMACY-P" + model.Pharmacy_Id.Value + "-";

                if (model.User_Id.HasValue)
                    model.ChannelName += "U" + model.User_Id.Value;
                else if (model.FamilyMember_Id.HasValue)
                    model.ChannelName += "F" + model.FamilyMember_Id.Value;

                //if (model.Doctor_Id.HasValue)
                //    model.ChannelName = Utility.CreateChatChannel(model.User_Id, model.Doctor_Id.Value, "D");
                //else
                //    model.ChannelName = Utility.CreateChatChannel(model.User_Id, model.Pharmacy_Id.Value, "D");


                //var chatModel = _ChatRepository.GetWithInclude(x => x.ChannelName.Contains(model.ChannelName),"User","Doctor","Pharmacy");
            }
            var chatModel = _ChatRepository.GetFirst(x => x.ChannelName.Contains(model.ChannelName));
            if (chatModel == null)
            {
                chatModel = new Chat
                {

                    ChannelName = model.ChannelName,
                    Status = (int)Utility.ChatStatuses.InProgress,
                };

                if (model.User_Id.HasValue)
                    chatModel.User_Id = model.User_Id.Value;
                if (model.FamilyMember_Id.HasValue)
                    chatModel.FamilyMember_Id = model.FamilyMember_Id.Value;
                if (model.Doctor_Id.HasValue)
                    chatModel.Doctor_Id = model.Doctor_Id.Value;
                if (model.Pharmacy_Id.HasValue)
                    chatModel.Pharmacy_Id = model.Pharmacy_Id.Value;

                _ChatRepository.Insert(chatModel);
            }
            else
            {
                chatModel.Status = (int)Utility.ChatStatuses.InProgress;
            }

            _ChatRepository.Save();

            chatModel = _ChatRepository.GetFirstWithInclude(x => x.Id == chatModel.Id, "User", "Doctor", "Pharmacy", "FamilyMember");
            return chatModel;

        }

        public AppRating RateApp(AppRatingModel model)
        {



            var RatingModel = new AppRating
            {
                Rating = model.Rating,
                Feedback = model.Feedback,
            };

            if (model.Doctor_Id != null && model.Doctor_Id!=0)
                RatingModel.Doctor_Id = model.Doctor_Id;
            if (model.User_Id != 0 && model.User_Id!= null)
                RatingModel.User_Id = model.User_Id;
            

            _RatingRepository.Insert(RatingModel);
            _RatingRepository.Save();

            return RatingModel;
        }

        public List<Notification> GetAllNotifications(int? User_Id = 0, int? Doctor_Id = 0)
        {
            List<Notification> Notifications = new List<Notification>();

            if (User_Id != 0)
                Notifications = _NotificationRepository.GetMany(x => x.User_Id == User_Id);
            else
                Notifications = _NotificationRepository.GetMany(x => x.Doctor_Id == Doctor_Id);

            return Notifications;
        }

        public List<Countries> GetAllCountries()
        {
            var Countries = _CountriesRepository.GetAll().ToList();
            return Countries;
        }

        public List<Cities> GetCityFromCountry(int Country_Id)
        {

            var Cities = _CitiesRepository.GetMany(x => x.Country_Id == Country_Id);
            return Cities;
        }

        public PharmacyRequest CreateChannelForRequest(CreateChannelForRequestBindingModel model)
        {

            PharmacyRequest returnResponse = new PharmacyRequest();
            returnResponse = _PharmacyRequestRepository.GetFirstWithInclude(x => x.Id == model.PharmacyRequest_Id, "User", "FamilyMember", "Pharmacy");

            //if (returnResponse.FamilyMember != null)
            //{
            //    returnResponse.ChannelName = "PHARMACY-P" + returnResponse.Pharmacy_Id + "-F" + returnResponse.FamilyMember_Id + "";
            //}
            //else
            //{
                returnResponse.ChannelName = "PHARMACY-P" + returnResponse.Pharmacy_Id + "-U" + returnResponse.User_Id + "-Test";
            //}

            

            return returnResponse;
        }


    }
}

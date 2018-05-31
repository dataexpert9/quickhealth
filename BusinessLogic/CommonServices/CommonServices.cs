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




        public CommonServices()
        {
            _SettingsRepository = new GenericRepository<Settings>(_DBContext);
            _ChatRepository = new GenericRepository<Chat>(_DBContext);
            _RatingRepository = new GenericRepository<AppRating>(_DBContext);

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
                    model.ChannelName = Utility.CreateChatChannel(model.User_Id, model.Doctor_Id.Value);
                else
                    model.ChannelName = Utility.CreateChatChannel(model.User_Id, model.Pharmacy_Id.Value);
            }

            //var chatModel = _ChatRepository.GetWithInclude(x => x.ChannelName.Contains(model.ChannelName),"User","Doctor","Pharmacy");
            var chatModel = _ChatRepository.GetFirst(x => x.ChannelName.Contains(model.ChannelName));
            if (chatModel == null)
            {
                chatModel = new Chat
                {

                    ChannelName = model.ChannelName,
                    Doctor_Id = model.Doctor_Id,
                    Pharmacy_Id = model.Pharmacy_Id,
                    Status = (int)Utility.ChatStatuses.InProgress,
                    User_Id = model.User_Id
                    
                };
                _ChatRepository.Insert(chatModel);
            }
            else
            {
                chatModel.Status = (int)Utility.ChatStatuses.InProgress;
            }

            _ChatRepository.Save();
            return chatModel;
        }

        public AppRating RateApp(AppRatingModel model)
        {



            var RatingModel = new AppRating {
                Rating=model.Rating,
                Feedback=model.Feedback,
                User_Id=model.User_Id,
                Doctor_Id=model.Doctor_Id,
                Pharmacy_Id=model.Pharmacy_Id

            };

            _RatingRepository.Insert(RatingModel);
            _RatingRepository.Save();
            
            return RatingModel;
        }

    }
}

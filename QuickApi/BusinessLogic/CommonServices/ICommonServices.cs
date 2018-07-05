using DBAccess.Models;
using DBAccess.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.CommonServices
{
    public interface ICommonServices
    {
        Settings GetAppSettings();

        Chat CreateChannel(CreateChannelBindingModel model);

        AppRating RateApp(AppRatingModel model);

        List<Notification> GetAllNotifications(int? User_Id = 0, int? Doctor_Id = 0);

        List<Countries> GetAllCountries();

        List<Cities> GetCityFromCountry(int Country_Id);

        PharmacyRequest CreateChannelForRequest(CreateChannelForRequestBindingModel model);
    }
}

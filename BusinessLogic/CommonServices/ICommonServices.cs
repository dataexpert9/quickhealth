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

    }
}

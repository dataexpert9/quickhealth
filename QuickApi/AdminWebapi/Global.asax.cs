using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.Http;
using MultipartDataMediaFormatter;
using MultipartDataMediaFormatter.Infrastructure;
using PubnubApi;
using AdminWebapi.App_Start;

namespace AdminWebapi
{
    public class Global : HttpApplication
    {
        public static Pubnub objPubnub;
        void Application_Start(object sender, EventArgs e)
        {
            //this is used for multipart direct model binding
            GlobalConfiguration.Configuration.Formatters.Add(new FormMultipartEncodedMediaTypeFormatter(new MultipartFormatterSettings()));
            // Code that runs on application startup
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(App_Start.WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            Bootstrapper.Initialise();
            PubNub.Initialise(objPubnub);
            AutoMapperConfig.Register();
        }
    }
}
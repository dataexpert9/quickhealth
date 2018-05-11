using BasketApi.DomainModels;
using DAL;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Spatial;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using static BasketApi.Global;

namespace BasketApi
{
    public static class Utility
    {
        private static HttpClient client = new HttpClient();

        public static string BaseUrl = ConfigurationManager.AppSettings["BaseUrl"];

        public static IEnumerable<T> Page<T>(this IEnumerable<T> en, int pageSize, int page)
        {
            return en.Skip(page * pageSize).Take(pageSize);
        }

        public static IQueryable<T> Page<T>(this IQueryable<T> en, int pageSize, int page)
        {
            return en.Skip(page * pageSize).Take(pageSize);
        }

        public static async Task GenerateToken(this User user, HttpRequestMessage request)
        {
            try
            {
                var parameters = new Dictionary<string, string>{
                            { "username", user.Email },
                            { "password", user.Password },
                            { "grant_type", "password" },
                            { "signintype", "0" },
                            { "userid",user.Id.ToString()}
                        };

                var content = new FormUrlEncodedContent(parameters);
                var baseUrl = request.RequestUri.AbsoluteUri.Substring(0, request.RequestUri.AbsoluteUri.IndexOf("api"));
                var response = await client.PostAsync(baseUrl + "token", content);

                user.Token = await response.Content.ReadAsAsync<Token>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void SendEmail(string subject,string body,string ToEmail,string AppendToBody="")
        {
            try
            {
                if (!string.IsNullOrEmpty(AppendToBody))
                {
                    body = body + " " + AppendToBody;
                }

                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(EmailUtil.FromMailAddress.Address, EmailUtil.FromPassword)
                };

                var message = new MailMessage(EmailUtil.FromMailAddress, new MailAddress(ToEmail))
                {
                    Subject = subject,
                    Body = body
                };

            smtp.Send(message);
            }
            catch (Exception ex)
            {
                throw ex;
            }
     
        }

        public static async Task GenerateToken(this DeliveryMan user, HttpRequestMessage request)
        {
            try
            {
                var parameters = new Dictionary<string, string>{
                            { "username", user.Email },
                            { "password", user.Password },
                            { "grant_type", "password" },
                            { "signintype", "1" },
                            { "userid",user.Id.ToString()}
                        };

                var content = new FormUrlEncodedContent(parameters);
                var baseUrl = request.RequestUri.AbsoluteUri.Substring(0, request.RequestUri.AbsoluteUri.IndexOf("api"));
                var response = await client.PostAsync(baseUrl + "token", content);

                user.Token = await response.Content.ReadAsAsync<Token>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static async Task GenerateToken(this Admin user, HttpRequestMessage request)
        {
            try
            {
                var parameters = new Dictionary<string, string>{
                            { "username", user.Email },
                            { "password", user.Password },
                            { "grant_type", "password" },
                            { "signintype", user.Role.ToString() },
                            { "userid", user.Id.ToString()}
                        };

                var content = new FormUrlEncodedContent(parameters);
                var baseUrl = request.RequestUri.AbsoluteUri.Substring(0, request.RequestUri.AbsoluteUri.IndexOf("api"));
                var response = await client.PostAsync(baseUrl + "token", content);

                user.Token = await response.Content.ReadAsAsync<Token>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static HttpStatusCode LogError(Exception ex)
        {
            try
            {
                using (StreamWriter sw = File.AppendText(AppDomain.CurrentDomain.BaseDirectory + "/ErrorLog.txt"))
                {
                    sw.WriteLine("DateTime : " + DateTime.Now + Environment.NewLine);
                    if (ex.Message != null)
                    {
                        sw.WriteLine(Environment.NewLine + "Message" + ex.Message);
                        sw.WriteLine(Environment.NewLine + "StackTrace" + ex.StackTrace);
                    }
                    again: if (ex.InnerException != null)
                    {
                        sw.WriteLine(Environment.NewLine + "Inner Exception : " + ex.InnerException.Message);
                        //if (ex.InnerException.InnerException != null)
                        //{
                        //    sw.WriteLine(Environment.NewLine + "Inner Exception : " + ex.InnerException.Message);
                        //}
                        if (ex.InnerException.InnerException != null)
                        {
                            ex = ex.InnerException;
                            goto again;
                        }
                    }
                    

                    sw.WriteLine("------******------");
                }
                return HttpStatusCode.InternalServerError;
            }
            catch (Exception ex1)
            {
                return HttpStatusCode.InternalServerError;
            }
        }

        public static DbGeography CreatePoint(double lat, double lon, int srid = 4326)
        {
            string wkt = String.Format("POINT({0} {1})", lon, lat);

            return DbGeography.PointFromText(wkt, srid);
        }

        public enum BasketEntityTypes
        {
            Product,
            Category,
            Store,
            Package,
            Admin,
            Offer
        }
        
        public static string GetOrderStatusName(int orderStatus)
        {
            try
            {
                switch (orderStatus)
                {
                    case (int)OrderStatuses.AssignedToDeliverer:
                        return "Assigned To Deliverer";
                    case (int)OrderStatuses.DelivererInProgress:
                        return "Deliverer In Progress";
                    case (int)OrderStatuses.InProgress:
                        return "In Progress";
                    case (int)OrderStatuses.ReadyForDelivery:
                        return "Ready For Delivery";
                    default:
                        return ((OrderStatuses)orderStatus).ToString();
                }
            }
            catch (Exception ex)
            {
                Utility.LogError(ex);
                return null;
            }
        }

        public static void DeleteFileIfExists(string path)
        {
            try
            {
                var filePath = HttpContext.Current.Server.MapPath("~/" + path);

                if (File.Exists(filePath))
                    File.Delete(filePath);
            }
            catch (Exception ex)
            {
                LogError(ex);
            }
        }

        public static void SendPushNotifications(List<UserDevice> usersToPushAndroid, List<UserDevice> usersToPushIOS, Notification Notification, int PushType)
        {
            try
            {
                HostingEnvironment.QueueBackgroundWorkItem(cancellationToken =>
                {
                    Global.objPushNotifications.SendAndroidPushNotification
                    (
                        usersToPushAndroid,
                        OtherNotification: Notification,
                        Type: PushType);

                    Global.objPushNotifications.SendIOSPushNotification
                    (
                        usersToPushIOS,
                        OtherNotification: Notification,
                        Type: PushType);
                });
            }
            catch (Exception ex)
            {
                Utility.LogError(ex);
            }
        }

        public static string GetClaimValue(this IPrincipal currentPrincipal, string key)
        {
            var identity = currentPrincipal.Identity as ClaimsIdentity;
            if (identity == null)
                return null;

            var claim = identity.Claims.FirstOrDefault(c => c.Type == key);
            return claim?.Value;
        }
    }

    public class EmailUtil
    {
        public static string FromEmail = ConfigurationManager.AppSettings["FromMailAddress"];
        public static string FromName = ConfigurationManager.AppSettings["FromMailName"];
        public static string FromPassword = ConfigurationManager.AppSettings["FromPassword"];
        public static MailAddress FromMailAddress = new MailAddress(FromEmail, FromName);
    }
}
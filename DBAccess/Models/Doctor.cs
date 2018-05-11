using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DBAccess.Models
{
    public partial class Doctor
    {
        public Doctor()
        {
            DoctorDocuments = new HashSet<DoctorDocument>();
            ForgotPasswordTokens = new HashSet<ForgotPasswordToken>();
            Notifications = new HashSet<Notification>();
            UserDevices = new HashSet<UserDevice>();
        }

        public int Id { get; set; }

        public string FullName { get; set; }

        public string SurName { get; set; }

        public string Phone { get; set; }

        public string DateofBirth { get; set; }

        public string Email { get; set; }

        public int? SignInType { get; set; }

        public string Country { get; set; }

        public string City { get; set; }

        public int ProviderType { get; set; }

        public int Gender { get; set; }

        [JsonIgnore]
        public string Password { get; set; }

        public string Specialization { get; set; }
        
        public string Department { get; set; }

        public string LatestQualification { get; set; }

        public string Bio { get; set; }

        public string Address { get; set; }

        public short? Status { get; set; }

        public bool EmailConfirmed { get; set; }
        
        public bool PhoneConfirmed { get; set; }
        
        public bool IsNotificationsOn { get; set; }

        public bool IsDeleted { get; set; }

        public string ProfilePictureUrl { get; set; }
        
        [NotMapped]
        public Token Token { get; set; }

        [NotMapped]
        public Settings AppSettings { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DoctorDocument> DoctorDocuments { get; set; }

        [JsonIgnore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ForgotPasswordToken> ForgotPasswordTokens { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Notification> Notifications { get; set; }
        
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UserDevice> UserDevices { get; set; }

    }

    public static class DoctorExtension
    {
        private static HttpClient client = new HttpClient();
        public static async Task GenerateToken(this Doctor doctor, HttpRequestMessage request)
        {
            try
            {
                var parameters = new Dictionary<string, string>{
                            { "username", doctor.Email },
                            { "password", doctor.Password },
                            { "grant_type", "password" },
                            { "signintype", Convert.ToString(doctor.SignInType)},
                            { "userid", doctor.Id.ToString()}
                        };

                var content = new FormUrlEncodedContent(parameters);
                var baseUrl = request.RequestUri.AbsoluteUri.Substring(0, request.RequestUri.AbsoluteUri.IndexOf("api"));
                var response = await client.PostAsync(baseUrl + "token", content);

                doctor.Token = await response.Content.ReadAsAsync<Token>();
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

    }
}

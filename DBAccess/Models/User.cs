namespace DBAccess.Models
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Net.Http;
    using System.Threading.Tasks;

    public partial class User
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public User()
        {
            //PaymentCards = new HashSet<PaymentCard>();
            //Favourites = new HashSet<Favourite>();
            ForgotPasswordTokens = new HashSet<ForgotPasswordToken>();
            Notifications = new HashSet<Notification>();
            //Orders = new HashSet<Order>();
            //ProductRatings = new HashSet<ProductRating>();
            //AppRatings = new HashSet<AppRatings>();
            //UserAddresses = new HashSet<UserAddress>();
            UserDevices = new HashSet<UserDevice>();
            //Feedback = new HashSet<ContactUs>();
            DoctorDocuments = new HashSet<DoctorDocument>();
        }

        public int Id { get; set; }

        //[StringLength(100)]
        //public string FirstName { get; set; }

        //[StringLength(100)]
        //public string LastName { get; set; }

        public int Gender { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        [JsonIgnore]
        public string Password { get; set; }

        //public string AccountType { get; set; }

        //public string ZipCode { get; set; }

        public string DateofBirth { get; set; }

        public int? SignInType { get; set; }

        public short? Status { get; set; }

        public bool EmailConfirmed { get; set; }

        public string Location_Name { get; set; }

        public DbGeography Location { get; set; }

        public bool PhoneConfirmed { get; set; }

        public string Country { get; set; }

        public string City { get; set; }

        public int ProviderType { get; set; }

        public string Specialization { get; set; }

        public string Department { get; set; }

        public string LatestQualification { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DoctorDocument> DoctorDocuments { get; set; }
        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        //public virtual ICollection<PaymentCard> PaymentCards { get; set; }

        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        //public virtual ICollection<Favourite> Favourites { get; set; }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        [JsonIgnore]
        public virtual ICollection<ForgotPasswordToken> ForgotPasswordTokens { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Notification> Notifications { get; set; }

        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        //public virtual ICollection<Order> Orders { get; set; }

        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        //public virtual ICollection<AppRatings> AppRatings { get; set; }

        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        //public virtual ICollection<ProductRating> ProductRatings { get; set; }

        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        //public virtual ICollection<UserAddress> UserAddresses { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UserDevice> UserDevices { get; set; }


        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        //public virtual ICollection<ContactUs> Feedback { get; set; }


        public bool IsNotificationsOn { get; set; }

        [NotMapped]
        public Token Token { get; set; }

        [NotMapped]
        public Settings AppSettings { get; set; }

        public bool IsDeleted { get; set; }

        public string ProfilePictureUrl { get; set; }

        public string SurName { get; set; }
        public string Bio { get; set; }
        //public string PassportNo { get; set; }
        //public string PassportCountryIssued { get; set; }
        //public string PassportExpiryDate { get; set; }
        public string FullName { get; set; }
    }

    public class UserRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public static class UserExtension
    {
        private static HttpClient client = new HttpClient();
        public static async Task GenerateToken(this User user, HttpRequestMessage request)
        {
            try
            {
                var parameters = new Dictionary<string, string>{
                            { "username", user.Email },
                            { "password", user.Password },
                            { "grant_type", "password" },
                            { "signintype", Convert.ToString(user.SignInType)},
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

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

    public partial class Admin
    {
        public int Id { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string Email { get; set; }

        public string Phone { get; set; }

        [Required]
        public short Role { get; set; }

        [JsonIgnore]
        public string Password { get; set; }
        
        public string AccountNo { get; set; }

        //public int? Store_Id { get; set; }

        public short? Status { get; set; }

        //public virtual Store Store { get; set; }

        public bool IsDeleted { get; set; }

        [NotMapped]
        public Token Token { get; set; }

        [NotMapped]
        public bool ImageDeletedOnEdit { get; set; }
        public string ImageUrl { get; set; }
        [NotMapped]
        public string Base64EncodedString { get; set; }

        [ForeignKey("Pharmacy")]
        public int? Pharmacy_Id { get; set; }

        [JsonIgnore]
        public virtual Pharmacy Pharmacy { get; set; }


        public List<AdminSubAdminNotifications> AdminSubAdminNotifications { get; set; }
    }


    public static class AdminExtension
    {
        private static HttpClient client = new HttpClient();
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

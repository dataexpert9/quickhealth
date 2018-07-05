using BusinessLogic.CustomAuthorization;
using BusinessLogic.SecurityServices;
using DBAccess;
using DBAccess.Models;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace AdminWebapi.App_Start
{
    public class SimpleAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        ISecurityService _SecurityService;
        public SimpleAuthorizationServerProvider()
        {
            _SecurityService = new SecurityService();
        }

        public static OAuthGrantResourceOwnerCredentialsContext AuthorizeContext;
        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            //try
            //{
            var identity = new ClaimsIdentity(context.Options.AuthenticationType);
            var form = await context.Request.ReadFormAsync();
            var SignInType = form["signintype"];
            var UserId = form["userid"];

            //using (AdminDBContext ctx = new AdminDBContext())
            //{
            User userModel = null;
            Admin adminModel = null;
            Doctor doctorModel = null;
            if (SignInType == "6" || SignInType == "7")
            {
                userModel = _SecurityService.GetUser(context.UserName);
                if (userModel != null)
                {
                    identity.AddClaim(new Claim("username", context.UserName));
                    identity.AddClaim(new Claim(ClaimTypes.Name, context.UserName));
                    identity.AddClaim(new Claim("userid", UserId));
                    identity.AddClaim(new Claim(ClaimTypes.Role, RoleTypes.User.ToString()));
                    context.Validated(identity);
                }
                //else
                //{
                //    var json = Newtonsoft.Json.JsonConvert.SerializeObject(ctx.Users.FirstOrDefault());
                //    context.SetError("invalid username or password!", json);
                //}
            }
            else if (SignInType == "0")
            {
                userModel = _SecurityService.GetUser(context.UserName, context.Password);

                if (userModel != null)
                {
                    identity.AddClaim(new Claim("username", context.UserName));
                    identity.AddClaim(new Claim(ClaimTypes.Name, context.UserName));
                    identity.AddClaim(new Claim("userid", UserId));
                    switch (userModel.SignInType)
                    {
                        case 0:
                            identity.AddClaim(new Claim(ClaimTypes.Role, RoleTypes.User.ToString()));
                            break;
                        case 1:
                            identity.AddClaim(new Claim(ClaimTypes.Role, RoleTypes.Doctor.ToString()));
                            break;
                        case 2:
                            identity.AddClaim(new Claim(ClaimTypes.Role, RoleTypes.SubAdmin.ToString()));
                            break;
                        case 3:
                            identity.AddClaim(new Claim(ClaimTypes.Role, RoleTypes.SuperAdmin.ToString()));
                            break;
                        case 4:
                            identity.AddClaim(new Claim(ClaimTypes.Role, RoleTypes.ApplicationAdmin.ToString()));
                            break;
                        case 5:
                            identity.AddClaim(new Claim(ClaimTypes.Role, RoleTypes.Guest.ToString()));
                            break;
                        default:
                            break;
                    }
                    context.Validated(identity);
                }
                //else
                //{
                //    var json = Newtonsoft.Json.JsonConvert.SerializeObject(ctx.Users.FirstOrDefault());
                //    context.SetError("invalid username or password!", json);
                //}
            }else if (SignInType == "1")
            {
                //userModel = ctx.Users.FirstOrDefault(x => x.Email == context.UserName && x.Password == context.Password);
                doctorModel = _SecurityService.GetDoctor(context.UserName, context.Password);

                if (doctorModel != null)
                {
                    identity.AddClaim(new Claim("username", context.UserName));
                    identity.AddClaim(new Claim(ClaimTypes.Name, context.UserName));
                    identity.AddClaim(new Claim("userid", UserId));
                    switch (doctorModel.SignInType)
                    {
                        case 0:
                            identity.AddClaim(new Claim(ClaimTypes.Role, RoleTypes.User.ToString()));
                            break;
                        case 1:
                            identity.AddClaim(new Claim(ClaimTypes.Role, RoleTypes.Doctor.ToString()));
                            break;
                        case 2:
                            identity.AddClaim(new Claim(ClaimTypes.Role, RoleTypes.SubAdmin.ToString()));
                            break;
                        case 3:
                            identity.AddClaim(new Claim(ClaimTypes.Role, RoleTypes.SuperAdmin.ToString()));
                            break;
                        case 4:
                            identity.AddClaim(new Claim(ClaimTypes.Role, RoleTypes.ApplicationAdmin.ToString()));
                            break;
                        case 5:
                            identity.AddClaim(new Claim(ClaimTypes.Role, RoleTypes.Guest.ToString()));
                            break;
                        default:
                            break;
                    }
                    context.Validated(identity);
                }
                //else
                //{
                //    var json = Newtonsoft.Json.JsonConvert.SerializeObject(ctx.Users.FirstOrDefault());
                //    context.SetError("invalid username or password!", json);
                //}
            }
            else if (SignInType == "2" || SignInType == "3" || SignInType == "4")
            {

                //   adminModel = ctx.Admins.FirstOrDefault(x => x.Email == context.UserName && x.Password == context.Password);
                adminModel = _SecurityService.GetAdmin(context.UserName, context.Password);
                if (adminModel != null)
                {
                    identity.AddClaim(new Claim("username", context.UserName));
                    identity.AddClaim(new Claim(ClaimTypes.Name, context.UserName));
                    identity.AddClaim(new Claim("userid", UserId));
                    switch (adminModel.Role)
                    {
                        case 0:
                            identity.AddClaim(new Claim(ClaimTypes.Role, RoleTypes.User.ToString()));
                            break;
                        case 1:
                            identity.AddClaim(new Claim(ClaimTypes.Role, RoleTypes.Doctor.ToString()));
                            break;
                        case 2:
                            identity.AddClaim(new Claim(ClaimTypes.Role, RoleTypes.SubAdmin.ToString()));
                            break;
                        case 3:
                            identity.AddClaim(new Claim(ClaimTypes.Role, RoleTypes.SuperAdmin.ToString()));
                            break;
                        case 4:
                            identity.AddClaim(new Claim(ClaimTypes.Role, RoleTypes.ApplicationAdmin.ToString()));
                            break;
                        default:
                            break;
                    }
                    context.Validated(identity);
                }
                //else
                //{
                //    var json = Newtonsoft.Json.JsonConvert.SerializeObject(ctx.Users.FirstOrDefault());
                //    context.SetError("invalid username or password!", json);
                //}
            }
            // }
            //}
            //catch (Exception ex)
            //{

            //}
        }

        public static MemoryStream GenerateStreamFromString(string value)
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(value ?? ""));
        }
    }
}
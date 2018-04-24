using System.ComponentModel.Composition;
using Resolver;

namespace BusinessLogic
{
    [Export(typeof(IComponent))]
    public class DependencyResolver : IComponent
    {
        //Latest
        public void SetUp(IRegisterComponent registerComponent)
        {
            registerComponent.RegisterType<UserServices.IUserService, UserServices.UserService>();
            registerComponent.RegisterType<CommonServices.IImageUpload, CommonServices.ImageUpload>();
            registerComponent.RegisterType<SecurityServices.ISecurityService, SecurityServices.SecurityService>();
        }
    }
}
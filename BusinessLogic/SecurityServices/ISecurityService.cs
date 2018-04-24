using DBAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.SecurityServices
{
    public interface ISecurityService
    {
        User GetUser(string email);
        User GetUser(string email, string password);
        Admin GetAdmin(string email);
        Admin GetAdmin(string email, string password);
    }
}

using DBAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.ViewModels
{
    class AdminViewModel
    {
    }
    public class SearchAdminsViewModel
    {
        public SearchAdminsViewModel()
        {
            Admins = new List<Admin>();
        }
        public List<Admin> Admins { get; set; }
    }
}

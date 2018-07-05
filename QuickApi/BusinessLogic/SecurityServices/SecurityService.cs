using DBAccess;
using DBAccess.GenericRepository;
using DBAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.SecurityServices
{
    public class SecurityService : ISecurityService
    {
        private readonly AdminDBContext _DBContext = new AdminDBContext();
        private readonly GenericRepository<User> _UserRepository;
        private readonly GenericRepository<Admin> _AdminRepository;
        private readonly GenericRepository<Doctor> _DoctorRepository;


        public SecurityService()
        {
            _UserRepository = new GenericRepository<User>(_DBContext);
            _AdminRepository = new GenericRepository<Admin>(_DBContext);
            _DoctorRepository = new GenericRepository<Doctor>(_DBContext);
        }
        public User GetUser(string email)
        {
            return _UserRepository.GetFirst(x => x.Email == email);
        }

        public User GetUser(string email,string password)
        {
            return _UserRepository.GetFirst(x => x.Email == email && x.Password==password);
        }



        public Doctor GetDoctor(string email)
        {
            return _DoctorRepository.GetFirst(x => x.Email == email);
        }

        public Doctor GetDoctor(string email, string password)
        {
            return _DoctorRepository.GetFirst(x => x.Email == email && x.Password == password);
        }



        public Admin GetAdmin(string email)
        {
            return _AdminRepository.GetFirst(x => x.Email == email);
        }
        public Admin GetAdmin(string email, string password)
        {
            return _AdminRepository.GetFirst(x => x.Email == email && x.Password == password);
        }
    }
}

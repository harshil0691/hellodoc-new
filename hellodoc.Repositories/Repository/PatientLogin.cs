using hellodoc.DbEntity.DataContext;
using hellodoc.DbEntity.DataModels;
using hellodoc.Repositories.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hellodoc.Repositories.Repository
{
    public class PatientLogin : IPatientLogin
    {
        private readonly ApplicationDbContext _context;

        public PatientLogin(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<AspNetUser> GetAspNetUser(string email, string password)
        {
            var aspnetuser = _context.AspNetUsers.FirstOrDefault(x => x.Email == email && x.Passwordhash == password);

            return aspnetuser;
        }
    }
}

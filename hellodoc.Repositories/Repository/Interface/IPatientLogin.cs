using hellodoc.DbEntity.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hellodoc.Repositories.Repository.Interface
{
    public interface IPatientLogin
    {
        Task<AspNetUser> GetAspNetUser(string email,string password); 
    }
}

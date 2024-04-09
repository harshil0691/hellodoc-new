using hellodoc.DbEntity.DataModels;
using hellodoc.Repositories.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hellodoc.Repositories.Services
{
    public class UserServices : IUserServices
    {
        public AspNetUser Login(string username, string password)
        {
            AspNetUser user = new AspNetUser();
            return user;    
        }

    }
}

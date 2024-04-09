using hellodoc.DbEntity.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hellodoc.Repositories.Services.Interface
{
    public interface IUserServices
    {
        AspNetUser Login(string username, string password);
    }
}

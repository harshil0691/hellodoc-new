using hellodoc.DbEntity.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hellodoc.Repositories.Repository.Interface
{
    public partial interface IAuthManager
    {
        public AspNetUser Login(string email, string password);
        int GetPhysician(int aspid);
    }
}

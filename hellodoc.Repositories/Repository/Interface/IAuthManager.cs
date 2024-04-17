using hellodoc.DbEntity.DataModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        void authaction(HttpContext controller, string role);
    }
}

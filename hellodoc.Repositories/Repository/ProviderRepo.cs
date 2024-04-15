using hellodoc.DbEntity.DataContext;
using hellodoc.Repositories.Repository.Interface;
using Microsoft.AspNetCore.Http.HttpResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hellodoc.Repositories.Repository
{
    public class ProviderRepo : IProviderRepo
    {
        private readonly ApplicationDbContext _context;

        public ProviderRepo(ApplicationDbContext context)
        {
            _context = context;
        }
        public string AcceptRequest(int requestid)
        {
            var request = _context.Requests.FirstOrDefault(r => r.Requestid == requestid);
            request.Status = 2;
            _context.SaveChanges();
            return "ok";
        }
    }
}

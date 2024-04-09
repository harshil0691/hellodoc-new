using hellodoc.DbEntity.DataContext;
using hellodoc.DbEntity.ViewModels;
using hellodoc.Repositories.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hellodoc.Repositories.Repository
{
    public class AdminProviderLocation : IAdminProviderLocation
    {
        private readonly ApplicationDbContext _context;

        public AdminProviderLocation(ApplicationDbContext context)
        {
            _context = context;
        }


        public List<ProviderLocation> GetProviderLocations()
        {
            var providerlocation = _context.PhysicianLocations;

            var list = providerlocation.Select(l => new ProviderLocation
            {
                Id = l.Locationid,
                Latitude = l.Latitude??0,
                Longitude = l.Longitude??0,
                ProviderName = l.Physician.Firstname,
            });

            return list.ToList();
        }
    }
}

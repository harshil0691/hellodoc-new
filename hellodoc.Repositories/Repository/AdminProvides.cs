using hellodoc.DbEntity.DataContext;
using hellodoc.DbEntity.DataModels;
using hellodoc.DbEntity.ViewModels;
using hellodoc.Repositories.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hellodoc.Repositories.Repository
{
    public class AdminProvides : IAdminProviders
    {
        private readonly ApplicationDbContext _context;

        public AdminProvides(ApplicationDbContext context)
        {
            _context = context;
        }

        public Physician GetPhysicianAsync(int physicianid)
        {
            Physician physician = _context.Physicians.FirstOrDefault(u => u.Physicianid == physicianid);
            return physician;
        }
        public List<ProvidersTableModal> ProvidersTable()
        {
            var physicians = _context.Physicians;

            var list = physicians.Select(p => new ProvidersTableModal
            {
                physicianid = p.Physicianid,
                status = p.Status ?? 0,
                name = p.Firstname + " " + p.Lastname,
                role = p.Role.Name,
                stopnotification = p.PhysicianNotifications.FirstOrDefault(),
            }).ToList();

            return list;
        }

        public async Task StopNotification(List<int> idlist, List<int> totallist)
        {
            var phy = _context.PhysicianNotifications.Where(u => totallist.Contains(u.Physicianid));

           // var list;
            foreach(var i in phy)
            {
                
            }
        }
    }
}

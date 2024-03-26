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

        public async Task<ProviderProfileModal> ProviderProfileData(int physicianid)
        {
            var physician = _context.Physicians.FirstOrDefault(u=>u.Physicianid == physicianid);
            var aspuser = _context.AspNetUsers.FirstOrDefault(u => u.Id == physician.Aspnetuserid);

            ProviderProfileModal providerProfile = new ProviderProfileModal
            {
                username = aspuser.Username,
                status = physician.Status??0,
                role = _context.Roles.FirstOrDefault(u => u.Roleid == physician.Roleid).Name,
                aspid = aspuser.Id,

                Firstname = physician.Firstname,
                Lastname = physician.Lastname,
                Email = physician.Email,
                Phone = physician.Mobile,

                Address1 = physician.Address1,
                Address2 = physician.Address2,
                City = physician.City,
                Zipcode = physician.Zip,
            };

            return providerProfile;
        }

        public async Task StopNotification(List<int> idlist, List<int> totallist)
        {
            var list = _context.PhysicianNotifications.Where(u => totallist.Contains(u.Physicianid)).Select(u => u.Physicianid).ToList();

            foreach (var phyid in totallist)
            {
                if (idlist.Contains(phyid))
                {
                    if (!list.Contains(phyid))
                    {
                        PhysicianNotification physician = new PhysicianNotification()
                        {
                            Physicianid = phyid,
                            Isnotificationstopped = 1
                        };
                        _context.PhysicianNotifications.Add(physician);
                    }
                }
                else
                {
                    if (list.Contains(phyid))
                    {
                        var physiciannotificatiion = _context.PhysicianNotifications.FirstOrDefault(pid => pid.Physicianid == phyid);
                        _context.PhysicianNotifications.Remove(physiciannotificatiion);
                    }
                }

            }
            _context.SaveChanges();
        }



    }
}


using hellodoc.DbEntity.DataContext;
using hellodoc.DbEntity.DataModels;
using hellodoc.DbEntity.ViewModels;
using hellodoc.DbEntity.ViewModels.Shifts;
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

        public List<ShiftDetailsmodal> ShiftDetailsmodal(int year, int month)
        {
            var shift = _context.Shifts;
            var shiftdetails = _context.ShiftDetails.Where(u => u.Shiftdate.Month == month && u.Shiftdate.Year == year);

            var list = shiftdetails.Select(s => new ShiftDetailsmodal
            {
                Shiftid = s.Shiftid,
                Shiftdetailid = s.Shiftdetailid,
                Shiftdate = s.Shiftdate,
                Startdate = s.Shift.Startdate,
                Starttime = s.Starttime,
                Endtime = s.Endtime,
                Physicianid = s.Shift.Physicianid,
                PhysicianName = s.Shift.Physician.Firstname,

            });

            return list.ToList();
        }

        public ShiftDetailsmodal GetShift(int shiftdetailsid)
        {
            var shiftdetails = _context.ShiftDetails.FirstOrDefault(s => s.Shiftdetailid == shiftdetailsid);
            var physicianlist = _context.PhysicianRegions.Where(p => p.Regionid == shiftdetails.Regionid).Select(s=>s.Physicianid).ToList();

            ShiftDetailsmodal shift = new ShiftDetailsmodal
            {
                Shiftdetailid = shiftdetailsid,
                Shiftdate = shiftdetails.Shiftdate,
                Shiftid = shiftdetails.Shiftid,
                Starttime = shiftdetails.Starttime,
                Endtime = shiftdetails.Endtime,
                Regionid = shiftdetails.Regionid,
                regions = _context.Regions.ToList(),
                physics = _context.Physicians.Where(p => physicianlist.Contains(p.Physicianid)).ToList(),
            };

            return shift;
        }

        public DayShiftModal DayShift(int year, int month, int date)
        {
            var shift = _context.Shifts;
            var shiftdetails = _context.ShiftDetails.Where(u => u.Shiftdate.Month == month && u.Shiftdate.Year == year && u.Shiftdate.Day == date);

            
            var list = shiftdetails.Select(s => new ShiftDetailsmodal
            {
                Shiftid = s.Shiftid,
                Shiftdetailid = s.Shiftdetailid,
                Shiftdate = s.Shiftdate,
                Startdate = s.Shift.Startdate,
                Starttime = s.Starttime,
                Endtime = s.Endtime,
                Physicianid = s.Shift.Physicianid,
                PhysicianName = s.Shift.Physician.Firstname,

            });

            DayShiftModal dayShift = new DayShiftModal();
            dayShift.Physicians = _context.Physicians.ToList();
            dayShift.shiftDetailsmodals = list.ToList();
            dayShift.shiftphysician = list.Select(u=>u.Physicianid).ToList();

            return dayShift;
        }


    }
}


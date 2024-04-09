using hellodoc.DbEntity.DataContext;
using hellodoc.DbEntity.DataModels;
using hellodoc.DbEntity.ViewModels;
using hellodoc.DbEntity.ViewModels.DashboardLists;
using hellodoc.DbEntity.ViewModels.Shifts;
using hellodoc.Repositories.Repository.Interface;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

        public List<ShiftDetailsmodal> ShiftDetailsmodal(DateTime date, DateTime sunday, DateTime saturday,string type)
        {
            var shiftdetails = _context.ShiftDetails.Where(u => u.Shiftdate.Month == date.Month && u.Shiftdate.Year == date.Year);

            switch (type)
            {
                case "month":
                    shiftdetails = _context.ShiftDetails.Where(u => u.Shiftdate.Month == date.Month && u.Shiftdate.Year == date.Year && u.Isdeleted != 1);
                    break;

                case "week":
                    shiftdetails = _context.ShiftDetails.Where(u => u.Shiftdate >= sunday && u.Shiftdate <= saturday && u.Isdeleted != 1);
                    break;

                case "day":
                    shiftdetails = _context.ShiftDetails.Where(u => u.Shiftdate.Month == date.Month && u.Shiftdate.Year == date.Year && u.Shiftdate.Day == date.Day && u.Isdeleted != 1);
                    break;
            }

           
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
                Status = s.Status,
                regionname = s.Region.Name
            });

            return list.ToList();
        }

        public List<Physician> physicians()
        {
            return _context.Physicians.ToList();
        }

        public ShiftDetailsmodal GetShift(int shiftdetailsid)
        {
            var shiftdetails = _context.ShiftDetails.FirstOrDefault(s => s.Shiftdetailid == shiftdetailsid);
            var shifts = _context.Shifts.FirstOrDefault(s => s.Shiftid == shiftdetails.Shiftid);

            ShiftDetailsmodal shift = new ShiftDetailsmodal
            {
                Shiftdetailid = shiftdetailsid,
                Shiftdate = shiftdetails.Shiftdate,
                Shiftid = shiftdetails.Shiftid,
                Starttime = shiftdetails.Starttime,
                Endtime = shiftdetails.Endtime,
                Regionid = shiftdetails.Regionid,
                regionname = _context.Regions.FirstOrDefault(r => r.Regionid == shiftdetails.Regionid).Name,
                Physicianid = shifts.Physicianid,
                PhysicianName = _context.Physicians.FirstOrDefault(u=> u.Physicianid == shifts.Physicianid).Firstname,
            };

            return shift;
        }

        public void  EditShift(int shiftdetailsid, ShiftDetailsmodal shiftDetailsmodal, int aspid)
        {
            var shiftdetail = _context.ShiftDetails.FirstOrDefault(s => s.Shiftdetailid ==shiftdetailsid);

            shiftdetail.Shiftdate = shiftDetailsmodal.Shiftdate;
            shiftdetail.Starttime = shiftDetailsmodal.Starttime;
            shiftdetail.Endtime = shiftDetailsmodal.Endtime;
            shiftdetail.Modifiedby = aspid.ToString();
            shiftdetail.Modifieddate = DateTime.Now;

            _context.SaveChanges();

        }
        public void DeleteShift(int shiftdetailsid, int aspid)
        {
            var shiftdetail = _context.ShiftDetails.FirstOrDefault(s => s.Shiftdetailid == shiftdetailsid);

            shiftdetail.Isdeleted = 1;
            shiftdetail.Modifiedby = aspid.ToString();
            shiftdetail.Modifieddate = DateTime.Now;

            _context.SaveChanges();

        }
        public void StatusChangeShift(int shiftdetailsid,int aspid)
        {
            var shiftdetail = _context.ShiftDetails.FirstOrDefault(s => s.Shiftdetailid == shiftdetailsid);

            if(shiftdetail.Status == 1)
            {
                shiftdetail.Status = 2;
                
            }else if(shiftdetail.Status == 2)
            {
                shiftdetail.Status = 1;
            }

            shiftdetail.Modifiedby = aspid.ToString();
            shiftdetail.Modifieddate = DateTime.Now;

            _context.SaveChanges();
        }

        public void CreateShift(ShiftDetailsmodal shiftDetailsmodal, int aspid)
        {
            List<int> daylist = JsonConvert.DeserializeObject<List<int>>(shiftDetailsmodal.Weekdays);

            Shift shift = new Shift 
            {
                Createdby = aspid.ToString(),
                Createddate = DateTime.Now,
                Isrepeat = shiftDetailsmodal.Isrepeat,
                Physicianid = shiftDetailsmodal.Physicianid,
                Weekdays = shiftDetailsmodal.Weekdays,
                Repeatupto = shiftDetailsmodal.Repeatupto,
            };

            _context.Shifts.Add(shift);

            ShiftDetail shiftDetail = new ShiftDetail
            {
                Shiftdate = shiftDetailsmodal.Shiftdate,
                Starttime = shiftDetailsmodal.Starttime,
                Endtime = shiftDetailsmodal.Endtime,
                Status = 1,
                Isdeleted = 0,
                Regionid = shiftDetailsmodal.Regionid,

                Shift = shift
            };
            _context.ShiftDetails.Add(shiftDetail);


            
            var daynumber = (double)shiftDetailsmodal.Shiftdate.DayOfWeek;
            var d = 0.0;

            foreach (var day in daylist)
            {
                DateTime date = shiftDetailsmodal.Shiftdate;
                var repeat = shiftDetailsmodal.Repeatupto;

                if (daynumber < day)
                {
                    d = day - daynumber;
                }
                else
                {
                    d = 6 - daynumber + day + 1;
                    if(daynumber == day)
                    {
                        repeat = repeat - 1;
                    }
                }

                date = date.AddDays(d);

                for (var i = 0; i < repeat; i++)
                {
                    ShiftDetail shiftDetail1 = new ShiftDetail
                    {
                        Shiftdate = date,
                        Starttime = shiftDetailsmodal.Starttime,
                        Endtime = shiftDetailsmodal.Endtime,
                        Status = 1,
                        Isdeleted = 0,
                        Regionid = shiftDetailsmodal.Regionid,

                        Shift = shift
                    };

                    date = date.AddDays(7);
                    _context.ShiftDetails.Add(shiftDetail1);
                }
            }


            _context.SaveChanges();
        }

        public bool CheckShift(int physicianid,TimeOnly starttime, TimeOnly endtime,DateTime shiftdate)
        {
            var shiftdetail = _context.ShiftDetails.Where(s => s.Shift.Physicianid == physicianid && s.Shiftdate == shiftdate).ToList();
            foreach(var shift in shiftdetail)
            {
                if (starttime >= shift.Endtime && shift.Starttime <= endtime)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            return false;
        }

        public DashboardListsModal ProvidersOnCallList(int regionid,DateTime date)
        {
            DateTime date1 = DateTime.Now;
            TimeOnly timeOnly = TimeOnly.FromDateTime(date);

            DashboardListsModal listsModal = new DashboardListsModal();
            listsModal.regions = _context.Regions.ToList();

            var oncallids = _context.ShiftDetails.Where(s => s.Shiftdate.Date == date.Date && s.Endtime > timeOnly && s.Starttime < timeOnly).Select(p => p.Shift.Physicianid).ToList();

            var physicianlist = _context.Physicians.ToList();

            var oncalllist = new List<ProvidersOnCallModal>();
            var offcalllist = new List<ProvidersOnCallModal>();

            var la = 1;

            foreach (var physician in physicianlist)
            {
                if(regionid != 0)
                {
                    la = _context.PhysicianRegions.Where(p => p.Physicianid == physician.Physicianid && p.Regionid == regionid).Count();
                }
                

                if(la > 0)
                {
                    if (oncallids.Contains(physician.Physicianid))
                    {
                        ProvidersOnCallModal oncall = new ProvidersOnCallModal
                        {
                            physicianId = physician.Physicianid,
                            physicianName = physician.Firstname,
                            PhotoPath = physician.Photo,
                        };
                        oncalllist.Add(oncall);
                    }
                    else
                    {
                        ProvidersOnCallModal oncall = new ProvidersOnCallModal
                        {
                            physicianId = physician.Physicianid,
                            physicianName = physician.Firstname,
                            PhotoPath = physician.Photo,
                        };
                        offcalllist.Add(oncall);
                    }
                }

            }

            listsModal.offDuty = offcalllist;
            listsModal.onCall = oncalllist;
            listsModal.regionselect = regionid;

            return listsModal;
        }

        public DashboardListsModal ShiftsDetailsList(int regionid,int pageNumber)
        {
            var shiftdetail = _context.ShiftDetails.Where(s => s.Isdeleted != 1);
            var pageSize = 5;

            if(regionid !=0 )
            {
                shiftdetail = _context.ShiftDetails.Where(s => s.Regionid == regionid);
            }

            var list = shiftdetail.Select(s => new ShiftDetailsmodal
            {
                Shiftdetailid = s.Shiftdetailid,
                Shiftid = s.Shiftid,
                Shiftdate = s.Shiftdate,
                Starttime = s.Starttime,
                Endtime = s.Endtime,
                region = s.Region.Name,
                Status = s.Status,
                datename = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(s.Shiftdate.Month) + " " + s.Shiftdate.Day + " " + s.Shiftdate.Year,
                PhysicianName = s.Shift.Physician.Firstname + ' ' + s.Shift.Physician.Lastname
            });

            DashboardListsModal listsModal = new DashboardListsModal();
            listsModal.regions = _context.Regions.ToList();
            listsModal.shiftDetailslist = list.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            listsModal.pageNumber = pageNumber;
            listsModal.pageSize = pageSize;
            listsModal.totalEntries = list.Count();
            if(list.Skip((pageNumber) * pageSize).Take(pageSize).Count() > 0)
            {
                listsModal.morePages = true;
            }
            listsModal.entries = ((pageNumber-1)*pageSize + 1)+ "-" + (((pageNumber - 1) * pageSize)+listsModal.shiftDetailslist.Count());
            listsModal.regionselect = regionid;

            return listsModal;

        }

        public void SelectedShiftOperation(List<int> shiftdetailid, string actionType)
        {
            switch (actionType)
            {
                case "approved":

                    foreach (var item in shiftdetailid)
                    {
                        var shift = _context.ShiftDetails.FirstOrDefault(u => u.Shiftdetailid == item);
                        shift.Status = 2;
                        _context.ShiftDetails.Update(shift);
                    }

                    break;

                case "delete":
                    foreach (var item in shiftdetailid)
                    {
                        var shift1 = _context.ShiftDetails.FirstOrDefault(u => u.Shiftdetailid == item);
                        shift1.Isdeleted = 1;
                        _context.ShiftDetails.Update(shift1);
                    }
                    break;

            }

            _context.SaveChanges();
        }
    }
}


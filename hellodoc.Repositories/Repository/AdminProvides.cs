using hellodoc.DbEntity.DataContext;
using hellodoc.DbEntity.DataModels;
using hellodoc.DbEntity.ViewModels;
using hellodoc.DbEntity.ViewModels.DashboardLists;
using hellodoc.DbEntity.ViewModels.Shifts;
using hellodoc.Repositories.Repository.Interface;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twilio.TwiML;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace hellodoc.Repositories.Repository
{
    public class AdminProvides : IAdminProviders
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment HostingEnviroment;
        public AdminProvides(ApplicationDbContext context, IHostingEnvironment hostingEnviroment)
        {
            _context = context;
            HostingEnviroment = hostingEnviroment;
        }

        public Physician GetPhysicianAsync(int physicianid)
        {
            var physician = _context.Physicians.FirstOrDefault(u => u.Physicianid == physicianid);
            if (physician != null)
            {
                return physician;
            }
            return new Physician();
        }
        public DashboardListsModal ProvidersTable(int pageNumber,int regionid)
        {
            var pageSize = 5;
            if (pageNumber <= 0)
            {
                pageNumber = 1;
            }

            var physicians = _context.Physicians.Where(
                p => 
                ((regionid != 0)? p.Regionid == regionid :true) && 
                (p.Isdeleted != new BitArray(1,true))
            );

            var list = physicians.Select(p => new ProvidersTableModal
            {
                physicianid = p.Physicianid,
                status = p.Status ?? 0,
                name = p.Firstname + " " + p.Lastname,
                role = p.Role.Name,
                stopnotification = p.PhysicianNotifications.FirstOrDefault(),
            }).ToList();

            list.OrderBy(p => p.physicianid);

            DashboardListsModal dashboardListsModal = new DashboardListsModal();
            dashboardListsModal.regions = _context.Regions.ToList();
            dashboardListsModal.providersTableModal = list.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            dashboardListsModal.pageNumber = pageNumber;
            dashboardListsModal.pageSize = pageSize;
            dashboardListsModal.totalEntries = list.Count();
            if (list.Skip((pageNumber) * pageSize).Take(pageSize).Count() > 0)
            {
                dashboardListsModal.morePages = true;
            }
            dashboardListsModal.entries = ((pageNumber - 1) * pageSize + 1) + "-" + (((pageNumber - 1) * pageSize) + dashboardListsModal.providersTableModal.Count());
            dashboardListsModal.regionselect = regionid;
            return dashboardListsModal;
        }

        public async Task<ProviderProfileModal> ProviderProfileData(int physicianid)
        {
            var physician = _context.Physicians.FirstOrDefault(u=>u.Physicianid == physicianid);
            if(physician != null)
            {
                var aspuser = _context.AspNetUsers.Include(u => u.AspNetUserRole.Role.RoleMenus).FirstOrDefault(a => a.Id == physician.Aspnetuserid);

                ProviderProfileModal providerProfile = new ProviderProfileModal
                {
                    username = aspuser.Username,
                    status = physician.Status ?? 0,
                    role = _context.Roles.FirstOrDefault(u => u.Roleid == physician.Roleid).Name,
                    aspid = aspuser.Id,
                    selectrole = aspuser.AspNetUserRole.Roleid,
                    Firstname = physician.Firstname,
                    Lastname = physician.Lastname,
                    ProviderEmail = physician.Email,
                    Phone = physician.Mobile.ToString(),

                    Address1 = physician.Address1,
                    Address2 = physician.Address2,
                    City = physician.City,
                    Zipcode = physician.Zip ?? 0,
                    State = physician.Regionid ??0,
                    MailingNumber = physician.Altphone,

                    BusinessName = physician.Businessname,
                    BusinessWebsite = physician.Businesswebsite,

                    photoPath = physician.Photo,
                    IndependentContractorManagementPath = physician.Isagreementdoc,
                    BackgroungCheckPath = physician.Isbackgrounddoc,
                    HIPAAPath = physician.Iscredentialdoc,
                    NondisclosureAggrementPath = physician.Isnondisclosuredoc,
                    SignaturePath = physician.Signature,
                    LicensePath = physician.Islicensedoc,
                };

                var menu = aspuser.AspNetUserRole?.Role.RoleMenus.Select(r => r.Menuid).ToList();
                var menunames = _context.Menus.Where(m => menu.Contains(m.Menuid)).Select(m => m.Name).ToList();
                if (menunames != null)
                {
                    if (menunames.Contains("EditProfile"))
                    {
                        providerProfile.IsAccessToEdit = true;
                    }
                }


                providerProfile.roles = _context.Roles.ToList();
                providerProfile.regions = _context.Regions.ToList();
                providerProfile.regionList = _context.PhysicianRegions.Where(p => p.Physicianid == physicianid).Select(r => r.Regionid).ToList();
                providerProfile.physicianId = physicianid;

                return providerProfile;
            }
            return new ProviderProfileModal();
        }

        public bool UpdateProvider(ProviderProfileModal providerProfile)
        {
            var physician = _context.Physicians.FirstOrDefault(p => p.Physicianid == providerProfile.physicianId);
            if(physician != null)
            {
                var aspuser = _context.AspNetUsers.FirstOrDefault(a => a.Id == physician.Aspnetuserid);
                var aspnetuserrole = _context.AspNetUserRoles.FirstOrDefault(r => r.Userid == aspuser.Id);
                switch (providerProfile.updateType)
                {
                    case "AspDetails":
                        aspuser.Username = providerProfile.username;
                        physician.Status = providerProfile.status;
                        aspnetuserrole.Roleid = providerProfile.selectrole;

                        _context.SaveChanges();
                        return true;

                    case "resetPassword":
                        aspuser.Passwordhash = providerProfile.password;
                        _context.SaveChanges();
                        return true;

                    case "physician":
                        physician.Firstname = providerProfile.Firstname;
                        physician.Lastname = providerProfile.Lastname;
                        physician.Email = providerProfile.ProviderEmail;
                        physician.Mobile = long.Parse(providerProfile.Phone);
                        physician.Medicallicense = providerProfile.MediacalLicense;
                        physician.Npinumber = providerProfile.NPI;
                        physician.Syncemailaddress = providerProfile.SynEmail;
                        _context.SaveChanges();

                        var physicianregion = _context.PhysicianRegions.Where(p => p.Physicianid == providerProfile.physicianId);
                        foreach (var i in physicianregion)
                        {
                            _context.PhysicianRegions.Remove(i);
                        }
                        _context.SaveChanges();

                        foreach (var region in providerProfile.regionList)
                        {
                            PhysicianRegion physicianRegion = new PhysicianRegion
                            {
                                Physicianid = physician.Physicianid,
                                Regionid = region,
                            };
                            _context.PhysicianRegions.Add(physicianRegion);
                        }
                        _context.SaveChanges();

                        return true;

                    case "mailing":
                        physician.Address1 = providerProfile.Address1;
                        physician.Address2 = providerProfile.Address2;
                        physician.City = providerProfile.City;
                        physician.Zip = providerProfile.Zipcode;
                        physician.Regionid = providerProfile.State;
                        physician.Altphone = providerProfile.MailingNumber;

                        _context.SaveChanges();
                        return true;

                    case "providerProfile":
                        physician.Businessname = providerProfile.BusinessName;
                        physician.Businesswebsite = providerProfile.BusinessWebsite;
                        var photo = "";
                        if (providerProfile.photo != null)
                        {
                            photo = Guid.NewGuid().ToString() + "_" + providerProfile.photo.FileName;
                            var filepath = Path.Combine(Path.Combine(HostingEnviroment.WebRootPath, "PhysicianDoc"), photo);
                            providerProfile.photo.CopyTo(new FileStream(filepath, FileMode.Create));

                        }
                        if (providerProfile.Signature != null)
                        {
                            photo = Guid.NewGuid().ToString() + "_" + providerProfile.Signature.FileName;
                            var filepath = Path.Combine(Path.Combine(HostingEnviroment.WebRootPath, "PhysicianDoc"), photo);
                            providerProfile.Signature.CopyTo(new FileStream(filepath, FileMode.Create));
                            physician.Signature = providerProfile.SignaturePath;

                        }
                        physician.Photo = photo;
                        physician.Signature = providerProfile.SignaturePath;
                        physician.Adminnotes = providerProfile.AdminNotes;

                        _context.SaveChanges();

                        return true;
                    case "onboarding":

                        var IndependentContractorManagement = "";
                        var BackgroungCheck = "";
                        var HIPAA = "";
                        var NondisclosureAggrement = "";

                        if (providerProfile.IndependentContractorManagement != null)
                        {
                            IndependentContractorManagement = Guid.NewGuid().ToString() + "_" + providerProfile.IndependentContractorManagement.FileName;
                            var filepath = Path.Combine(Path.Combine(HostingEnviroment.WebRootPath, "PhysicianDoc"), IndependentContractorManagement);
                            providerProfile.IndependentContractorManagement.CopyTo(new FileStream(filepath, FileMode.Create));
                        }
                        if (providerProfile.BackgroungCheck != null)
                        {
                            BackgroungCheck = Guid.NewGuid().ToString() + "_" + providerProfile.BackgroungCheck.FileName;
                            var filepath = Path.Combine(Path.Combine(HostingEnviroment.WebRootPath, "PhysicianDoc"), BackgroungCheck);
                            providerProfile.BackgroungCheck.CopyTo(new FileStream(filepath, FileMode.Create));
                        }
                        if (providerProfile.HIPAA != null)
                        {
                            HIPAA = Guid.NewGuid().ToString() + "_" + providerProfile.HIPAA.FileName;
                            var filepath = Path.Combine(Path.Combine(HostingEnviroment.WebRootPath, "PhysicianDoc"), HIPAA);
                            providerProfile.HIPAA.CopyTo(new FileStream(filepath, FileMode.Create));
                        }
                        if (providerProfile.NondisclosureAggrement != null)
                        {
                            NondisclosureAggrement = Guid.NewGuid().ToString() + "_" + providerProfile.NondisclosureAggrement.FileName;
                            var filepath = Path.Combine(Path.Combine(HostingEnviroment.WebRootPath, "PhysicianDoc"), NondisclosureAggrement);
                            providerProfile.NondisclosureAggrement.CopyTo(new FileStream(filepath, FileMode.Create));
                        }

                        physician.Isagreementdoc = IndependentContractorManagement;
                        physician.Isbackgrounddoc = BackgroungCheck;
                        physician.Iscredentialdoc = HIPAA;
                        physician.Isnondisclosuredoc = NondisclosureAggrement;

                        _context.SaveChanges();
                        return true;

                    default:
                        return false;
                }
            }
            return false;
        }

        public bool DeleteProvider(int physicianid)
        {
            var physician = _context.Physicians.FirstOrDefault(p => p.Physicianid == physicianid);
            if(physician != null)
            {
                physician.Isdeleted = new BitArray(1,true);
                _context.SaveChanges();
                return true;
            }
            return false;
        }

        public ProviderProfileModal GetForCreateProvider()
        {
            ProviderProfileModal providerProfile = new ProviderProfileModal
            {
                roles = _context.Roles.ToList(),
                regions = _context.Regions.ToList(),
            };

            return providerProfile;
        }

        public void CreateProvider(ProviderProfileModal providerProfile)
        {
            if (providerProfile != null)
            {
                AspNetUser aspNetUser = new AspNetUser
                {
                    Username = providerProfile.username,
                    Passwordhash = providerProfile.password,
                    Email = providerProfile.ProviderEmail,
                    Phonenumber = long.Parse(providerProfile.Phone),
                    Createddate = DateTime.Now,
                };
                _context.AspNetUsers.Add(aspNetUser);
                _context.SaveChanges();

                var photo = "";
                var IndependentContractorManagement = "";
                var BackgroungCheck = "";
                var HIPAA = "";
                var NondisclosureAggrement = "";

                if (providerProfile.photo != null)
                {
                    photo = Guid.NewGuid().ToString() + "_" + providerProfile.photo.FileName;
                    var filepath = Path.Combine(Path.Combine(HostingEnviroment.WebRootPath, "PhysicianDoc"), photo);
                    providerProfile.photo.CopyTo(new FileStream(filepath, FileMode.Create));

                }
                if (providerProfile.IndependentContractorManagement != null)
                {
                    IndependentContractorManagement = Guid.NewGuid().ToString() + "_" + providerProfile.IndependentContractorManagement.FileName;
                    var filepath = Path.Combine(Path.Combine(HostingEnviroment.WebRootPath, "PhysicianDoc"), IndependentContractorManagement);
                    providerProfile.IndependentContractorManagement.CopyTo(new FileStream(filepath, FileMode.Create));
                }
                if (providerProfile.BackgroungCheck != null)
                {
                    BackgroungCheck = Guid.NewGuid().ToString() + "_" + providerProfile.BackgroungCheck.FileName;
                    var filepath = Path.Combine(Path.Combine(HostingEnviroment.WebRootPath, "PhysicianDoc"), BackgroungCheck);
                    providerProfile.BackgroungCheck.CopyTo(new FileStream(filepath, FileMode.Create));
                }
                if (providerProfile.HIPAA != null)
                {
                    HIPAA = Guid.NewGuid().ToString() + "_" + providerProfile.HIPAA.FileName;
                    var filepath = Path.Combine(Path.Combine(HostingEnviroment.WebRootPath, "PhysicianDoc"), HIPAA);
                    providerProfile.HIPAA.CopyTo(new FileStream(filepath, FileMode.Create));
                }
                if (providerProfile.NondisclosureAggrement != null)
                {
                    NondisclosureAggrement = Guid.NewGuid().ToString() + "_" + providerProfile.NondisclosureAggrement.FileName;
                    var filepath = Path.Combine(Path.Combine(HostingEnviroment.WebRootPath, "PhysicianDoc"), NondisclosureAggrement);
                    providerProfile.NondisclosureAggrement.CopyTo(new FileStream(filepath, FileMode.Create));
                }



                Physician physician = new Physician
                {
                    Aspnetuser = aspNetUser,
                    Firstname = providerProfile.Firstname,
                    Lastname = providerProfile.Lastname,
                    Email = providerProfile.ProviderEmail,
                    Mobile = long.Parse(providerProfile.Phone),
                    Address1 = providerProfile.Address1,
                    Address2 = providerProfile.Address2,
                    Adminnotes = providerProfile.AdminNotes,
                    Zip = providerProfile.Zipcode,
                    City = providerProfile.City,
                    Createdby = "admin",
                    Regionid = providerProfile.State,
                    Businessname = providerProfile.BusinessName,
                    Npinumber = providerProfile.NPI,
                    Photo = photo,
                    Isagreementdoc = IndependentContractorManagement,
                    Isbackgrounddoc = BackgroungCheck,
                    Iscredentialdoc = HIPAA,
                    Isnondisclosuredoc = NondisclosureAggrement,
                    Roleid = providerProfile.selectrole,
                };

                _context.Physicians.Add(physician);
                _context.SaveChanges();

                foreach (var i in providerProfile.regionList)
                {
                    PhysicianRegion physicianRegion = new PhysicianRegion()
                    {
                        Regionid = i,
                        Physicianid = physician.Physicianid
                    };
                    _context.PhysicianRegions.Add(physicianRegion);
                }

                AspNetUserRole aspNetUserRole = new AspNetUserRole()
                {
                    User = aspNetUser,
                    Roleid = providerProfile.selectrole,
                };

                PhysicianLocation physicianLocation = new PhysicianLocation
                {
                    Physician = physician,
                    Physicianname = physician.Firstname + physician.Lastname,
                    Latitude = providerProfile.Lat,
                    Longitude = providerProfile.Lang,
                };
                _context.AspNetUserRoles.Add(aspNetUserRole);
                _context.PhysicianLocations.Add(physicianLocation);
                _context.SaveChanges();
            }
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

        public List<ShiftDetailsmodal> ShiftDetailsmodal(DateTime date, DateTime sunday, DateTime saturday,string type, int physicianid,int regionid,string schedulingFor)
        {
            var shiftdetails = _context.ShiftDetails.Where(u => u.Shiftdate.Month == date.Month && 
                                                                u.Shiftdate.Year == date.Year && 
                                                                ((regionid != 0) ? u.Regionid == regionid :true)&&
                                                                ((schedulingFor == "provider") && (physicianid !=0)? u.Shift.Physicianid == physicianid:true)
                                                                );

            switch (type)
            {
                case "month":
                    shiftdetails = _context.ShiftDetails.Where(
                        u => u.Shiftdate.Month == date.Month && 
                        u.Shiftdate.Year == date.Year 
                        && u.Isdeleted != 1 && 
                        ((physicianid !=0 )? u.Shift.Physicianid == physicianid : true) &&
                        ((regionid != 0) ? u.Regionid == regionid : true)
                        );
                    break;

                case "week":
                    shiftdetails = _context.ShiftDetails.Where(u => u.Shiftdate >= sunday && u.Shiftdate <= saturday && u.Isdeleted != 1 &&
                                                                    ((regionid != 0) ? u.Regionid == regionid : true)
                                                                    );
                    break;

                case "day":
                    shiftdetails = _context.ShiftDetails.Where(u => u.Shiftdate.Month == date.Month && u.Shiftdate.Year == date.Year && u.Shiftdate.Day == date.Day && u.Isdeleted != 1 && ((regionid != 0) ? u.Regionid == regionid : true));
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

        public List<Physician> physicians(int regionid)
        {
            return _context.Physicians.Where(p => ((regionid !=0 )?p.PhysicianRegions.Select(p => p.Regionid).Contains(regionid):true)).ToList();
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
            if (shiftDetailsmodal.Isrepeat != false)
            {
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
                        if (daynumber == day)
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

            var oncallids = _context.ShiftDetails.Where(s => s.Shiftdate.Date == date.Date && s.Endtime >= timeOnly && s.Starttime <= timeOnly && s.Isdeleted != 1).Select(p => p.Shift.Physicianid).ToList();

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


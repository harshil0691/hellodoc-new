﻿using hellodoc.DbEntity.DataContext;
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
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Drawing;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using NUnit.Framework.Constraints;
using Twilio.TwiML.Voice;

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
                    AdminNotes = physician.Adminnotes,

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
                            string webRootPath = HostingEnviroment.WebRootPath;

                            var folderpath = Path.Combine("physician", physician.Physicianid.ToString());

                            string directoryPath = Path.Combine(webRootPath, folderpath);
                            if (!Directory.Exists(directoryPath))
                            {
                                Directory.CreateDirectory(directoryPath);
                            }

                            var filepath = Path.Combine(Path.Combine(HostingEnviroment.WebRootPath, folderpath), "Signature");
                            providerProfile.Signature.CopyTo(new FileStream(filepath, FileMode.Create));
                            physician.Signature = filepath;
                        }

                        if(providerProfile.SignaturePath != null)
                        {

                            providerProfile.SignaturePath = ConvertToImageAndSave(providerProfile.SignaturePath,physician.Physicianid);
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

        public string ConvertToImageAndSave(string dataUrl,int physicianid)
        {
            string[] parts = dataUrl.Split(',');
            if (parts.Length != 2)
            {
                throw new ArgumentException("Invalid data URL format.");
            }
            string imageData = parts[1];
            byte[] imageBytes = Convert.FromBase64String(imageData);

            using (MemoryStream ms = new MemoryStream(imageBytes))
            {
                System.Drawing.Image image = System.Drawing.Image.FromStream(ms);

                string webRootPath = HostingEnviroment.WebRootPath;

                var folderpath = Path.Combine("physician", physicianid.ToString());

                string directoryPath = Path.Combine(webRootPath,folderpath);
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                string filePath = Path.Combine(directoryPath, "Signature.png");

                image.Save(filePath);

                return Path.Combine(folderpath, "Signature.png");
            }
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
                    Businesswebsite = providerProfile.BusinessWebsite,
                    Medicallicense = providerProfile.MediacalLicense,
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

        public void StopNotification(List<int> idlist, List<int> totallist)
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
                    shiftdetails = _context.ShiftDetails.Where(u => u.Shiftdate.Date >= sunday.Date && u.Shiftdate.Date <= saturday.Date && u.Isdeleted != 1 &&
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
                regionname = s.Region.Abbreviation,
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

        public List<Timesheet> GetTimesheets(PartialViewModal partialView)
        {
            var date = new DateOnly((partialView.currentYear!=0?partialView.currentYear:DateTime.Now.Year), (partialView.currentMonth != 0 ? partialView.currentMonth : DateTime.Now.Month), 1);
            var fromdate = date;
            var todate = date;
            
            if (partialView.timeSlot == 1 || partialView.timeSlot == 0)
            {
                fromdate = date;
                todate = date.AddDays(14);
            }
            else if (partialView.timeSlot == 2)
            {
                fromdate = date.AddDays(15);
                todate = date.AddMonths(1).AddDays(-1);
            }


            var invoice = _context.Invoicings.FirstOrDefault(
                i => i.Physicianid == partialView.physicianid &&
                i.Monthhalf == partialView.timeSlot && 
                i.Monthnumber == partialView.currentMonth &&
                i.Year == partialView.currentYear
                );
            
            if (invoice == null)
            {
                List<Timesheet> timesheets = new List<Timesheet>();


                for (DateOnly i = fromdate; i <= todate; i = i.AddDays(1))
                {
                    Timesheet timesheet = new Timesheet();
                    timesheet.Date = i;
                    timesheet.Oncallhours = 0;
                    timesheet.Weekend = false; 
                    timesheets.Add(timesheet); 
                }

                return timesheets.OrderBy(t => t.Date).ToList();

            }

            return _context.Timesheets.Where(t => t.Invoicingid == invoice.Invoicingid).OrderBy(t => t.Date).ToList();
        }

        public List<PayrateCountModal> GetDashTimeSheet(PartialViewModal partialView)
        {
            var date = new DateOnly((partialView.currentYear != 0 ? partialView.currentYear : DateTime.Now.Year), (partialView.currentMonth != 0 ? partialView.currentMonth : DateTime.Now.Month), 1);
            var fromdate = date;
            var todate = date;

            if (partialView.timeSlot == 1 || partialView.timeSlot == 0)
            {
                fromdate = date;
                todate = date.AddDays(14);
            }
            else if (partialView.timeSlot == 2)
            {
                fromdate = date.AddDays(15);
                todate = date.AddMonths(1).AddDays(-1);
            }


            var invoice = _context.Invoicings.FirstOrDefault(
                i => i.Physicianid == partialView.physicianid &&
                (partialView.timeSlot == 0 ? i.Monthhalf == 1 :i.Monthhalf == partialView.timeSlot )&&
                i.Monthnumber == date.Month &&
                i.Year == date.Year
                );

            if(invoice != null)
            {
                var list = _context.Timesheets.Where(t => t.Invoicingid == invoice.Invoicingid && t.Weekend == true).Select(s => s.Date);

                List<PayrateCountModal> payrateCount = new List<PayrateCountModal>();
                for (DateOnly i = fromdate; i <= todate; i = i.AddDays(1))
                {
                    var timesheet = _context.Timesheets.FirstOrDefault(t => t.Invoicingid == invoice.Invoicingid && t.Date == i);

                    PayrateCountModal countModal = new PayrateCountModal();
                    countModal.Date = i;
                    countModal.Shift = _context.ShiftDetails.Where(s => s.Shift.Physicianid == partialView.physicianid && DateOnly.FromDateTime(s.Shiftdate) == i).Count();
                    countModal.Nightshiftweekend = _context.ShiftDetails.Where(s => s.Shift.Physicianid == partialView.physicianid && DateOnly.FromDateTime(s.Shiftdate) == i && list.Contains(i)).Count();
                    countModal.Housecall = timesheet.Housecalls??0;
                    countModal.Phonecounsults = timesheet.Phoneconsults??0;
                    if(timesheet.Weekend == true)
                    {
                        countModal.Housecallnightweekend = timesheet.Housecalls ?? 0;
                        countModal.Phonecounsultsnightweekend = timesheet.Phoneconsults ?? 0;
                    }
                    else
                    {
                        countModal.Housecallnightweekend = 0;
                        countModal.Phonecounsultsnightweekend = 0;
                    }
                    countModal.Batchtesting = 0;
                    
                    payrateCount.Add(countModal);
                }

                return payrateCount;
            }
            return new List<PayrateCountModal>();
        }

        public List<Invoicing> GetInvoicings(PartialViewModal partialView)
        {
            var invoice = _context.Invoicings.Where(
                i => (partialView.physicianid != 0 ? i.Physicianid == partialView.physicianid : true)  &&
                (partialView.timeSlot != 0 ? i.Monthhalf == partialView.timeSlot : true) &&
                (partialView.currentMonth != 0 ? i.Monthnumber == partialView.currentMonth : true) &&
                (partialView.currentYear != 0 ? i.Year == partialView.currentYear : true)
                );

            return invoice.ToList();
        }

        public void SaveTimesheet(List<Timesheet> timesheets, int aspid, int physicianid, int month, int year, int timeSlot)
        {
            var date = new DateOnly(year,month, 1);
            var fromdate = date;
            var todate = date;

            if (timeSlot == 1)
            {
                fromdate = date;
                todate = date.AddDays(14);
            }
            else if (timeSlot == 2)
            {
                fromdate = date.AddDays(15);
                todate = date.AddMonths(1).AddDays(-1);
            }

            var invoice = _context.Invoicings.FirstOrDefault(
                i => i.Physicianid == physicianid &&
                i.Monthhalf == timeSlot &&
                i.Monthnumber == month &&
                i.Year == year
                );

            if (invoice == null)
            {
                Invoicing invoicing = new Invoicing();
                invoicing.Physicianid = physicianid;
                invoicing.Createddate = new DateOnly(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                invoicing.Createdby = aspid;
                invoicing.Fromdate = fromdate;
                invoicing.Todate = todate;
                invoicing.Monthnumber = month;
                invoicing.Monthhalf = timeSlot;
                invoicing.Year = year;

                _context.Invoicings.Add(invoicing);
                _context.SaveChanges();

                foreach(var sheet in timesheets)
                {
                    sheet.Invoicing = invoicing;
                    _context.Timesheets.Add(sheet);
                    _context.SaveChanges();
                }

            }
            else
            {
                foreach (var sheet in timesheets)
                {
                    var timesheet = _context.Timesheets.FirstOrDefault(t => t.Date == sheet.Date && sheet.Invoicingid == invoice.Invoicingid);
                    timesheet.Totalhours = sheet.Totalhours;
                    timesheet.Housecalls = sheet.Housecalls;
                    timesheet.Phoneconsults = sheet.Phoneconsults;
                    timesheet.Weekend = sheet.Weekend;

                    _context.SaveChanges();
                }
            }
        }

        public void SaveReciept(RecieptModal reciept, string saveType)
        {
            string webRootPath = HostingEnviroment.WebRootPath;
            var folderpath = Path.Combine("invoicing", reciept.Invoicingid.ToString());
            string directoryPath = Path.Combine(webRootPath, folderpath);
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            if(reciept.Billdoc != null)
            {
                var filepath = Path.Combine(Path.Combine(directoryPath, reciept.Billdoc.FileName));
                reciept.Billdoc.CopyTo(new FileStream(filepath, FileMode.Create));
            }
            

            var timesheet = _context.Timesheets.FirstOrDefault(t => t.Timesheetid == reciept.Timesheetid);
            timesheet.Item = reciept.Item;
            timesheet.Amount = reciept.Amount;
            if(reciept.Billdoc != null) {
                timesheet.Billdoc = reciept.Billdoc.FileName;
            }
            
            _context.SaveChanges();
        }

        public void DeleteReciept(int timesheetid,int aspid)
        {
            var timesheet = _context.Timesheets.FirstOrDefault(t => t.Timesheetid == timesheetid);
            if (timesheet != null)
            {
                var invoicing = _context.Invoicings.FirstOrDefault(i => i.Invoicingid == timesheet.Invoicingid);
                invoicing.Modifieddate = new DateOnly(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.Day);
                invoicing.Modifiedby = aspid;

                timesheet.Amount = 0;
                timesheet.Billdoc = null;
                timesheet.Item = null;
            }
            _context.SaveChanges();
        }

        public void finalizeTimeSheet(int invoicingid, int aspid)
        {
            var invoice = _context.Invoicings.FirstOrDefault(i => i.Invoicingid == invoicingid);
            if (invoice != null)
            {
                invoice.Finalizeddate = new DateOnly(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                invoice.Finalizedby = aspid;
            }
            _context.SaveChanges();
        }

        public void approveTimesheet(int invoicingid, int aspid)
        {
            var invoice = _context.Invoicings.FirstOrDefault(i => i.Invoicingid == invoicingid);
            if (invoice != null)
            {
                invoice.Aproveddate = new DateOnly(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                invoice.Approvedby = aspid;
            }
            _context.SaveChanges();
        }

        public Payrate GetPayrate(int physicianid)
        {
            var payrate = _context.Payrates.FirstOrDefault(p => p.Physicianid == physicianid);
            if(payrate != null)
            {
                return payrate;
            }
            
            return new Payrate { Physicianid = physicianid };
        }

        public void SavePayrate(int physicianid, int payratetype, int amount,int aspid)
        {
            var payrate = _context.Payrates.FirstOrDefault(p => p.Physicianid == physicianid);
            if (payrate == null)
            {
                payrate = new Payrate
                {
                    Physicianid = physicianid
                };
                _context.Payrates.Add(payrate);
                _context.SaveChanges();
            }

            switch (payratetype)
            {
                case 1:
                    payrate.Nightshiftweekend = amount;
                    break;

                case 2:
                    payrate.Shift = amount;
                    break;

                case 3:
                    payrate.Housecallnightweekend = amount;
                    break;

                case 4:
                    payrate.Phonecounsults = amount;
                    break;

                case 5:
                    payrate.Phonecounsultsnightweekend = amount;
                    break;

                case 6:
                    payrate.Batchtesting = amount;
                    break;

                case 7:
                    payrate.Housecall = amount;
                    break;
            }
            payrate.Modifiedby = aspid.ToString();
            payrate.Modifieddate = new DateOnly(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.Day);
            _context.SaveChanges();
        }
    }
}


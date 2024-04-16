using hellodoc.DbEntity.DataContext;
using hellodoc.DbEntity.DataModels;
using hellodoc.DbEntity.ViewModels;
using hellodoc.DbEntity.ViewModels.AdminAccess;
using hellodoc.DbEntity.ViewModels.PopUpModal;
using hellodoc.Repositories.Repository.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace hellodoc.Repositories.Repository
{
    public class AdminDashRepository : IAdminDashRepository
    {

        private readonly ApplicationDbContext _context;

        public AdminDashRepository(ApplicationDbContext context)
        {
            _context = context;
        }


        public AdminParent GetRequests(PartialViewModal partialView)
        {
            var page = 1;
            if (partialView.pageNumber >1)
            {
                page = partialView.pageNumber;
            }
            var request = _context.Requests.Where(
                r => 
                (partialView.status.Contains(r.Status)) &&
                (((partialView.search != null )? r.RequestClients.FirstOrDefault().Firstname.Contains(partialView.search) :true) ||
                ((partialView.search != null) ? r.RequestClients.FirstOrDefault().Lastname.Contains(partialView.search):true)) && 
                ((partialView.regionid !=0 )? r.RequestClients.FirstOrDefault().Regionid == partialView.regionid : true) && 
                ((partialView.physicianid != 0) ? r.Physicianid == partialView.physicianid : true) && 
                ((partialView.requesttype != 0)? r.Requesttypeid == partialView.requesttype:true)
            );
            int pagesize = 5;

            var requestlist = new List<AdminDashModel>();

            if(partialView.provider == true)
            {
                requestlist = request.Select(r => new AdminDashModel
                {
                    Requestid = r.Requestid,
                    PatientName = r.RequestClients.Select(rc => rc.Firstname).FirstOrDefault() + " " + r.RequestClients.Select(rc => rc.Lastname).FirstOrDefault(),
                    Phonenumber_P = r.RequestClients.Select(rc => rc.Phonenumber).FirstOrDefault(),
                    Phonenumber_R = r.Phonenumber,
                    Status = r.Status,
                    Address = r.RequestClients.Select(rc => rc.City + " " + rc.State + " " + rc.Zipcode).FirstOrDefault(),
                    Requesttypeid = r.Requesttypeid,
                    Requestclientid = r.RequestClients.Select(rc => rc.Requestclientid).FirstOrDefault(),
                    Email = r.Email,
                    regionid = r.RequestClients.Select(rc => rc.Regionid).FirstOrDefault() ?? 0,
                    CallType = (r.Calltype != 0 && r.Calltype != null)? 
                                ((r.Calltype == 1)? "HouseCall": "Consult") : "-" ,

                }).ToList();
            }
            else
            {
                requestlist = request.Select(r => new AdminDashModel
                {
                    Requestid = r.Requestid,
                    PatientName = r.RequestClients.Select(rc => rc.Firstname).FirstOrDefault() + " " + r.RequestClients.Select(rc => rc.Lastname).FirstOrDefault(),
                    DateOfBirth = r.DateOfBirth,
                    Phonenumber_P = r.RequestClients.Select(rc => rc.Phonenumber).FirstOrDefault(),
                    Phonenumber_R = r.Phonenumber,
                    Status = r.Status,
                    //Createddate = r.Createddate,
                    Address = r.RequestClients.Select(rc => rc.City + " " + rc.State + " " + rc.Zipcode).FirstOrDefault(),
                    RequestorName = r.Firstname + " " + r.Lastname,
                    Requesttypeid = r.Requesttypeid,
                    Requestclientid = r.RequestClients.Select(rc => rc.Requestclientid).FirstOrDefault(),
                    Email = r.Email,
                    Physicianname = _context.Physicians.FirstOrDefault(p => p.Physicianid == r.Physicianid).Firstname,
                    regionid = r.RequestClients.Select(rc => rc.Regionid).FirstOrDefault() ?? 0,
                }).ToList();
            }

            AdminParent admin = new AdminParent();
            admin.requestcount = requestlist.Count();
            admin.search = partialView.search;
            admin.regionid = partialView.regionid;

            if(partialView.export == true)
            {
                admin.adminDashModels = requestlist.ToList();
                return admin;
            }

            admin.adminDashModels = requestlist.Skip((page - 1) * pagesize).Take(pagesize).ToList();
            admin.pageNumber = page;
            admin.pageSize = pagesize;
            admin.totalEntries = requestlist.Count();
            if (requestlist.Skip((page) * pagesize).Take(pagesize).Count() > 0)
            {
                admin.morePages = true;
            }
            admin.entries = ((page - 1) * pagesize + 1) + "-" + (((page - 1) * pagesize) + admin.adminDashModels.Count());

            return admin;
        }

        public async Task<PatientReqModel> Getpatientdata(int rid)
        {
            PatientReqModel model = new PatientReqModel();

            var data = _context.RequestClients.FirstOrDefault(u => u.Requestid == rid);
            var data1 = _context.Requests.FirstOrDefault(u => u.Requestid == rid);

            model.Firstname = data.Firstname;
            model.Lastname = data.Lastname;
            model.Symptoms = data.Notes;
            model.City = data.City;
            model.Roomno = data.Address;
            model.Confirmationnumber = data1.Confirmationnumber;
            model.Email = data.Email;
            model.Phonenumber = data.Phonenumber??1;
            model.Requestid = rid;

            return model;
        }

        public async Task<RequestCountByStatus> GetCount(string accountType,int physicianid)
        {
            RequestCountByStatus requestCount = new RequestCountByStatus();

            if (accountType == "provider")
            {
                requestCount.NewCount = _context.Requests.Where(u => u.Status == 1 && u.Physicianid == physicianid).Count();
                requestCount.PendingCount = _context.Requests.Where(u => u.Status == 2 && u.Physicianid == physicianid).Count();
                requestCount.ActiveCount = _context.Requests.Where(u => (u.Status == 4 || u.Status == 5) && u.Physicianid == physicianid).Count();
                requestCount.ConcludeCount = _context.Requests.Where(u => u.Status == 6 && u.Physicianid == physicianid).Count();
                requestCount.TocloseCount = _context.Requests.Where(u => (u.Status == 7 || u.Status == 8 || u.Status == 3) && u.Physicianid == physicianid).Count();
                requestCount.UnpaidCount = _context.Requests.Where(u => u.Status == 9 && u.Physicianid == physicianid).Count();
            }
            else
            {
                requestCount.NewCount = _context.Requests.Where(u => u.Status == 1).Count();
                requestCount.PendingCount = _context.Requests.Where(u => u.Status == 2).Count();
                requestCount.ActiveCount = _context.Requests.Where(u => u.Status == 4 || u.Status == 5).Count();
                requestCount.ConcludeCount = _context.Requests.Where(u => u.Status == 6).Count();
                requestCount.TocloseCount = _context.Requests.Where(u => u.Status == 7 || u.Status == 8 || u.Status == 3).Count();
                requestCount.UnpaidCount = _context.Requests.Where(u => u.Status == 9).Count();
            }

            return requestCount;
        }

        public async Task SetNotes(NotesModel note,int? reqid,string? username)
        {
            
            RequestNote rnote = _context.RequestNotes.FirstOrDefaultAsync(u => u.Requestid == reqid).Result;

            if (rnote == null)
            {
                RequestNote rnote1 = new RequestNote();
                rnote1.Adminnotes = note.Adminnotes;
                rnote1.Requestid = reqid ??0;
                rnote1.Createddate = DateTime.Now;
                rnote1.Createdby = username;


                _context.RequestNotes.Add(rnote1);
                _context.SaveChanges();
            }
            else
            {
                rnote.Adminnotes = note.Adminnotes;
                rnote.Modifieddate = DateTime.Now;
                rnote.Modifiedby = username;
                _context.SaveChanges();
            }

        }

        public async Task<NotesModel> GetNotes(int reqid)
        {
            var rnote = _context.RequestNotes.FirstOrDefaultAsync(u => u.Requestid == reqid).Result;
            var rslnote = _context.RequestStatusLogs.Where(u => u.Requestid==reqid);

            NotesModel requestNote = new NotesModel();
            if(rnote != null)
            {
                requestNote.Adminnotes = rnote.Adminnotes;
                requestNote.Requestid = reqid;
            }

            foreach (var a in rslnote)
            {
                if (a.Transtophysicianid != null)
                {
                    requestNote.TransferNotes = a.Notes;
                }

            }


            return requestNote;
        }


        public async Task CancelRequest(int? reqid,CancelCaseModel cancelCase, int? adminid)
        {
            Request request = _context.Requests.FirstOrDefaultAsync(u => u.Requestid == reqid).Result;

            if (request != null)
            {
                RequestStatusLog requestStatusLog = new RequestStatusLog();
                requestStatusLog.Status = 8;
                requestStatusLog.Notes = cancelCase.CancelNotes;
                requestStatusLog.Requestid = request.Requestid;
                requestStatusLog.Adminid = adminid;
                requestStatusLog.Createddate = DateTime.Now;

                _context.RequestStatusLogs.Add(requestStatusLog);

                request.Status = 8;
                request.Casetag = cancelCase.CancelReasonValue;

                _context.SaveChanges();
            }
        }

        public async Task Clearcase(int? reqid, int? adminid)
        {
            Request request = _context.Requests.FirstOrDefaultAsync(u => u.Requestid == reqid).Result;

            if (request != null)
            {
                RequestStatusLog requestStatusLog = new RequestStatusLog();
                requestStatusLog.Status = 8;
                requestStatusLog.Requestid = request.Requestid;
                requestStatusLog.Adminid = adminid;
                requestStatusLog.Createddate = DateTime.Now;

                _context.RequestStatusLogs.Add(requestStatusLog);

                request.Status = 8;

                _context.SaveChanges();
            }
        }

        public async Task AssignCase(int? reqid, AssignCaseModal assignCase, int? adminid)
        {
            Request request = _context.Requests.FirstOrDefaultAsync(u => u.Requestid == reqid).Result;

            if (request != null)
            {
                RequestStatusLog requestStatusLog = new RequestStatusLog();
                requestStatusLog.Status = 1;
                requestStatusLog.Notes = assignCase.Discription;
                requestStatusLog.Requestid = request.Requestid;
                requestStatusLog.Adminid = adminid;
                requestStatusLog.Physicianid = assignCase.Physicianid;
                requestStatusLog.Createddate = DateTime.Now;
                if(assignCase.Modaltype == "Transfer")
                {
                    requestStatusLog.Transtophysicianid = assignCase.Physicianid;
                }
                
                _context.RequestStatusLogs.Add(requestStatusLog);

                request.Physicianid = assignCase.Physicianid;

                _context.SaveChanges();
            }
        }

        public async Task BlockCase(int? reqid, BlockCaseModal blockCase, int? adminid)
        {
            Request request = _context.Requests.FirstOrDefaultAsync(u => u.Requestid == reqid).Result;
            RequestClient requestClient = _context.RequestClients.FirstOrDefaultAsync(u => u.Requestid == reqid).Result;

            if (request != null)
            {
                RequestStatusLog requestStatusLog = new RequestStatusLog();
                requestStatusLog.Status = 3;
                requestStatusLog.Notes = blockCase.Blocknotes;
                requestStatusLog.Requestid = request.Requestid;
                requestStatusLog.Adminid = adminid;
                requestStatusLog.Createddate = DateTime.Now;

                _context.RequestStatusLogs.Add(requestStatusLog);

                BlockRequest blockRequest = new BlockRequest { 
                    Requestid = request.Requestid,
                    Phonenumber = requestClient.Phonenumber,
                    Email = requestClient.Email,
                    Createddate = DateTime.Now,
                    Reason = blockCase.Blocknotes
                };

                request.Status = 3;

                _context.SaveChanges();
            }
        }

        public  List<Region> GetRegions(int physicianid)
        {
            if (physicianid !=0 )
            {
                var regionidlist = _context.PhysicianRegions.Where(p => p.Physicianid == physicianid).Select(p => p.Regionid).ToList();
                var regions = _context.Regions.Where(r  => regionidlist.Contains(r.Regionid)).ToList();
                return regions;
            }
            else
            {
                var regions = _context.Regions.ToList();
                return regions;
            }
            
        }

        public List<Physician> GetPhysicianList()
        {
            var physician = _context.Physicians.ToList();

            return physician;
        }

        public List<Physician> GetPhysicianList2(int select)
        {
            var physician = _context.Physicians.Where(r => r.Regionid == select).ToList();

            return physician;
        }

        public async Task DeleteDocument(int docid)
        {
            RequestWiseFile requestWise =  _context.RequestWiseFiles.FirstOrDefault(u => u.Requestwisefileid == docid);
            requestWise.Isdeleted = 1;

            _context.SaveChanges();
        }

        public List<string> GetListFilename(List<int> rwfid)
        {
            var list = _context.RequestWiseFiles.Where(r => rwfid.Contains(r.Requestwisefileid)).Select(r => r.Filename).ToList();

            return list;
        }

        public List<HealthProfessionalType> GetListProfessionTypes()
        {
            var list = _context.HealthProfessionalTypes;

            return list.ToList();
        }

        public List<HealthProfessional> GetHealthProfessionals(int select)
        {
            var list = _context.HealthProfessionals.Where(u => u.Profession == select);

            return list.ToList();
        }

        public HealthProfessional GetVendorData(int vendorid)
        {
            var data = _context.HealthProfessionals.FirstOrDefault(u => u.Vendorid == vendorid);

            return data;
        }

        public async Task SetOrder(OrdersModal orders, int requestid,int aspid)
        {
            OrderDetail orderDetail = new OrderDetail();
            orderDetail.Prescription = orders.Prescription;
            orderDetail.Createddate = DateTime.Now;
            orderDetail.Noofrefill = orders.NumberOfRefills;
            orderDetail.Requestid = requestid;
            orderDetail.Vendorid = orders.Business;
            orderDetail.Faxnumber = orders.Faxnumber.ToString();
            orderDetail.Email = orders.Email;
            orderDetail.Createdby = aspid.ToString();
            orderDetail.Businesscontact = orders.BusinessContact.ToString();

            _context.OrderDetails.Add(orderDetail);
            _context.SaveChanges();
        }

        public async Task<CloseCaseModal> GetCloseCaseModal(int requestid){
            var requestclient = _context.RequestClients.FirstOrDefault(u => u.Requestid == requestid);
            var request = _context.Requests.FirstOrDefault(u => u.Requestid==requestid);
            var files = _context.RequestWiseFiles.Where(u => u.Requestid == requestid).ToList();

            CloseCaseModal closeCase = new CloseCaseModal
            {
                requestid = requestid,
                confirmationnumber = request.Confirmationnumber,
                PatientDocuments = files,
                Firstname = requestclient.Firstname,
                Lastname = requestclient.Lastname,
                Email = requestclient.Email,
                Phone = requestclient.Phonenumber,
                DateOfBirth = requestclient.DateOfBirth, 
            };

            return closeCase;
        }

        public async Task CloseCase(int reqid,int adminid)
        {
            var req = _context.Requests.FirstOrDefault(u => u.Requestid == reqid);
            
            RequestStatusLog requestStatusLog = new RequestStatusLog();
            requestStatusLog.Status = 9;
            requestStatusLog.Notes = "Request is closed";
            requestStatusLog.Requestid = reqid;
            requestStatusLog.Adminid = adminid;
            requestStatusLog.Createddate = DateTime.Now;

            _context.RequestStatusLogs.Add(requestStatusLog);

            req.Status = 9;

           await _context.SaveChangesAsync();
        }

        public void SetSMSLogs(Smslog smslog)
        {
            _context.Smslogs.Add(smslog);
            _context.SaveChanges();
        }

        public AdminProfileModal GetAdminProfileData(int aspnetuserid)
        {
            var admin = _context.Admins.FirstOrDefault(u => u.Aspnetuserid == aspnetuserid);
            var aspuser = _context.AspNetUsers.FirstOrDefault( u => u.Id == aspnetuserid);

            AdminProfileModal adminProfile = new AdminProfileModal
            {
                username = aspuser.Username,
                status = admin.Status.Value,
                role = _context.Roles.FirstOrDefault(u => u.Roleid == admin.Roleid).Name,
                aspid = aspnetuserid,

                Firstname = admin.Firstname,
                Lastname = admin.Lastname,
                Email = admin.Email,
                Phone = admin.Mobile,

                Address1 = admin.Address1,
                Address2 = admin.Address2,
                City = admin.City,
                Zipcode = admin.Zip,
            };

            return adminProfile;
        }

        public async Task UpdatePassword(int aspid, string password)
        {
            var user = _context.AspNetUsers.FirstOrDefault(u => u.Id == aspid);
            user.Passwordhash = password;
            _context.SaveChanges();
        }

        public async Task UpdateAdmin(AdminProfileModal adminProfile,int aspid)
        {
            var admin = _context.Admins.FirstOrDefault(u => u.Aspnetuserid == aspid);

            admin.Firstname = adminProfile.Firstname;
            admin.Lastname = adminProfile.Lastname;
            admin.Email = adminProfile.Email;
            admin.Mobile = adminProfile.Phone;

            _context.SaveChanges();

        }

        public async Task UpdateAdminAddress(AdminProfileModal adminProfile, int aspid)
        {
            var admin = _context.Admins.FirstOrDefault(u => u.Aspnetuserid == aspid);

            admin.Address1 = adminProfile.Address1;
            admin.Address2 = adminProfile.Address2;
            admin.City = adminProfile.City;
            admin.Zip = adminProfile.Zipcode;

            _context.SaveChanges();

        }

        public Encounter GetEncounter(int requestid)
        {
            var encounter = _context.Encounters.FirstOrDefault(u=> u.Requestid == requestid);
            if(encounter == null)
            {
                encounter = new Encounter();
                var requestclient = _context.RequestClients.FirstOrDefault(u => u.Requestid == requestid);
                encounter.Firstname = requestclient.Firstname;
                encounter.Lastname = requestclient.Lastname;
                encounter.Email = requestclient.Email;
                encounter.Phone = requestclient.Phonenumber;
                //encounter.DateOfBirth = requestclient.DateOfBirth;
                encounter.Requestid = requestid;
            }
           
            return encounter;
        }

        public Encounter SetEncounter(int requestid, Encounter encounter1)
        {
            var encounter = _context.Encounters.FirstOrDefault(u => u.Requestid == requestid);
            if(encounter == null)
            {
                Encounter encounter2 = new Encounter();
                encounter2 = encounter1 as Encounter;
                encounter2.Createdby = "admin";
                encounter2.Createddate = DateTime.Now;
                encounter2.Requestid = requestid;
                
                _context.Encounters.Add(encounter2);
                _context.SaveChanges();

                return encounter2;
            }
            else
            {
                encounter = encounter1 as Encounter;
                encounter.Modifiedby = "admin2";
                encounter.Modifieddate = DateTime.Now;

                _context.Encounters.Add(encounter);
                _context.SaveChanges();
                return encounter;
            }
        }

        public async Task FinalizeEncounter(int requestid, Encounter encounter1)
        {
            var encounter = _context.Encounters.FirstOrDefault(u => u.Requestid == requestid);
            var encounterdata = _context.Encounters.FirstOrDefault(u => u.Requestid == requestid);

            if (encounterdata == null)
            {
                Encounter encounter2 = new Encounter();
                encounter2 = encounter1 as Encounter;
                encounter2.Createdby = "admin";
                encounter2.Createddate = DateTime.Now;
                encounter2.Finalizedby = "admin";
                encounter2.Finalizeddate = DateTime.Now;
                encounter2.Isfinalized = 1;
                encounter2.Requestid = requestid;

                _context.Encounters.Add(encounter2);
                _context.SaveChanges();

            }
            else
            {
                Encounter encounter3 = new Encounter();
                encounter3 = encounter1 as Encounter;
                encounter3.Modifiedby = "admin2";
                encounter3.Modifieddate = DateTime.Now;
                encounter3.Finalizedby = "admin2";
                encounter3.Finalizeddate = DateTime.Now;
                encounter3.Isfinalized = 1;
                encounter3.Createdby = encounterdata.Createdby;
                encounter3.Createddate = encounterdata.Createddate;
                encounter3.Encounterid = encounterdata.Encounterid;
                encounter3.Requestid = encounterdata.Requestid;

                _context.Encounters.Update(encounter3);
                _context.SaveChanges();
            }
        }

        public string Encouter(int requestid,string callType)
        {
            var request = _context.Requests.FirstOrDefault(r => r.Requestid == requestid);
            if(callType == "HouseCall")
            {
                request.Calltype = 1;
            }
            else if(callType == "Consult")
            {
                request.Calltype = 2;
            }
            _context.SaveChanges();
            return "ok";
        }
        public List<AccessTableModal> accessTables()
        {
            var role = _context.Roles;
            var list = role.Select(a => new AccessTableModal
            {
                accessName = a.Name,
                accountType = a.Accounttype,
                roleid = a.Roleid,
            });

            return list.ToList();
        }

        public CreateRoleModal CreateRole(int accounttype)
        {
            CreateRoleModal createRoleModal = new CreateRoleModal();
            if (accounttype == 0 || accounttype == null)
            {
                var menu = _context.Menus;
                createRoleModal.menus = menu.ToList();
            }
            else
            {
                var menu = _context.Menus.Where(u => u.Accounttype == accounttype);
                createRoleModal.menus = menu.ToList();
            }
           

            return createRoleModal;
        }

        public string TransferToAdmin(int requestid, string transferNotes, int physicianid)
        {
            var request = _context.Requests.FirstOrDefault(r => r.Requestid == requestid);

            request.Status = 1;

            RequestStatusLog requestStatus = new RequestStatusLog
            {
                Notes = transferNotes,
                Createddate = DateTime.Now,
                Physicianid = physicianid,
                Requestid = requestid,
                Transtoadmin = new BitArray(1,true),
            };

            _context.RequestStatusLogs.Add(requestStatus);
            _context.SaveChanges();

            return "ok";
        }

        public List<NotificationMessage> GetNotification()
        {
            return _context.NotificationMessages.ToList();
        }
    }
}

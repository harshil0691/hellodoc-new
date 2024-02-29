using hellodoc.DbEntity.DataContext;
using hellodoc.DbEntity.DataModels;
using hellodoc.DbEntity.ViewModels;
using hellodoc.DbEntity.ViewModels.PopUpModal;
using hellodoc.Repositories.Repository.Interface;
using Microsoft.EntityFrameworkCore;
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


        public AdminParent GetRequests(List<int> status)
        { 
            var request = _context.Requests.Where(r => status.Contains(r.Status)); 

            var requestlist = request.Select(r => new AdminDashModel
            {
                Requestid = r.Requestid,
                PatientName = r.RequestClients.Select(rc => rc.Firstname).FirstOrDefault() + " " + r.RequestClients.Select(rc => rc.Lastname).FirstOrDefault(),
                DateOfBirth = r.DateOfBirth,
                Phonenumber_P = r.RequestClients.Select(rc => rc.Phonenumber).FirstOrDefault(),
                Phonenumber_R = r.Phonenumber,
                Status = r.Status,
                Createddate = r.Createddate,
                Address = r.RequestClients.Select(rc => rc.City).FirstOrDefault(),
                RequestorName = r.Firstname + " " + r.Lastname,
                Requesttypeid = r.Requesttypeid,
                Requestclientid = r.RequestClients.Select(rc => rc.Requestclientid).FirstOrDefault(),
                Email = r.Email,
            }

            ) ;

            var region = _context.Regions;

            AdminParent admin = new AdminParent();
            admin.adminDashModels = requestlist.ToList();
            admin.regions = region.ToList();

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

        public async Task<RequestCountByStatus> GetCount()
        {
            RequestCountByStatus requestCount  = new RequestCountByStatus();

            requestCount.NewCount = _context.Requests.Where(u => u.Status == 1).Count();
            requestCount.PendingCount = _context.Requests.Where(u =>u.Status == 2).Count();
            requestCount.ActiveCount = _context.Requests.Where(u =>u.Status == 4 || u.Status == 5).Count();
            requestCount.ConcludeCount = _context.Requests.Where(u => u.Status == 6).Count();
            requestCount.TocloseCount = _context.Requests.Where(u => u.Status == 7  || u.Status == 8 || u.Status == 3).Count();
            requestCount.UnpaidCount = _context.Requests.Where(u => u.Status == 9).Count();

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

        public async Task AssignCase(int? reqid, AssignCaseModal assignCase, int? adminid)
        {
            Request request = _context.Requests.FirstOrDefaultAsync(u => u.Requestid == reqid).Result;

            if (request != null)
            {
                RequestStatusLog requestStatusLog = new RequestStatusLog();
                requestStatusLog.Status = 2;
                requestStatusLog.Notes = assignCase.Discription;
                requestStatusLog.Requestid = request.Requestid;
                requestStatusLog.Adminid = adminid;
                requestStatusLog.Createddate = DateTime.Now;
                requestStatusLog.Transtophysicianid = assignCase.Physicianid;

                _context.RequestStatusLogs.Add(requestStatusLog);

                request.Status = 2;
                request.Physicianid = assignCase.Physicianid;

                _context.SaveChanges();
            }
        }

        public  List<Region> GetRegions()
        {
            var regions = _context.Regions.ToList();
            return  regions;
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
    }
}

using hellodoc.DbEntity.DataContext;
using hellodoc.DbEntity.DataModels;
using hellodoc.DbEntity.ViewModels;
using hellodoc.DbEntity.ViewModels.PopUpModal;
using hellodoc.Repositories.Repository.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static hellodoc.DbEntity.ViewModels.RequestTableModel;

namespace hellodoc.Repositories.Repository
{
    public class PatientDashboard : IPatientDashboard
    {
        private readonly ApplicationDbContext _context;

        public PatientDashboard(ApplicationDbContext context)
        {
            _context = context;
        }

        public PatientReqModel GetRequestList(int? uid)
        {
            var userid = uid;

            var request = _context.Requests.Where(u => u.User.Aspnetuserid == userid);


            var requestlist = request.Select(r => new RequestTableModel
            {
                Createddate = r.Createddate,
                Status = r.Status,
                Requestid = r.Requestid,
                Documents = r.RequestWiseFiles.Select(f => f.Filename).Count().ToString(),
            });

            PatientReqModel patientReq = new PatientReqModel();
            patientReq.requestTable = requestlist.ToList();

            var user = _context.Users.FirstOrDefault(u=> u.Aspnetuserid == userid);

            patientReq.Firstname = user.Firstname;
            patientReq.Lastname = user.Lastname;
            patientReq.Email = user.Email;
            patientReq.Phonenumber = user.Mobile??0;
            patientReq.Street = user.Street;
            patientReq.State = user.State;
            patientReq.Zipcode = user.Zipcode??0;
            patientReq.City = user.City;

            return patientReq;
        }

        public async Task AgreeAgreement(int reqid, string ip)
        {
            RequestStatusLog requestStatus = new RequestStatusLog
            {
                Requestid = reqid,
                Status =4,
                Createddate = DateTime.Now,
                Notes = "Patient Accept a Agreement",
                Ip = ip
            };
            _context.RequestStatusLogs.Add(requestStatus);

            var req =  _context.Requests.FirstOrDefault(u => u.Requestid == reqid);
            req.Status = 4;
            _context.SaveChanges();
        }

        public async Task CancelAgreement(SendAgreementModal sendAgreement, string ip)
        {
            RequestStatusLog requestStatus = new RequestStatusLog
            {
                Requestid = sendAgreement.reqid,
                Status = 4,
                Createddate = DateTime.Now,
                Notes = "Patient cancel Agreement notes : "+sendAgreement.CancelNotes,
                Ip = ip
            };
            _context.RequestStatusLogs.Add(requestStatus);

            var req = _context.Requests.FirstOrDefault(u => u.Requestid == sendAgreement.reqid);
            req.Status = 8;
            _context.SaveChanges();
        }

        public async Task<int> GetRequestStatus(int reqid)
        {
            var req = _context.Requests.FirstOrDefault(u => u.Requestid==reqid);

            return req.Status;
        }

    }
}

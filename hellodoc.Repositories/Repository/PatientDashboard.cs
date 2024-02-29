using hellodoc.DbEntity.DataContext;
using hellodoc.DbEntity.DataModels;
using hellodoc.DbEntity.ViewModels;
using hellodoc.Repositories.Repository.Interface;
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

            var request = _context.Requests.Where(u => u.Userid == userid);


            var requestlist = request.Select(r => new RequestTableModel
            {
                Createddate = r.Createddate,
                Status = r.Status,
                Requestid = r.Requestid,
                Documents = r.RequestWiseFiles.Select(f => f.Filename).Count().ToString(),
            });

            PatientReqModel patientReq = new PatientReqModel();
            patientReq.requestTable = requestlist.ToList();

            var user = _context.Users.FirstOrDefault(u=> u.Userid == userid);
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


    }
}

using hellodoc.DbEntity.DataContext;
using hellodoc.DbEntity.DataModels;
using hellodoc.DbEntity.ViewModels;
using hellodoc.DbEntity.ViewModels.AdminRecords;
using hellodoc.Repositories.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hellodoc.Repositories.Repository
{
    public class AdminRecords : IAdminRecords
    {
        private readonly ApplicationDbContext _context;

        public AdminRecords(ApplicationDbContext context)
        {
            _context = context;
        }

        public AdminRecordsListModal BlokedHistory(AdminRecordsListModal adminRecords)
        { 
            DateTime date = DateTime.Parse("1001-01-01");
            var pageSize = 2;
            var pageNumber = 1;
            if (adminRecords.pageNumber > 0)
            {
                pageNumber = adminRecords.pageNumber;
            }

            var blocked = _context.BlockRequests.Where(
                br =>
                (((adminRecords.PatientName != null) ? br.Request.Firstname.Contains(adminRecords.PatientName) : true) ||
                ((adminRecords.PatientName != null) ? br.Request.Lastname.Contains(adminRecords.PatientName) : true) )&&
                ((adminRecords.BlokedDate > date) ? br.Createddate == adminRecords.BlokedDate : true) && 
                ((adminRecords.Email != null)? br.Email.Contains(adminRecords.Email) :true) && 
                ((adminRecords.PhoneNumber !=0 ) ? br.Phonenumber == adminRecords.PhoneNumber :true)
            );

            var list = blocked.Select(b => new BlokedHistory
            {
                BlokedId = b.Blockrequestid,
                Requestid = b.Requestid,
                PatientName = b.Request.Firstname + ' ' + b.Request.Lastname,
                PhoneNumber = b.Request.Phonenumber??0,
                Email = b.Email,
            });

            adminRecords.blokedHistory = list.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            adminRecords.pageNumber = pageNumber;
            adminRecords.pageSize = pageSize;
            adminRecords.totalEntries = list.Count();
            if (list.Skip((pageNumber) * pageSize).Take(pageSize).Count() > 0)
            {
                adminRecords.morePages = true;
            }
            adminRecords.entries = ((pageNumber - 1) * pageSize + 1) + "-" + (((pageNumber - 1) * pageSize) + adminRecords.blokedHistory.Count());

            return adminRecords;
        }

        public AdminRecordsListModal EmailLogs(AdminRecordsListModal adminRecords)
        {
            var emails = _context.EmailLogs.Where(
                e => 
                ((adminRecords.SearchByRole != 0) ? e.Roleid == adminRecords.SearchByRole : true) &&
                ((adminRecords.Email != null) ? e.Emailid.Contains(adminRecords.Email): true) &&
                ((adminRecords.CreatedDate > new DateTime(1,1,1))? e.Createdate.Date == adminRecords.CreatedDate.Date : true)
            );
            var pageSize = 2;
            var pageNumber = 1;
            if (adminRecords.pageNumber > 0)
            {
                pageNumber = adminRecords.pageNumber;
            }
            EmailLogs emailLogs = new EmailLogs();
            foreach(var email in emails)
            {

            }
            var list = emails.Select(e => new EmailLogs
            {
                EmailId = e.Emailid,
                Sent = (e.Isemailsent==1)? "yes":"no",
                EmaillogsId = e.Emaillogid,
                RoleName = e.Role.Name,
                ConfirmationNumber = e.Confirmationnumber,
                CreateDate = e.Createdate.ToString("MMM") + e.Createdate.ToString("dd") + ", " + e.Createdate.ToString("yyyy"),
                Recipient = (e.Requestid != null) ? e.Request.Firstname + "" + e.Request.Lastname: "",
            });

            adminRecords.emailLogs = list.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            adminRecords.pageNumber = pageNumber;
            adminRecords.pageSize = pageSize;
            adminRecords.totalEntries = list.Count();
            if (list.Skip((pageNumber) * pageSize).Take(pageSize).Count() > 0)
            {
                adminRecords.morePages = true;
            }
            adminRecords.entries = ((pageNumber - 1) * pageSize + 1) + "-" + (((pageNumber - 1) * pageSize) + adminRecords.emailLogs.Count());

            return adminRecords;
        }

        public AdminRecordsListModal PatientRecords(AdminRecordsListModal adminRecords)
        {
            var request = _context.RequestClients.Where(r => r.Request.Userid == adminRecords.UserId);
            var pageSize = 2;
            var pageNumber = adminRecords.pageNumber;
            if (pageNumber <= 0)
            {
                pageNumber = 1;
            }
           
            var list = request.Select(r => new PatientRecords
            {
                PatientName = r.Firstname + " " + r.Lastname,
                CreatedDate = r.Request.Createddate.ToString("MMM") + r.Request.Createddate.ToString("dd") + ", " + r.Request.Createddate.ToString("yyyy"),
                ProviderName = r.Request.Physician.Firstname + " " + r.Request.Lastname,
                Status = r.Request.Status.ToString(),
                RequestId = r.Requestid??1,
            });

            adminRecords.patientRecords = list.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            adminRecords.pageNumber = pageNumber;
            adminRecords.pageSize = pageSize;
            adminRecords.totalEntries = list.Count();
            if (list.Skip((pageNumber) * pageSize).Take(pageSize).Count() > 0)
            {
                adminRecords.morePages = true;
            }
            adminRecords.entries = ((pageNumber - 1) * pageSize + 1) + "-" + (((pageNumber - 1) * pageSize) + adminRecords.patientRecords.Count());

            return adminRecords;
        }

        public AdminRecordsListModal PatientHistory(AdminRecordsListModal adminRecords)
        {
            var request = _context.Users.Where(
                r =>
                ((adminRecords.FirstName != null) ? r.Firstname.Contains(adminRecords.FirstName) : true) &&
                ((adminRecords.LastName != null) ? r.Lastname.Contains(adminRecords.LastName) : true) &&
                ((adminRecords.Email != null) ? r.Email.Contains(adminRecords.Email) : true) &&
                ((adminRecords.PhoneNumber != 0) ? r.Mobile.ToString().Contains(adminRecords.PhoneNumber.ToString()) : true)
                );
            var pageSize = 10;
            var pageNumber = 1;
            if (adminRecords.pageNumber > 0)
            {
                pageNumber = adminRecords.pageNumber;
            }
            
            var list = request.Select(r => new PatientHistory
            {
                Firstname = r.Firstname,
                Lastname = r.Lastname,
                Email = r.Email,
                PhoneNumber = r.Mobile??0,
                UserId = r.Userid,
                
            });

            adminRecords.patientHistories = list.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            adminRecords.pageNumber = pageNumber;
            adminRecords.pageSize = pageSize;
            adminRecords.totalEntries = list.Count();
            if (list.Skip((pageNumber) * pageSize).Take(pageSize).Count() > 0)
            {
                adminRecords.morePages = true;
            }
            adminRecords.entries = ((pageNumber - 1) * pageSize + 1) + "-" + (((pageNumber - 1) * pageSize) + adminRecords.patientHistories.Count());

            return adminRecords;
        }

        public AdminRecordsListModal SearchRecords(AdminRecordsListModal adminRecords, bool export)
        {
            DateTime date = DateTime.Parse("0001-01-01");
            var request = _context.RequestClients.Where(
                r => 
                ((adminRecords.RequestStatus != 0) ? r.Request.Status == adminRecords.RequestStatus-1 : true ) &&
                (((adminRecords.PatientName != null) ? r.Firstname.Contains(adminRecords.PatientName) : true )||
                ((adminRecords.PatientName != null) ? r.Lastname.Contains(adminRecords.PatientName) : true ) )&&
                ((adminRecords.RequestType != 0) ? r.Request.Requesttypeid == adminRecords.RequestType: true )&&
                ((adminRecords.ProviderName != null) ? r.Request.Physician.Firstname.Contains(adminRecords.ProviderName) : true )&&
                ((adminRecords.Email != null) ? r.Email.Contains(adminRecords.Email): true )&&
                ((adminRecords.PhoneNumber !=0 )?r.Phonenumber.ToString().Contains(adminRecords.PhoneNumber.ToString()): true) &&
                ((adminRecords.FromDateOfService > date)? r.Request.Createddate>= adminRecords.CreatedDate :true)
                );
            var pageSize = 10;
            var pageNumber = 1;
            if(adminRecords.pageNumber > 0)
            {
                pageNumber = adminRecords.pageNumber;
            }
            List<string> statusname = new List<string>{ "Unassigned", "Accepted", "MDEnRoute", "MDOnSite", "Conclude", "Cancelled", "CancelledByPatient", "Closed", "Unpaid", "Clear" };

            var list = request.Select(r => new SearchRecords
            {
                RequestId = r.Requestid ?? 0,
                Patientname = r.Firstname + " " + r.Lastname,
                RequestClientId = r.Requestclientid,
                Requestor = r.Request.Firstname + " " + r.Request.Lastname,
                Email = r.Email,
                PhoneNumber = r.Phonenumber,
                Address = r.Address,
                Zip = r.Zipcode,
                RequestStatus = statusname[r.Request.Status],
                Physician = "Dr." + r.Request.Physician.Firstname + " " + r.Request.Physician.Lastname,
                PhysicianNote = _context.RequestNotes.FirstOrDefault(rn => rn.Requestid == r.Requestid).Physiciannotes,
                AdminNote = _context.RequestNotes.FirstOrDefault(rn => rn.Requestid == r.Requestid).Adminnotes,
                PatientNote = r.Notes,
                DateOfService = r.Request.Createddate.ToString("MMM") + r.Request.Createddate.ToString("dd") + ", " + r.Request.Createddate.ToString("yyyy"),
            }) ;

            if (export == true)
            {
                adminRecords.searchRecords = list.ToList();
                return adminRecords ;
            }

            adminRecords.searchRecords = list.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            adminRecords.pageNumber = pageNumber;
            adminRecords.pageSize = pageSize;
            adminRecords.totalEntries = list.Count();
            if (list.Skip((pageNumber) * pageSize).Take(pageSize).Count() > 0)
            {
                adminRecords.morePages = true;
            }
            adminRecords.entries = ((pageNumber - 1) * pageSize + 1) + "-" + (((pageNumber - 1) * pageSize) + adminRecords.searchRecords.Count());

            return adminRecords;
        }

        public AdminRecordsListModal SMSLogs(AdminRecordsListModal adminRecords)
        {
            var sms = _context.Smslogs.Where(
                e =>
                ((adminRecords.SearchByRole != 0) ? e.Roleid == adminRecords.SearchByRole : true) &&
                (e.Mobilenumber.ToString().Contains(adminRecords.PhoneNumber.ToString())) && 
                ((adminRecords.ReceiverName != null)? e.Request.Firstname.Contains(adminRecords.ReceiverName):true)
            );
            var pageSize = 2;
            var pageNumber = 1;
            if (adminRecords.pageNumber > 0)
            {
                pageNumber = adminRecords.pageNumber;
            }
            var list = sms.Select(e => new SMSLogs
            {
                SMSLogsId = e.Smslogid,
                Sent = (e.Issmssent == 1)?"yes":"no",
                MobileNumber = e.Mobilenumber,
                RoleName = e.Role.Name,
                ConfirmationNumber = e.Confirmationnumber ?? "",
                Recipient = (e.Requestid != null) ? e.Request.Firstname + "" + e.Request.Lastname : "",
                CreatedDate = e.Createdate.ToString("MMM") + e.Createdate.ToString("dd") + ", " + e.Createdate.ToString("yyyy"),
            });

            adminRecords.smsLogs = list.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            adminRecords.pageNumber = pageNumber;
            adminRecords.pageSize = pageSize;
            adminRecords.totalEntries = list.Count();
            if (list.Skip((pageNumber) * pageSize).Take(pageSize).Count() > 0)
            {
                adminRecords.morePages = true;
            }
            adminRecords.entries = ((pageNumber - 1) * pageSize + 1) + "-" + (((pageNumber - 1) * pageSize) + adminRecords.smsLogs.Count());

            return adminRecords;
        }

        public void DeletePermenantly(int requstid)
        {
            var request = _context.Requests.FirstOrDefault(r => r.Requestid == requstid);
            _context.Requests.Remove(request);
            _context.SaveChanges();
        }

        public void UnBlock(int requestid)
        {
            var request = _context.BlockRequests.FirstOrDefault(r => r.Requestid == requestid);
            if(request != null)
            {
                _context.BlockRequests.Remove(request);
                _context.SaveChanges();
            }
           
        }
    }
}

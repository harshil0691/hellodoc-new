using hellodoc.DbEntity.DataContext;
using hellodoc.DbEntity.DataModels;
using hellodoc.DbEntity.ViewModels;
using hellodoc.Repositories.Repository.Interface;
using Microsoft.AspNetCore.Http.HttpResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace hellodoc.Repositories.Repository
{
    public class ProviderRepo : IProviderRepo
    {
        private readonly ApplicationDbContext _context;

        public ProviderRepo(ApplicationDbContext context)
        {
            _context = context;
        }
        public string AcceptRequest(int requestid)
        {
            var request = _context.Requests.FirstOrDefault(r => r.Requestid == requestid);
            if (request != null)
            {
                request.Status = 2;
                _context.SaveChanges();
                return "ok";
            }
            return "not accept";
        }

        public string RequestToAdmin(PartialViewModal partialView)
        {
            var physician = _context.Physicians.FirstOrDefault(p => p.Physicianid == partialView.physicianid);
            var admin = _context.Admins.ToList();

            var subject = "Request To Edit Profile From Physician";
            var message = "Physician :"+ physician?.Firstname + physician?.Lastname + " want To edit There profile " + partialView.physicianNotes + 
                             "Email Of Physician" + physician?.Email;

            foreach (var a in admin)
            {
                SendMail(subject,message,a.Email,a.Adminid);
            }

            return "ok";
        }

        public void SendMail(string subject, string message, string mailto, int adminid)
        {
            var mail = "tatva.dotnet.harshildhaduk@outlook.com";
            var password = "harshil@9184";

            var client = new SmtpClient("smtp.office365.com", 587)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(mail, password)
            };

            MailMessage mailMessage = new MailMessage(from: mail, to: mailto, subject, message);

            client.SendMailAsync(mailMessage);

            EmailLog emailLog = new EmailLog()
            {
                Adminid = adminid,
                Subjectname = subject,
                Createdate = DateTime.Now,
                Sentdate = DateTime.Now,
                Isemailsent = 1,
                Emailtemplate = "Create User"
            };

            _context.EmailLogs.Add(emailLog);
            _context.SaveChanges();
        }

        public void SendSMS(string message, long? phone, int requestid)
        {
            const string accountSid = "ACbab82685c56693b60c76b5f7e372f1fc";
            const string authToken = "32ff33fe5e79b7524965ba1e39cfd249";
            const string twilioPhoneNumber = "+15162999172";

            TwilioClient.Init(accountSid, authToken);

            var message1 = MessageResource.Create(
                body: message,
                from: new Twilio.Types.PhoneNumber(twilioPhoneNumber),
                to: new Twilio.Types.PhoneNumber("+917984752378")
            );

            Smslog smslog = new Smslog
            {
                Requestid = requestid,
                Createdate = DateTime.Now,
                Sentdate = DateTime.Now,
                Smstemplate = "Create User",
                Issmssent = 1
            };
            _context.Smslogs.Add(smslog);
            _context.SaveChanges();
        }
    }
}

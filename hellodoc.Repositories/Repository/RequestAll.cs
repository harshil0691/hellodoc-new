using hellodoc.DbEntity.DataContext;
using hellodoc.DbEntity.DataModels;
using hellodoc.DbEntity.ViewModels;
using hellodoc.DbEntity.ViewModels.DocumentModal;
using hellodoc.DbEntity.ViewModels.PopUpModal;
using hellodoc.Repositories.Repository.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace hellodoc.Repositories.Repository;

public class RequestAll : IRequests
{
    private readonly ApplicationDbContext _context;

    public RequestAll(ApplicationDbContext context)
    {
        _context = context;
    }

    public string PatientRequest(RequestFormModal requestForm)
    {
        var userexist = _context.Users.FirstOrDefault(u => u.Email == requestForm.Email);
        var checkBlokedUser = _context.BlockRequests.Where(u => u.Email == requestForm.Email);

        if (checkBlokedUser != null)
        {
            return "Patient Are Bloked To Create A Requeat";
        }
        User user1 = new User();

        if (userexist == null)
        {
            AspNetUser aspNetUser = new AspNetUser()
            {
                Username = requestForm.Firstname + " " + requestForm.Lastname,
                Email = requestForm.Email,
                Phonenumber = requestForm.Phonenumber,
                Passwordhash = requestForm.Password,
                Createddate = DateTime.Now,
            };

            User user = new User()
            {
                Firstname = requestForm.Firstname,
                Lastname = requestForm.Lastname,
                Email = requestForm.Email,
                Mobile = requestForm.Phonenumber,
                Street = requestForm.Street,
                City = requestForm.City,
                State = requestForm.State,
                Zipcode = requestForm.Zipcode,
                Createdby = requestForm.Firstname + " " + requestForm.Lastname,
                Createddate = DateTime.Now,

                Aspnetuser = aspNetUser,
            };

            _context.AspNetUsers.Add(aspNetUser);
            _context.Users.Add(user);
            user1 = user;
        }
        else
        {
            user1 = userexist;
        }

        Request request = new Request()
        {
            Status = 1,
            Firstname = requestForm.Firstname,
            Lastname = requestForm.Lastname,
            Email = requestForm.Email,
            Phonenumber = requestForm.Phonenumber,
            Createddate = DateTime.Now,

            User = user1
        };

        RequestClient requestClient = new RequestClient()
        {
            Firstname = requestForm.Firstname,
            Lastname = requestForm.Lastname,
            Email = requestForm.Email,
            Phonenumber = requestForm.Phonenumber,
            Street = requestForm.Street,
            City = requestForm.City,
            State = requestForm.State,
            Zipcode = requestForm.Zipcode,
            Notes = requestForm.Symptoms,
        };

        _context.Requests.Add(request);
        _context.RequestClients.Add(requestClient);
        _context.SaveChanges();

        return "Your Request Are Successfully Ceated";

    }


    public string FriendRequest(RequestFormModal requestForm)
    {
        var link = "https://localhost:7036/Admin_agreement/agreement?requestid=";
        var subject = "Agreement for patiet request";
        var message = "\n \n \n please perform a action on the agreement which are given in link \n \n \n" + link;

        SendMail(subject, message, "");

        SendSMS(message, 0);

        return "ok";
    }
    public string ConciergeRequest(RequestFormModal requestForm)
    {
        return "ok";
    }
    public string BusinessRequest(RequestFormModal requestForm)
    {
        return "ok";
    }

    public void SendMail(string subject, string message, string mailto)
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
    }

    public void SendSMS(string message, long? phone)
    {
        const string accountSid = "ACbab82685c56693b60c76b5f7e372f1fc";
        const string authToken = "192147860503a6d615f8e333a5fbc049";
        const string twilioPhoneNumber = "+15162999172";

        TwilioClient.Init(accountSid, authToken);

        // Send an SMS
        var message1 = MessageResource.Create(
            body: message,
            from: new Twilio.Types.PhoneNumber(twilioPhoneNumber),
            to: new Twilio.Types.PhoneNumber("+917984752378")
        );
    }

    public async Task<AspNetUser> SetAspNetUser(PatientReqModel patientReq)
    {
        AspNetUser aspNetUser1 = new AspNetUser
        {
            Username = patientReq.Firstname + " " + patientReq.Lastname,
            Email = patientReq.Email,
            Phonenumber = patientReq.Phonenumber,
            Createddate = DateTime.Now,
            Passwordhash = patientReq.Password,
        };

        _context.AspNetUsers.Add(aspNetUser1);
        _context.SaveChanges();

        return  aspNetUser1;
    }

    public async Task<User> SetUser(PatientReqModel patientReq,int aspid)
    {
        User user = new User
        {
            Firstname = patientReq.Firstname,
            Lastname = patientReq.Lastname,
            Email = patientReq.Email,
            Mobile = patientReq.Phonenumber,
            Street = patientReq.Street,
            City = patientReq.City,
            State = patientReq.State,
            Zipcode = patientReq.Zipcode,
            Createdby = patientReq.Firstname + " " + patientReq.Lastname,
            Createddate = DateTime.Now,
            Aspnetuserid = aspid,

        };

        _context.Users.Add(user);
        _context.SaveChanges();

        return user;
    }

    public async Task<Request> SetRequest(PatientReqModel patientReq, int uid)
    {
        Request request = new Request
        {
            Status = 2,
            Firstname = patientReq.Firstname,
            Lastname = patientReq.Lastname,
            Email = patientReq.Email,
            Phonenumber = patientReq.Phonenumber,
            Createddate = DateTime.Now,
            Userid = uid,
        };

        _context.Requests.Add(request);
        _context.SaveChanges();

        return request;
    }
    public async Task<Int32> SetRequest(FriendReqModel friendReq)
    {
        Request request = new Request
        {
            Status = 2,
            Firstname = friendReq.Firstname,
            Lastname = friendReq.Lastname,
            Email = friendReq.Email,
            Phonenumber = friendReq.Phonenumber,
            Createddate = DateTime.Now,
        };

        _context.Requests.Add(request);
        _context.SaveChanges();


        return request.Requestid;
    }

    public async Task<Int32> SetRequest(ConciergeReqModel conciergeReq)
    {
        Request request = new Request
        {
            Requesttypeid = 4,
            Firstname = conciergeReq.C_Firstname,
            Lastname = conciergeReq.C_Lastname,
            Email = conciergeReq.C_Email,
            Phonenumber = conciergeReq.C_Phonenumber,
            Createddate = DateTime.Now,

        };

        _context.Requests.Add(request);
        _context.SaveChanges();


        return request.Requestid;
    }
    public async Task<Int32> SetRequest(BusinessReqModel businessReq)
    {
        Request request = new Request
        {
            Requesttypeid = 1,
            Firstname = businessReq.B_Firstname,
            Lastname = businessReq.B_Lastname,
            Email = businessReq.B_Email,
            Phonenumber = businessReq.B_Phonenumber,
            Createddate = DateTime.Now,
            Casenumber = businessReq.B_CaseNo

        };

        _context.Requests.Add(request);
        _context.SaveChanges();


        return request.Requestid;
    }

    public async Task<RequestClient> SetRequestClient(PatientReqModel patientReq, int rid)
    {
        RequestClient requestClient = new RequestClient
        {
            Firstname = patientReq.Firstname,
            Lastname = patientReq.Lastname,
            Email = patientReq.Email,
            Phonenumber = patientReq.Phonenumber,
            Street = patientReq.Street,
            City = patientReq.City,
            State = patientReq.State,
            Zipcode = patientReq.Zipcode,
            Notes = patientReq.Symptoms,

            Requestid = rid
        };

        _context.RequestClients.Add(requestClient);
        _context.SaveChanges();

        return requestClient;
    }

    public async Task SetRequestClient(FriendReqModel friendReq, int rid)
    {
        RequestClient requestClient = new RequestClient
        {
            Firstname = friendReq.Firstname,
            Lastname = friendReq.Lastname,
            Email = friendReq.Email,
            Phonenumber = friendReq.Phonenumber,
            Street = friendReq.Street,
            City = friendReq.City,
            State = friendReq.State,
            Zipcode = friendReq.Zipcode,
            Notes = friendReq.Symptoms,

            Requestid = rid
        };

        _context.RequestClients.Add(requestClient);
        _context.SaveChanges();

    }
    public async Task SetRequestClient(ConciergeReqModel conciergeReq, int rid)
    {
        RequestClient requestClient = new RequestClient
        {
            Firstname = conciergeReq.Firstname,
            Lastname = conciergeReq.Lastname,
            Email = conciergeReq.Email,
            Phonenumber = conciergeReq.Phonenumber,
            Notes = conciergeReq.Symptoms,
            Location = conciergeReq.Roomno,

            Requestid = rid
        };

        _context.RequestClients.Add(requestClient);
        _context.SaveChanges();

    }
    public async Task SetRequestClient(BusinessReqModel businessReq, int rid)
    {
        RequestClient requestClient = new RequestClient
        {
            Firstname = businessReq.Firstname,
            Lastname = businessReq.Lastname,
            Email = businessReq.Email,
            Phonenumber = businessReq.Phonenumber,
            Notes = businessReq.Symptoms,
            Location = businessReq.Roomno,
            City = businessReq.City,
            State = businessReq.State,
            Street = businessReq.Street,
            Zipcode = businessReq.Zipcode,

            Requestid = rid
        };

        _context.RequestClients.Add(requestClient);
        _context.SaveChanges();

    }

    public async Task SetRequestWiseFile(string uniqueFilename, int rid)
    {
        RequestWiseFile requestWiseFile = new RequestWiseFile
        {

            Createddate = DateTime.Now,
            Filename = uniqueFilename,
            Requestid = rid,
            Doctype = Path.GetExtension(uniqueFilename),
        };
        
        _context.RequestWiseFiles.Add(requestWiseFile);
        _context.SaveChanges();

    }

    public async Task SetConcierge(ConciergeReqModel conciergeReq)
    {
        Concierge requestConcierge = new Concierge
        {
            Conciergename = conciergeReq.C_PropertyName,
            City = conciergeReq.C_City,
            State = conciergeReq.C_State,
            Street = conciergeReq.C_Street,
            Zipcode = conciergeReq.C_Zipcode
        };

        _context.Concierges.Add(requestConcierge);
        _context.SaveChanges();

    }

    public async Task Setbusiness(BusinessReqModel businessReq)
    {
        Business business = new Business
        {
            Name = businessReq.B_BusinessName,
            City = businessReq.City,
            Phonenumber = businessReq.B_Phonenumber,
            Createddate = DateTime.Now,
        };
        _context.Businesses.Add(business);
        _context.SaveChanges();

    }

    public async Task<Int32> GetAspUser(string email)
    {
        var aspNetUser = _context.AspNetUsers.FirstOrDefault(u => u.Email == email);

        if (aspNetUser == null)
        {
            return 0;
        }

        return aspNetUser.Id;
    }

    public async Task<Int32> GetUser(int aspid)
    {
        var aspNetUser = _context.Users.FirstOrDefaultAsync(u => u.Aspnetuserid == aspid);

        return aspNetUser.Result.Userid;
    }

    public async Task UpdateUser(PatientReqModel patientReq,int userid)
    {

        var user1 = _context.Users.FirstOrDefault(u => u.Userid == userid);

        user1.Firstname = patientReq.Firstname;
        user1.Lastname = patientReq.Lastname;
        user1.Email = patientReq.Email;
        user1.Mobile = patientReq.Phonenumber;
        user1.Modifieddate = DateTime.Now;
        user1.State = patientReq.State;
        user1.Street = patientReq.Street;
        user1.City = patientReq.City;
        user1.Zipcode = patientReq.Zipcode;

        var aspnetuser = _context.AspNetUsers.FirstOrDefault(u => u.Id == user1.Aspnetuserid);

        aspnetuser.Phonenumber = patientReq.Phonenumber;
        aspnetuser.Email = patientReq.Email;
        aspnetuser.Username = patientReq.Firstname + " " + patientReq.Lastname;

        _context.SaveChanges();

    }

    public PatientReqModel GetDocuments(int rid,int uid)
    {
        var doc = _context.RequestWiseFiles.Where(u => u.Requestid == rid && u.Isdeleted != 1);

        var showdoc = doc.Select(r => new ShowDocModal
        {
            Requestid = r.Requestid,
            Requestwisefileid = r.Requestwisefileid,
            Filename = r.Filename.Substring(37),
            Createddate = r.Createddate.ToString("MMM dd,yyyy"),
            Doctype = r.Doctype,
            Name = r.Request.Firstname + " " + r.Request.Lastname,
        });

        PatientReqModel patient = new PatientReqModel {
            patientDocuments = showdoc.ToList(),
            Requestid = rid,
        };

        return  patient;
    }

    public async Task<string> GetFilename(int reqcliid)
    {
        var file = _context.RequestWiseFiles.FirstOrDefaultAsync(u => u.Requestwisefileid == reqcliid);

        return file.Result.Filename;
    }

    public async Task UpdateCloseCase(int requestid, CloseCaseModal closeCase)
    {
        var reqclient = _context.RequestClients.FirstOrDefault(u => u.Requestid == requestid);

        reqclient.Firstname = closeCase.Firstname;
        reqclient.Lastname = closeCase.Lastname;
        reqclient.Phonenumber = closeCase.Phone;
        reqclient.DateOfBirth = closeCase.DateOfBirth;
        reqclient.Email = closeCase.Email;
        
        _context.SaveChanges();
    }
}


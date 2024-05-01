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
using Microsoft.AspNetCore.Hosting;
using NUnit.Framework.Constraints;
using System.Globalization;


namespace hellodoc.Repositories.Repository;

public class RequestAll : IRequests
{
    private readonly ApplicationDbContext _context;
    private readonly IHostingEnvironment HostingEnviroment;

    public RequestAll(ApplicationDbContext context, IHostingEnvironment hostingEnvironment)
    {
        _context = context;
        HostingEnviroment = hostingEnvironment;
    }

    public string PatientRequest(RequestFormModal requestForm)
    {
        var userexist = _context.Users.FirstOrDefault(u => u.Email == requestForm.PatientEmail);
        var checkBlokedUser = _context.BlockRequests.Where(u => u.Email == requestForm.PatientEmail).Count();

        if (checkBlokedUser > 0)
        {
            return "blocked";
        }
        User user1 = new User();

        if (userexist == null)
        {
            AspNetUser aspNetUser = new AspNetUser()
            {
                Username = requestForm.Firstname + " " + requestForm.Lastname,
                Email = requestForm.PatientEmail,
                Phonenumber = requestForm.Phonenumber,
                Passwordhash = requestForm.Password,
                Createddate = DateTime.Now,
            };

            User user = new User()
            {
                Firstname = requestForm.Firstname,
                Lastname = requestForm.Lastname,
                Email = requestForm.PatientEmail,
                Mobile = requestForm.Phonenumber,
                Street = requestForm.Street,
                City = requestForm.City,
                State = requestForm.State.ToString(),
                Zipcode = requestForm.Zipcode,
                Strmonth = requestForm.DOB.ToString("MMMM"),
                Intdate = requestForm.DOB.Day,
                Intyear = requestForm.DOB.Year,
                Createdby = requestForm.Firstname + " " + requestForm.Lastname,
                Createddate = DateTime.Now,

                Aspnetuser = aspNetUser,
            };

            _context.AspNetUsers.Add(aspNetUser);
            _context.Users.Add(user);
            _context.SaveChanges();

            AspNetUserRole aspNetUserRole = new AspNetUserRole()
            {
                Userid = aspNetUser.Id,
                Roleid = 2
            };
            _context.AspNetUserRoles.Add(aspNetUserRole);
            _context.SaveChanges();
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
            Email = requestForm.PatientEmail,
            Phonenumber = requestForm.Phonenumber,
            Createddate = DateTime.Now,
            Requesttypeid = 2,
            Physicianid = (requestForm.RequestCreatedBy == "provider")? requestForm.PhysicianId:null,
            Confirmationnumber = _context.Regions.FirstOrDefault(r => r.Regionid== requestForm.State).Name.Substring(0, 2) + DateTime.Now.ToString().Substring(0, 4)
                                      + requestForm.Lastname.Substring(0, 2) + requestForm.Firstname.Substring(0, 2) + _context.Requests.Where(r => r.Createddate == DateTime.Now).Count().ToString().ToUpper(),

            User = user1
        };

        RequestClient requestClient = new RequestClient()
        {
            Firstname = requestForm.Firstname,
            Lastname = requestForm.Lastname,
            Email = requestForm.PatientEmail,
            Phonenumber = requestForm.Phonenumber,
            Street = requestForm.Street,
            City = requestForm.City,
            State = requestForm.State.ToString(),
            Zipcode = requestForm.Zipcode,
            Notes = (requestForm.RequestCreatedBy == "admin")?requestForm.AdminNotes :requestForm.Symptoms,
            Strmonth = requestForm.DOB.ToString("MMMM"),
            Intdate = requestForm.DOB.Day,
            Intyear = requestForm.DOB.Year,
            Request = request,
            Regionid = requestForm.State,
        };

        _context.Requests.Add(request);
        _context.RequestClients.Add(requestClient);
        _context.SaveChanges();

        if (requestForm.Doc != null)
        {
            SaveFile(requestForm.Doc, request.Requestid);
        }

        return "ok";

    }


    public string FriendRequest(RequestFormModal requestForm)
    {
        var userexist = _context.Users.FirstOrDefault(u => u.Email == requestForm.PatientEmail);
        var checkBlokedUser = _context.BlockRequests.Where(u => u.Email == requestForm.PatientEmail).Count();

        if (checkBlokedUser > 0)
        {
            return "blocked";
        }

        Request request = new Request()
        {
            Status = 1,
            Firstname = requestForm.F_Firstname,
            Lastname = requestForm.F_Lastname,
            Email = requestForm.F_Email,
            Phonenumber = requestForm.F_Phonenumber,
            Relationname = requestForm.F_RelationType,
            Requesttypeid =3,
            Createddate = DateTime.Now,
            Confirmationnumber = _context.Regions.FirstOrDefault(r => r.Regionid == requestForm.State).Name.Substring(0, 2) + DateTime.Now.ToString().Substring(0, 4)
                                      + requestForm.Lastname.Substring(0, 2) + requestForm.Firstname.Substring(0, 2) + _context.Requests.Where(r => r.Createddate == DateTime.Now).Count().ToString().ToUpper(),
        };
        if (userexist != null)
        {
            request.User = userexist;
        }
        RequestClient requestClient = new RequestClient()
        {
            Firstname = requestForm.Firstname,
            Lastname = requestForm.Lastname,
            Email = requestForm.PatientEmail,
            Phonenumber = requestForm.Phonenumber,
            Street = requestForm.Street,
            City = requestForm.City,
            State = requestForm.State.ToString(),
            Zipcode = requestForm.Zipcode,
            Notes = requestForm.Symptoms,
            Regionid = requestForm.State,

            Request = request,
        };

        _context.Requests.Add(request);
        _context.RequestClients.Add(requestClient);
        _context.SaveChanges();

        if (requestForm.Doc != null)
        {
            SaveFile(requestForm.Doc, request.Requestid);
        }

        if (userexist == null)
        {
            SendMail("Do Your Register to Hellodoc", "Registration Link", requestForm.PatientEmail, request.Requestid);
            SendSMS("Do Your Registration", 23323, request.Requestid);

            return "userNotExist";
        }

        return "ok";
    }
    public string ConciergeRequest(RequestFormModal requestForm)
    {
        var userexist = _context.Users.FirstOrDefault(u => u.Email == requestForm.PatientEmail);
        var checkBlokedUser = _context.BlockRequests.Where(u => u.Email == requestForm.PatientEmail).Count();

        if (checkBlokedUser > 0)
        {
            return "blocked";
        }

        Request request = new Request()
        {
            Status = 1,
            Firstname = requestForm.C_Firstname,
            Lastname = requestForm.C_Lastname,
            Email = requestForm.C_Email,
            Phonenumber = requestForm.C_Phonenumber,
            Createddate = DateTime.Now,
            Requesttypeid = 4,
            Confirmationnumber = _context.Regions.FirstOrDefault(r => r.Regionid == requestForm.State).Name.Substring(0, 2) + DateTime.Now.ToString().Substring(0, 4)
                                      + requestForm.Lastname.Substring(0, 2) + requestForm.Firstname.Substring(0, 2) + _context.Requests.Where(r => r.Createddate == DateTime.Now).Count().ToString().ToUpper(),
            User = (userexist != null) ? userexist : new User(),
        };

        RequestClient requestClient = new RequestClient()
        {
            Firstname = requestForm.Firstname,
            Lastname = requestForm.Lastname,
            Email = requestForm.PatientEmail,
            Phonenumber = requestForm.Phonenumber,
            Street = requestForm.Street,
            City = requestForm.City,
            State = requestForm.State.ToString(),
            Zipcode = requestForm.Zipcode,
            Notes = requestForm.Symptoms,

            Request = request,
        };



        _context.Requests.Add(request);
        _context.RequestClients.Add(requestClient);
        _context.SaveChanges();

        Concierge concierge = new Concierge()
        {
            Conciergename = requestForm.C_PropertyName,
            City = requestForm.C_City,
            State = requestForm.State.ToString(),
            Street = requestForm.C_Street,
            Zipcode = requestForm.C_Zipcode,
            Createddate = DateTime.Now,
        };

        RequestConcierge requestConcierge = new RequestConcierge()
        {
            Request = request,
            Concierge = concierge
        };
        _context.Concierges.Add(concierge);
        _context.RequestConcierges.Add(requestConcierge);
        _context.SaveChanges();

        if (requestForm.Doc != null)
        {
            SaveFile(requestForm.Doc, request.Requestid);
        }

        if (userexist == null)
        {
            SendMail("Do Your Register to Hellodoc", "Registration Link", requestForm.PatientEmail, request.Requestid);
            SendSMS("Do Your Registration", 23323, request.Requestid);
            return "userNotExist";
        }


        return "ok";
    }
    public string BusinessRequest(RequestFormModal requestForm)
    {
        var userexist = _context.Users.FirstOrDefault(u => u.Email == requestForm.PatientEmail);
        var checkBlokedUser = _context.BlockRequests.Where(u => u.Email == requestForm.PatientEmail).Count();

        if (checkBlokedUser > 0)
        {
            return "Patient Are Bloked To Create A Request";
        }

        Request request = new Request()
        {
            Status = 1,
            Firstname = requestForm.B_Firstname,
            Lastname = requestForm.B_Lastname,
            Email = requestForm.B_Email,
            Phonenumber = requestForm.B_Phonenumber,
            Createddate = DateTime.Now,
            Requesttypeid=1,
            Confirmationnumber = _context.Regions.FirstOrDefault(r => r.Regionid == requestForm.State).Name.Substring(0, 2) + DateTime.Now.ToString().Substring(0, 4)
                                      + requestForm.Lastname.Substring(0, 2) + requestForm.Firstname.Substring(0, 2) + _context.Requests.Where(r => r.Createddate == DateTime.Now).Count().ToString().ToUpper(),

        };

        RequestClient requestClient = new RequestClient()
        {
            Firstname = requestForm.Firstname,
            Lastname = requestForm.Lastname,
            Email = requestForm.PatientEmail,
            Phonenumber = requestForm.Phonenumber,
            Street = requestForm.Street,
            City = requestForm.City,
            State = requestForm.State.ToString(),
            Zipcode = requestForm.Zipcode,
            Notes = requestForm.Symptoms,
        };

        _context.Requests.Add(request);
        _context.RequestClients.Add(requestClient);
        _context.SaveChanges();
        if (requestForm.Doc != null)
        {
            SaveFile(requestForm.Doc, request.Requestid);
        }

        if (userexist == null)
        {
            SendMail("Do Your Register to Hellodoc", "Registration Link", requestForm.PatientEmail, request.Requestid);
            SendSMS("Do Your Registration", 23323, request.Requestid);
            return "userNotExist";
        }

        return "ok";
    }

    public string RequestMe(RequestFormModal requestForm,int Aspid)
    {
        var user = _context.Users.FirstOrDefault(u => u.Aspnetuserid == Aspid);
        var checkBlokedUser = _context.BlockRequests.Where(u => u.Email == requestForm.PatientEmail).Count();

        if (checkBlokedUser > 0)
        {
            return "blocked";
        }

        Request request = new Request()
        {
            Status = 1,
            Firstname = user.Firstname,
            Lastname = user.Lastname,
            Email = user.Email,
            Phonenumber = user.Mobile,
            Requesttypeid = 2,
            Createddate = DateTime.Now,
            User = user,
            Confirmationnumber = _context.Regions.FirstOrDefault(r => r.Regionid == requestForm.State).Name.Substring(0, 2) + DateTime.Now.ToString().Substring(0, 4)
                                      + requestForm.Lastname.Substring(0, 2) + requestForm.Firstname.Substring(0, 2) + _context.Requests.Where(r => r.Createddate == DateTime.Now).Count().ToString().ToUpper(),
        };

        RequestClient requestClient = new RequestClient()
        {
            Firstname = requestForm.Firstname,
            Lastname = requestForm.Lastname,
            Email = requestForm.PatientEmail,
            Phonenumber = requestForm.Phonenumber,
            Street = requestForm.Street,
            City = requestForm.City,
            State = requestForm.State.ToString(),
            Zipcode = requestForm.Zipcode,
            Notes = requestForm.Symptoms,

            Request = request,
        };

        _context.Requests.Add(request);
        _context.RequestClients.Add(requestClient);
        _context.SaveChanges();

        if (requestForm.Doc != null)
        {
            SaveFile(requestForm.Doc, request.Requestid);
        }

        return "ok";

    }
    public string RequestSomeone(RequestFormModal requestForm,int Aspid)
    {
        var userexist = _context.Users.FirstOrDefault(u => u.Aspnetuserid == Aspid);
        var checkBlokedUser = _context.BlockRequests.Where(u => u.Email == requestForm.PatientEmail).Count();

        if (checkBlokedUser > 0)
        {
            return "blocked";
        }

        Request request = new Request()
        {
            Status = 1,
            Firstname = userexist.Firstname,
            Lastname = userexist.Lastname,
            Email = userexist.Email,
            Phonenumber = userexist.Mobile,
            Relationname = requestForm.F_RelationType,
            Requesttypeid = 3,
            Createddate = DateTime.Now,
            Confirmationnumber = _context.Regions.FirstOrDefault(r => r.Regionid == requestForm.State).Name.Substring(0, 2) + DateTime.Now.ToString().Substring(0, 4)
                                      + requestForm.Lastname.Substring(0, 2) + requestForm.Firstname.Substring(0, 2) + _context.Requests.Where(r => r.Createddate == DateTime.Now).Count().ToString(),
        };
        if (userexist != null)
        {
            request.User = userexist;
        }
        RequestClient requestClient = new RequestClient()
        {
            Firstname = requestForm.Firstname,
            Lastname = requestForm.Lastname,
            Email = requestForm.PatientEmail,
            Phonenumber = requestForm.Phonenumber,
            Street = requestForm.Street,
            City = requestForm.City,
            State = requestForm.State.ToString(),
            Zipcode = requestForm.Zipcode,
            Notes = requestForm.Symptoms,

            Request = request,
        };

        _context.Requests.Add(request);
        _context.RequestClients.Add(requestClient);
        _context.SaveChanges();

        if (requestForm.Doc != null)
        {
            SaveFile(requestForm.Doc, request.Requestid);
        }

        if (userexist == null)
        {
            SendMail("Do Your Register to Hellodoc", "Registration Link", requestForm.PatientEmail, request.Requestid);
            SendSMS("Do Your Registration", 23323, request.Requestid);

            return "userNotExist";
        }

        return "ok";
    }

    public void SendMail(string subject, string message, string mailto, int requestid)
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
            Requestid = requestid,
            Subjectname = subject,
            Confirmationnumber = _context.Requests.FirstOrDefault(r => r.Requestid == requestid).Confirmationnumber,
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

    public void SaveFile(IFormFile formFile, int requestid)
    {
        string uploadfolder = Path.Combine(HostingEnviroment.WebRootPath, "uploads");
        string uniqueFilename = Guid.NewGuid().ToString() + "_" + formFile.FileName;
        string filename = Path.Combine(uploadfolder, uniqueFilename);
        using (var fileStream = new FileStream(filename, FileMode.Create))
        {
            formFile.CopyTo(fileStream);
        }

        RequestWiseFile requestWiseFile = new RequestWiseFile
        {
            Createddate = DateTime.Now,
            Filename = uniqueFilename,
            Requestid = requestid,
            Doctype = Path.GetExtension(uniqueFilename),
        };

        _context.RequestWiseFiles.Add(requestWiseFile);
        _context.SaveChanges();
    }

    public RequestFormModal GetPatientProfile(int uid)
    {
        var user = _context.Users.FirstOrDefault(u => u.Aspnetuserid == uid);
        if(user != null)
        {
            RequestFormModal requestForm = new RequestFormModal
            {
                Firstname = user.Firstname,
                Lastname = user.Lastname,
                PatientEmail = user.Email,
                Phonenumber = user.Mobile ?? 0,
                Street = user.Street,
                State = int.Parse(user.State),
                Zipcode = user.Zipcode ?? 0,
                City = user.City,
                DateOfBirth = new DateOnly(user.Intyear??2000, DateTime.ParseExact(user.Strmonth??"January", "MMMM", CultureInfo.InvariantCulture).Month,user.Intdate??1)
            };

            requestForm.regions = _context.Regions.ToList();

            return requestForm;
        }
        else
        {
            RequestFormModal requestForm = new RequestFormModal();
            requestForm.regions = _context.Regions.ToList() ;
            return requestForm;
        }
        
    }

    public AspNetUser GetAspUser(string email)
    {
        var aspNetUser = _context.AspNetUsers.Include(u => u.AspNetUserRole.Role).FirstOrDefault(x => x.Email == email);

        if (aspNetUser == null)
        {
            return new AspNetUser();
        }

        return aspNetUser;
    }

    public async Task<Int32> GetUser(int aspid)
    {
        var aspNetUser = _context.Users.FirstOrDefaultAsync(u => u.Aspnetuserid == aspid);

        return aspNetUser.Result.Userid;
    }

    public async Task UpdateUser(RequestFormModal updateForm,int userid)
    {

        var user1 = _context.Users.FirstOrDefault(u => u.Aspnetuserid == userid);
        if (user1 != null)
        {
            user1.Firstname = updateForm.Firstname;
            user1.Lastname = updateForm.Lastname;
            user1.Email = updateForm.PatientEmail;
            user1.Mobile = updateForm.Phonenumber;
            user1.Modifieddate = DateTime.Now;
            user1.State = updateForm.State.ToString();
            user1.Street = updateForm.Street;
            user1.City = updateForm.City;
            user1.Zipcode = updateForm.Zipcode;
            user1.Intyear = updateForm.DateOfBirth.Value.Year;
            user1.Intdate = updateForm.DateOfBirth.Value.Day;
            user1.Strmonth = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(updateForm.DateOfBirth.Value.Month);


            var aspnetuser = _context.AspNetUsers.FirstOrDefault(u => u.Id == user1.Aspnetuserid);

            aspnetuser.Phonenumber = updateForm.Phonenumber;
            aspnetuser.Email = updateForm.PatientEmail;
            aspnetuser.Username = updateForm.Firstname + " " + updateForm.Lastname;
        }
        else
        {
            User newuser = new User();
            newuser.Firstname = updateForm.Firstname;
            newuser.Lastname = updateForm.Lastname;
            newuser.Email = updateForm.PatientEmail;
            newuser.Mobile = updateForm.Phonenumber;
            newuser.Modifieddate = DateTime.Now;
            newuser.State = updateForm.State.ToString();
            newuser.Street = updateForm.Street;
            newuser.City = updateForm.City;
            newuser.Zipcode = updateForm.Zipcode;
            newuser.Aspnetuserid = userid;
            newuser.Createddate = DateTime.Now;
            newuser.Createdby = updateForm.Firstname +" "+ updateForm.Lastname;

            var aspnetuser = _context.AspNetUsers.FirstOrDefault(u => u.Id == userid);

            aspnetuser.Phonenumber = updateForm.Phonenumber;
            aspnetuser.Email = updateForm.PatientEmail;
            aspnetuser.Username = updateForm.Firstname + " " + updateForm.Lastname;

            _context.Users.Add(newuser);
        }

        _context.SaveChanges();
    }

    public PatientReqModel GetDocuments(int rid)
    {
        var doc = _context.RequestWiseFiles.Where(u => u.Requestid == rid && u.Isdeleted != 1);
        var request = _context.Requests.FirstOrDefault(r => r.Requestid == rid);
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
            Firstname = request.Firstname,
            Lastname = request.Lastname,
            userid = request.Userid??0,
            Confirmationnumber = request.Confirmationnumber
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
        reqclient.Email = closeCase.CloseCaseEmail;
        
        _context.SaveChanges();
    }

    public string CreateUser(RequestFormModal requestForm)
    {
        var aspnetuser = _context.AspNetUsers.FirstOrDefault(a => a.Email == requestForm.UserEmail);
        if (aspnetuser == null)
        {
            AspNetUser aspNetUser = new AspNetUser
            {
                Email = requestForm.UserEmail,
                Username = "new user",
                Passwordhash = requestForm.Password,
                Createddate = DateTime.Now,
            };
            _context.AspNetUsers.Add(aspNetUser);

            AspNetUserRole aspNetRole = new AspNetUserRole
            {
                User = aspNetUser,
                Roleid = 2,
            };
            _context.AspNetUserRoles.Add(aspNetRole);

            var requestClient = _context.RequestClients.FirstOrDefault(r => r.Email == requestForm.UserEmail);
            if (requestClient != null)
            {
                User user = new User
                {
                    Email = requestForm.UserEmail,
                    Firstname = requestClient.Firstname,
                    Lastname = requestClient.Lastname,
                    Createddate = DateTime.Now,
                    City = requestClient.City,
                    State = requestClient.State,
                    Intdate = requestClient.Intdate,
                    Intyear = requestClient.Intyear,
                    Strmonth = requestClient.Strmonth,

                    Aspnetuser = aspNetUser,
                };

                _context.Users.Add(user);
            }

            _context.SaveChanges();
            return "created";
        }
        else
        {
            return "exists";
        }
        
       
    }

    public string ResetPassword(RequestFormModal requestForm)
    {
        var aspnetuser = _context.AspNetUsers.FirstOrDefault(a => a.Email == requestForm.UserEmail);
        if (aspnetuser != null)
        {
            aspnetuser.Passwordhash = requestForm.Password;
            _context.SaveChanges();
            return "Password Updated";
        }
        else
        {
            return "error";
        }


    }
}


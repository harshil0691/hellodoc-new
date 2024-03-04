using hellodoc.DbEntity.DataContext;
using hellodoc.DbEntity.DataModels;
using hellodoc.DbEntity.ViewModels;
using hellodoc.Repositories.Repository.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace hellodoc.Repositories.Repository;

public class RequestAll : IRequests
{
    private readonly ApplicationDbContext _context;

    public RequestAll(ApplicationDbContext context)
    {
        _context = context;
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

    public async Task<User> SetUser(PatientReqModel patientReq,short aspid)
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

            //  Document1 = patientReq.Doc, 
            Createddate = DateTime.Now,
            Filename = uniqueFilename,
            Requestid = rid
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
        var doc = _context.RequestWiseFiles.Where(u => u.Requestid == rid && u.Isdeleted != 1).ToList();

        PatientReqModel patient = new PatientReqModel { 
            patientDocuments = doc
        };

        return  patient;
    }

    public async Task<string> GetFilename(int reqcliid)
    {
        var file = _context.RequestWiseFiles.FirstOrDefaultAsync(u => u.Requestwisefileid == reqcliid);

        return file.Result.Filename;
    }
}


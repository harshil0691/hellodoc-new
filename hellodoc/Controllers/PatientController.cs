using hellodoc.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
//using hellodoc.ViewModels;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore;
using System.Runtime.Intrinsics.X86;
using Microsoft.AspNetCore.Http;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
using Microsoft.CodeAnalysis.Elfie.Serialization;
using System.Xml.Linq;
using System;
using Microsoft.AspNetCore.Identity;
using hellodoc.Repositories.Repository.Interface;
using hellodoc.DbEntity.ViewModels;
using static hellodoc.DbEntity.ViewModels.RequestTableModel;
using hellodoc.Repositories.Repository;
using NUnit.Framework;
using hellodoc.DbEntity.DataModels;
using hellodoc.Repositories.Services.Interface;
//using hellodoc.DbEntity.DataModels;

namespace hellodoc.Controllers
{
    public class PatientController : Controller
    {
        private readonly ILogger<PatientController> _logger;
        private readonly IHostingEnvironment  HostingEnviroment;
        private readonly IRequests _requests;
        private readonly IPatientDashboard _patientDashboard;
        private readonly IAuthManager _authManager;
        private readonly IJwtServices _jwtServices;
        
        public PatientController(ILogger<PatientController> logger,IHostingEnvironment hostingEnvironment,IRequests  requests, IPatientDashboard patientDashboard,IAuthManager authManager,IJwtServices jwtServices)
        {
            _logger = logger;
            HostingEnviroment = hostingEnvironment;
            _requests = requests;
            _patientDashboard = patientDashboard;
            _authManager = authManager;
            _jwtServices = jwtServices;
        }

        public IActionResult GetPatientView(PartialViewModal partialView)
        {
            switch (partialView.actionType)
            {
                case "patient_request":
                    return PartialView("_PatientRequest");
                case "friend_request":
                    return PartialView("_FriendRequest");
                case "concierge_request":
                    return PartialView("_ConciergeRequest");
                case "business_request":
                    return PartialView("_BusinessRequest");
                case "patient_dashboard":
                    var u1 = HttpContext.Session.GetInt32("Aspid");

                    PatientReqModel patient = _patientDashboard.GetRequestList(u1);
                    return PartialView("_Patientdashboard",patient);
                case "patient_profile":
                    var u2 = HttpContext.Session.GetInt32("Aspid");

                    RequestFormModal patient2 = _requests.GetPatientProfile(u2??0);
                    return PartialView("_PatientProfile", patient2);
                case "patient_documents":
                    if (partialView.requestid == 0)
                    {
                        partialView.requestid = HttpContext.Session.GetInt32("requestid") ??0;
                    }
                    HttpContext.Session.SetInt32("requestid", partialView.requestid);

                    var document = _requests.GetDocuments(partialView.requestid);
                    return PartialView("_PatientDocument",document);

                case "requestModal":
                    return PartialView("_CreateRequest");
                case "request_me":
                    return PartialView("_RequestMe");
                case "request_someone":
                    return PartialView("_RequestSomeone");
                default:
                    return PartialView("_Default");
            }
        }


        public IActionResult CreateRequest(RequestFormModal requestForm,string RequestType)
        {
            var responseString = "";
            switch (RequestType)
            {
                case "patient_request":
                    if (_requests.PatientRequest(requestForm) == "ok")
                    {
                        var aspnetuser = new AspNetUser();
                        if (requestForm.Password == null)
                        {
                            aspnetuser = _requests.GetAspUser(requestForm.PatientEmail);
                            
                        }
                        else
                        {
                            aspnetuser = _authManager.Login(requestForm.PatientEmail, requestForm.Password);
                        }
                        Response.Cookies.Delete("jwt");
                        var jwttoken = _jwtServices.GenarateJwtToken(aspnetuser);
                        Response.Cookies.Append("jwt", jwttoken);
                        return RedirectToAction("PatientLogin","Login",aspnetuser);
                    }
                    else
                    {
                        return RedirectToAction("send_request","",new { type = "error",  tempdata = "User Not Created Internal Issue"});
                    }

                case "friend_request":
                    responseString = _requests.FriendRequest(requestForm);
                    break;

                case "concierge_request":
                    responseString =  _requests.ConciergeRequest(requestForm);
                    break;

                case "business_request":
                    responseString =  _requests.BusinessRequest(requestForm);
                    break;
                case "request_me":
                    responseString = _requests.RequestMe(requestForm,HttpContext.Session.GetInt32("Aspid")??1);
                    break;
                case "request_someone":
                    responseString = _requests.RequestSomeone(requestForm, HttpContext.Session.GetInt32("Aspid") ?? 1);
                    break;

            }
            if (RequestType != "patient_request" || RequestType != "request_me" || RequestType != "request_someone")
            {
                if (responseString == "ok")
                {
                    return RedirectToAction("send_request", "Patient", new { type = "success", tempdata = "Request Created Successfully" });
                }
                else if (responseString == "bloked")
                {
                    return RedirectToAction("send_request", "Patient", new { type = "error", tempdata = "User Bloked To Create Request" });
                }
                else if (responseString == "userNotExist")
                {
                    return RedirectToAction("send_request", "Patient", new { type = "warn", tempdata = "User Not Exist Mail/SMS Sent To User" });
                }
                else
                {
                    return RedirectToAction("send_request", "Patient", new { type = "error", tempdata = "Request Not Created Internal Error" });
                }
            }
            return RedirectToAction("patient_dashboard");
        }

        public IActionResult send_request(string type,string tempdata)
        {
            if(type == "success")
            {
                TempData["success"] = tempdata;
            }
            else if(type == "warn")
            {
                TempData["warn"] = tempdata;
            }
            else
            {
                TempData["error"] = tempdata;
            }
            
            return View();
        }



        public IActionResult request()
        {
            return View();
        }

        [HttpGet("checkemail")]
        [Route("/Patient/patient_request/checkemail/{email}")]
        public IActionResult CheckEmailExistence(string email)
        {
            // Check if email exists in the database
            var emailex = _requests.GetAspUser(email);
            bool emailExists;
            if (emailex.Id == 0)
            {
                emailExists = false;
            }
            else
            {
                emailExists = true;
            }
            
            return Ok(new { Exists = emailExists });
        }


        [CustomUserAuthorize("user")]
        public IActionResult patient_dashboard()
        {
            var u1 = HttpContext.Session.GetInt32("Aspid");
            PatientReqModel patient = _patientDashboard.GetRequestList(u1);

            return View(patient);
        }

        [HttpPost]
        public string PatientUpdateData(PartialViewModal partialView, RequestFormModal updateForm)
        {
            switch (partialView.actionType)
            {
                case "upload_doc":
                    _requests.SaveFile(updateForm.Doc, HttpContext.Session.GetInt32("requestid") ?? 0);
                    return "Document Uploded";
                case "update_profile":
                    var uid = HttpContext.Session.GetInt32("Aspid");
                    _requests.UpdateUser(updateForm, uid ?? 1);
                    return "User Profile Updated";
            }
            return "internal error";
        }

        public IActionResult patient_login()
        {
            return View();
        }

        public IActionResult request_me()
        {
            return View();
        }

        public IActionResult request_someone()
        {
            return View();
        }

        public IActionResult download(int download)
        {
            var download1 = download;
            var fname1 = _requests.GetFilename(download1).Result;

            var filepath = Path.Combine(HostingEnviroment.WebRootPath, "uploads", fname1);
            return File(System.IO.File.ReadAllBytes(filepath), "multipart/form-data", System.IO.Path.GetFileName(filepath));

        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
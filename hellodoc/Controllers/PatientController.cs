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

            }
            if (RequestType != "patient_request")
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
            return RedirectToAction("send_request");
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



        [CustomAuthorize("user")]
        public IActionResult patient_dashboard()
        {
            var u1 = HttpContext.Session.GetInt32("Aspid");

            PatientReqModel patient = _patientDashboard.GetRequestList(u1);

            return View(patient);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult update_profile(PatientReqModel patientReq)
        {
            var uid =  HttpContext.Session.GetInt32("userid");

            _requests.UpdateUser(patientReq, uid ??1);

            return RedirectToAction("patient_dashboard","Patient");
        }

        public IActionResult request_me()
        {
            return View();
        }

        public IActionResult request_someone()
        {
            return View();
        }

        public IActionResult patient_documents(int reqid)
        {
            var doc1 =  HttpContext.Session.GetInt32("userid");
            HttpContext.Session.SetInt32("requestid", reqid);

            var document = _requests.GetDocuments(reqid,doc1??1);

            return View(document);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult patient_documents(PatientReqModel patientReq)
        {
            var doc1 = HttpContext.Session.GetInt32("userid");
            var req1 = HttpContext.Session.GetInt32("requestid");

            //SaveFile(patientReq.Doc, req1??1);

            var document = _requests.GetDocuments(req1 ??1 , doc1 ?? 1);

            return View(document);
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
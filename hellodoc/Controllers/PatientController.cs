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
//using hellodoc.DbEntity.DataModels;

namespace hellodoc.Controllers
{
    public class PatientController : Controller
    {
        private readonly ILogger<PatientController> _logger;
        private readonly IHostingEnvironment  HostingEnviroment;
        private readonly IRequests _requests;
        private readonly IPatientDashboard _patientDashboard;
        
        public PatientController(ILogger<PatientController> logger,IHostingEnvironment hostingEnvironment,IRequests  requests, IPatientDashboard patientDashboard)
        {
            _logger = logger;
            HostingEnviroment = hostingEnvironment;
            _requests = requests;
            _patientDashboard = patientDashboard;
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

        public string CreateRequest(RequestFormModal requestForm)
        {
            switch (requestForm.RequestType)
            {
                case "patient_request":
                    _requests.PatientRequest(requestForm);
                    break;
                case "friend_request":
                    _requests.FriendRequest(requestForm);
                    break;
                case "concierge_request":
                    _requests.ConciergeRequest(requestForm);
                    break;
                case "business_request":
                    _requests.BusinessRequest(requestForm);
                    break;
            }

            return "ok";
        }

        public IActionResult send_request()
        {
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
            var emailex = _requests.GetAspUser(email).Result;
            bool emailExists;
            if (emailex == 0)
            {
                emailExists = false;
            }
            else
            {
                emailExists = true;
            }
            
            return Ok(new { Exists = emailExists });
        }

        public IActionResult patient_request()
        {
            return View();
        }

        // ----------------------------- patient request----------------------------------------------------------------------------------------

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> patient_request(PatientReqModel patientReq)
        {
            var aspNetUserid = _requests.GetAspUser(patientReq.Email).Result;

            if (aspNetUserid == null)
            {
                
                var aspNetUser1 = _requests.SetAspNetUser(patientReq).Result; 
                var user = _requests.SetUser(patientReq, aspNetUser1.Id).Result;
                var request = _requests.SetRequest(patientReq ,user.Userid).Result;
                var requestclient = _requests.SetRequestClient(patientReq , request.Requestid).Result;

                SaveFile(patientReq.Doc, request.Requestid);

                var userid1 = _requests.GetUser(aspNetUserid).Result;
                
                HttpContext.Session.SetInt32("userid", userid1);
                HttpContext.Session.SetInt32("requestid", request.Requestid);
                HttpContext.Session.SetString("username",patientReq.Firstname+" " + patientReq.Lastname);
                return RedirectToAction("patient_dashboard","Patient");

            }
            if (aspNetUserid != null)
            {
                var userid1 = _requests.GetUser(aspNetUserid).Result;

                var request = _requests.SetRequest(patientReq, userid1).Result;
                var requestClient = _requests.SetRequestClient(patientReq, request.Requestid);

                SaveFile(patientReq.Doc, request.Requestid);

                HttpContext.Session.SetInt32("userid", userid1);
                HttpContext.Session.SetInt32("requestid", request.Requestid);
                HttpContext.Session.SetString("username", patientReq.Firstname + " " + patientReq.Lastname);
                return RedirectToAction("patient_dashboard", "Patient");
            }

            return View();
        }

        public IActionResult friend_request()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult friend_request(FriendReqModel friendReq)
        {

            var requestid = _requests.SetRequest(friendReq).Result;

            _requests.SetRequestClient(friendReq, requestid);

            SaveFile(friendReq.Doc, requestid);

            return RedirectToAction("send_request", "Patient");

        }

        

        public IActionResult concierge_request() 
        {
            return View();   
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult concierge_request_info(ConciergeReqModel conciergeReq)
        {
            
            var rid = _requests.SetRequest(conciergeReq).Result;
            _requests.SetRequestClient(conciergeReq, rid);
            _requests.SetConcierge(conciergeReq);

            return RedirectToAction("send_request", "Patient");
        }

        public IActionResult business_request()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult business_request(BusinessReqModel businessReq)
        {
            if (ModelState.IsValid) {

              var rid =  _requests.SetRequest(businessReq).Result;
              _requests.SetRequestClient(businessReq, rid);
              _requests.Setbusiness(businessReq);

            return RedirectToAction("send_request", "Patient");
            }
            else
            {
                return View();
            }

        }

        [CustomAuthorize("user")]
        public IActionResult patient_dashboard()
        {
            var u1 = HttpContext.Session.GetInt32("userId");


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

            SaveFile(patientReq.Doc, req1??1);

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


        public void SaveFile(IFormFile uploadfile, int rid)
        {
            string uniqueFilename = null;

            if (uploadfile != null)
            {
                string uploadfolder = Path.Combine(HostingEnviroment.WebRootPath, "uploads");
                uniqueFilename = Guid.NewGuid().ToString() + "_" + uploadfile.FileName;
                string filename = Path.Combine(uploadfolder, uniqueFilename);
                uploadfile.CopyTo(new FileStream(filename, FileMode.Create));

                _requests.SetRequestWiseFile(uniqueFilename, rid);
            }

        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
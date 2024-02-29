using Microsoft.AspNetCore.Mvc;
using hellodoc.Models;
using hellodoc.DbEntity.DataModels;
using System.Diagnostics;
using hellodoc.Repositories.Repository.Interface;
using hellodoc.DbEntity.ViewModels;
using hellodoc.Repositories.Repository;
using Org.BouncyCastle.Asn1.Ocsp;
using hellodoc.DbEntity.ViewModels.PopUpModal;

namespace hellodoc.Controllers
{
    public class AdminDashController : Controller
    {
        private readonly ILogger<AdminDashController> _logger;
        private readonly IAdminDashRepository _adminDashRepository;
        private readonly IPatientLogin _patientLogin;
        private readonly IRequests _requests;


        public AdminDashController(ILogger<AdminDashController> logger,IAdminDashRepository adminDashRepository, IPatientLogin patientLogin, IRequests requests)
        {
            _logger = logger;
            _adminDashRepository = adminDashRepository;
            _patientLogin = patientLogin;
            _requests = requests;
        }

        public IActionResult admin_dash()
        {
            var request = _adminDashRepository.GetCount().Result;
            return View(request);
        }

        public IActionResult login(AspNetUser obj)
        {
            var aspnetuser = _patientLogin.GetAspNetUser(obj.Username, obj.Passwordhash);



            if (ModelState.IsValid)
            {
                if (aspnetuser.Result != null)
                {
                    var userId = _requests.GetUser(aspnetuser.Result.Id);
                    TempData["success"] = "User LogIn Successfully";
                    HttpContext.Session.SetInt32("userid", userId.Result);
                    HttpContext.Session.SetString("username", aspnetuser.Result.Username);

                    return RedirectToAction("admin_dash", "AdminDash");

                }
                else
                {

                    TempData["error"] = "Username or Password is Incorrect";
                    ModelState.AddModelError(string.Empty, "Invalid email or Password");
                    return View();
                }
            }
            return View();
        }


        public IActionResult view_notes(int Requestid)
        {
            
            NotesModel notesModel = new NotesModel();
            if(Requestid == 0)
            {
                var req = HttpContext.Session.GetInt32("requestid");
                notesModel = _adminDashRepository.GetNotes(req??1).Result;
            }
            else
            {
                HttpContext.Session.SetInt32("requestid", Requestid);
                notesModel = _adminDashRepository.GetNotes(Requestid).Result;
            }
           
            return View(notesModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult save_note(NotesModel note)
        {
            var req = HttpContext.Session.GetInt32("requestid");
            var user = HttpContext.Session.GetString("username");
            _adminDashRepository.SetNotes(note, req, user);

            return RedirectToAction("view_notes", "AdminDash");
        }



        public IActionResult view_case(int requestid)
        {
            PatientReqModel patientReq = _adminDashRepository.Getpatientdata(requestid).Result;


            return View(patientReq);
        }

        [HttpPost]
        public IActionResult cancel_case(int requestid,CancelCaseModel cancelCase)
        {
            var user = 1;

            _adminDashRepository.CancelRequest(requestid,cancelCase,user);

            return RedirectToAction("admin_dash","AdminDash");
        }

        public IActionResult assign_case(int requestid,AssignCaseModal assignCase)
        {
             var user = 1;

            _adminDashRepository.AssignCase(requestid,assignCase, user);

            return RedirectToAction("admin_dash", "AdminDash");
        }

        [HttpPost]
        public IActionResult Openmodal(int requestid,string patientname, string modalname)
        {
            switch (modalname)
            {
                case "CancelCase":
                    CancelCaseModel cancelCase = new CancelCaseModel
                    {
                        Requestid = requestid,
                        PatientName = patientname,
                    };

                    return PartialView("_CancelCase", cancelCase);

                case "AssignCase":
                    AssignCaseModal assignCase = new AssignCaseModal
                    {
                        Regions = _adminDashRepository.GetRegions(),
                        Physicians = _adminDashRepository.GetPhysicianList(),
                        Requestid = requestid,
                    };

                    return PartialView("_AssignCase", assignCase);

                case "BlockCase":
                    CancelCaseModel cancelCase2 = new CancelCaseModel
                    {
                        Requestid = requestid,
                        PatientName = patientname,
                    };

                    return PartialView("_AssignCase", cancelCase2);


                default:
                    return PartialView("_default");
            }
                    
        }

        public List<Physician> GetPhysicians(int select)
        {
            var phy = _adminDashRepository.GetPhysicianList2(select);

            return phy;
        }

        public IActionResult LoadPartialView(string tabId)
        {
            
            switch (tabId)
            {
                case "new":
                    var status1 = new List<int> { 1 };

                    var dashModel1 = _adminDashRepository.GetRequests(status1);

                    return PartialView("_NewAdmin",dashModel1);

                case "pending":
                    var status2 = new List<int> { 2 };
                    
                    var dashModel2 = _adminDashRepository.GetRequests(status2);

                    return PartialView("_PendingAdmin",dashModel2);

                case "active":
                    var status3 = new List<int> { 4, 5 };
                    
                    var dashModel3 = _adminDashRepository.GetRequests(status3);

                    return PartialView("_ActiveAdmin", dashModel3);

                case "conclude":
                    var status4 = new List<int> { 6 };
                    
                    var dashModel4 = _adminDashRepository.GetRequests(status4);

                    return PartialView("_ConcludeAdmin", dashModel4);

                case "toclose":
                    var status5 = new List<int> { 3,7,8 };
            
                    var dashModel5 = _adminDashRepository.GetRequests(status5);

                    return PartialView("_ToCloseAdmin", dashModel5);

                case "unpaid":
                    var status6 = new List<int> { 9 };

                    var dashModel6 = _adminDashRepository.GetRequests(status6);

                    return PartialView("_UnpaidAdmin", dashModel6);

                default:
                    return PartialView("_DefaultTab");
            }
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using hellodoc.Models;
using hellodoc.DbEntity.DataModels;
using System.Diagnostics;
using hellodoc.Repositories.Repository.Interface;
using hellodoc.DbEntity.ViewModels;
using hellodoc.Repositories.Repository;
using Org.BouncyCastle.Asn1.Ocsp;
using hellodoc.DbEntity.ViewModels.PopUpModal;
using Microsoft.AspNetCore.Identity;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;


namespace hellodoc.Controllers
{
    public class AdminDashController : Controller
    {
        private readonly ILogger<AdminDashController> _logger;
        private readonly IAdminDashRepository _adminDashRepository;
        private readonly IPatientLogin _patientLogin;
        private readonly IRequests _requests;
        private readonly IHostingEnvironment HostingEnviroment;

        public AdminDashController(ILogger<AdminDashController> logger,IAdminDashRepository adminDashRepository, IPatientLogin patientLogin, IRequests requests,IHostingEnvironment hostingEnvironment)
        {
            _logger = logger;
            _adminDashRepository = adminDashRepository;
            _patientLogin = patientLogin;
            _requests = requests;
            HostingEnviroment = hostingEnvironment;
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

        public IActionResult view_uploads(int requestid)
        {
            var doc1 = HttpContext.Session.GetInt32("userid");
            HttpContext.Session.SetInt32("requestid", requestid);

            var document = _requests.GetDocuments(requestid, doc1 ?? 1);

            return View(document);
        }

        [HttpPost]
        public IActionResult view_uploads(IFormFile file1)
        {
            var doc1 = HttpContext.Session.GetInt32("userid");
            var req1 = HttpContext.Session.GetInt32("requestid");

            //var file = Request.Files["file1"];

            SaveFile(file1, req1 ?? 1);

            var document = _requests.GetDocuments(req1 ?? 1, doc1 ?? 1);
            return PartialView("_ViewUploads",document);
        }

        public IActionResult download(int download)
        {
            var fname1 = _requests.GetFilename(download).Result;

            var filepath = Path.Combine(HostingEnviroment.WebRootPath, "uploads", fname1);
            return File(System.IO.File.ReadAllBytes(filepath), "multipart/form-data", System.IO.Path.GetFileName(filepath));

        }

        public void downloadAll()
        {
            var req1 = HttpContext.Session.GetInt32("requestid");

            var filelist = _requests.GetDocuments(req1 ?? 0,1);

            foreach ( var file in filelist.patientDocuments )
            {
                download(file.Requestwisefileid);
            }
        }

        public void  deleteDoc(int download)
        {
            _adminDashRepository.DeleteDocument(download);

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

        public IActionResult block_case(int requestid, BlockCaseModal blockCase)
        {
            var user = 1;

            _adminDashRepository.BlockCase(requestid,blockCase, user);

            return RedirectToAction("admin_dash", "AdminDash");
        }

        public async Task ClearCase(int requestid)
        {
            var user = 1;

            _adminDashRepository.Clearcase(requestid, user);
        }

        public async Task send_aggrement(int requestid)
        {
            var user = 1;

            _adminDashRepository.Clearcase(requestid, user);
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
                        Modaltype = patientname,
                    };

                    return PartialView("_AssignCase", assignCase);

                case "BlockCase":
                    BlockCaseModal blockCase = new BlockCaseModal
                    {
                        Requestid = requestid,
                        PatientName = patientname,
                    };

                    return PartialView("_BlockCase", blockCase);

                case "SendAggrement":



                default:
                    return PartialView("_default");
            }
                    
        }



        public List<Physician> GetPhysicians(int select)
        {
            var phy = _adminDashRepository.GetPhysicianList2(select);

            return phy;
        }

        public IActionResult LoadPartialView(string tabId, int requestid, int activeid)
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

                case "dashboard":

                    RequestCountByStatus request = _adminDashRepository.GetCount().Result;
                    request.activeid = activeid;

                    return PartialView("_dashboard", request);

                case "ViewUploads":
                    var doc1 = HttpContext.Session.GetInt32("userid");
                    HttpContext.Session.SetInt32("requestid", requestid);

                    var document = _requests.GetDocuments(requestid, doc1 ?? 1);

                    return PartialView("_ViewUploads",document);

                case "deleteDoc":

                    _adminDashRepository.DeleteDocument(activeid);

                    var doc2 = HttpContext.Session.GetInt32("userid");
                    HttpContext.Session.SetInt32("requestid", requestid);

                    var document1 = _requests.GetDocuments(requestid, doc2 ?? 1);

                    return PartialView("_ViewUploads", document1);

                default:
                    return PartialView("_DefaultTab");
            }
        }
    }
}

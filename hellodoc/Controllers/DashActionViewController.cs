using hellodoc.DbEntity.ViewModels.PopUpModal;
using hellodoc.DbEntity.ViewModels;
using hellodoc.Repositories.Repository.Interface;
using Microsoft.AspNetCore.Mvc;
using System.IO.Compression;
using System.Net.Mail;
using System.Net;
using hellodoc.Repositories.Repository;
using hellodoc.DbEntity.DataModels;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using System.ComponentModel;
using OfficeOpenXml;
using Microsoft.CodeAnalysis.CSharp.Syntax;
//using IronPdf;
using System.IO;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace hellodoc.Controllers
{
    [CustomUserAuthorize("admin","provider")]
    public class DashActionViewController : Controller

    {
        private readonly ILogger<AdminDashController> _logger;
        private readonly IAdminDashRepository _adminDashRepository;
        private readonly IPatientLogin _patientLogin;
        private readonly IRequests _requests;
        private readonly IWebHostEnvironment HostingEnviroment;
        private readonly IAuthManager _authManger;
        private readonly IAdminProviders _adminProviders;

        public DashActionViewController(ILogger<AdminDashController> logger, IAdminDashRepository adminDashRepository, IRequests requests, IWebHostEnvironment hostingEnvironment, IAdminProviders adminProviders)
        {
            _logger = logger;
            _adminDashRepository = adminDashRepository;
            _requests = requests;
            HostingEnviroment = hostingEnvironment;
            _adminProviders = adminProviders;
        }

        [HttpPost]
        public IActionResult Openmodal(PartialViewModal partialView)
        {
            switch (partialView.actionType)
            {
                case "CancelCase":
                    CancelCaseModel cancelCase = new CancelCaseModel
                    {
                        Requestid = partialView.requestid,
                        PatientName = partialView.patientName,
                    };

                    return PartialView("_CancelCase", cancelCase);

                case "AssignCase":
                    AssignCaseModal assignCase = new AssignCaseModal
                    {
                        Regions = _adminDashRepository.GetRegions(0),
                        Physicians = _adminDashRepository.GetPhysicianList(),
                        Requestid = partialView.requestid,
                        Modaltype = partialView.patientName,
                    };

                    return PartialView("_AssignCase", assignCase);

                case "BlockCase":
                    BlockCaseModal blockCase = new BlockCaseModal
                    {
                        Requestid = partialView.requestid,
                        PatientName = partialView.patientName,
                    };

                    return PartialView("_BlockCase", blockCase);

                case "SendAgreement":
                    SendAgreementModal sendAgreement = new SendAgreementModal
                    {
                        reqid = partialView.requestid,
                        reqtype = partialView.btext,
                        bcolor = partialView.bcolor,
                        email = partialView.email,
                        phonenumber = partialView.phonenumber,
                        patientName = partialView.patientName,
                    };

                    return PartialView("_SendAgreement", sendAgreement);

                case "SendLink":

                    return PartialView("_SendLink");
                case "RequestDTYSupport":
                    return PartialView("_RequestDTYSupport");
                case "finalizedencounter":
                    return PartialView("_FinalizedEncounter");
                case "ContactProvider":
                    ContactProviderModal contactProvider = new ContactProviderModal
                    {
                        physicianid = partialView.physicianid,
                    };
                    return PartialView("_ContactProvider", contactProvider);
                case "encounter":
                    ViewBag.requestid = partialView.requestid;
                    return PartialView("_Encounter");
                case "transfertoadmin":
                    ViewBag.requestid = partialView.requestid;
                    ViewBag.physicianid = HttpContext.Session.GetInt32("physicianid");
                    return PartialView("_TransferRequest");

                default:
                    return PartialView("_default");
            }

        }

        public IActionResult LoadActionViews(PartialViewModal partialView)
        {

            switch (partialView.actionType)
            {

                case "dashboard":
                    return RedirectToAction("LoadPartialDashView", "AdminDash", new { tabId = "dashboard" });

                case "ViewUploads":
                    var doc1 = HttpContext.Session.GetInt32("userid");
                    HttpContext.Session.SetInt32("requestid", partialView.requestid);

                    var document = _requests.GetDocuments(partialView.requestid);

                    return PartialView("_ViewUploads", document);

                case "deleteDoc":

                    _adminDashRepository.DeleteDocument(partialView.requestwisefileid);

                    var doc2 = HttpContext.Session.GetInt32("userid");
                    HttpContext.Session.SetInt32("requestid", partialView.requestid);

                    var document1 = _requests.GetDocuments(partialView.requestid);

                    return PartialView("_ViewUploads", document1);

                case "Orders":

                    OrdersModal ordersModal = new OrdersModal();
                    ordersModal.professionName = _adminDashRepository.GetListProfessionTypes();
                    ordersModal.requestid = partialView.requestid;
                    ordersModal.aspid = HttpContext.Session.GetInt32("userid");

                    return PartialView("_Orders", ordersModal);

                case "ViewCase":

                    PatientReqModel patientReq = _adminDashRepository.Getpatientdata(partialView.requestid).Result;
                    patientReq.bgcolor = partialView.bcolor;
                    patientReq.btext = partialView.btext;

                    return PartialView("_ViewCase", patientReq);

                case "ViewNotes":

                    NotesModel notesModel = new NotesModel();
                    if (partialView.requestid == 0)
                    {
                        var req = HttpContext.Session.GetInt32("requestid");
                        notesModel = _adminDashRepository.GetNotes(req ?? 1).Result;
                    }
                    else
                    {
                        HttpContext.Session.SetInt32("requestid", partialView.requestid);
                        notesModel = _adminDashRepository.GetNotes(partialView.requestid).Result;
                    }

                    return PartialView("_ViewNotes", notesModel);

                case "CloseCase":
                    HttpContext.Session.SetInt32("reqid", partialView.requestid);
                    CloseCaseModal closeCase = _adminDashRepository.GetCloseCaseModal(partialView.requestid).Result;
                    closeCase.patientName = partialView.patientName;

                    return PartialView("_closeCase", closeCase);

                case "EncounterForm":
                    HttpContext.Session.SetInt32("requestId", partialView.requestid);

                    Encounter encounter = _adminDashRepository.GetEncounter(partialView.requestid);
                    if (encounter.Isfinalized == 1)
                    {
                        return Json(new { isfinalized = 1 });
                    }
                    return PartialView("_EncounterForm", encounter);

                case "CreateRequest":
                    return PartialView("_CreateRequest");



                default:
                    return PartialView("_DefaultTab");
            }
        }

        public IActionResult ViewCase(PartialViewModal partialView)
        {
            PatientReqModel patientReq = _adminDashRepository.Getpatientdata(partialView.requestid).Result;
            patientReq.bgcolor = partialView.bcolor;
            patientReq.btext = partialView.btext;

            return PartialView("_ViewCase", patientReq);
        }

        public IActionResult dashboard(PartialViewModal partialView)
        {
            return RedirectToAction("LoadPartialDashView", "AdminDash", new { tabId = "dashboard" });
        }
        public IActionResult ViewUploads(PartialViewModal partialView)
        {
            var doc1 = HttpContext.Session.GetInt32("userid");
            HttpContext.Session.SetInt32("requestid", partialView.requestid);

            var document = _requests.GetDocuments(partialView.requestid);

            return PartialView("_ViewUploads", document);
        }
        public IActionResult ConcludeCare(int requestid)
        {
            Encounter encounter = _adminDashRepository.GetEncounter(requestid);
            ConcludCare concludCare = new ConcludCare();
            if (encounter.Isfinalized == 1)
            {
                concludCare.isfinalized = 1; 
            }
            concludCare.patientDocuments = _requests.GetDocuments(requestid).patientDocuments;
            concludCare.PatientName = _requests.GetDocuments(requestid).Firstname;
            return PartialView("_ConcludeCare",concludCare);
        }


        public IActionResult deleteDoc(PartialViewModal partialView)
        {
            _adminDashRepository.DeleteDocument(partialView.requestwisefileid);

            var doc2 = HttpContext.Session.GetInt32("userid");
            HttpContext.Session.SetInt32("requestid", partialView.requestid);

            var document1 = _requests.GetDocuments(partialView.requestid);

            return PartialView("_ViewUploads", document1);
        }
        
        public IActionResult Orders(PartialViewModal partialView)
        {
            OrdersModal ordersModal = new OrdersModal();
            ordersModal.professionName = _adminDashRepository.GetListProfessionTypes();
            ordersModal.requestid = partialView.requestid;
            ordersModal.aspid = HttpContext.Session.GetInt32("userid");

            return PartialView("_Orders", ordersModal);
        }

        public IActionResult ViewNotes(PartialViewModal partialView)
        {
            NotesModel notesModel = new NotesModel();
            if (partialView.requestid == 0)
            {
                var req = HttpContext.Session.GetInt32("requestid");
                notesModel = _adminDashRepository.GetNotes(req ?? 1).Result;
            }
            else
            {
                HttpContext.Session.SetInt32("requestid", partialView.requestid);
                notesModel = _adminDashRepository.GetNotes(partialView.requestid).Result;
            }

            return PartialView("_ViewNotes", notesModel);
        }

        public IActionResult CloseCase(PartialViewModal partialView)
        {
            HttpContext.Session.SetInt32("reqid", partialView.requestid);
            CloseCaseModal closeCase = _adminDashRepository.GetCloseCaseModal(partialView.requestid).Result;
            closeCase.patientName = partialView.patientName;

            return PartialView("_closeCase", closeCase);
        }
                    
        public IActionResult EncounterForm(PartialViewModal partialView)
        {
            HttpContext.Session.SetInt32("requestId", partialView.requestid);

            Encounter encounter = _adminDashRepository.GetEncounter(partialView.requestid);
            if (encounter.Isfinalized == 1)
            {
                return Json(new { isfinalized = 1 });
            }
            return PartialView("_EncounterForm", encounter);
        }

        public IActionResult CreateRequest()
        {
            return PartialView("_CreateRequest");
        }


    public IActionResult contact_provider(ContactProviderModal contactProvider,int physicianid)
        {
            var physician = _adminProviders.GetPhysicianAsync(physicianid);

            switch (contactProvider.MessagwType)
            {
                case "1":
                    SendSMS(contactProvider.Message,physician.Mobile);
                    break;
                
                case "2":
                    SendMail("Message From admin", contactProvider.Message, physician.Email);
                    break;

                case "3":
                    SendSMS(contactProvider.Message, physician.Mobile);
                    SendMail("Message From admin", contactProvider.Message, physician.Email);
                    break;
            }

            return RedirectToAction("admin_dash", "AdminDash");
        }

        //public IActionResult download_encounter()
        //{
        //    var requestId = HttpContext.Session.GetInt32("requestId") ?? 1;
        //    Encounter encounter = _adminDashRepository.GetEncounter(requestId);

        //    //IronPdf.License.LicenseKey = "IRONSUITE.HARSHIL.DHADUK.ETATVASOFT.COM.32452-E664BA0484-NJCBR-TEJYAREUGQB5-YEJM6R5HNOOW-5RCB4X6GDSFM-5ZQASR62CVDP-CYLR67INDBE5-274LGFHY7VCP-NTJQG7-TNAKXTGBBQWMEA-DEPLOYMENT.TRIAL-D2ALM2.TRIAL.EXPIRES.18.APR.2024";

        //    //IronPdf.HtmlToPdf Renderer = new IronPdf.HtmlToPdf();

        //    var htmlContent = "<html><body>";

        //    var logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "Fig56._Patient_site_1-removebg-preview.png");
        //    htmlContent += $"<img src='{logoPath}' alt='Company Logo' style='height: 50px; width: auto;' />";

        //    htmlContent += "<div>";
        //    htmlContent += "<h1>Encounter Form</h1>";
        //    htmlContent += "<p>Your finalized data in encounterform is given below: </p>";
        //    htmlContent += $"<h5>Firstname : {encounter.Firstname}</h5>";
        //    htmlContent += $"<h5>Lastname : {encounter.Lastname}</h5>";
        //    htmlContent += $"<h5>Email : {encounter.Email}</h5>";
        //    htmlContent += $"<h5>Phone : {encounter.Phone}</h5>";
        //    htmlContent += $"<h5>Temperature : {encounter.Temperature}</h5>";
        //    htmlContent += $"<h5>Hr : {encounter.Hr}</h5>";
        //    htmlContent += $"<h5>Rr : {encounter.Rr}</h5>";
        //    htmlContent += $"<h5>Bloodpressure1 : {encounter.Bloodpressure1}</h5>";
        //    htmlContent += $"<h5>DateOfBirth : {encounter.DateOfBirth}</h5>";
        //    htmlContent += $"<h5>Allergies : {encounter.Allergies}</h5>";
        //    htmlContent += $"<h5>MedicalHistory : {encounter.MedicalHistory}</h5>";
        //    htmlContent += $"<h5>Medications : {encounter.Medications}</h5>";
        //    htmlContent += $"<h5>Heent : {encounter.Heent}</h5>";
        //    htmlContent += "</div>";
        //    htmlContent += "</body></html>";

        //   // var pdfBytes = Renderer.RenderHtmlAsPdf(htmlContent).BinaryData;

        //    return File(pdfBytes, "application/pdf", "GeneratedPDF.pdf");

        //}

        [HttpPost]
        public IActionResult request_support(RequestSupportModal supportModal)
        {
            return RedirectToAction("admin_dash", "AdminDash");
        }

        //[HttpPost]
        //public IActionResult create_request(PatientReqModel patientReq)
        //{
        //    var aspNetUserid = _requests.GetAspUser(patientReq.Email).Result;

        //    if (aspNetUserid == 0)
        //    {
        //        var request = _requests.SetRequest(patientReq,18).Result;
        //        var requestclient = _requests.SetRequestClient(patientReq, request.Requestid).Result;

        //        var link = "https://localhost:7036/Patient/CreateAccount";

        //        string message = "Create a Account for track your request on hellodoc \n\n\n use below link :\n\n"+ link;

        //        SendMail("Patient Request from Admin", message , patientReq.Email);
        //    }else
        //    {
        //        var userid1 = _requests.GetUser(aspNetUserid).Result;
        //        var request = _requests.SetRequest(patientReq, userid1).Result;
        //        var requestClient = _requests.SetRequestClient(patientReq, request.Requestid);
        //    }

        //    return RedirectToAction("admin_dash","AdminDash");
        //}

        [HttpPost]
        public IActionResult DocumentActions(List<int> selectedCheck, int requestid, string actionType)
        {
            switch (actionType)
            {
                case "DeleteAll":

                    foreach (int id in selectedCheck)
                    {
                        _adminDashRepository.DeleteDocument(id);
                    }

                    var doc2 = HttpContext.Session.GetInt32("userid");
                    HttpContext.Session.SetInt32("requestid", requestid);

                    var document1 = _requests.GetDocuments(requestid);

                    return PartialView("_ViewUploads", document1);

                case "SendEmail":

                    var list = _adminDashRepository.GetListFilename(selectedCheck);

                    sendEmail("hpdhaduk0605@gmail.com", "your uploads", "document", list);

                    var document = _requests.GetDocuments(requestid);

                    return PartialView("_ViewUploads", document);

                case "DownloadAll":

                    var list1 = _adminDashRepository.GetListFilename(selectedCheck);

                    MemoryStream ms = new MemoryStream();
                    using (ZipArchive zip = new ZipArchive(ms, ZipArchiveMode.Create, true))
                        list1.ForEach(file =>
                        {
                            var filepath = Path.Combine(HostingEnviroment.WebRootPath, "uploads", file);
                            ZipArchiveEntry zipEntry = zip.CreateEntry(file);
                            using (FileStream fs = new FileStream(filepath, FileMode.Open, FileAccess.Read))
                            using (Stream zipEntryStream = zipEntry.Open())
                            {
                                fs.CopyTo(zipEntryStream);
                            }
                        });
                    return File(ms.ToArray(), "application/zip", "download.zip");


                default:
                    return PartialView("_DefaultTab");
            }


        }

        [HttpPost]
        public string TransferToAdmin(PartialViewModal partialView)
        {
            _adminDashRepository.TransferToAdmin(partialView.requestid,partialView.transferNotes,partialView.physicianid);
            return "Request is transfer to admin successfully";
        }

        public Task sendEmail(string email, string subject, string message, List<string> files)
        {
            var mail = "pdhaduk300@gmail.com";
            var password = "cqcq ixsm fpfx yulg";

            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(mail, password)
            };

            MailMessage mailMessage = new MailMessage(from: mail, to: email, subject, message);

            foreach (var item in files)
            {
                string filepath = Path.Combine(HostingEnviroment.WebRootPath, "uploads", item);
                Attachment attachment = new Attachment(filepath);
                mailMessage.Attachments.Add(attachment);
            }

            return client.SendMailAsync(mailMessage);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult save_note(NotesModel note)
        {
            var req = HttpContext.Session.GetInt32("requestid");
            var user = HttpContext.Session.GetString("username");
            _adminDashRepository.SetNotes(note, req, user);

            NotesModel notesModel = new NotesModel();
            notesModel = _adminDashRepository.GetNotes(req ?? 1).Result;

            return PartialView("_ViewNotes", notesModel);
        }

        [HttpPost]
        public IActionResult view_uploads(IFormFile file1)
        {
            var req = HttpContext.Session.GetInt32("requestid");
            var doc1 = HttpContext.Session.GetInt32("userid");

            _requests.SaveFile(file1,req??0);

            var document = _requests.GetDocuments(req ?? 1);
            
            return PartialView("_ViewUploads", document);
        }

        //public void SaveFile(IFormFile uploadfile, int rid)
        //{
        //    //string uniqueFilename = null;

        //    //if (uploadfile != null)
        //    //{
        //    //    string uploadfolder = Path.Combine(HostingEnviroment.WebRootPath, "uploads");
        //    //    uniqueFilename = Guid.NewGuid().ToString() + "_" + uploadfile.FileName;
        //    //    string filename = Path.Combine(uploadfolder, uniqueFilename);
        //    //    using (FileStream file = new FileStream(filename, FileMode.Create))
        //    //    {
        //    //        uploadfile.CopyTo(file);
        //    //    }

        //    //    _requests.SaveFile(uniqueFilename, rid);
        //    //}

        //}


        public IActionResult cancel_case(int requestid,CancelCaseModel cancelCase)
        {
            var user = 1;

            _adminDashRepository.CancelRequest(requestid,cancelCase,user);

            return RedirectToAction("admin_dash","AdminDash");
        }

        public IActionResult assign_case(int requestid, AssignCaseModal assignCase, string Modaltype)
        {
            var user = 1;

            assignCase.Modaltype = Modaltype;

            _adminDashRepository.AssignCase(requestid, assignCase, user);

            return RedirectToAction("admin_dash", "AdminDash");
        }

        public IActionResult block_case(int requestid, BlockCaseModal blockCase)
        {
            var user = 1;

            _adminDashRepository.BlockCase(requestid, blockCase, user);

            return RedirectToAction("admin_dash", "AdminDash");
        }

        public IActionResult ClearCase(int requestid)
        {
            var user = 1;

            _adminDashRepository.Clearcase(requestid, user);

            return RedirectToAction("admin_dash", "AdminDash");
        }


        public IActionResult send_order(int requestid, OrdersModal ordersModal)
        {

            _adminDashRepository.SetOrder(ordersModal, requestid, HttpContext.Session.GetInt32("userid") ?? 1);

            return RedirectToAction("admin_dash", "AdminDash");
        }


        public IActionResult send_agreement(SendAgreementModal sendAgreement,int requestid,string patientName)
        {
            var link = "https://localhost:7036/Admin_agreement/agreement?requestid=" + requestid +"&PatientName="+patientName;
            var subject = "Agreement for patiet request";
            var message = "\n \n \n please perform a action on the agreement which are given in link \n \n \n" + link;

            SendMail(subject, message,sendAgreement.email);

            SendSMS(message, sendAgreement.phonenumber);
            var role = HttpContext.Session.GetString("loginType");
            if (role == "provider")
            {
                return RedirectToAction("dashboard", "ProviderDashboard");
            }
            return RedirectToAction("admin_dash", "AdminDash");
        }

        public IActionResult send_link(SendLinkModal sendLink)
        {
            var subject = "Agreement for patiet request";
            var message = "hii" + sendLink.Firstname + " " + sendLink.Lastname + " \n please submit your request";

            SendMail(subject, message,sendLink.Email);

            SendSMS(message, sendLink.Phone);

            return RedirectToAction("admin_dash", "AdminDash");
        }

        

        public List<Physician> GetPhysicians(int select)
        {
            var phy = _adminDashRepository.GetPhysicianList2(select);

            return phy;
        }

        [HttpPost]
        public List<HealthProfessional> GetVendors(int select)
        {
            var list = _adminDashRepository.GetHealthProfessionals(select);
            return list;
        }

        public HealthProfessional GetVendorData(int select)
        {
            var data = _adminDashRepository.GetVendorData(select);
            return data;
        }

        public IActionResult update_closecase(CloseCaseModal closeCase)
        {
            int requestid = HttpContext.Session.GetInt32("reqid")??0;
            _requests.UpdateCloseCase(requestid, closeCase);

            CloseCaseModal closeCase1 = _adminDashRepository.GetCloseCaseModal(requestid).Result;

            return PartialView("_closeCase", closeCase1);
        }

        public IActionResult closeCase()
        {
            int requestid = HttpContext.Session.GetInt32("reqid") ?? 0;
            var doc1 = HttpContext.Session.GetInt32("userid");

            _adminDashRepository.CloseCase(requestid,doc1??0);

            return RedirectToAction("admin_dash", "AdminDash");
        }
        [HttpPost]
        public IActionResult encounter_form(Encounter encounter)
        {
            var reqid = HttpContext.Session.GetInt32("requestId");
            Encounter encounter1 = _adminDashRepository.SetEncounter(reqid??0, encounter);
            return PartialView("_EncounterForm", encounter1);
        }
        public IActionResult finalize_encounter(Encounter encounter)
        {
            var reqid = HttpContext.Session.GetInt32("requestId");
            _adminDashRepository.FinalizeEncounter(reqid??1, encounter);

            return RedirectToAction("dashboard", "ProviderDashboard");
        }
        [HttpPost]
        public string Encounter(PartialViewModal partialView)
        {
            _adminDashRepository.Encouter(partialView.requestid,partialView.callType);
            return "Request is Encounter To " + partialView.callType;
        }

        public void SendMail(string subject,string message,string mailto)
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

        public void SendSMS(string message,long? phone)
        {
            const string accountSid = "ACbab82685c56693b60c76b5f7e372f1fc";
            const string authToken = "d1fc25823b60dfa35f7278ccb354ac04";
            const string twilioPhoneNumber = "+15162999172";

            TwilioClient.Init(accountSid, authToken);

            // Send an SMS
            var message1 = MessageResource.Create(
                body: message,
                from: new Twilio.Types.PhoneNumber(twilioPhoneNumber),
                to: new Twilio.Types.PhoneNumber("+917984752378")
            );
        }

       
    }


}

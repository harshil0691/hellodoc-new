﻿using hellodoc.DbEntity.ViewModels.PopUpModal;
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
using System.IO;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.DotNet.Scaffolding.Shared.Project;
using System;
using Org.BouncyCastle.Ocsp;
using iTextSharp.text.pdf;
using iTextSharp.text;
using Microsoft.AspNetCore;

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
                    return PartialView("_CancelCase", new CancelCaseModel
                    {
                        Requestid = partialView.requestid,
                        PatientName = partialView.patientName,
                    });

                case "AssignCase":
                    return PartialView("_AssignCase", new AssignCaseModal
                    {
                        Regions = _adminDashRepository.GetRegions("", 0),
                        Physicians = _adminDashRepository.GetPhysicianList(),
                        Requestid = partialView.requestid,
                        Modaltype = partialView.patientName,
                    });

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
                        phonenumber = partialView.phonenumber.ToString(),
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
                    ViewBag.physicianid = HttpContext.Session.GetInt32("physiciandashid");
                    return PartialView("_TransferRequest");
                case "SelectMap":
                    return PartialView("_MapModal");
                case "RequestToAdmin":
                    ViewBag.physicianid = HttpContext.Session.GetInt32("physiciandashid");
                    return PartialView("_RequestToAdmin");

                default:
                    return PartialView("_default");
            }
        }

        public IActionResult LoadActionViews(PartialViewModal partialView)
        {
            ViewBag.loginType = HttpContext.Session.GetString("loginType");
            switch (partialView.actionType)
            {
                case "dashboard":
                    return RedirectToAction("LoadPartialDashView", "AdminDash", new { tabId = "dashboard" });

                case "ViewUploads":
                    try
                    {
                        HttpContext.Session.SetInt32("requestid", partialView.requestid);
                        var document = _requests.GetDocuments(partialView.requestid);
                        ViewBag.back = partialView.back;
                        return PartialView("_ViewUploads", document);
                    }
                    catch
                    {
                        TempData["error"] = "Internal Error View Case Is Not Displayed";
                        return RedirectToAction("admin_dash", "AdminDash");
                    }

                case "deleteDoc":

                    _adminDashRepository.DeleteDocument(partialView.requestwisefileid);
                    HttpContext.Session.SetInt32("requestid", partialView.requestid);
                    var document1 = _requests.GetDocuments(partialView.requestid);
                    return PartialView("_ViewUploads", document1);

                case "Orders":
                    try
                    {
                        OrdersModal ordersModal = new OrdersModal();
                        ordersModal.professionName = _adminDashRepository.GetListProfessionTypes();
                        ordersModal.healthProfessionals = _adminDashRepository.GetHealthProfessionals(0);
                        ordersModal.requestid = partialView.requestid;
                        ordersModal.aspid = HttpContext.Session.GetInt32("Aspid");

                        return PartialView("_Orders", ordersModal);
                    }
                    catch
                    {
                        TempData["error"] = "Internal Error To Display View";
                        return RedirectToAction("admin_dash", "AdminDash");
                    }
                    

                case "ViewCase":
                    var reqform = _adminDashRepository.Getpatientdata(partialView.requestid).Result;
                    if(reqform != null)
                    {
                        reqform.bgcolor = partialView.bcolor;
                        reqform.btext = partialView.btext;
                        reqform.regions = _adminDashRepository.GetRegions("",0);
                        ViewBag.back = partialView.back;
                        return PartialView("_ViewCase", reqform);
                    }
                    else
                    {
                        TempData["error"] = "Internal Error";
                        return RedirectToAction("admin_dash", "AdminDash");
                    }

                case "ViewNotes":
                    var user = HttpContext.Session.GetInt32("Aspid");
                    NotesModel notesModel = new NotesModel();
                    try
                    {
                        if (partialView.requestid == 0)
                        {
                            var req = HttpContext.Session.GetInt32("requestid");
                            notesModel = _adminDashRepository.GetNotes(req ?? 1,user??1).Result;
                        }
                        else
                        {
                            HttpContext.Session.SetInt32("requestid", partialView.requestid);
                            notesModel = _adminDashRepository.GetNotes(partialView.requestid,user??1).Result;
                        }

                        return PartialView("_ViewNotes", notesModel);
                    }
                    catch
                    {
                        TempData["error"] = "Internal Error to Load View Notes";
                        return RedirectToAction("admin_dash", "AdminDash");
                    }
                    

                case "CloseCase":
                    HttpContext.Session.SetInt32("reqid", partialView.requestid);
                    CloseCaseModal closeCase = _adminDashRepository.GetCloseCaseModal(partialView.requestid).Result;
                    closeCase.patientName = partialView.patientName;

                    return PartialView("_closeCase", closeCase);

                case "EncounterForm":
                    HttpContext.Session.SetInt32("requestId", partialView.requestid);

                    Encounter encounter = _adminDashRepository.GetEncounter(partialView.requestid);
                    if (encounter.Isfinalized == 1 && HttpContext.Session.GetString("loginType") == "provider")
                    {
                        return Json(new { isfinalized = 1 });
                    }
                    return PartialView("_EncounterForm", encounter);

                case "CreateRequest":
                    RequestFormModal requestForm = new RequestFormModal
                    {
                        regions = _adminDashRepository.GetRegions("", 0)
                    };
                    return PartialView("_CreateRequest",requestForm);
                case "ConcludeCare":
                    Encounter encounter1 = _adminDashRepository.GetEncounter(partialView.requestid);
                    ConcludCare concludCare = new ConcludCare();
                    if (encounter1.Isfinalized == 1)
                    {
                        concludCare.isfinalized = 1;
                    }
                    concludCare.patientDocuments = _requests.GetDocuments(partialView.requestid).patientDocuments;
                    concludCare.PatientName = _requests.GetDocuments(partialView.requestid).Firstname;
                    concludCare.Requestid = partialView.requestid;
                    HttpContext.Session.SetInt32("requestid",partialView.requestid);
                    return PartialView("_ConcludeCare", concludCare);

                case "Encounter":
                    _adminDashRepository.Encouter(partialView.requestid, partialView.callType);
                    return Json(new {data = "Request is Encounter To " + partialView.callType });
                default:
                    return PartialView("_DefaultTab");
            }
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

        public ActionResult download_encounter(int reqid)
        {
            var requestId = HttpContext.Session.GetInt32("requestId") ?? 1;
            var rowData = _adminDashRepository.GetEncounter(requestId);

            if (rowData != null)
            {
                // Create a new PDF document
                Document document = new Document();
                MemoryStream ms = new MemoryStream();
                PdfWriter.GetInstance(document, ms);
                document.Open();

                // Add table for header and data
                PdfPTable mainTable = new PdfPTable(1);
                mainTable.WidthPercentage = 100; // Set table width to 100%

                // Add header row
                PdfPCell headerCell = new PdfPCell(new Phrase("Table Data"));
                headerCell.HorizontalAlignment = Element.ALIGN_CENTER; // Center alignment
                headerCell.Border = PdfPCell.NO_BORDER; // Remove border
                mainTable.AddCell(headerCell);

                // Add data row
                PdfPTable dataTable = new PdfPTable(2); // Assuming 15 columns
                                                        // Add column headers
                foreach (var property in rowData.GetType().GetProperties())
                {
                    PdfPCell cell = new PdfPCell(new Phrase(property.Name));
                    cell.BackgroundColor = BaseColor.LIGHT_GRAY; // Background color for header
                    dataTable.AddCell(cell);
                    dataTable.AddCell(new Phrase(property.GetValue(rowData)?.ToString()));
                    dataTable.CompleteRow();

                }


                // Add data table to main table
                mainTable.AddCell(dataTable);

                // Add the main table to the document
                document.Add(mainTable);

                // Close the document
                document.Close();

                // Convert MemoryStream to byte array
                byte[] bytes = ms.ToArray();
                ms.Close();

                // Return the PDF as a FileResult
                return File(bytes, "application/pdf", "table_data.pdf");
            }
            else
            {
                // Handle the case where no data is found
                return Content("No data found.");
            }
        }


        [HttpPost]
        public IActionResult request_support(RequestSupportModal supportModal)
        {
            var physician = _adminDashRepository.GetUnAssignedPhysician();
            if (physician != null)
            {
                foreach (var p in physician)
                {
                    var subject = "U Note Have Any Shift For Today";

                    SendMail(subject, supportModal.Message,p.Email);
                }
            }
            TempData["success"] = "Mail Sent to All UnAssigned Physician Successfully";
            return RedirectToAction("admin_dash", "AdminDash");
        }

        [HttpPost]
        public IActionResult create_request(RequestFormModal requestForm)
        {
            requestForm.RequestCreatedBy = HttpContext.Session.GetString("loginType");
            if (requestForm.RequestCreatedBy == "provider")
            {
                requestForm.PhysicianId = HttpContext.Session.GetInt32("physiciandashid")??0;
                if (_requests.PatientRequest(requestForm) == "ok")
                {
                    return RedirectToAction("dashboard", "ProviderDashboard", new { type = "error", tempdata = "Request Is Created" });
                }
                else
                {
                    return RedirectToAction("dashboard", "ProviderDashboard", new { type = "error", tempdata = "Request Is Not Created Internal Error" });
                }
            }
            if (_requests.PatientRequest(requestForm) == "ok")
            {
                return RedirectToAction("admin_dash", "AdminDash", new { type = "error", tempdata = "Request Is Created" });
            }
            else
            {
                return RedirectToAction("admin_dash", "AdminDash", new { type = "error", tempdata = "Request Is Not Created Internal Error" });
            }
        }

        public IActionResult AssignCase(int requestid, AssignCaseModal assignCase, string Modaltype)
        {
            var aspnetuser = HttpContext.Session.GetInt32("Aspid");
            try
            {
                assignCase.Modaltype = Modaltype;
                _adminDashRepository.AssignCase(requestid, assignCase, aspnetuser);
                TempData["success"] = "Case Is Assigned SuccessFully";
            }
            catch
            {
                TempData["error"] = "Internal Error Case Is Not Assigned";
            }

            return RedirectToAction("admin_dash", "AdminDash");
        }

        public IActionResult BlockCase(int requestid, BlockCaseModal blockCase)
        {
            var user = HttpContext.Session.GetInt32("Aspid");
            if (_adminDashRepository.BlockCase(requestid, blockCase, user) == true)
            {
                TempData["success"] = "Case Is Bloked";
            }
            else
            {
                TempData["error"] = "Internal Error Case Is Not Blocked";
            }
            
            return RedirectToAction("admin_dash", "AdminDash");
        }

        public IActionResult CancelCase(int requestid, CancelCaseModel cancelCase)
        {
            var user = HttpContext.Session.GetInt32("Aspid");
            try
            {
                _adminDashRepository.CancelRequest(requestid, cancelCase, user??1);
                TempData["success"] = "Case Is Cancelled";
            }
            catch
            {
                TempData["error"] = "Internal Error Case Is Not Cancelled";
            }
            

            return RedirectToAction("admin_dash", "AdminDash");
        }

        public void ViewCaseUpdate(int requestid, RequestFormModal requestForm)
        {
            var user = HttpContext.Session.GetInt32("Aspid");
            if (_adminDashRepository.ViewCaseUpdate(requestid, requestForm, user ?? 1) == true)
            {
                TempData["success"] = "ViewCase Updated";
            }
            else
            {
                TempData["error"] = "Internal Error Case Is Not Assigned";
            }
        }

        [HttpPost]
        public IActionResult DocumentActions(List<int> selectedCheck, int requestid, string actionType)
        {
            switch (actionType)
            {
                case "DeleteAll":
                    try
                    {
                        foreach (int id in selectedCheck)
                        {
                            _adminDashRepository.DeleteDocument(id);
                        }
                        TempData["success"] = "Selected Document Deleted Successfully";
                    }
                    catch
                    {
                        TempData["error"] = "Internal Error Documnet Are Not Deleted";
                        return RedirectToAction("admin_dash", "AdminDash");
                    }
                    try
                    {
                        HttpContext.Session.SetInt32("requestid", requestid);
                        var document1 = _requests.GetDocuments(requestid);
                        return PartialView("_ViewUploads", document1);
                    }
                    catch
                    {
                        TempData["error"] = "Documnet Are Deleted But Not Able To Load View Load Page";
                        return RedirectToAction("admin_dash", "AdminDash");
                    }
                    

                case "SendEmail":
                    try
                    {
                        var list = _adminDashRepository.GetListFilename(selectedCheck);
                        sendEmail("hpdhaduk0605@gmail.com", "your uploads", "document", list);
                        var document = _requests.GetDocuments(requestid);
                        TempData["success"] = "Documnet Are Successfully Sent To Email";
                        return PartialView("_ViewUploads", document);
                    }
                    catch
                    {
                        TempData["error"] = "Documnet Are Not Sent To Email";
                        return RedirectToAction("admin_dash", "AdminDash");
                    }
                    


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
        public IActionResult SaveNote(NotesModel note)
        {
            try
            {
                var req = HttpContext.Session.GetInt32("requestid");
                var user = HttpContext.Session.GetString("username");
                var aspnetuser = HttpContext.Session.GetInt32("Aspid");
                _adminDashRepository.SetNotes(note, req, user,HttpContext.Session.GetString("loginType")??"admin");

                NotesModel notesModel = new NotesModel();
                notesModel = _adminDashRepository.GetNotes(req ?? 1, aspnetuser ?? 1).Result;
                TempData["success"] = "Notes Saved Successfully";
                return PartialView("_ViewNotes", notesModel);
            }
            catch
            {

                var req = HttpContext.Session.GetInt32("requestid");
                var aspnetuser = HttpContext.Session.GetInt32("Aspid");
                NotesModel notesModel = new NotesModel();
                notesModel = _adminDashRepository.GetNotes(req ?? 1, aspnetuser ?? 1).Result;
                TempData["error"] = "Notes Not Saved";
                return PartialView("_ViewNotes", notesModel);
                
            }
            
        }

        [HttpPost]
        public IActionResult view_uploads(IFormFile file1)
        {
            var req = HttpContext.Session.GetInt32("requestid");

            _requests.SaveFile(file1,req??0);

            var document = _requests.GetDocuments(req ?? 1);
            TempData["success"] = "Document Uploaded Successfully";
            return PartialView("_ViewUploads", document);
        }

        
        public IActionResult ClearCase(int requestid)
        {
            try
            {
                var user = HttpContext.Session.GetInt32("Aspid");
                _adminDashRepository.Clearcase(requestid, user);
                TempData["success"] = "Case Is Clear Now This Request In To Close";
            }
            catch
            {
                TempData["error"] = "Internal Error Case Is Not Cleared";
            }
            return RedirectToAction("admin_dash", "AdminDash");
        }


        public IActionResult send_order(int requestid, OrdersModal ordersModal)
        {

            _adminDashRepository.SetOrder(ordersModal, requestid, HttpContext.Session.GetInt32("userid") ?? 1);
            TempData["success"] = "Send Order Successfully";
            return RedirectToAction("admin_dash", "AdminDash");
        }


        public IActionResult send_agreement(SendAgreementModal sendAgreement,int requestid,string patientName)
        {
            var link = "https://localhost:7036/Admin_agreement/agreement?requestid=" + requestid +"&PatientName="+patientName;
            var subject = "Agreement for patiet request";
            var message = "\n \n \n please perform a action on the agreement which are given in link \n \n \n" + link;

            SendMail(subject, message,sendAgreement.email);

            SendSMS(message, long.Parse(sendAgreement.phonenumber));
            var role = HttpContext.Session.GetString("loginType");
            if (role == "provider")
            {
                TempData["success"] = "Aggrement Sent Successfully ";
                return RedirectToAction("dashboard", "ProviderDashboard");
            }
            TempData["success"] = "Aggrement Sent Successfully ";
            return RedirectToAction("admin_dash", "AdminDash");
        }

        [HttpPost]
        public IActionResult send_link(SendLinkModal sendLink)
        {
            var subject = "Create Your Request";
            var message = "hii" + sendLink.Firstname + " " + sendLink.Lastname + " \n please submit your request link given below \n https://localhost:7036/Patient/request";

            SendMail(subject, message,sendLink.MailEmail);

            var Message = SendSMS(message, sendLink.Phone);
            TempData["success"] = Message;
            return RedirectToAction("admin_dash", "AdminDash");
        }

        

        public List<Physician> GetPhysicians(int select)
        {
            var phy = _adminDashRepository.GetPhysicianList2(select);

            return phy;
        }

       
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
            var aspnetuserid = HttpContext.Session.GetInt32("Aspid");

            _adminDashRepository.CloseCase(requestid,aspnetuserid??0);
            TempData["success"] = "Request Case is Closed";
            return RedirectToAction("admin_dash", "AdminDash");
        }
        [HttpPost]
        public IActionResult encounter_form(Encounter encounter)
        {
            var reqid = HttpContext.Session.GetInt32("requestId");
            Encounter encounter1 = _adminDashRepository.SetEncounter(reqid??0, encounter);
            return PartialView("_EncounterForm", encounter1);
        }

        [HttpPost]
        public IActionResult Encounter(PartialViewModal partialView)
        {
            _adminDashRepository.Encouter(partialView.requestid, partialView.callType);
            return Json(new { data = "Request is Encounter To " + partialView.callType });
        }

        [HttpPost]
        public IActionResult finalize_encounter(Encounter encounter)
        {
            var reqid = HttpContext.Session.GetInt32("requestId");
            _adminDashRepository.FinalizeEncounter(reqid??1, encounter);
            if (HttpContext.Session.GetString("loginType") == "provider")
            {
                return RedirectToAction("dashboard", "ProviderDashboard");
            }
            else
            {
                return RedirectToAction("admin_dash", "AdminDash");
            }

           
        }

        public IActionResult ConcludeCare()
        {
            var requestid = HttpContext.Session.GetInt32("requestid");
            _adminDashRepository.ConcludeCase(requestid??0,HttpContext.Session.GetInt32("physiciandashid")??0);
            TempData["success"] = "Case Transfer To Closed Successfully";
            return RedirectToAction("dashboard", "ProviderDashboard");
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

        public string SendSMS(string message,long? phone)
        {
            const string accountSid = "ACbab82685c56693b60c76b5f7e372f1fc";
            const string authToken = "d1fc25823b60dfa35f7278ccb354ac04";
            const string twilioPhoneNumber = "+15162999172";

            TwilioClient.Init(accountSid, authToken);

            // Send an SMS
            try
            {
                var message1 = MessageResource.Create(
               body: message,
               from: new Twilio.Types.PhoneNumber(twilioPhoneNumber),
               to: new Twilio.Types.PhoneNumber("+917984752378")
               );
                return "Message Sent SuccessFully";
            }
            catch(Exception ex)
            {
                return "Message Not Sent" + ex.Message;
            }
           
        }

       
    }


}

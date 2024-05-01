
using Microsoft.AspNetCore.Mvc;
using hellodoc.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using hellodoc.Repositories.Repository.Interface;
using hellodoc.DbEntity.ViewModels.PopUpModal;
using System.Web;
using hellodoc.Repositories.Repository;

namespace hellodoc.Controllers
{
    public class Admin_agreementController : Controller
    {
        private readonly ILogger<Admin_agreementController> _logger;
        private readonly IAdminDashRepository adminDashRepository;
        private readonly IPatientDashboard patientDashboard;


        public Admin_agreementController(ILogger<Admin_agreementController> logger,IAdminDashRepository adminDash, IPatientDashboard patientDashboard)
        {
            _logger = logger;
            adminDashRepository = adminDash;
            this.patientDashboard = patientDashboard;
        }

        public IActionResult agreement()
        {
            var requestid = HttpContext.Request.Query["requestid"];

            ViewBag.requestid = HttpContext.Request.Query["requestid"];
            ViewBag.patientName = HttpContext.Request.Query["PatientName"];

            var status = patientDashboard.GetRequestStatus(int.Parse(requestid)).Result;

            if(status == 2)
            {
                return View();
            }
            else
            {
                TempData["AggrementApproved"] = "Your aggrement already approved";
                return RedirectToAction("login", "Login",new {loginType  = "user"});
            }
            
        }
        public IActionResult agree_agreement(int reqid)
        {
            string ip = HttpContext.Connection.RemoteIpAddress.ToString();

            patientDashboard.AgreeAgreement(reqid,ip);
            return RedirectToAction("login", "Login", new { loginType = "user" });
        }

        [HttpPost]
        public IActionResult cancel_agreement(int reqid,SendAgreementModal sendAgreement)
        {
            string ip = HttpContext.Connection.RemoteIpAddress.ToString();
            sendAgreement.reqid = reqid;
            patientDashboard.CancelAgreement(sendAgreement,ip);
            return RedirectToAction("login", "Login", new { loginType = "user" });
        }
    }
}

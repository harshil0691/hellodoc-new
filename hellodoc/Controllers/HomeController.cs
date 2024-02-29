using hellodoc.DbEntity.DataModels;
using hellodoc.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using MimeKit;
using System.Net;
using hellodoc.Repositories.Repository.Interface;

namespace hellodoc.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IPatientLogin _patientLogin;
        private readonly IRequests _requests;
        public HomeController(ILogger<HomeController> logger,IPatientLogin patientLogin, IRequests requests)
        {
            _logger = logger;
            _patientLogin = patientLogin;
            _requests = requests;
        }

        //get
        public IActionResult Index()
        {
            return View();
        }

        //post 

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(AspNetUser obj)
        {
            var aspnetuser = _patientLogin.GetAspNetUser(obj.Username,obj.Passwordhash);

            
            
            if (ModelState.IsValid) { 
            if (aspnetuser.Result != null)
            {
                    var userId = _requests.GetUser(aspnetuser.Result.Id);
                    TempData["success"] = "User LogIn Successfully";
                    HttpContext.Session.SetInt32("userid", userId.Result);
                    HttpContext.Session.SetString("username", aspnetuser.Result.Username);
                    

                return RedirectToAction("patient_dashboard", "Patient");

            }
            else
            {

                TempData["error"] = "Username or Password is Incorrect";
                ModelState.AddModelError(string.Empty, "Invalid email or Password");
                return View("index");
            }
            }
            return View();
        }

        
        public  IActionResult forgot_password()
        {
            
            return View();
        }

        [HttpPost]
        public IActionResult send_mail(AspNetUser aspNet)
        {
                sendEmail(aspNet.Email, "hello" , "hello reset password");
                return RedirectToAction("send_request", "Patient");
        }

        public Task sendEmail(string email, string subject, string message)
        {
            var mail = "pdhaduk300@gmail.com";
            var password = "oqkl bgzk vloe ejrt";

            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(mail, password)
            };

            return client.SendMailAsync(new MailMessage(from: mail, to: email, subject, message));

        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
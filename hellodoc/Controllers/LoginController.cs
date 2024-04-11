using hellodoc.DbEntity.DataModels;
using hellodoc.Repositories.Repository.Interface;
using hellodoc.Repositories.Services.Interface;
using hellodoc.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using System.Net;

namespace hellodoc.Controllers
{
    public class LoginController : Controller
    {
        private readonly ILogger<AdminDashController> _logger;
        private readonly IAdminDashRepository _adminDashRepository;
        private readonly IPatientLogin _patientLogin;
        private readonly IAuthManager _authManger;
        private readonly IRequests _requests;
        private readonly IJwtServices _jwtServices;

        public LoginController(ILogger<AdminDashController> logger, IAdminDashRepository adminDashRepository, IPatientLogin patientLogin, IAuthManager authManager, IRequests requests,IJwtServices jwtServices)
        {
            _logger = logger;
            _adminDashRepository = adminDashRepository;
            _patientLogin = patientLogin;
            _authManger = authManager;
            _requests = requests;
            _jwtServices = jwtServices;
        }


        public IActionResult login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult login(AspNetUser obj)
        {

            //var aspnetuser = _patientLogin.GetAspNetUser(obj.Username, obj.Passwordhash);

            var  aspnetuser = _authManger.Login(obj.Email, obj.Passwordhash);

                if (aspnetuser != null)
                {
                    TempData["success"] = "User LogIn Successfully";
                    HttpContext.Session.SetInt32("Aspid",aspnetuser.Id);

                    SessionUtils.SetLoggedInUser(HttpContext.Session, aspnetuser);

                    var jwttoken = _jwtServices.GenarateJwtToken(aspnetuser);
                    Response.Cookies.Append("jwt", jwttoken);
                    

                    return RedirectToAction("admin_dash", "AdminDash",aspnetuser.AspNetUserRole.Role.Name);

                }
                else
                {

                    TempData["error"] = "Username or Password is Incorrect";
                    ModelState.AddModelError(string.Empty, "Invalid email or Password");
                    return View();
                }
        }

        public IActionResult PatientLogin(AspNetUser obj)
        {

            var aspnetuser = _authManger.Login(obj.Email, obj.Passwordhash);

            if (aspnetuser != null)
            {
                TempData["success"] = "User LogIn Successfully";
                HttpContext.Session.SetInt32("Aspid", aspnetuser.Id);

                SessionUtils.SetLoggedInUser(HttpContext.Session, aspnetuser);

                var jwttoken = _jwtServices.GenarateJwtToken(aspnetuser);
                Response.Cookies.Append("jwt", jwttoken);


                return RedirectToAction("patient_dashboard", "Patient");

            }
            else
            {

                TempData["error"] = "Username or Password is Incorrect";
                ModelState.AddModelError(string.Empty, "Invalid email or Password");
                return View();
            }
        }


        public IActionResult Logout()
        {
            Response.Cookies.Delete("jwt");
            return RedirectToAction("Login", "login");
        }

        public IActionResult Index()
        {
            return View();
        }

        //post 

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(AspNetUser obj)
        {
            var aspnetuser = _authManger.Login(obj.Email, obj.Passwordhash);

            if (aspnetuser != null)
                {
                    var userId = _requests.GetUser(aspnetuser.Id).Result;
                    TempData["success"] = "User LogIn Successfully";
                    HttpContext.Session.SetInt32("userId", userId);
                    HttpContext.Session.SetString("username", aspnetuser.Username);

                SessionUtils.SetLoggedInUser(HttpContext.Session, aspnetuser);

                var jwttoken = _jwtServices.GenarateJwtToken(aspnetuser);
                Response.Cookies.Append("jwt", jwttoken);


                return RedirectToAction("patient_dashboard", "Patient");

                }
                else
                {

                    TempData["error"] = "Username or Password is Incorrect";
                    ModelState.AddModelError(string.Empty, "Invalid email or Password");
                    return View();
                }
        }


        public IActionResult forgot_password()
        {

            return View();
        }

        [HttpPost]
        public IActionResult send_mail(AspNetUser aspNet)
        {
            sendEmail(aspNet.Email, "hello", "hello reset password");
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
    }
}

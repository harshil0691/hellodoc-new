﻿using hellodoc.DbEntity.DataModels;
using hellodoc.Repositories.Repository.Interface;
using hellodoc.Repositories.Services.Interface;
using hellodoc.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using hellodoc.DbEntity.ViewModels;

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

        public LoginController(ILogger<AdminDashController> logger, IAdminDashRepository adminDashRepository, IPatientLogin patientLogin, IAuthManager authManager, IRequests requests, IJwtServices jwtServices)
        {
            _logger = logger;
            _adminDashRepository = adminDashRepository;
            _patientLogin = patientLogin;
            _authManger = authManager;
            _requests = requests;
            _jwtServices = jwtServices;
        }


        public IActionResult login(string loginType)
        {
            if (loginType != null)
            {
                HttpContext.Session.SetString("loginType", loginType);
            }
            ViewBag.loginType = loginType;
            return View();
        }

        [HttpPost]
        public IActionResult login(AspNetUser obj)
        {
            var loginType = HttpContext.Session.GetString("loginType");
            var aspnetuser = _authManger.Login(obj.Email, obj.Passwordhash);

            if (loginType == "provider" && aspnetuser != null)
            {
                HttpContext.Session.SetString("username", aspnetuser.Username);
                HttpContext.Session.SetInt32("PhysicianAspid", aspnetuser.Id);
                HttpContext.Session.SetInt32("Aspid", aspnetuser.Id);
                try
                {
                    var physicianid = _authManger.GetPhysician(aspnetuser.Id);
                    if (physicianid != 0)
                    {
                        TempData["success"] = "User LogIn Successfully";
                        HttpContext.Session.SetInt32("physiciandashid", physicianid);
                    }
                }
                catch
                {
                    return RedirectToAction("login", "Login");
                }


                var jwttoken = _jwtServices.GenarateJwtToken(aspnetuser);
                Response.Cookies.Append("jwt", jwttoken);
                Response.Cookies.Append("Aspid", aspnetuser.Id.ToString(), new CookieOptions { Expires = DateTime.Now.AddDays(1) });
                Response.Cookies.Append("username", aspnetuser.Username);

                return RedirectToAction("dashboard", "ProviderDashboard", aspnetuser.AspNetUserRole.Role.Name);
            }
            else if (aspnetuser != null && loginType == "admin")
            {
                HttpContext.Session.SetString("username", aspnetuser.Username);
                TempData["success"] = "User LogIn Successfully";
                HttpContext.Session.SetInt32("Aspid", aspnetuser.Id);

                //SessionUtils.SetLoggedInUser(HttpContext.Session, aspnetuser);
                var jwttoken = _jwtServices.GenarateJwtToken(aspnetuser);
                Response.Cookies.Append("jwt", jwttoken);
                Response.Cookies.Append("Aspid", aspnetuser.Id.ToString(), new CookieOptions { Expires = DateTime.Now.AddDays(1) });
                Response.Cookies.Append("username", aspnetuser.Username);

                return RedirectToAction("admin_dash", "AdminDash", aspnetuser.AspNetUserRole.Role.Name);
            }
            else
            {
                if(aspnetuser == null)
                {
                    TempData["error"] = "Incorrect Password Or Email";
                    return RedirectToAction("login", "Login",new {loginType = "user"});
                    
                }
                else
                {
                    HttpContext.Session.SetString("username", aspnetuser.Username ?? "new user");
                    HttpContext.Session.SetInt32("Aspid", aspnetuser.Id);
                    TempData["success"] = "User LogIn Successfully";


                    //SessionUtils.SetLoggedInUser(HttpContext.Session, aspnetuser);

                    var jwttoken = _jwtServices.GenarateJwtToken(aspnetuser);
                    Response.Cookies.Append("jwt", jwttoken);
                    Response.Cookies.Append("jwt", jwttoken);
                    Response.Cookies.Append("Aspid", aspnetuser.Id.ToString(), new CookieOptions { Expires = DateTime.Now.AddDays(1) });
                    Response.Cookies.Append("username", aspnetuser.Username);

                    return RedirectToAction("patient_dashboard", "Patient");
                }
            }

        }

        public IActionResult Logout()
        {
            var login = HttpContext.Session.GetString("loginType");
            Response.Cookies.Delete("jwt");
            TempData["success"] = "Logout Successfully";
            return RedirectToAction("Login", "login", new { loginType = login });

        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult AccessDenied()
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

                //SessionUtils.SetLoggedInUser(HttpContext.Session, aspnetuser);

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
            sendEmail(aspNet.Email, "hello", "reset password link given below \n https://localhost:7036/Login/ResetPassword");
            return RedirectToAction("send_request", "Patient");
        }

        public IActionResult ResetPassword()
        {
            return View();
        }


        [HttpPost]
        public IActionResult ResetPassword(RequestFormModal requestForm)
        {
            if(_requests.ResetPassword(requestForm) != "error")
            {
                TempData["success"] = "Password Reset Successfully";
            }
            else
            {
                TempData["error"] = "User Not Exists";
            }
            
            return RedirectToAction("login", "Login", new { loginType = "user" });
        }

        public Task sendEmail(string email, string subject, string message)
        {
            var mail = "tatva.dotnet.harshildhaduk@outlook.com";
            var password = "harshil@9184";

            var client = new SmtpClient("smtp.office365.com", 587)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(mail, password)
            };

            return client.SendMailAsync(new MailMessage(from: mail, to: email, subject, message));
        }
    }
}

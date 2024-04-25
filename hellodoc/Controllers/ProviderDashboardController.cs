﻿using hellodoc.DbEntity.ViewModels;
using hellodoc.Repositories.Repository;
using hellodoc.Repositories.Repository.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace hellodoc.Controllers
{
    [CustomUserAuthorize("provider")]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public class ProviderDashboardController : Controller
    {
        private readonly ILogger<AdminDashController> _logger;
        private readonly IAdminDashRepository _adminDashRepository;
        private readonly IPatientLogin _patientLogin;
        private readonly IRequests _requests;
        private readonly IHostingEnvironment HostingEnviroment;
        private readonly IAuthManager _authManger;
        private readonly IAdminProviders _adminProviders;
        private readonly IAdminAccess _adminAccess;
        private readonly IAdminProviderLocation _providerLocation;
        private readonly IAdminRecords _adminRecords;
        private readonly IProviderRepo _providerRepo;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public ProviderDashboardController(ILogger<AdminDashController> logger, IAdminDashRepository adminDashRepository, IPatientLogin patientLogin, IRequests requests, IHostingEnvironment hostingEnvironment, IAuthManager authManager, IAdminProviders adminProviders, IAdminAccess adminAccess, IAdminProviderLocation providerLocation, IAdminRecords adminRecords,IProviderRepo providerRepo, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _adminDashRepository = adminDashRepository;
            _patientLogin = patientLogin;
            _requests = requests;
            HostingEnviroment = hostingEnvironment;
            _authManger = authManager;
            _adminProviders = adminProviders;
            _adminAccess = adminAccess;
            _providerLocation = providerLocation;
            _adminRecords = adminRecords;
            _providerRepo = providerRepo;
            _httpContextAccessor = httpContextAccessor;
        }

        public IActionResult dashboard()
        {
            var claims = JwtHelper.GetClaimsFromCookie(_httpContextAccessor.HttpContext, "jwt");

            if (claims != null)
            {
                if (claims.ContainsKey("username"))
                {
                    string userName = claims["username"];

                    HttpContext.Session.SetString("username", userName);
                }
                if (claims.ContainsKey("Aspid"))
                {
                    string userId = claims["Aspid"];
                    HttpContext.Session.SetInt32("Aspid", int.Parse(userId));
                }
                if (claims.ContainsKey("physicianId"))
                {
                    string userId = claims["physicianId"];
                }
            }

            return View();
        }

        public IActionResult LoadPartialDashView(string tabId)
        {
            var physicianid = HttpContext.Session.GetInt32("physiciandashid");
            switch (tabId)
            {
                case "dashboard":
                    RequestCountByStatus request = _adminDashRepository.GetCount("provider",physicianid??0).Result;
                    request.regions = _adminDashRepository.GetRegions("", 0);
                    request.activeid = HttpContext.Session.GetInt32("activeid") ?? 1;

                    return PartialView("_dashboard", request);

                case "invoicing":
                    return RedirectToAction("GetProvidersView", "AdminProviders", new { actionType = "invoicing" });

                case "myprofile":
                    return RedirectToAction("edit_physician", "AdminProviders", new { physicianid = HttpContext.Session.GetInt32("physiciandashid") ?? 1 });

                case "scheduling":
                    return RedirectToAction("GetProvidersView", "AdminProviders", new { actionType = "scheduling" });


                default:
                    return PartialView("_DefaultTab");

            }

        }

        [HttpPost]
        public string AcceptRequest(PartialViewModal partialView)
        {
            if (_providerRepo.AcceptRequest(partialView.requestid) == "ok")
            {
                return "ok";
            }
            return "internal error";
            
        }

        public string RequestToAdmin(PartialViewModal partialView)
        {
            partialView.physicianid = HttpContext.Session.GetInt32("physiciandashid")??0;
            if (_providerRepo.RequestToAdmin(partialView) == "ok")
            {
                return "ok";
            }
            return "internal error";
        }
    }
}

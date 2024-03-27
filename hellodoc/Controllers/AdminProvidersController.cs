using hellodoc.DbEntity.ViewModels;
using hellodoc.Repositories.Repository.Interface;
using Microsoft.AspNetCore.Mvc;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace hellodoc.Controllers
{
    public class AdminProvidersController : Controller
    {
        private readonly ILogger<AdminDashController> _logger;
        private readonly IAdminDashRepository _adminDashRepository;
        private readonly IPatientLogin _patientLogin;
        private readonly IRequests _requests;
        private readonly IHostingEnvironment HostingEnviroment;
        private readonly IAuthManager _authManger;
        private readonly IAdminProviders _adminProviders;

        public AdminProvidersController(ILogger<AdminDashController> logger, IAdminDashRepository adminDashRepository, IPatientLogin patientLogin, IRequests requests, IHostingEnvironment hostingEnvironment, IAuthManager authManager, IAdminProviders adminProviders)
        {
            _logger = logger;
            _adminDashRepository = adminDashRepository;
            _patientLogin = patientLogin;
            _requests = requests;
            HostingEnviroment = hostingEnvironment;
            _authManger = authManager;
            _adminProviders = adminProviders;
        }

        [HttpPost]
        public void stopnotification(List<int> idlist,List<int> totallist)
        {
            _adminProviders.StopNotification(idlist, totallist);
        }

        [HttpPost]
        public IActionResult edit_physician(int physicianid)
        {
            var userid1 = HttpContext.Session.GetInt32("Aspid");

            ProviderProfileModal providerProfile = _adminProviders.ProviderProfileData(physicianid).Result;

            return PartialView("_ProviderProfile", providerProfile);
        }

        [HttpPost]
        public IActionResult loadshift()
        {
            return PartialView("_MonthWiseShift");
        }
    }
}

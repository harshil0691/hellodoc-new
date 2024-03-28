using hellodoc.DbEntity.ViewModels;
using hellodoc.DbEntity.ViewModels.Shifts;
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
        public IActionResult loadshift(int year1 , int month1,string shifttype)
        {
            switch (shifttype)
            {
                case "month":
                    var currentDate = DateTime.Today;
                    
                    MonthShiftModal monthShift = new MonthShiftModal();

                    monthShift.daysInMonth = DateTime.DaysInMonth(year1, month1);

                    var firstDayOfMonth = new DateTime(year1, month1, 1);
                    var startDayIndex = (int)firstDayOfMonth.DayOfWeek;

                    monthShift.firstDayOfMonth = firstDayOfMonth;
                    monthShift.startDayIndex = startDayIndex;
                    monthShift.dayNames = new string[] { "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" };
                    monthShift.shiftDetailsmodals = _adminProviders.ShiftDetailsmodal(year1, month1);

                    return PartialView("_MonthWiseShift", monthShift);

                case "week":

                    var currentDate1 = DateTime.Today;

                    MonthShiftModal monthShift1 = new MonthShiftModal();

                    monthShift1.daysInMonth = DateTime.DaysInMonth(year1, month1);

                    var firstDayOfMonth1 = new DateTime(year1, month1, 1);
                    var startDayIndex1 = (int)firstDayOfMonth1.DayOfWeek;

                    monthShift1.firstDayOfMonth = firstDayOfMonth1;
                    monthShift1.startDayIndex = startDayIndex1;
                    monthShift1.dayNames = new string[] { "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" };
                    monthShift1.shiftDetailsmodals = _adminProviders.ShiftDetailsmodal(year1, month1);

                    return PartialView("_MonthWiseShift", monthShift1);

                case "day":

                    var dayshift = _adminProviders.DayShift(2024, 3, 1);

                    return PartialView("_DayWiseShift",dayshift);

                default:
                    return PartialView("_default");
            }
            
        }

        public IActionResult OpenModal(PartialViewModal partialView)
        {
            switch(partialView.actionType)
            {
                case "shiftdetails":
                    ShiftDetailsmodal shift = _adminProviders.GetShift(partialView.shiftdetailsid);
                    return PartialView("_ViewShift",shift);

                default:
                    return PartialView("_default");
            }
        }
    }
}

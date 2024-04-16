using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Calendar.v3;
using Google.Apis.Services;
using hellodoc.DbEntity.ViewModels;
using hellodoc.DbEntity.ViewModels.Shifts;
using hellodoc.Repositories.Repository.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
using NUnit.Framework.Internal.Execution;
using hellodoc.DbEntity.ViewModels.DashboardLists;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.AspNetCore.Http;
using hellodoc.DbEntity.DataModels;
using Microsoft.AspNetCore.Http.HttpResults;

namespace hellodoc.Controllers
{
    public class AdminAccessController : Controller
    {
        private readonly ILogger<AdminAccessController> _logger;
        private readonly IAdminDashRepository _adminDashRepository;
        private readonly IPatientLogin _patientLogin;
        private readonly IRequests _requests;
        private readonly IHostingEnvironment HostingEnviroment;
        private readonly IAuthManager _authManger;
        private readonly IAdminProviders _adminProviders;
        private readonly IAdminAccess _adminAccess;

        public AdminAccessController(ILogger<AdminAccessController> logger, IAdminDashRepository adminDashRepository, IPatientLogin patientLogin, IRequests requests, IHostingEnvironment hostingEnvironment, IAuthManager authManager, IAdminProviders adminProviders,IAdminAccess adminAccess)
        {
            _logger = logger;
            _adminDashRepository = adminDashRepository;
            _patientLogin = patientLogin;
            _requests = requests;
            HostingEnviroment = hostingEnvironment;
            _authManger = authManager;
            _adminProviders = adminProviders;
            _adminAccess = adminAccess;
        }

        public IActionResult GetaccessView(PartialViewModal partialView)
        {
            var date = DateTime.Now;
            if (partialView.datestring != null)
            {
                date = DateTime.Parse(partialView.datestring);
            }

            switch (partialView.actionType)
            {
                case "accountAccess":
                    return PartialView("_Access", _adminAccess.AccountAccessData(partialView.pageNumber,"access"));
                case "userAccess":
                    return PartialView("_UserAccess", _adminAccess.AccountAccessData(partialView.pageNumber, "userAccess"));
                case "createAdmin":
                    return PartialView("_CreateAdmin", _adminAccess.GetForCreateAdmin());
                case "createRole":
                    return PartialView("_CreateRole", _adminAccess.CreateRole(partialView.accounttype,0));
                case "editRole":
                    return PartialView("_EditRole", _adminAccess.CreateRole(0, partialView.roleid));
                default:
                    return PartialView("_default");
            }
        }

        public IActionResult CRUDAccess(PartialViewModal partialView,AdminProfileModal adminProfile)
        {
            switch (partialView.actionType)
            {
                case "createAdmin":
                    return Json(new { type = "success", message = _adminAccess.CreateAdmin(adminProfile) });

                default:
                    return PartialView("_default");
            }
        }
    }
}

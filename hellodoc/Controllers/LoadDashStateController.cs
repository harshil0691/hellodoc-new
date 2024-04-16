using hellodoc.DbEntity.ViewModels;
using hellodoc.Repositories.Repository;
using hellodoc.Repositories.Repository.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using OfficeOpenXml;
using IHostingEnvironment = Microsoft.Extensions.Hosting.IHostingEnvironment;

namespace hellodoc.Controllers
{
    [CustomUserAuthorize("admin", "provider")]
    public class LoadDashStateController : Controller
    {
        private readonly ILogger<AdminDashController> _logger;
        private readonly IAdminDashRepository _adminDashRepository;

        public LoadDashStateController(ILogger<AdminDashController> logger, IAdminDashRepository adminDashRepository)
        {
            _logger = logger;
            _adminDashRepository = adminDashRepository;
        }

        //[CustomAuthorize("newtab")]
        public IActionResult newtab(PartialViewModal partialView)
        {
            partialView.status = new List<int> { 1 };
            partialView.physicianid = HttpContext.Session.GetInt32("physicianid") ?? 0;
            var dashModel = _adminDashRepository.GetRequests(partialView);
            dashModel.RequestStatus = partialView.tabId;
            if (partialView.provider == true)
            {
                return PartialView("_ProviderTable", dashModel);
            }
            return PartialView("_NewAdmin", dashModel);
        }

        public IActionResult pending(PartialViewModal partialView)
        {
            partialView.status = new List<int> { 2 };
            partialView.physicianid = HttpContext.Session.GetInt32("physicianid") ?? 0;
            var dashModel = _adminDashRepository.GetRequests(partialView);
            dashModel.RequestStatus = partialView.tabId;
            if (partialView.provider == true)
            {
                return PartialView("_ProviderTable", dashModel);
            }
            return PartialView("_PendingAdmin", dashModel);
        }

        public IActionResult active(PartialViewModal partialView)
        {
            partialView.status = new List<int> { 4, 5 };
            partialView.physicianid = HttpContext.Session.GetInt32("physicianid") ?? 0;
            var dashModel = _adminDashRepository.GetRequests(partialView);
            dashModel.RequestStatus = partialView.tabId;
            if (partialView.provider == true)
            {
                return PartialView("_ProviderTable", dashModel);
            }
            return PartialView("_ActiveAdmin", dashModel);
        }

        public IActionResult conclude(PartialViewModal partialView)
        {
            partialView.status = new List<int> { 6 };
            partialView.physicianid = HttpContext.Session.GetInt32("physicianid") ?? 0;
            var dashModel = _adminDashRepository.GetRequests(partialView);
            dashModel.RequestStatus = partialView.tabId;
            if (partialView.provider == true)
            {
                return PartialView("_ProviderTable", dashModel);
            }
            return PartialView("_ConcludeAdmin", dashModel);
        }

        public IActionResult toclose(PartialViewModal partialView)
        {
            partialView.status = new List<int> { 3, 7, 8 };

            var dashModel = _adminDashRepository.GetRequests(partialView);
            return PartialView("_ToCloseAdmin", dashModel);
        }

        public IActionResult unpaid(PartialViewModal partialView)
        {
            partialView.status = new List<int> { 9 };

            var dashModel = _adminDashRepository.GetRequests(partialView);
            return PartialView("_UnpaidAdmin", dashModel);
        }


    }
}

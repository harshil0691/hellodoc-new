using hellodoc.DbEntity.ViewModels;
using hellodoc.Repositories.Repository.Interface;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using IHostingEnvironment = Microsoft.Extensions.Hosting.IHostingEnvironment;

namespace hellodoc.Controllers
{
    public class LoadDashStateController : Controller
    {
        private readonly ILogger<AdminDashController> _logger;
        private readonly IAdminDashRepository _adminDashRepository;

        public LoadDashStateController(ILogger<AdminDashController> logger, IAdminDashRepository adminDashRepository)
        {
            _logger = logger;
            _adminDashRepository = adminDashRepository;
        }

        public IActionResult LoadPartialView(PartialViewModal partialView)
        {
            switch (partialView.tabId)
            {
                case "new":
                    partialView.status = new List<int> { 1 };
                    partialView.physicianid = HttpContext.Session.GetInt32("physicianid") ?? 0;
                    var dashModel1 = _adminDashRepository.GetRequests(partialView);
                    dashModel1.RequestStatus = partialView.tabId;
                    if(partialView.provider == true)
                    {
                        return PartialView("_ProviderTable",dashModel1);
                    }
                    return PartialView("_NewAdmin", dashModel1);

                case "pending":
                    partialView.status = new List<int> { 2 };
                    partialView.physicianid = HttpContext.Session.GetInt32("physicianid") ?? 0;
                    var dashModel2 = _adminDashRepository.GetRequests(partialView);
                    dashModel2.RequestStatus = partialView.tabId;
                    if (partialView.provider == true)
                    {
                        return PartialView("_ProviderTable", dashModel2);
                    }
                    return PartialView("_PendingAdmin", dashModel2);


                case "active":
                    partialView.status = new List<int> { 4, 5 };
                    partialView.physicianid = HttpContext.Session.GetInt32("physicianid") ?? 0;
                    var dashModel3 = _adminDashRepository.GetRequests(partialView);
                    dashModel3.RequestStatus = partialView.tabId;
                    if (partialView.provider == true)
                    {
                        return PartialView("_ProviderTable", dashModel3);
                    }
                    return PartialView("_ActiveAdmin", dashModel3);

                case "conclude":
                    partialView.status = new List<int> { 6 };
                    partialView.physicianid = HttpContext.Session.GetInt32("physicianid") ?? 0;
                    var dashModel4 = _adminDashRepository.GetRequests(partialView);
                    dashModel4.RequestStatus = partialView.tabId;
                    if (partialView.provider == true)
                    {
                        return PartialView("_ProviderTable", dashModel4);
                    }
                    return PartialView("_ConcludeAdmin", dashModel4);

                case "toclose":
                    partialView.status = new List<int> { 3, 7, 8 };

                    var dashModel5 = _adminDashRepository.GetRequests(partialView);
                    return PartialView("_ToCloseAdmin", dashModel5);

                case "unpaid":
                    partialView.status = new List<int> { 9 };

                    var dashModel6 = _adminDashRepository.GetRequests(partialView);
                    return PartialView("_UnpaidAdmin", dashModel6);


                default:
                    return PartialView("_DefaultTab");
            }
        }

    }
}

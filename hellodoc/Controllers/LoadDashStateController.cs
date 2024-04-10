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

        public IActionResult LoadPartialView(string tabId,int pagenumber, string search, int regionid)
        {
            switch (tabId)
            {
                case "new":
                    var status1 = new List<int> { 1 };

                    var dashModel1 = _adminDashRepository.GetRequests(status1,pagenumber,search,regionid,false);
                    return PartialView("_NewAdmin", dashModel1);

                case "pending":
                    var status2 = new List<int> { 2 };

                    var dashModel2 = _adminDashRepository.GetRequests(status2,pagenumber, search, regionid,false);
                    return PartialView("_PendingAdmin", dashModel2);


                case "active":
                    var status3 = new List<int> { 4, 5 };

                    var dashModel3 = _adminDashRepository.GetRequests(status3, pagenumber, search, regionid, false);
                    return PartialView("_ActiveAdmin", dashModel3);

                case "conclude":
                    var status4 = new List<int> { 6 };

                    var dashModel4 = _adminDashRepository.GetRequests(status4,pagenumber, search, regionid,false);
                    return PartialView("_ConcludeAdmin", dashModel4);

                case "toclose":
                    var status5 = new List<int> { 3, 7, 8 };

                    var dashModel5 = _adminDashRepository.GetRequests(status5, pagenumber, search, regionid, false);
                    return PartialView("_ToCloseAdmin", dashModel5);

                case "unpaid":
                    var status6 = new List<int> { 9 };

                    var dashModel6 = _adminDashRepository.GetRequests(status6,pagenumber, search, regionid,false);
                    return PartialView("_UnpaidAdmin", dashModel6);


                default:
                    return PartialView("_DefaultTab");
            }
        }

    }
}

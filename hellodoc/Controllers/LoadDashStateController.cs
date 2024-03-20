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

                    var dashModel1 = _adminDashRepository.GetRequests(status1,pagenumber,search,regionid);
                    HttpContext.Session.SetInt32("activeid", 1);
                    HttpContext.Session.SetInt32("totalrequest", dashModel1.requestcount);
                    return PartialView("_NewAdmin", dashModel1);

                case "pending":
                    var status2 = new List<int> { 2 };

                    var dashModel2 = _adminDashRepository.GetRequests(status2,pagenumber, search, regionid);
                    HttpContext.Session.SetInt32("activeid", 2);
                    HttpContext.Session.SetInt32("totalrequest",dashModel2.requestcount);
                    return PartialView("_PendingAdmin", dashModel2);


                case "active":
                    var status3 = new List<int> { 4, 5 };

                    var dashModel3 = _adminDashRepository.GetRequests(status3, pagenumber, search, regionid);
                    HttpContext.Session.SetInt32("activeid", 3);
                    HttpContext.Session.SetInt32("totalrequest", dashModel3.requestcount);
                    return PartialView("_ActiveAdmin", dashModel3);

                case "conclude":
                    var status4 = new List<int> { 6 };

                    var dashModel4 = _adminDashRepository.GetRequests(status4,pagenumber, search, regionid);
                    HttpContext.Session.SetInt32("activeid", 4);
                    HttpContext.Session.SetInt32("totalrequest", dashModel4.requestcount);
                    return PartialView("_ConcludeAdmin", dashModel4);

                case "toclose":
                    var status5 = new List<int> { 3, 7, 8 };

                    var dashModel5 = _adminDashRepository.GetRequests(status5, pagenumber, search, regionid);
                    HttpContext.Session.SetInt32("activeid", 5);
                    HttpContext.Session.SetInt32("totalrequest", dashModel5.requestcount);
                    return PartialView("_ToCloseAdmin", dashModel5);

                case "unpaid":
                    var status6 = new List<int> { 9 };

                    var dashModel6 = _adminDashRepository.GetRequests(status6,pagenumber, search, regionid);
                    HttpContext.Session.SetInt32("activeid", 6);
                    HttpContext.Session.SetInt32("totalrequest", dashModel6.requestcount);
                    return PartialView("_UnpaidAdmin", dashModel6);


                default:
                    return PartialView("_DefaultTab");
            }
        }

        public IActionResult exportAll()
        {
            var status1 = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

            var dashModel1 = _adminDashRepository.GetRequests(status1,1,"ewe",3);

            var stream = new MemoryStream();
             
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            using (var package = new ExcelPackage(stream))
            {
                var workSheet = package.Workbook.Worksheets.Add("Sheet1");
                workSheet.Cells.LoadFromCollection(dashModel1.adminDashModels, true);
                package.Save();
            }
            stream.Position = 0;
            string excelName = $"UserList-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";

            return File(stream, "application/octet-stream", excelName);
        }

    }
}

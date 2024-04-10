using Microsoft.AspNetCore.Mvc;
using hellodoc.Repositories.Repository.Interface;
using hellodoc.DbEntity.ViewModels;
using hellodoc.Repositories.Repository;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
using hellodoc.DbEntity.ViewModels.DashboardLists;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using System;
using OfficeOpenXml;
using static Org.BouncyCastle.Bcpg.Attr.ImageAttrib;

namespace hellodoc.Controllers

{
    [CustomAuthorize("admin")]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public class AdminDashController : Controller
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

        public AdminDashController(ILogger<AdminDashController> logger,IAdminDashRepository adminDashRepository, IPatientLogin patientLogin, IRequests requests,IHostingEnvironment hostingEnvironment,IAuthManager authManager,IAdminProviders adminProviders,IAdminAccess adminAccess,IAdminProviderLocation providerLocation,IAdminRecords adminRecords)
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
        }

        public IActionResult admin_dash()
        {
                var request = _adminDashRepository.GetCount().Result;
                return View(request);
            
        }

        public IActionResult download(int download)
        {
            var fname1 = _requests.GetFilename(download).Result;

            var filepath = Path.Combine(HostingEnviroment.WebRootPath, "uploads", fname1);
            return File(System.IO.File.ReadAllBytes(filepath), "multipart/form-data", System.IO.Path.GetFileName(filepath));

        }

        public void SaveFile(IFormFile uploadfile, int rid)
        {
            string uniqueFilename = null;

            if (uploadfile != null)
            {
                string uploadfolder = Path.Combine(HostingEnviroment.WebRootPath, "uploads");
                uniqueFilename = Guid.NewGuid().ToString() + "_" + uploadfile.FileName;
                string filename = Path.Combine(uploadfolder, uniqueFilename);
                uploadfile.CopyTo(new FileStream(filename, FileMode.Create));

                _requests.SetRequestWiseFile(uniqueFilename, rid);
            }

        }

        public IActionResult LoadPartialDashView(string tabId)
        {

            switch (tabId)
            {
                case "dashboard":
                    RequestCountByStatus request = _adminDashRepository.GetCount().Result;
                    request.regions = _adminDashRepository.GetRegions();
                    request.activeid = HttpContext.Session.GetInt32("activeid") ?? 1;

                    return PartialView("_dashboard", request);

                case "providerlocation":

                    return PartialView("_ProviderLocation");


                case "myprofile":
                    var userid = HttpContext.Session.GetInt32("Aspid");
                    AdminProfileModal profileModal = _adminDashRepository.GetAdminProfileData(userid ?? 1);
                    return PartialView("_MyProfile", profileModal);

                case "provider":
                    return RedirectToAction("GetProvidersView","AdminProviders", new {actionType = "provider"});

                case "partner":

                    return RedirectToAction("GetView", "AdminPartners",new { actionType = "Partners" });

                case "access":
                    return PartialView("_Access", _adminDashRepository.accessTables());

                case "records":
                    return RedirectToAction("GetView", "AdminRecords",new {actionType = "SearchRecords"});


                default:
                    return PartialView("_DefaultTab");

            }

        }

        [HttpPost]
        public IActionResult update_password(AdminProfileModal admin)
        {
            var aspid = HttpContext.Session.GetInt32("Aspid");

            _adminDashRepository.UpdatePassword(aspid??1, admin.password);

            AdminProfileModal profileModal = _adminDashRepository.GetAdminProfileData(aspid??1);
            return PartialView("_MyProfile", profileModal);
        }

        [HttpPost]
        public IActionResult update_admin(AdminProfileModal profile)
        {
            var aspid = HttpContext.Session.GetInt32("Aspid");

            _adminDashRepository.UpdateAdmin(profile, aspid??1);

            AdminProfileModal profileModal = _adminDashRepository.GetAdminProfileData(aspid ?? 1);
            return PartialView("_MyProfile", profileModal);
        }
        [HttpPost]
        public IActionResult update_admin_address(AdminProfileModal profile)
        {
            var aspid = HttpContext.Session.GetInt32("Aspid");
            _adminDashRepository.UpdateAdminAddress(profile, aspid??1);
            AdminProfileModal profileModal = _adminDashRepository.GetAdminProfileData(aspid ?? 1);
            return PartialView("_MyProfile", profileModal);
        }


        public IActionResult createrole(int accounttype)
        {
            var role = _adminDashRepository.CreateRole(accounttype);
            return PartialView("_CreateRole",role);
        }

        [HttpPost]
        public void newrole(List<int> menulist,string name,short accounttype)
        {
            var aspid = HttpContext.Session.GetInt32("userId");
            _adminAccess.NewRole(menulist,aspid??1,name,accounttype);
        }
        [HttpPost]
        public IActionResult editroleview(int roleid)
        {
            var role = _adminAccess.CreateRole(0,roleid);
            return PartialView("_EditRole", role);
        }
        public void editrole(List<int> menulist, string name, short accounttype,int roleid)
        {
            var aspid = HttpContext.Session.GetInt32("userId");
            var role = _adminAccess.EditRole(menulist,accounttype,roleid,aspid??1,name);
        }

        [HttpPost]
        public void deleterole(int roleid)
        {
            var aspid = HttpContext.Session.GetInt32("userId");
            _adminAccess.DeleteRole(roleid,aspid??1);
        }


        public List<ProviderLocation> providerLocations()
        {
            return _providerLocation.GetProviderLocations();
        }

        [HttpPost]
        public IActionResult exportToExcel(PartialViewModal partialView , AdminRecordsListModal adminRecords)
        {
            var stream = new MemoryStream();
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

            switch (partialView.exportType)
            {
                case "DashboardAll":
                    var status1 = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

                    var dashModel1 = _adminDashRepository.GetRequests(status1, 1, "", 0,true);

                    using (var package = new ExcelPackage(stream))
                    {
                        var workSheet = package.Workbook.Worksheets.Add("Sheet1");
                        workSheet.Cells.LoadFromCollection(dashModel1.adminDashModels, true);
                        package.Save();
                    }

                    break;

                case "DashboardFilterd":

                    var status = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

                    var dashModel = _adminDashRepository.GetRequests(status, 1, partialView.search, partialView.regionid, true);

                    using (var package = new ExcelPackage(stream))
                    {
                        var workSheet = package.Workbook.Worksheets.Add("Sheet1");
                        workSheet.Cells.LoadFromCollection(dashModel.adminDashModels, true);
                        package.Save();
                    }

                    break;

                case "SearchRecords":

                    using (var package = new ExcelPackage(stream))
                    {
                        var workSheet = package.Workbook.Worksheets.Add("Sheet1");
                        workSheet.Cells.LoadFromCollection(_adminRecords.SearchRecords(adminRecords,true).searchRecords, true);
                        package.Save();
                    }

                    break;
            }

            
            stream.Position = 0;
            string excelName = $"UserList-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";

            return File(stream, "application/octet-stream", excelName);
        }

    }
}


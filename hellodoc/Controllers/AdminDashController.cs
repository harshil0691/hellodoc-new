using Microsoft.AspNetCore.Mvc;
using hellodoc.Repositories.Repository.Interface;
using hellodoc.DbEntity.ViewModels;
using hellodoc.Repositories.Repository;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
using OfficeOpenXml;
using hellodoc.DbEntity.DataModels;
using System.IdentityModel.Tokens.Jwt;

namespace hellodoc.Controllers
{
    public class JwtHelper
    {
        public static Dictionary<string, string> GetClaimsFromCookie(HttpContext context, string cookieName)
        {
            if (context.Request.Cookies.TryGetValue(cookieName, out string jwtToken))
            {
                var handler = new JwtSecurityTokenHandler();
                var token = handler.ReadJwtToken(jwtToken);

                var claims = new Dictionary<string, string>();

                foreach (var claim in token.Claims)
                {
                    claims.Add(claim.Type, claim.Value);
                }

                return claims;
            }

            return null; // Cookie not found or no token present
        }
    }

    [CustomUserAuthorize("admin")]
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
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AdminDashController(ILogger<AdminDashController> logger,IAdminDashRepository adminDashRepository, IPatientLogin patientLogin, IRequests requests,IHostingEnvironment hostingEnvironment,IAuthManager authManager,IAdminProviders adminProviders,IAdminAccess adminAccess,IAdminProviderLocation providerLocation,IAdminRecords adminRecords,IHttpContextAccessor httpContextAccessor)
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
            _httpContextAccessor = httpContextAccessor;
        }

        public IActionResult admin_dash()
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
                    HttpContext.Session.SetInt32("Aspid",int.Parse(userId));
                }
                if (claims.ContainsKey("physicianId"))
                {
                    string userId = claims["physicianId"];
                }
            }

            var request = _adminDashRepository.GetCount("admin",0).Result;
            return View(request);
            
        }
        public List<NotificationMessage> GetNotification()
        {
            return _adminDashRepository.GetNotification();
        }

        public IActionResult download(int download)
        {
            var fname1 = _requests.GetFilename(download).Result;

            var filepath = Path.Combine(HostingEnviroment.WebRootPath, "uploads", fname1);
            return File(System.IO.File.ReadAllBytes(filepath), "multipart/form-data", System.IO.Path.GetFileName(filepath));

        }

        public IActionResult LoadPartialDashView(string tabId)
        {

            switch (tabId)
            {
                case "dashboard":
                    RequestCountByStatus request = _adminDashRepository.GetCount("admin",0).Result;
                    request.regions = _adminDashRepository.GetRegions(0);
                    request.activeid = HttpContext.Session.GetInt32("activeid") ?? 1;

                    return PartialView("_dashboard", request);

                case "providerlocation":
                    return PartialView("_ProviderLocation");

                case "myprofile":
                    var userid = HttpContext.Session.GetInt32("Aspid");
                    AdminProfileModal adminProfile = _adminDashRepository.GetAdminProfileData(userid ?? 1);
                    if ( adminProfile != null)
                    {
                        return PartialView("_MyProfile", adminProfile);
                    }
                    else
                    {
                        TempData["error"] = "Internal Error To Load View";
                        return RedirectToAction("admin_dash", "AdminDash");
                    }
                    

                case "provider":
                    return RedirectToAction("GetProvidersView","AdminProviders", new {actionType = "provider"});

                case "partner":
                    return RedirectToAction("GetView", "AdminPartners",new { actionType = "Partners" });

                case "access":
                    return RedirectToAction("GetAccessView","AdminAccess",new { actionType = "accountAccess"});

                case "records":
                    return RedirectToAction("GetView", "AdminRecords",new {actionType = "SearchRecords"});


                default:
                    return PartialView("_DefaultTab");

            }

        }

        [HttpPost]
        public IActionResult UpdatePassword(AdminProfileModal admin)
        {
            var aspid = HttpContext.Session.GetInt32("Aspid");
            _adminDashRepository.UpdatePassword(aspid??1, admin.password);

            return RedirectToAction("admin_dash", "AdminDash");
        }

        [HttpPost]
        public IActionResult UpdateAdmin(AdminProfileModal profile)
        {
            var aspid = HttpContext.Session.GetInt32("Aspid");
            _adminDashRepository.UpdateAdmin(profile, aspid??1);

            return RedirectToAction("admin_dash","AdminDash");
        }

        [HttpPost]
        public IActionResult UpdateAdminAddress(AdminProfileModal profile)
        {
            var aspid = HttpContext.Session.GetInt32("Aspid");
            _adminDashRepository.UpdateAdminAddress(profile, aspid??1);
            return RedirectToAction("admin_dash", "AdminDash");
        }



        public List<ProviderLocation> providerLocations()
        {
            return _providerLocation.GetProviderLocations();
        }

        [HttpPost]
        public IActionResult exportToExcel(PartialViewModal partialView , AdminRecordsListModal adminRecords)
        {
            // Set the LicenseContext before creating the ExcelPackage instance
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

            var stream = new MemoryStream();

            switch (partialView.exportType)
            {
                case "DashboardAll":
                    var status1 = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
                    var dashModel1 = _adminDashRepository.GetRequests(partialView);

                    using (var package = new ExcelPackage(stream))
                    {
                        var workSheet = package.Workbook.Worksheets.Add("Sheet1");
                        //workSheet.Cells.LoadFromCollection(dashModel1.adminDashModels, true);
                        workSheet.Cells[1, 1].Value = "data1";
                        workSheet.Cells[1, 2].Value = "data2";

                        for (int i = 0; i < dashModel1.adminDashModels.Count; i++)
                        {
                            workSheet.Cells[i + 2, 1].Value = dashModel1.adminDashModels[i].Requestid;
                            workSheet.Cells[i + 2, 2].Value = dashModel1.adminDashModels[i].PatientName;
                            // Populate more properties as needed
                        }
                        package.Save();
                    }
                    break;

                case "DashboardFiltered":
                    var status = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
                    var dashModel = _adminDashRepository.GetRequests(partialView);

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
                        //workSheet.Cells.LoadFromCollection(_adminRecords.SearchRecords(adminRecords, true).searchRecords, true);
                        var list = _adminRecords.SearchRecords(adminRecords, true).searchRecords;
                        workSheet.Cells[1, 1].Value = "data1";
                        workSheet.Cells[1, 2].Value = "data2";

                        for (int i = 0; i < list.Count; i++)
                        {
                            workSheet.Cells[i + 2, 1].Value = list[i].RequestClientId;
                            workSheet.Cells[i + 2, 2].Value = list[i].PhoneNumber;
                            // Populate more properties as needed
                        }
                        package.Save();
                    }
                    break;
            }

            // Reset the position of the MemoryStream
            stream.Position = 0;

            // Generate a unique file name for the Excel file
            string excelName = $"UserList-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";

            // Return the Excel file as a FileResult
            return File(stream, "application/octet-stream", excelName);

        }

    }
}


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

        public AdminDashController(ILogger<AdminDashController> logger, IAdminDashRepository adminDashRepository, IPatientLogin patientLogin, IRequests requests, IHostingEnvironment hostingEnvironment, IAuthManager authManager, IAdminProviders adminProviders, IAdminAccess adminAccess, IAdminProviderLocation providerLocation, IAdminRecords adminRecords, IHttpContextAccessor httpContextAccessor)
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
                    HttpContext.Session.SetInt32("Aspid", int.Parse(userId));
                }
                if (claims.ContainsKey("physicianId"))
                {
                    string userId = claims["physicianId"];
                }
            }
            var request = _adminDashRepository.GetCount("admin", 0).Result;
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
            //_authManger.authaction(HttpContext, tabId);
            switch (tabId)
            {
                case "dashboard":
                    
                    RequestCountByStatus request = _adminDashRepository.GetCount("admin", 0).Result;
                    request.regions = _adminDashRepository.GetRegions("", 0);
                    request.activeid = HttpContext.Session.GetInt32("activeid") ?? 1;

                    return PartialView("_dashboard", request);

                case "providerlocation":
                    return PartialView("_ProviderLocation");

                case "myprofile":
                    var userid = HttpContext.Session.GetInt32("Aspid");
                    AdminProfileModal adminProfile = _adminDashRepository.GetAdminProfileData(userid ?? 1);
                    if (adminProfile != null)
                    {
                        return PartialView("_MyProfile", adminProfile);
                    }
                    else
                    {
                        TempData["error"] = "Internal Error To Load View";
                        return RedirectToAction("admin_dash", "AdminDash");
                    }


                case "provider":
                    return RedirectToAction("GetProvidersView", "AdminProviders", new { actionType = "provider" });

                case "partner":
                    return RedirectToAction("GetView", "AdminPartners", new { actionType = "Partners" });

                case "access":
                    return RedirectToAction("GetAccessView", "AdminAccess", new { actionType = "accountAccess" });

                case "records":
                    return RedirectToAction("GetView", "AdminRecords", new { actionType = "SearchRecords" });

                default:
                    return PartialView("_DefaultTab");

            }

        }

        [HttpPost]
        public IActionResult UpdatePassword(AdminProfileModal admin)
        {
            var aspid = HttpContext.Session.GetInt32("Aspid");

            if (admin.password != null)
            {
                _adminDashRepository.UpdatePassword(aspid ?? 1, admin.password);
                TempData["success"] = "Password Updated Successfully";
                return RedirectToAction("admin_dash", "AdminDash");
            }
            else
            {
                TempData["error"] = "Please Provide Password to Update it";
                return RedirectToAction("admin_dash", "AdminDash");
            }

        }

        [HttpPost]
        public IActionResult UpdateAdmin(AdminProfileModal profile)
        {
            var aspid = HttpContext.Session.GetInt32("Aspid");
            if (profile != null)
            {
                TempData["success"] = "Profile Updated Successfully";
                _adminDashRepository.UpdateAdmin(profile, aspid ?? 1);
            }
            return RedirectToAction("admin_dash", "AdminDash");
        }

        [HttpPost]
        public IActionResult UpdateAdminAddress(AdminProfileModal profile)
        {
            var aspid = HttpContext.Session.GetInt32("Aspid");
            if (profile != null)
            {
                TempData["success"] = "Profile Updated Successfully";
                _adminDashRepository.UpdateAdminAddress(profile, aspid ?? 1);
            }
            return RedirectToAction("admin_dash", "AdminDash");
        }



        public List<ProviderLocation> providerLocations()
        {
            return _providerLocation.GetProviderLocations();
        }


        public IActionResult export()
        {
            // Set the LicenseContext before creating the ExcelPackage instance
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

            var stream = new MemoryStream();

            var status1 = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            var dashModel1 = _adminDashRepository.GetRequests(new PartialViewModal());

            using (var package = new ExcelPackage(stream))
            {
                var workSheet = package.Workbook.Worksheets.Add("Sheet1");
                workSheet.Cells[1, 1].Value = "Requestid";
                workSheet.Cells[1, 2].Value = "PatientName";
                workSheet.Cells[1, 3].Value = "DateOfBirth";
                workSheet.Cells[1, 4].Value = "Phonenumber_Patient";
                workSheet.Cells[1, 5].Value = "Phonenumber_Requestor";
                workSheet.Cells[1, 6].Value = "Status";
                workSheet.Cells[1, 7].Value = "Createddate";
                workSheet.Cells[1, 8].Value = "Address";
                workSheet.Cells[1, 9].Value = "RequestorName";
                workSheet.Cells[1, 10].Value = "Requesttypeid";
                workSheet.Cells[1, 11].Value = "Requestclientid";
                workSheet.Cells[1, 12].Value = "Email";
                workSheet.Cells[1, 13].Value = "Physicianname";
                workSheet.Cells[1, 14].Value = "Notes";


                for (int i = 0; i < dashModel1.adminDashModels.Count; i++)
                {
                    workSheet.Cells[i + 2, 1].Value = dashModel1.adminDashModels[i].Requestid;
                    workSheet.Cells[i + 2, 2].Value = dashModel1.adminDashModels[i].PatientName;
                    workSheet.Cells[i + 2, 3].Value = dashModel1.adminDashModels[i].DateOfBirth;
                    workSheet.Cells[i + 2, 4].Value = dashModel1.adminDashModels[i].Phonenumber_P;
                    workSheet.Cells[i + 2, 5].Value = dashModel1.adminDashModels[i].Phonenumber_R;
                    workSheet.Cells[i + 2, 6].Value = dashModel1.adminDashModels[i].Status;
                    workSheet.Cells[i + 2, 7].Value = dashModel1.adminDashModels[i].Createddate;
                    workSheet.Cells[i + 2, 8].Value = dashModel1.adminDashModels[i].Address;
                    workSheet.Cells[i + 2, 9].Value = dashModel1.adminDashModels[i].RequestorName;
                    workSheet.Cells[i + 2, 10].Value = dashModel1.adminDashModels[i].Requesttypeid;
                    workSheet.Cells[i + 2, 11].Value = dashModel1.adminDashModels[i].RequestClientId;
                    workSheet.Cells[i + 2, 12].Value = dashModel1.adminDashModels[i].Email;
                    workSheet.Cells[i + 2, 13].Value = dashModel1.adminDashModels[i].Physicianname;
                    workSheet.Cells[i + 2, 14].Value = dashModel1.adminDashModels[i].Notes;
                    // Populate more properties as needed
                }
                package.Save();
            }
            stream.Position = 0;

            string excelName = $"UserList-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";

            return File(stream, "application/octet-stream", excelName);

        }

        [HttpPost]
        public IActionResult exportToExcel(PartialViewModal partialView, AdminRecordsListModal adminRecords, string exportType)
        {
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

            var stream = new MemoryStream();

            switch (partialView.exportType)
            {
                case "DashboardFiltered":
                    var status = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
                    var dashModel1 = _adminDashRepository.GetRequests(partialView);

                    using (var package = new ExcelPackage(stream))
                    {
                        var workSheet = package.Workbook.Worksheets.Add("Sheet1");

                        workSheet.Cells[1, 1].Value = "Requestid";
                        workSheet.Cells[1, 2].Value = "PatientName";
                        workSheet.Cells[1, 3].Value = "DateOfBirth";
                        workSheet.Cells[1, 4].Value = "Phonenumber_Patient";
                        workSheet.Cells[1, 5].Value = "Phonenumber_Requestor";
                        workSheet.Cells[1, 6].Value = "Status";
                        workSheet.Cells[1, 7].Value = "Createddate";
                        workSheet.Cells[1, 8].Value = "Address";
                        workSheet.Cells[1, 9].Value = "RequestorName";
                        workSheet.Cells[1, 10].Value = "Requesttypeid";
                        workSheet.Cells[1, 11].Value = "Requestclientid";
                        workSheet.Cells[1, 12].Value = "Email";
                        workSheet.Cells[1, 13].Value = "Physicianname";
                        workSheet.Cells[1, 14].Value = "Notes";


                        for (int i = 0; i < dashModel1.adminDashModels.Count; i++)
                        {
                            workSheet.Cells[i + 2, 1].Value = dashModel1.adminDashModels[i].Requestid;
                            workSheet.Cells[i + 2, 2].Value = dashModel1.adminDashModels[i].PatientName;
                            workSheet.Cells[i + 2, 3].Value = dashModel1.adminDashModels[i].DateOfBirth;
                            workSheet.Cells[i + 2, 4].Value = dashModel1.adminDashModels[i].Phonenumber_P;
                            workSheet.Cells[i + 2, 5].Value = dashModel1.adminDashModels[i].Phonenumber_R;
                            workSheet.Cells[i + 2, 6].Value = dashModel1.adminDashModels[i].Status;
                            workSheet.Cells[i + 2, 7].Value = dashModel1.adminDashModels[i].Createddate;
                            workSheet.Cells[i + 2, 8].Value = dashModel1.adminDashModels[i].Address;
                            workSheet.Cells[i + 2, 9].Value = dashModel1.adminDashModels[i].RequestorName;
                            workSheet.Cells[i + 2, 10].Value = dashModel1.adminDashModels[i].Requesttypeid;
                            workSheet.Cells[i + 2, 11].Value = dashModel1.adminDashModels[i].RequestClientId;
                            workSheet.Cells[i + 2, 12].Value = dashModel1.adminDashModels[i].Email;
                            workSheet.Cells[i + 2, 13].Value = dashModel1.adminDashModels[i].Physicianname;
                            workSheet.Cells[i + 2, 14].Value = dashModel1.adminDashModels[i].Notes;
                            // Populate more properties as needed
                        }
                        package.Save();
                    }
                    break;

                case "SearchRecords":
                    using (var package = new ExcelPackage(stream))
                    {
                        var workSheet = package.Workbook.Worksheets.Add("Sheet1");
                        var dashModel2 = _adminRecords.SearchRecords(adminRecords, true);

                        workSheet.Cells[1, 1].Value = "Requestid";
                        workSheet.Cells[1, 2].Value = "PatientName";
                        workSheet.Cells[1, 3].Value = "Date Of Service";
                        workSheet.Cells[1, 4].Value = "Phonenumber";
                        workSheet.Cells[1, 6].Value = "Status";
                        workSheet.Cells[1, 7].Value = "Close Case date";
                        workSheet.Cells[1, 8].Value = "Address";
                        workSheet.Cells[1, 9].Value = "RequestorName";
                        workSheet.Cells[1, 11].Value = "Requestclientid";
                        workSheet.Cells[1, 12].Value = "Email";
                        workSheet.Cells[1, 14].Value = "Notes";


                        for (int i = 0; i < dashModel2.searchRecords.Count; i++)
                        {
                            workSheet.Cells[i + 2, 1].Value = dashModel2.searchRecords[i].RequestId;
                            workSheet.Cells[i + 2, 2].Value = dashModel2.searchRecords[i].Patientname;
                            workSheet.Cells[i + 2, 3].Value = dashModel2.searchRecords[i].DateOfService;
                            workSheet.Cells[i + 2, 4].Value = dashModel2.searchRecords[i].PhoneNumber;
                            workSheet.Cells[i + 2, 6].Value = dashModel2.searchRecords[i].RequestStatus;
                            workSheet.Cells[i + 2, 7].Value = dashModel2.searchRecords[i].ClosedCaseDate;
                            workSheet.Cells[i + 2, 8].Value = dashModel2.searchRecords[i].Address;
                            workSheet.Cells[i + 2, 9].Value = dashModel2.searchRecords[i].Requestor;
                            workSheet.Cells[i + 2, 11].Value = dashModel2.searchRecords[i].RequestClientId;
                            workSheet.Cells[i + 2, 12].Value = dashModel2.searchRecords[i].Email;
                            workSheet.Cells[i + 2, 14].Value = dashModel2.searchRecords[i].PhysicianNote;
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


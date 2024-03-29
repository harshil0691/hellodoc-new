﻿using Microsoft.AspNetCore.Mvc;
using hellodoc.Models;
using hellodoc.DbEntity.DataModels;
using System.Diagnostics;
using hellodoc.Repositories.Repository.Interface;
using hellodoc.DbEntity.ViewModels;
using hellodoc.Repositories.Repository;
using Org.BouncyCastle.Asn1.Ocsp;
using hellodoc.DbEntity.ViewModels.PopUpModal;
using Microsoft.AspNetCore.Identity;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
using System.Net.Mail;
using MimeKit;
using System.Net;
using hellodoc.Utils;
using System.IO.Compression;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;
using System.Text;
using hellodoc.DbEntity.ViewModels.DashboardLists;
using System.Drawing;

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

        public AdminDashController(ILogger<AdminDashController> logger,IAdminDashRepository adminDashRepository, IPatientLogin patientLogin, IRequests requests,IHostingEnvironment hostingEnvironment,IAuthManager authManager,IAdminProviders adminProviders,IAdminAccess adminAccess)
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
                    request.activeid = 1;

                    return PartialView("_dashboard",request);

                case "providerlocation":
                    
                    return PartialView("_ProviderLocation");


                case "myprofile":
                    var userid = HttpContext.Session.GetInt32("Aspid");
                    AdminProfileModal profileModal = _adminDashRepository.GetAdminProfileData(userid??1);
                    return PartialView("_MyProfile",profileModal);

                case "provider":
                    var phy = _adminProviders.ProvidersTable();
                    DashboardListsModal dashboardLists = new DashboardListsModal();
                    dashboardLists.providersTableModal = phy;
                    return PartialView("_Providers",dashboardLists);

                

                case "partner":
                    
                    return PartialView("_Partners");

                case "access":
                    
                    var list = _adminDashRepository.accessTables();

                    return PartialView("_Access",list);

                case "records":

                    return PartialView("_Records");


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

        public IActionResult scheduling()
        {
            return PartialView("_Scheduling");

        }
        public IActionResult invoicing()
        {

            return PartialView("_Invoicing");
        }
        public IActionResult providers()
        {
            var phy = _adminProviders.ProvidersTable();
            DashboardListsModal dashboardLists = new DashboardListsModal();
            dashboardLists.providersTableModal = phy;
            return PartialView("_Providers", dashboardLists);
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

    }
}


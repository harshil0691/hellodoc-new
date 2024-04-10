using hellodoc.Repositories.Repository.Interface;
using hellodoc.DbEntity.ViewModels;
using hellodoc.DbEntity.ViewModels.Shifts;
using hellodoc.Repositories.Repository.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
using NUnit.Framework.Internal.Execution;


namespace hellodoc.Controllers
{
    public class AdminRecordsController : Controller
    {
        private readonly ILogger<AdminDashController> _logger;
        private readonly IAdminRecords _adminRecords;

        public AdminRecordsController(ILogger<AdminDashController> logger,IAdminRecords adminRecords)
        {
            _logger = logger;
            _adminRecords = adminRecords;
        }

        public IActionResult GetView(string actionType)
        {
            switch(actionType)
            {
                case "SearchRecords":
                    return PartialView("_SearchRecords");
                case "EmailLogs":
                    return PartialView("_EmailLogs");
                case "SMSLogs":
                    return PartialView("_SMSLogs");
                case "PatientHistory":
                    return PartialView("_PatientHistory");
                case "BlockedHistory":
                    return PartialView("_BlockedHistory");
                default:
                    return PartialView("'_default");
            }
        }

        public IActionResult GetTable(AdminRecordsListModal recordsListModal)
        {
            switch (recordsListModal.actionType)
            {
                case "SearchRecords":
                    return PartialView("_SearchRecordsTable", _adminRecords.SearchRecords(recordsListModal,false));
                case "EmailLogs":
                    return PartialView("_EmailLogsTable", _adminRecords.EmailLogs(recordsListModal));
                case "SMSLogs":
                    return PartialView("_SMSLogsTable", _adminRecords.SMSLogs(recordsListModal));
                case "PatientHistory":
                    if(recordsListModal.back == true)
                    {
                        recordsListModal.pageNumber = HttpContext.Session.GetInt32("page") ?? 1;
                    }
                    else
                    {
                        HttpContext.Session.SetInt32("page", recordsListModal.pageNumber);
                    }
                    return PartialView("_PatientHistoryTable", _adminRecords.PatientHistory(recordsListModal));
                case "PatientRecords":
                    
                    return PartialView("_PatientRecords", _adminRecords.PatientRecords(recordsListModal));
                case "BlockedHistory":
                    return PartialView("_BlockedHistoryTable", _adminRecords.BlokedHistory(recordsListModal));
                default:
                    return PartialView("'_default");
            }
        }

        public void DBOperations(PartialViewModal partialView)
        {
            switch (partialView.actionType)
            {
                case "DeletePermanantly":
                    _adminRecords.DeletePermenantly(partialView.requestid);
                    break;
            }
        }


    }
}

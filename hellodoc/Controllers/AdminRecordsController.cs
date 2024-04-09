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

        public IActionResult GetTable(AdminRecordsListModal recordsListModal,int userid)
        {
            switch (recordsListModal.actionType)
            {
                case "SearchRecords":
                    return PartialView("_SearchRecordsTable", _adminRecords.SearchRecords(recordsListModal));
                case "EmailLogs":
                    return PartialView("_EmailLogsTable", _adminRecords.EmailLogs(recordsListModal));
                case "SMSLogs":
                    return PartialView("_SMSLogsTable", _adminRecords.SMSLogs(recordsListModal));
                case "PatientHistory":
                    return PartialView("_PatientHistoryTable", _adminRecords.PatientHistory(recordsListModal));
                case "PatientRecords":
                    return PartialView("_PatientRecords", _adminRecords.PatientRecords(userid));
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

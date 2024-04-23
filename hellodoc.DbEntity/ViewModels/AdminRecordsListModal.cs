using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using hellodoc.DbEntity.ViewModels.AdminAccess;
using hellodoc.DbEntity;
using hellodoc.DbEntity.ViewModels.AdminRecords;
using System.ComponentModel.DataAnnotations;

namespace hellodoc.DbEntity.ViewModels
{
    public partial class AdminRecordsListModal
    {
        public int RequestStatus { get; set; }
        public string Email { get; set; }
        [RegularExpression("^[0-9]*$", ErrorMessage = "Phone number must contain only numbers")]
        public long PhoneNumber { get; set; }
        public string PatientName { get; set; }
        public int RequestType { get; set; }
        public DateTime FromDateOfService { get; set; }
        public DateTime ToDateOfService { get; set; }
        public string ProviderName { get; set; }
        public int SearchByRole { get; set; }
        public string ReceiverName { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime SentDate { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BlokedDate { get; set; }
        public bool back { get; set; }

        public List<SearchRecords> searchRecords;
        public List<EmailLogs> emailLogs;
        public List<SMSLogs> smsLogs;
        public List<BlokedHistory> blokedHistory;
        public List<PatientRecords> patientRecords;
        public List<PatientHistory> patientHistories;
        public int UserId { get; set; }
        public string actionType { get; set; }
        public int pageNumber { get; set; }
        public int totalEntries { get; set; }
        public int pageSize { get; set; }
        public string entries { get; set; }
        public bool morePages { get; set; }
    }
}

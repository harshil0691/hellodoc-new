using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hellodoc.DbEntity.ViewModels.AdminRecords
{
    public partial class SMSLogs
    {
        public int SMSLogsId { get; set; }
        public string Recipient { get; set; }
        public string RoleName { get; set; }
        public long MobileNumber { get; set; }
        public string CreatedDate { get; set; }
        public string SentDate { get; set; }
        public string Sent { get; set; }
        public int SentTimes { get; set; }
        public string ConfirmationNumber { get; set; }

    }
}

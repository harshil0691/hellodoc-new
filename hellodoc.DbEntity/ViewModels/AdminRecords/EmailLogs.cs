using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hellodoc.DbEntity.ViewModels.AdminRecords
{
    public partial class EmailLogs
    {
        public int EmaillogsId { get; set; }
        public string Recipient { get; set; } = "-";
        public string Actions { get; set; } = "-";
        public string? RoleName { get; set; }
        public string? EmailId { get; set;}
        public string? CreateDate { get; set; }
        public string? SentDate { get; set; }
        public string Sent { get; set; }
        public int SentTimes { get; set; }
        public string ConfirmationNumber { get; set; } = "-";
    }
}

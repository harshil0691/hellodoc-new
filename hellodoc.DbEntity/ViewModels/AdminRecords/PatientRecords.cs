using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hellodoc.DbEntity.ViewModels.AdminRecords
{
    public partial class PatientRecords
    {
        public int RequestId { get; set; }
        public int UserId { get; set; }
        public string PatientName { get; set; } = "-";
        public long PhoneNumber { get; set; }
        public string Email { get; set; } = "-";
        public string CreatedDate { get; set; } = "-";
        public string ConfirmationNumber { get; set; }
        public string ProviderName { get; set; }
        public string Status { get; set; } = "-";
        public string Concludeddate { get; set; } = "-";
        public bool FinalReport { get; set; }
    }
}

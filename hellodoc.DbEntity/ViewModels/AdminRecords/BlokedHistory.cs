using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hellodoc.DbEntity.ViewModels.AdminRecords
{
    public partial class BlokedHistory
    {
        public int BlokedId { get; set; }
        public string PatientName { get; set; } = "-";
        public long PhoneNumber { get; set; }
        public string Email { get; set; } = "-";
        public string CreatedDate { get; set; } = "-";
        public string Note { get; set; } = "-";
        public bool IsActive { get; set; }
        public int Requestid { get;set; } 
    }
}

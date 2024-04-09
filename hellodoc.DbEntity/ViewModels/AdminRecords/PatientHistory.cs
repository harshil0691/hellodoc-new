using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hellodoc.DbEntity.ViewModels.AdminRecords
{
    public partial class PatientHistory
    {
        public int RequestId { get; set; }
        public int UserId { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public long PhoneNumber { get; set; }
        public string Email { get; set; } = "-";
        public string Address { get; set; }
    }
}

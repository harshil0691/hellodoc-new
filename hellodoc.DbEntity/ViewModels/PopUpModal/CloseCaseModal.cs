using hellodoc.DbEntity.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hellodoc.DbEntity.ViewModels.PopUpModal
{
    public partial class CloseCaseModal
    {
        public string patientName { get; set; }
        public string? confirmationnumber { get; set; }
        public int requestid { get; set; }
        public string? Firstname { get; set; }
        public string? Lastname { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public string? CloseCaseEmail { get; set; }
        public long? Phone { get; set; }
        public List<RequestWiseFile> PatientDocuments { get; set; }
    }
}

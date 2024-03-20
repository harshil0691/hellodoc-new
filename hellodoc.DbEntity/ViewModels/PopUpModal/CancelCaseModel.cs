using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hellodoc.DbEntity.ViewModels
{
    public partial class CancelCaseModel
    {
        public string CancelNotes { get; set; }
        public int CancelReasonValue { get; set; }
        public int Requestid { get; set; }
        public string PatientName { get; set; }
    }
}

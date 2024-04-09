using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hellodoc.DbEntity.ViewModels.PopUpModal
{
    public partial class SendAgreementModal
    {
        public string reqtype { get; set; }
        public int reqid { get; set; }
        public string email { get; set; }
        public long? phonenumber { get; set; }
        public string bcolor { get; set; }
        public string patientName { get; set;}
        public string CancelNotes { get; set;}
    }
}

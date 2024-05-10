using hellodoc.DbEntity.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hellodoc.DbEntity.ViewModels
{
    public  class InvoicingModal
    {
        public int currentMonth { get; set; }
        public int currentYear { get; set; }
        public int timeSlot { get; set; }
        public int numberOfDays { get; set; }
        public bool finalized { get; set; }
        public string loginType { get; set; }
        public int selectedPhysician { get;set; }

        public List<Invoicing> invoicings { get; set; } 

        public List<Timesheet> timesheets { get; set; }
        public List<Physician> physicians { get; set; }
        public List<PayrateCountModal> payrateCounts { get; set; }
    }
}

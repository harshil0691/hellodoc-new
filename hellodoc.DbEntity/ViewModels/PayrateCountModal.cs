using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hellodoc.DbEntity.ViewModels
{
    public class PayrateCountModal
    {
        public int TimesheetId { get; set; }
        public DateOnly? Date { get; set; }
        public int? Invoicingid { get; set; }
        public int? Nightshiftweekend { get; set; }
        public int? Shift { get; set; }
        public int? Housecallnightweekend { get; set; }
        public long? Phonecounsults { get; set; }
        public long? Phonecounsultsnightweekend { get; set; }
        public int? Batchtesting { get; set; }
        public int? Housecall { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hellodoc.DbEntity.ViewModels.Shifts
{
    public partial class MonthShiftModal
    {
        public DateTime currentDate { get ; set; }
        public int year { get; set; }
        public int month { get; set; }
        public int daysInMonth { get; set; }
        public int daysLoop { get; set; }
        public DateTime firstDayOfMonth { get; set; }
        public int startDayIndex { get; set; }
        public string[] dayNames { get; set; }  
        public List<ShiftDetailsmodal> shiftDetailsmodals { get; set; }
    }
}

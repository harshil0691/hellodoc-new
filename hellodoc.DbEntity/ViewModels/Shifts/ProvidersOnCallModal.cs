using hellodoc.DbEntity.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hellodoc.DbEntity.ViewModels.Shifts
{
    public partial class ProvidersOnCallModal
    {
        public int physicianId { get; set; }
        public string physicianName { get; set; }
        public string PhotoPath { get; set; }
    }
}

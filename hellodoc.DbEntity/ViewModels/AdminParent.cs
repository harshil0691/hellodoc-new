using hellodoc.DbEntity.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hellodoc.DbEntity.ViewModels
{
    public partial class AdminParent
    {
        public List<AdminDashModel> adminDashModels { get; set; }

        public string CancelNotes { get; set; }

        public int CancelReasonValue { get; set; }

        public List<Region> regions { get; set; }

        public int region12 { get; set; }
    }
}

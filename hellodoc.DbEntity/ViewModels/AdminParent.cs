
using hellodoc.DbEntity.DataModels;
using Microsoft.VisualBasic;
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

        public int requestcount { get; set; }
        public string search { get;set; }
        public int regionid { get; set; }

        public int pageNumber { get; set; }
        public int totalEntries { get; set; }
        public int pageSize { get; set; }
        public string entries { get; set; }
        public bool morePages { get; set; }
        public string RequestStatus { get; set; }
        public string CallType { get; set; }
    }
}

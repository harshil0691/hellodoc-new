using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using hellodoc.DbEntity.DataModels;

namespace hellodoc.DbEntity.ViewModels
{
    public partial class RequestCountByStatus
    {
        public int NewCount { get; set; }

        public int PendingCount { get; set; }

        public int ActiveCount { get; set; }

        public int ConcludeCount { get; set; }

        public int TocloseCount { get; set; }

        public int UnpaidCount { get; set; }

        public int activeid { get; set; }

        public List<Region> regions { get; set; }

        public string dashboardType { get; set; }
        public string search { get; set; }
        public int regionid { get; set; }

    }
}

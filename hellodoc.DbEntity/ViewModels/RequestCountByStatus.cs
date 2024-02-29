using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    }
}

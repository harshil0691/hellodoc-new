using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hellodoc.DbEntity.ViewModels.AdminAccess
{
    public class AdminAccessModal
    {
        public List<AccessTableModal> accessTables;

        public int accountSelect { get; set; }
        public int pageNumber { get; set; }
        public int totalEntries { get; set; }
        public int pageSize { get; set; }
        public string entries { get; set; }
        public bool morePages { get; set; }

    }
}

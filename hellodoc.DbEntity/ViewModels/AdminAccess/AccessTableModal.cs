using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hellodoc.DbEntity.ViewModels.AdminAccess
{
    public partial class AccessTableModal
    {
        public string accessName { get; set; }
        public int accountType { get; set; }
        public int roleid { get; set; }
    }
}

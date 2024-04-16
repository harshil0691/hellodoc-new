using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hellodoc.DbEntity.ViewModels.AdminAccess
{
    public partial class AccessTableModal
    {
        public int aspid { get; set; }
        public string accessName { get; set; }
        public int accountType { get; set; }
        public int roleid { get; set; } 
        public string accountType1 { get; set; }
        public string accountPOC { get; set; }
        public long phoneNumber { get; set; }
        public string Email { get; set; }
        public string status { get; set; }
        public int openRequest { get; set; }
    }
}

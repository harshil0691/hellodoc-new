using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using hellodoc.DbEntity.DataModels;

namespace hellodoc.DbEntity.ViewModels
{
    public partial class ProvidersTableModal
    {
        public int physicianid { get; set; }
        public string name { get; set; }
        public string role { get; set; }
        public short status { get; set; }
        public string oncallstatus { get; set; }
        public PhysicianNotification stopnotification { get; set; }

    }
}

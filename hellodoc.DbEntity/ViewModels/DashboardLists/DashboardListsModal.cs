using hellodoc.DbEntity.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hellodoc.DbEntity.ViewModels.DashboardLists
{
    public partial class DashboardListsModal
    {
        public List<Physician> physicians ;
        public List<Smslog> smslog ;
        public List<EmailLog> emailLogs ;
        public List<ProvidersTableModal> providersTableModal ;
    }
}

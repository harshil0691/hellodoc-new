using hellodoc.DbEntity.ViewModels;
using hellodoc.DbEntity.ViewModels.AdminRecords;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hellodoc.Repositories.Repository.Interface
{
    public interface IAdminRecords
    {
        AdminRecordsListModal SearchRecords(AdminRecordsListModal adminRecords);

        AdminRecordsListModal EmailLogs(AdminRecordsListModal adminRecords);
        AdminRecordsListModal SMSLogs(AdminRecordsListModal adminRecords);
        AdminRecordsListModal PatientRecords(int userid);
        AdminRecordsListModal BlokedHistory(AdminRecordsListModal adminRecords);
        AdminRecordsListModal PatientHistory(AdminRecordsListModal adminRecords);
        void DeletePermenantly(int requstid);
    }
}

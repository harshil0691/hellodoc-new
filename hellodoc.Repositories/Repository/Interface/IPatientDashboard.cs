using hellodoc.DbEntity.DataModels;
using hellodoc.DbEntity.ViewModels;
using hellodoc.DbEntity.ViewModels.PopUpModal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hellodoc.Repositories.Repository.Interface
{
    public interface IPatientDashboard
    {
        PatientReqModel GetRequestList(int? uid);
        Task AgreeAgreement(int reqid,string ip);
        Task CancelAgreement(SendAgreementModal sendAgreement,string ip);

        Task<int> GetRequestStatus(int reqid);
    }
}

using hellodoc.DbEntity.DataModels;
using hellodoc.DbEntity.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hellodoc.Repositories.Repository.Interface
{
    public interface IRequests
    {
        Task<AspNetUser> SetAspNetUser(PatientReqModel patientReq);

        Task<User> SetUser(PatientReqModel patientReq, short aspid);

        Task<Request> SetRequest(PatientReqModel patientReq, int uid);

        Task<Int32> SetRequest(FriendReqModel friendReq);
        Task<Int32> SetRequest(BusinessReqModel businessReq);
        Task<Int32> SetRequest(ConciergeReqModel conciergeReq);

        Task<RequestClient> SetRequestClient(PatientReqModel patientReq, int rid);

        Task SetRequestClient(FriendReqModel friendReq, int rid);
        Task SetRequestClient(BusinessReqModel businessReq, int rid);
        Task SetRequestClient(ConciergeReqModel conciergeReq, int rid);

        Task SetRequestWiseFile(string uniqueFilename, int rid);

        Task SetConcierge(ConciergeReqModel conciergeReq);
        Task Setbusiness(BusinessReqModel businessReq);

        Task<Int32> GetAspUser(string email);
        Task<Int32> GetUser(int aspid);
        Task UpdateUser(PatientReqModel patientReq, int userid);
        PatientReqModel GetDocuments(int rid, int uid);
        Task<string> GetFilename(int reqcliid);
    }
}

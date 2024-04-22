using hellodoc.DbEntity.DataModels;
using hellodoc.DbEntity.ViewModels;
using hellodoc.DbEntity.ViewModels.PopUpModal;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hellodoc.Repositories.Repository.Interface
{
    public interface IRequests
    {
        string PatientRequest(RequestFormModal requestForm);
        string FriendRequest(RequestFormModal requestForm);
        string ConciergeRequest(RequestFormModal requestForm);
        string BusinessRequest(RequestFormModal requestForm);
        string RequestMe(RequestFormModal requestForm,int Aspid);
        string RequestSomeone(RequestFormModal requestForm, int Aspid);
        void SaveFile(IFormFile formFile,int requestid);
        RequestFormModal GetPatientProfile(int uid);
        AspNetUser GetAspUser(string email);
        Task<Int32> GetUser(int aspid);
        Task UpdateUser(RequestFormModal updateForm, int userid);
        PatientReqModel GetDocuments(int rid);
        Task<string> GetFilename(int reqcliid);

        void CreateUser(RequestFormModal requestForm);

        Task UpdateCloseCase(int requestid,CloseCaseModal closeCase);
    }
}

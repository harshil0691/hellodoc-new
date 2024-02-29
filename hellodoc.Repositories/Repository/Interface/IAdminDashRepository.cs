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
    public interface IAdminDashRepository
    {
        AdminParent GetRequests(List<int> status);

        Task<PatientReqModel> Getpatientdata(int rid);
        Task<RequestCountByStatus> GetCount();

        Task SetNotes(NotesModel note, int? reqid, string? username);

        Task<NotesModel> GetNotes(int reqid);

        Task CancelRequest(int? reqid, CancelCaseModel cancelCase, int? adminid);

        List<Region> GetRegions();
        List<Physician> GetPhysicianList();

        List<Physician> GetPhysicianList2(int select);

        Task AssignCase(int? reqid, AssignCaseModal assignCase, int? adminid);
    }
}

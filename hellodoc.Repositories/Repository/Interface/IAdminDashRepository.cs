using hellodoc.DbEntity.DataModels;
using hellodoc.DbEntity.ViewModels;
using hellodoc.DbEntity.ViewModels.PopUpModal;
using hellodoc.DbEntity.ViewModels.AdminAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hellodoc.Repositories.Repository.Interface
{
    public interface IAdminDashRepository
    {
        AdminParent GetRequests(List<int> status,int page,string search , int regionid);

        Task<PatientReqModel> Getpatientdata(int rid);
        Task<RequestCountByStatus> GetCount();

        Task SetNotes(NotesModel note, int? reqid, string? username);

        Task<NotesModel> GetNotes(int reqid);

        Task CancelRequest(int? reqid, CancelCaseModel cancelCase, int? adminid);

        List<Region> GetRegions();
        List<Physician> GetPhysicianList();

        List<Physician> GetPhysicianList2(int select);

        Task AssignCase(int? reqid, AssignCaseModal assignCase, int? adminid);
        Task BlockCase(int? reqid, BlockCaseModal blockCase, int? adminid);
        Task Clearcase(int? reqid, int? adminid);

        Task DeleteDocument(int docid); 

        List<string> GetListFilename(List<int> rwfid);

        List<HealthProfessionalType> GetListProfessionTypes();
        List<HealthProfessional> GetHealthProfessionals(int select);

        HealthProfessional GetVendorData(int vendorid);

        Task SetOrder(OrdersModal orders, int requestid, int aspid);

        Task<CloseCaseModal> GetCloseCaseModal(int requestid);
        Task CloseCase(int reqid, int adminid);

        AdminProfileModal GetAdminProfileData(int aspnetuserid);
        Task UpdatePassword(int aspid, string password);
        Task UpdateAdmin(AdminProfileModal adminProfile,int aspid);

        Task UpdateAdminAddress(AdminProfileModal adminProfile, int aspid);
        Encounter GetEncounter(int requestid);
        Encounter SetEncounter(int requestid, Encounter encounter);
        Task FinalizeEncounter(int requestid, Encounter encounter1);

        List<AccessTableModal> accessTables();
        CreateRoleModal CreateRole(int accounttype);
    }
}

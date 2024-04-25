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
        AdminParent GetRequests(PartialViewModal partialView);

        Task<RequestFormModal> Getpatientdata(int rid);
        Task<RequestCountByStatus> GetCount(string accountType,int physicianid);

        Task SetNotes(NotesModel note, int? reqid, string? username,string logintype);

        Task<NotesModel> GetNotes(int reqid,int aspid);

        Task CancelRequest(int? reqid, CancelCaseModel cancelCase, int aspid);

        List<Region> GetRegions(string loginType,int physicanid);
        List<Physician> GetPhysicianList();

        List<Physician> GetPhysicianList2(int select);

        Task AssignCase(int? reqid, AssignCaseModal assignCase, int? adminid);
        bool BlockCase(int? reqid, BlockCaseModal blockCase, int? adminid);
        bool ViewCaseUpdate(int requestid,RequestFormModal requestForm,int aspid);
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
        void UpdatePassword(int aspid, string password);
        bool UpdateAdmin(AdminProfileModal adminProfile,int aspid);

        void UpdateAdminAddress(AdminProfileModal adminProfile, int aspid);
        Encounter GetEncounter(int requestid);
        Encounter SetEncounter(int requestid, Encounter encounter);
        string Encouter(int requestid,string callType);
        Task FinalizeEncounter(int requestid, Encounter encounter1);

        List<AccessTableModal> accessTables();
        CreateRoleModal CreateRole(int accounttype);

        string TransferToAdmin(int requestid , string transferNotes,int physicianid);
        List<NotificationMessage> GetNotification();
        List<Physician> GetUnAssignedPhysician();
    }
}

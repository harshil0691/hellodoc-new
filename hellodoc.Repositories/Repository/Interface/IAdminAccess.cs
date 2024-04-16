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
    public interface IAdminAccess
    {
        AdminAccessModal AccountAccessData(int pageNumber,string accessType);
        CreateRoleModal CreateRole(int accounttype,int roleid);
        Task NewRole(List<int> menulist,int aspid,string name, short accounttype);
        Task EditRole(List<int> menulist,short accounttype,int roleid,int aspid, string name);
        Task DeleteRole(int roleid, int aspid);
        string CreateAdmin(AdminProfileModal adminProfile);
        AdminProfileModal GetForCreateAdmin();
    }
}

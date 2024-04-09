using hellodoc.DbEntity.DataModels;
using hellodoc.DbEntity.ViewModels;
using hellodoc.DbEntity.ViewModels.DashboardLists;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hellodoc.Repositories.Repository.Interface
{
    public interface IAdminPartners
    {
        DashboardListsModal ProviderList(int ProfessionType, string search,int pageNumber);
        void CreateBusiness(AdminPartnersModal vendors);
        void UpdateBusiness(AdminPartnersModal vendors);
        List<HealthProfessionalType> GetHealthProfessionalType();
        AdminPartnersModal updateBusinessView(int vendorid);
        void DeleteBusiness(int vendorid); 
    }
}

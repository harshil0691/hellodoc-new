using hellodoc.DbEntity.DataModels;
using hellodoc.DbEntity.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hellodoc.Repositories.Repository.Interface
{
    public interface IAdminProviders
    {
        Physician GetPhysicianAsync(int physicianid);
        List<ProvidersTableModal> ProvidersTable();
        Task StopNotification(List<int> idlist,List<int> totallist);
        Task<ProviderProfileModal> ProviderProfileData(int physicianid);
    }
}

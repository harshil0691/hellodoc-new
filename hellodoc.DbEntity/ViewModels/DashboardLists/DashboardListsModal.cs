using hellodoc.DbEntity.DataModels;
using hellodoc.DbEntity.ViewModels.Shifts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hellodoc.DbEntity.ViewModels.DashboardLists
{
    public partial class DashboardListsModal
    {
        public List<Physician> physicians ;
        public List<Smslog> smslog ;
        public List<EmailLog> emailLogs ;
        public List<ProvidersTableModal> providersTableModal ;

        public List<ProvidersOnCallModal> onCall;
        public List<Region> regions;
        public List<ProvidersOnCallModal> offDuty;
        public List<ShiftDetailsmodal> shiftDetailslist ;
        public List<AdminPartnersModal> healthProfessionals;
        public List<HealthProfessionalType> healthProfessionalTypes;
        public List<PhysicianLocation> physicianLocations; 
        public int healthProfessionType { get; set; }
        public string VendorSearch { get; set; }  
        public int regionselect { get; set; }
        public int pageNumber { get; set; }
        public int totalEntries { get; set; }
        public int pageSize { get; set; }
        public string entries { get; set; }
        public bool morePages { get; set; }
        public int physicianId { get; set; }    
        public string role { get; set; }
    }
}

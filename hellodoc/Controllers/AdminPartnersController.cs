using hellodoc.Repositories.Repository.Interface;
using hellodoc.DbEntity.ViewModels;
using hellodoc.DbEntity.ViewModels.Shifts;
using hellodoc.Repositories.Repository.Interface;
using Microsoft.AspNetCore.Mvc;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
using NUnit.Framework.Internal.Execution;
using hellodoc.Repositories.Repository;
using hellodoc.DbEntity.ViewModels.DashboardLists;

namespace hellodoc.Controllers
{
    public class AdminPartnersController: Controller
    {
        private readonly ILogger<AdminDashController> _logger;
        private readonly IAdminPartners _adminPartners;
        private readonly IAdminDashRepository _adminDashRepository;


        public AdminPartnersController(ILogger<AdminDashController> logger, IAdminPartners adminPartners,IAdminDashRepository adminDashRepository)
        {
            _adminPartners = adminPartners;
            _logger = logger;
            _adminDashRepository = adminDashRepository;
        }

        public IActionResult GetView(PartialViewModal partialView)
        {
            switch (partialView.actionType)
            {
                case "partnerstable":
                    return PartialView("_PartnersTable", _adminPartners.ProviderList(partialView.professionalType, partialView.search, partialView.pageNumber));

                case "create_business":
                    AdminPartnersModal adminPartners = new AdminPartnersModal();
                    adminPartners.healthProfessionalTypes = _adminPartners.GetHealthProfessionalType();
                    adminPartners.regions = _adminDashRepository.GetRegions(0);
                    return PartialView("_CreateBusiness", adminPartners);

                case "edit_business":
                    return PartialView("_EditBusiness", _adminPartners.updateBusinessView(partialView.venorid));

                case "Partners":
                    DashboardListsModal listsModal = new DashboardListsModal();
                    listsModal.healthProfessionalTypes = _adminPartners.GetHealthProfessionalType();
                    return PartialView("_Partners",listsModal);

                default:
                    return PartialView("_default");
            }
        }

        [HttpPost]
        public void DBOperations(AdminPartnersModal adminPartners)
        {
            switch (adminPartners.actionType)
            {
                case "new_business":
                    _adminPartners.CreateBusiness(adminPartners);
                    break;

                case "update_business":
                    _adminPartners.UpdateBusiness(adminPartners);
                    break;

                case "delete_business":
                    _adminPartners.DeleteBusiness(adminPartners.Vendorid);
                    break;
            }
        }
        
    }
}

using hellodoc.DbEntity.DataContext;
using hellodoc.DbEntity.DataModels;
using hellodoc.DbEntity.ViewModels;
using hellodoc.DbEntity.ViewModels.DashboardLists;
using hellodoc.Repositories.Repository.Interface;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hellodoc.Repositories.Repository
{
    public class AdminPartners : IAdminPartners
    {
        private readonly ApplicationDbContext _context;

        public AdminPartners(ApplicationDbContext context)
        {
            _context = context;
        }


        public void CreateBusiness(AdminPartnersModal vendors)
        {
            HealthProfessional professional = new HealthProfessional()
            {
                Businesscontact = vendors.Businesscontact,
                Vendorname = vendors.Vendorname,
                Faxnumber = vendors.Faxnumber,
                Email = vendors.BusinessEmail,
                Phonenumber = vendors.Phonenumber,
                Profession = vendors.Profession,
                State = vendors.State.ToString(),
                City  = vendors.City,
                Createddate = DateTime.Now,
                Zip = vendors.Zip,
                Street = vendors.Street,
            };

            _context.HealthProfessionals.Add(professional); 
            _context.SaveChanges();
        }

        public DashboardListsModal ProviderList(int ProfessionType, string search,int pageNumber)
        {
            var pageSize = 3;
            if (pageNumber <= 0)
            {
                pageNumber = 1;
            }

            DashboardListsModal providerList = new DashboardListsModal();
            providerList.healthProfessionalTypes = _context.HealthProfessionalTypes.ToList();

            
            var vendors = _context.HealthProfessionals.Where(
                hp =>
                    ((ProfessionType !=0)? hp.Profession == ProfessionType :true)&&
                    ((search != null)? hp.Vendorname.ToLower().Contains(search.ToLower()):true) && 
                    (hp.Isdeleted != new System.Collections.BitArray(1, true))
                ).ToList();


            var list = vendors.Select(v => new AdminPartnersModal
            {
                Vendorid = v.Vendorid,
                ProfessionName = _context.HealthProfessionalTypes.FirstOrDefault(p => p.Healthprofessionalid == v.Profession)?.Professionname,
                Businesscontact = v.Businesscontact,
                Faxnumber = v.Faxnumber,
                BusinessEmail = v.Email,
                Phonenumber = v.Phonenumber,
                Vendorname = v.Vendorname,
            });

            providerList.healthProfessionals = list.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            providerList.pageNumber = pageNumber;
            providerList.pageSize = pageSize;
            providerList.totalEntries = list.Count();
            if (list.Skip((pageNumber) * pageSize).Take(pageSize).Count() > 0)
            {
                providerList.morePages = true;
            }
            providerList.entries = ((pageNumber - 1) * pageSize + 1) + "-" + (((pageNumber - 1) * pageSize) + providerList.healthProfessionals.Count());

            return providerList;
        }

        public void UpdateBusiness(AdminPartnersModal vendors)
        {
            var vendor = _context.HealthProfessionals.FirstOrDefault(h => h.Vendorid == vendors.Vendorid);
            if(vendor != null)
            {
                vendor.Profession = vendors.Profession;
                vendor.Businesscontact = vendors.Businesscontact;
                vendor.Email = vendors.BusinessEmail;
                vendor.Phonenumber = vendors.Phonenumber;
                vendor.Faxnumber = vendors.Faxnumber;
                vendor.State = vendors.State.ToString();
                vendor.City = vendors.City;
                vendor.Vendorname = vendors.Vendorname;
                vendor.Modifieddate = DateTime.Now;
                vendor.Zip = vendors.Zip;
                vendor.Street = vendors.Street;
            }
            _context.SaveChanges();
        }

        public AdminPartnersModal updateBusinessView(int vendorid)
        {
            var vendor = _context.HealthProfessionals.FirstOrDefault(h => h.Vendorid == vendorid);
            AdminPartnersModal adminPartners = new AdminPartnersModal();
            if (vendor != null)
            {
                adminPartners.Vendorid = vendorid;
                adminPartners.Profession = vendor.Profession ?? 1;
                adminPartners.Faxnumber = vendor.Faxnumber;
                adminPartners.BusinessEmail = vendor.Email;
                adminPartners.Phonenumber = vendor.Phonenumber;
                adminPartners.Address = vendor.Address;
                adminPartners.City = vendor.City;
                adminPartners.State = int.Parse(vendor.State??"0");
                adminPartners.Zip = vendor.Zip ?? 0;
                adminPartners.Businesscontact = vendor.Businesscontact;
                adminPartners.healthProfessionalTypes = _context.HealthProfessionalTypes.ToList();
                adminPartners.Vendorname = vendor.Vendorname;
                adminPartners.Street = vendor.Street;
                adminPartners.regions = _context.Regions.ToList();
            }
            return adminPartners;
        }

        public void DeleteBusiness(int vendorid)
        {
            var vendor = _context.HealthProfessionals.FirstOrDefault(h => h.Vendorid == vendorid);
            if (vendor != null)
            {
                vendor.Isdeleted = new System.Collections.BitArray(1, true);
            }
            
            _context.SaveChanges();
        }

        public List<HealthProfessionalType> GetHealthProfessionalType()
        {
            return _context.HealthProfessionalTypes.ToList();
        }
    }
}

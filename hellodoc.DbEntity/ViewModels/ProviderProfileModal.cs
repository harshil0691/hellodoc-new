using hellodoc.DbEntity.DataModels;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hellodoc.DbEntity.ViewModels
{
    public partial class ProviderProfileModal
    {
        public string patientName { get; set; }
        public string? confirmationnumber { get; set; }
        public int requestid { get; set; }
        public short status { get; set; }
        public int aspid { get; set; }
        public string role { get; set; }
        public string? password { get; set; }
        public string username { get; set; }
        public string updateType { get; set; }
        public string? Firstname { get; set; }
        public string? Lastname { get; set; }
        public string MediacalLicense { get; set; }
        public long? NPI { get; set; }
        public string Location { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public DateOnly? DateOfService { get; set; }
        public long? Phone { get; set; }

        public int physicianId { get; set; }
        public string resetPassword { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Email is Invalid")]
        public string? ProviderEmail { get; set; }

        public string? SynEmail { get; set; }

        public string? Address1 { get; set; }
        public string? Address2 { get; set; }
        public long? Zipcode { get; set; }
        public string? State { get; set; }
        public string? City { get; set; }
        public long? MailingNumber { get; set; }

        public string BusinessName { get; set; }
        public string BusinessEmail { get; set; }
        public string BusinessWebsite { get; set; }
        public IFormFile photo { get; set; }
        public IFormFile IndependentContractorManagement { get; set; }
        public IFormFile BackgroungCheck { get; set; }
        public IFormFile HIPAA { get; set; }
        public IFormFile NondisclosureAggrement { get; set; }
        public IFormFile Signature { get; set; }
        public IFormFile License { get; set; }

        public string? photoPath { get; set; }
        public string SignaturePath { get; set; }
        public string IndependentContractorManagementPath { get; set; }
        public string BackgroungCheckPath { get; set; }
        public string HIPAAPath { get; set; }
        public string NondisclosureAggrementPath { get; set; }
        public string LicensePath { get; set; }

        public string AdminNotes { get; set; }
        public string selectedRegion {  get; set; }
        public int selectrole { get; set; }
        public List<Role> roles { get; set; }
        public List<Region> regions { get; set; }
        public List<int> regionList { get; set; }
        public List<RequestWiseFile> PatientDocuments { get; set; }
    }
}

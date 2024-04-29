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

        [Required(ErrorMessage = "Password is required")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z]).+$", ErrorMessage = "Password must contain at least one lowercase letter and one uppercase letter")]
        public string? password { get; set; }
        [StringLength(15, MinimumLength = 5, ErrorMessage = "Username must be between 5 and 15 characters")]
        public string username { get; set; }
        public string updateType { get; set; }
        [Required]
        public string? Firstname { get; set; }
        [Required]
        public string? Lastname { get; set; }
        public string MediacalLicense { get; set; }
        [RegularExpression("^[0-9]*$", ErrorMessage = "Only numbers are allowed")]
        public long? NPI { get; set; }
        public string Location { get; set; }
        [Required]
        public double Lat { get; set; }
        [Required]
        public double Lang { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public DateOnly? DateOfService { get; set; }
        [Required(ErrorMessage = "Phone number is required")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Phone number must be 10 digits long")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Phone number must contain only numbers")]
        public string? Phone { get; set; }

        public int physicianId { get; set; }
        public string resetPassword { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Email is Invalid")]
        public string? ProviderEmail { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [Compare("ProviderEmail",ErrorMessage = "Email And Confirm Email is Not Equal")]
        public string? SynEmail { get; set; }

        public string Address1 { get; set; }
        public string? Address2 { get; set; }
        [Required]
        [Range(100000, 999999, ErrorMessage = "Zip code must be a 6-digit number")]
        public long Zipcode { get; set; }
        [Required]
        public int State { get; set; }
        [Required]
        public string? City { get; set; }
        [Required(ErrorMessage = "Phone number is required")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Phone number must be 10 digits long")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Phone number must contain only numbers")]
        public string? MailingNumber { get; set; }

        public string BusinessName { get; set; }
        public string BusinessWebsite { get; set; }
        public IFormFile? photo { get; set; }
        public IFormFile? IndependentContractorManagement { get; set; }
        public IFormFile? BackgroungCheck { get; set; }
        public IFormFile? HIPAA { get; set; }
        public IFormFile? NondisclosureAggrement { get; set; }
        public IFormFile? Signature { get; set; }
        public IFormFile? License { get; set; }

        public string? photoPath { get; set; }
        public string? SignaturePath { get; set; }
        public string? IndependentContractorManagementPath { get; set; }
        public string? BackgroungCheckPath { get; set; }
        public string? HIPAAPath { get; set; }
        public string? NondisclosureAggrementPath { get; set; }
        public string? LicensePath { get; set; }

        public string? AdminNotes { get; set; }
        public string selectedRegion {  get; set; }
        [Required(ErrorMessage = "Role Selection Is Required")]
        public int selectrole { get; set; }
        public bool IsAccessToEdit { get; set; }
        public List<Role> roles { get; set; }
        public List<Region> regions { get; set; }
        public List<int> regionList { get; set; }
        public List<RequestWiseFile> PatientDocuments { get; set; }
    }
}

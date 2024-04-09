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

        public string? Firstname { get; set; }
        public string? Lastname { get; set; }
        public string MediacalLicense { get; set; }
        public long? NPI { get; set; }
        public string Location { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public DateOnly? DateOfService { get; set; }
        public long? Phone { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Email is Invalid")]
        public string? Email { get; set; }

        public string? SynEmail { get; set; }

        public string? Address1 { get; set; }
        public string? Address2 { get; set; }
        public long? Zipcode { get; set; }
        public string? State { get; set; }
        public string? City { get; set; }
        public long? MailingNumber { get; set; }

        public string BusinessName { get; set; }
        public string BusinessEmail { get; set; }
        public IFormFile photo { get; set; }
        public IFormFile sign { get; set; }
        public string AdminNotes { get; set; }


        public List<RequestWiseFile> PatientDocuments { get; set; }
    }
}

using hellodoc.DbEntity.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hellodoc.DbEntity.ViewModels
{
    public partial class AdminProfileModal
    {
        public string patientName { get; set; }
        public string? confirmationnumber { get; set; }
        public int requestid { get; set; }
        public int status { get; set; }
        public int aspid { get; set; }
        public string role { get; set; }
        public string? password { get; set; }
        public string username { get; set; }

        public string? Firstname { get; set; }
        public string? Lastname { get; set; }
        public string Location { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public DateOnly? DateOfService { get; set; }
        public long? Phone { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Email is Invalid")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Confirm Email is Required")]
        [Compare("Email", ErrorMessage = "Confirm Email Address do not Match Email")]
        public string? ConfirmEmail { get; set; }
       
        public string? Address1 { get; set; }
        public string? Address2 { get; set; }
        public long? Zipcode { get; set; }
        public string? State { get; set; }
        public string? City { get; set; }
        public long? MailingNumber { get; set; }

        public List<RequestWiseFile> PatientDocuments { get; set; }
    }
}

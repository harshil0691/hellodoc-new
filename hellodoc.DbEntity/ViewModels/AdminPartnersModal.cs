using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using hellodoc.DbEntity.DataModels;

namespace hellodoc.DbEntity.ViewModels
{
    public partial class AdminPartnersModal
    {
        public int Vendorid { get; set; }
        [StringLength(16, ErrorMessage = "The field must be no more than 16 characters.")]
        [Required]
        public string? Vendorname { get; set; }
        [Required]
        public string? ProfessionName { get; set; }
        [Required]
        public string? Faxnumber { get; set; } 
        public string? Address { get; set; }
        [Required]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Business Contact must be 10 digits long")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Business Contact must contain only numbers")]
        public string? Phonenumber { get; set; }
        public BitArray? Isdeleted { get; set; }
        public string? Ip { get; set; }
        [Required]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string BusinessEmail { get; set; }
        [Required]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Business Contact must be 10 digits long")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Business Contact must contain only numbers")]
        public string? Businesscontact { get; set; }
        [Required]
        public int Profession { get; set; }
        [Required]
        public int State { get; set; }
        [Required]
        public string? City { get; set; }   
        public string? Street { get; set; }
        [Range(100000, 999999, ErrorMessage = "Zip code must be a 6-digit number")]
        [Required]
        public int Zip { get; set; }
        public string? actionType { get; set; }
        public List<HealthProfessionalType>? healthProfessionalTypes;
        public List<Region>? regions;
    }
}

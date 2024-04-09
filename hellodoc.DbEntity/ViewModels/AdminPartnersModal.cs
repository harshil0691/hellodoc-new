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
        public string Vendorname { get; set; } 
        public string? ProfessionName { get; set; }
        public string Faxnumber { get; set; } 
        public string? Address { get; set; }
        public string? Phonenumber { get; set; }
        public BitArray? Isdeleted { get; set; }
        public string? Ip { get; set; }
        public string? Email { get; set; }
        public string? Businesscontact { get; set; }
        public int Profession { get; set; }
        public string State { get; set; }
        public string City { get; set; }   
        public string Street { get; set; }
        public int Zip { get; set; }
        public string actionType { get; set; }
        public List<HealthProfessionalType> healthProfessionalTypes;
    }
}

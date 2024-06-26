﻿using hellodoc.DbEntity.DataModels;
using hellodoc.DbEntity.ViewModels.DocumentModal;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security;

namespace hellodoc.DbEntity.ViewModels
{
    public partial class RequestFormModal
    {
        public string RequestType { get; set; }
        public int userid { get; set; }
        public string RequestCreatedBy { get; set; }
        public int PhysicianId { get; set; }
        public DateTime DOB { get; set; }
        public int Requestid { get; set; }
        [Required]
        public string Firstname { get; set; } = null!;
        [Required]
        public string Lastname { get; set; } = null!;
        [Required]
        public DateOnly? DateOfBirth { get; set; }
        public string Symptoms { get; set; }
        [Required]
        [EmailAddress(ErrorMessage = "email is required")]
        public string PatientEmail { get; set; }
        [Required]
        [EmailAddress(ErrorMessage = "email is required")]
        public string UserEmail { get; set; }
        [Required(ErrorMessage = "Phone number is required")]
        [RegularExpression(@"^[1-9]\d{9}$", ErrorMessage = "Invalid phone number format. Must not start with 0 and must be exactly 10 digits.")]
        public long Phonenumber { get; set; }
        public string Street { get; set; } = null!;
        public string City { get; set; } = null!;
        [Required]
        public int State { get; set; }
        [Required(ErrorMessage = "ZIP code is required")]
        [RegularExpression(@"^[1-9]\d{5}$", ErrorMessage = "Invalid ZIP code format. Must be exactly 6 digits.")]
        public long Zipcode { get; set; }
        public string? Roomno { get; set; }
        [Required(ErrorMessage = "Password is required")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z]).+$", ErrorMessage = "Password must contain at least one lowercase letter and one uppercase letter")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Confirm Password is Required")]
        [Compare("Password", ErrorMessage = "Confirm Password Address do not Match Password")]
        public string Conform_password { get; set; }
        public IFormFile? Doc { get; set; }

        public string? Confirmationnumber { get; set; }
        public string requesttypeid { get; set; }
        public string bgcolor { get; set; }
        public string btext { get; set; }
        public int activeid { get; set; }

        public string AdminNotes { get; set; }
        public string PhysicianNotes { get; set; }

        public List<RequestTableModel> requestTable { get; set; }

        public List<ShowDocModal> patientDocuments { get; set; }

        public List<RequestWiseFile> RequestWiseFiles { get; set; }

        public User users { get; set; }
        public List<Region> regions { get; set; }   

        public string B_Firstname { get; set; } = null!;

        public string B_Lastname { get; set; } = null!;

        public string B_Email { get; set; } = null!;

        public long B_Phonenumber { get; set; }

        public string B_BusinessName { get; set; }

        public string B_CaseNo { get; set; }

        
        public string C_Firstname { get; set; } = null!;

        public string C_Lastname { get; set; } = null!;

        public string C_Email { get; set; } = null!;

        public long C_Phonenumber { get; set; }

        public string C_PropertyName { get; set; }

        public string C_Street { get; set; } = null!;

        public string C_City { get; set; } = null!;

        public string C_State { get; set; } = null!;

        public long C_Zipcode { get; set; }


        public string F_Firstname { get; set; } = null!;

        public string F_Lastname { get; set; } = null!;

        public string F_Email { get; set; } = null!;

        public long F_Phonenumber { get; set; }

        public string F_RelationType { get; set; }
        public string BackActionName { get; set; }

    }
}

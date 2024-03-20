using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using hellodoc.DbEntity.DataModels;
using System.Runtime.Serialization;
using hellodoc.DbEntity.ViewModels.DocumentModal;

namespace hellodoc.DbEntity.ViewModels;

public partial class PatientReqModel
{
    [Column("requestid")]
    public int Requestid { get; set; }

    [Required]

    public string Firstname { get; set; } = null!;

    [Required]
    [Column("lastname", TypeName = "character varying")]
    public string Lastname { get; set; } = null!;
    [Required]
    [Column("dateOfBirth")]
    public DateOnly? DateOfBirth { get; set; }
    [Required]
    [Column("symptoms")]
    [StringLength(256)]
    public string Symptoms { get; set; } = null!;
    [Required]
    [Column("email", TypeName = "character varying")]
    public string Email { get; set; } = null!;
    [Required]
    [Column("phonenumber")]
    public long Phonenumber { get; set; }
    [Required]
    [Column("street", TypeName = "character varying")]
    public string Street { get; set; } = null!;
    [Required]
    [Column("city", TypeName = "character varying")]
    public string City { get; set; } = null!;
    [Required]
    [Column("state", TypeName = "character varying")]
    public string State { get; set; } = null!;
    [Required]
    [Column("zipcode")]
    public long Zipcode { get; set; }
    [Required]
    [Column("roomno", TypeName = "character varying")]
    public string? Roomno { get; set; }

    [Required]
    public string Password { get; set; }

    [Required]
    public string Conform_password { get; set; }

    [Column("document1")]
    public IFormFile Doc { get; set; }

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
}

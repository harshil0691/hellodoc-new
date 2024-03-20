using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace hellodoc.DbEntity.ViewModels;

public partial class FriendReqModel
{
    [Required]
    public string F_Firstname { get; set; } = null!;

    [Required]
    [Column("lastname", TypeName = "character varying")]
    public string F_Lastname { get; set; } = null!;

    [Required]
    [Column("email", TypeName = "character varying")]
    public string F_Email { get; set; } = null!;

    [Required]
    [Column("phonenumber")]
    public long F_Phonenumber { get; set; }


    [Required]
    [Column("relationtype")]
    public string F_RelationType { get; set;}


    [Required]
    public string Firstname { get; set; } = null!;

    [Required]
    [Column("lastname", TypeName = "character varying")]
    public string Lastname { get; set; } = null!;
    [Required]
    [Column("dateOfBirth")]
    public DateOnly DateOfBirth { get; set; }
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


    [Column("document1")]
    public IFormFile Doc { get; set; }
}

﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace hellodoc.DbEntity.DataModels;

[Index("Username", Name = "AspNetUsers_username_key", IsUnique = true)]
public partial class AspNetUser
{
    [Key]
    [Column("id")]
    public short Id { get; set; }

    [Column("username")]
    [StringLength(256)]
    public string Username { get; set; } = null!;

    [Column("passwordhash", TypeName = "character varying")]
    public string? Passwordhash { get; set; }

    [Column("email")]
    [StringLength(256)]
    public string? Email { get; set; }

    [Column("phonenumber")]
    public long? Phonenumber { get; set; }

    [Column("ip")]
    [StringLength(20)]
    public string? Ip { get; set; }

    [Column("createddate", TypeName = "timestamp without time zone")]
    public DateTime? Createddate { get; set; }

    [InverseProperty("Aspnetuser")]
    public virtual ICollection<Physician> Physicians { get; } = new List<Physician>();

    [InverseProperty("Aspnetuser")]
    public virtual ICollection<User> Users { get; } = new List<User>();
}
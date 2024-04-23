using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace hellodoc.DataModels;

[PrimaryKey("LUsername", "LRole", "LPassward", "LEmail")]
[Table("login")]
public partial class Login
{
    [Key]
    [Column("L_username", TypeName = "character varying[]")]
    public string[] LUsername { get; set; } = null!;

    [Key]
    [Column("L_email")]
    public char[] LEmail { get; set; } = null!;

    [Key]
    [Column("L_passward ")]
    public byte[][] LPassward { get; set; } = null!;

    [Key]
    [Column("L_role")]
    public byte[] LRole { get; set; } = null!;
}

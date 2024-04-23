using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace hellodoc.DataModels;

public partial class AspNetUserRole
{
    [Key]
    [Column("userid")]
    public int Userid { get; set; }

    [Column("roleid")]
    public int Roleid { get; set; }

    [ForeignKey("Roleid")]
    [InverseProperty("AspNetUserRoles")]
    public virtual Role Role { get; set; } = null!;

    [ForeignKey("Userid")]
    [InverseProperty("AspNetUserRole")]
    public virtual AspNetUser User { get; set; } = null!;
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace hellodoc.DbEntity.DataModels;

[Table("RoleMenu")]
public partial class RoleMenu
{
    [Key]
    [Column("rolemenuid")]
    public int Rolemenuid { get; set; }

    [Column("roleid")]
    public int Roleid { get; set; }

    [Column("menuid")]
    public int Menuid { get; set; }

    [ForeignKey("Menuid")]
    [InverseProperty("RoleMenus")]
    public virtual Menu Menu { get; set; } = null!;

    [ForeignKey("Roleid")]
    [InverseProperty("RoleMenus")]
    public virtual Role Role { get; set; } = null!;
}

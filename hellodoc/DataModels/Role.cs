using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace hellodoc.DataModels;

[Table("Role")]
public partial class Role
{
    [Key]
    [Column("roleid")]
    public int Roleid { get; set; }

    [Column("name")]
    [StringLength(50)]
    public string Name { get; set; } = null!;

    [Column("accounttype")]
    public int Accounttype { get; set; }

    [Column("createdby")]
    [StringLength(128)]
    public string Createdby { get; set; } = null!;

    [Column("createddate", TypeName = "timestamp without time zone")]
    public DateTime Createddate { get; set; }

    [Column("modifiedby")]
    [StringLength(128)]
    public string? Modifiedby { get; set; }

    [Column("modifieddate", TypeName = "timestamp without time zone")]
    public DateTime? Modifieddate { get; set; }

    [Column("isdeleted")]
    public short? Isdeleted { get; set; }

    [Column("ip")]
    [StringLength(20)]
    public string? Ip { get; set; }

    [ForeignKey("Accounttype")]
    [InverseProperty("Roles")]
    public virtual AspNetRole AccounttypeNavigation { get; set; } = null!;

    [InverseProperty("Role")]
    public virtual ICollection<Admin> Admins { get; } = new List<Admin>();

    [InverseProperty("Role")]
    public virtual ICollection<AspNetUserRole> AspNetUserRoles { get; } = new List<AspNetUserRole>();

    [InverseProperty("Role")]
    public virtual ICollection<Physician> Physicians { get; } = new List<Physician>();

    [InverseProperty("Role")]
    public virtual ICollection<RoleMenu> RoleMenus { get; } = new List<RoleMenu>();
}

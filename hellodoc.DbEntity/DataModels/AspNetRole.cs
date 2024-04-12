using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace hellodoc.DbEntity.DataModels;

public partial class AspNetRole
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("name")]
    [StringLength(256)]
    public string Name { get; set; } = null!;

    [InverseProperty("Role")]
    public virtual ICollection<EmailLog> EmailLogs { get; } = new List<EmailLog>();

    [InverseProperty("AccounttypeNavigation")]
    public virtual ICollection<Role> Roles { get; } = new List<Role>();

    [InverseProperty("Role")]
    public virtual ICollection<Smslog> Smslogs { get; } = new List<Smslog>();
}

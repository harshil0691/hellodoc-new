using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace hellodoc.DataModels;

[Table("Admin")]
public partial class Admin
{
    [Key]
    [Column("adminid")]
    public int Adminid { get; set; }

    [Column("aspnetuserid")]
    public int Aspnetuserid { get; set; }

    [Column("firstname")]
    [StringLength(100)]
    public string Firstname { get; set; } = null!;

    [Column("lastname")]
    [StringLength(100)]
    public string? Lastname { get; set; }

    [Column("email")]
    [StringLength(50)]
    public string Email { get; set; } = null!;

    [Column("mobile")]
    public long? Mobile { get; set; }

    [Column("address1")]
    [StringLength(500)]
    public string? Address1 { get; set; }

    [Column("address2")]
    [StringLength(500)]
    public string? Address2 { get; set; }

    [Column("city")]
    [StringLength(100)]
    public string? City { get; set; }

    [Column("regionid")]
    public int? Regionid { get; set; }

    [Column("zip")]
    public long? Zip { get; set; }

    [Column("altphone")]
    public long? Altphone { get; set; }

    [Column("createdby")]
    [StringLength(128)]
    public string Createdby { get; set; } = null!;

    [Column("createddate")]
    public DateOnly Createddate { get; set; }

    [Column("modifiedby")]
    [StringLength(128)]
    public string? Modifiedby { get; set; }

    [Column("modifieddate")]
    public DateOnly? Modifieddate { get; set; }

    [Column("status")]
    public short? Status { get; set; }

    [Column("isdeleted", TypeName = "bit(1)")]
    public BitArray? Isdeleted { get; set; }

    [Column("roleid")]
    public int? Roleid { get; set; }

    [InverseProperty("Admin")]
    public virtual ICollection<AdminRegion> AdminRegions { get; } = new List<AdminRegion>();

    [ForeignKey("Aspnetuserid")]
    [InverseProperty("Admins")]
    public virtual AspNetUser Aspnetuser { get; set; } = null!;

    [InverseProperty("Admin")]
    public virtual ICollection<EmailLog> EmailLogs { get; } = new List<EmailLog>();

    [InverseProperty("Admin")]
    public virtual ICollection<NotificationMessage> NotificationMessages { get; } = new List<NotificationMessage>();

    [ForeignKey("Regionid")]
    [InverseProperty("Admins")]
    public virtual Region? Region { get; set; }

    [InverseProperty("Admin")]
    public virtual ICollection<RequestStatusLog> RequestStatusLogs { get; } = new List<RequestStatusLog>();

    [ForeignKey("Roleid")]
    [InverseProperty("Admins")]
    public virtual Role? Role { get; set; }

    [InverseProperty("Admin")]
    public virtual ICollection<Smslog> Smslogs { get; } = new List<Smslog>();
}

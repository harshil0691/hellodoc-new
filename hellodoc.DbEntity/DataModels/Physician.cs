using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace hellodoc.DbEntity.DataModels;

[Table("Physician")]
public partial class Physician
{
    [Key]
    [Column("physicianid")]
    public int Physicianid { get; set; }

    [Column("aspnetuserid")]
    public int? Aspnetuserid { get; set; }

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

    [Column("medicallicense")]
    [StringLength(500)]
    public string? Medicallicense { get; set; }

    [Column("photo", TypeName = "character varying")]
    public string? Photo { get; set; }

    [Column("adminnotes")]
    [StringLength(500)]
    public string? Adminnotes { get; set; }

    [Column("isagreementdoc", TypeName = "character varying")]
    public string? Isagreementdoc { get; set; }

    [Column("isbackgrounddoc", TypeName = "character varying")]
    public string? Isbackgrounddoc { get; set; }

    [Column("istrainingdoc", TypeName = "character varying")]
    public string? Istrainingdoc { get; set; }

    [Column("isnondisclosuredoc", TypeName = "character varying")]
    public string? Isnondisclosuredoc { get; set; }

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
    [StringLength(20)]
    public string? Altphone { get; set; }

    [Column("createdby")]
    [StringLength(128)]
    public string? Createdby { get; set; }

    [Column("createddate", TypeName = "timestamp without time zone")]
    public DateTime? Createddate { get; set; }

    [Column("modifiedby")]
    [StringLength(128)]
    public string? Modifiedby { get; set; }

    [Column("modifieddate", TypeName = "timestamp without time zone")]
    public DateTime? Modifieddate { get; set; }

    [Column("status")]
    public short? Status { get; set; }

    [Column("businessname")]
    [StringLength(100)]
    public string? Businessname { get; set; }

    [Column("businesswebsite")]
    [StringLength(200)]
    public string? Businesswebsite { get; set; }

    [Column("isdeleted", TypeName = "bit(1)")]
    public BitArray? Isdeleted { get; set; }

    [Column("roleid")]
    public int? Roleid { get; set; }

    [Column("npinumber")]
    public long? Npinumber { get; set; }

    [Column("islicensedoc", TypeName = "character varying")]
    public string? Islicensedoc { get; set; }

    [Column("signature")]
    public string? Signature { get; set; }

    [Column("iscredentialdoc", TypeName = "character varying")]
    public string? Iscredentialdoc { get; set; }

    [Column("istokengenerate", TypeName = "bit(1)")]
    public BitArray? Istokengenerate { get; set; }

    [Column("syncemailaddress")]
    [StringLength(50)]
    public string? Syncemailaddress { get; set; }

    [ForeignKey("Aspnetuserid")]
    [InverseProperty("Physicians")]
    public virtual AspNetUser? Aspnetuser { get; set; }

    [InverseProperty("Physician")]
    public virtual ICollection<EmailLog> EmailLogs { get; } = new List<EmailLog>();

    [InverseProperty("Physician")]
    public virtual ICollection<Invoicing> Invoicings { get; } = new List<Invoicing>();

    [InverseProperty("Physician")]
    public virtual ICollection<NotificationMessage> NotificationMessages { get; } = new List<NotificationMessage>();

    [InverseProperty("Physician")]
    public virtual ICollection<Payrate> Payrates { get; } = new List<Payrate>();

    [InverseProperty("Physician")]
    public virtual ICollection<PhysicianLocation> PhysicianLocations { get; } = new List<PhysicianLocation>();

    [InverseProperty("Physician")]
    public virtual ICollection<PhysicianNotification> PhysicianNotifications { get; } = new List<PhysicianNotification>();

    [InverseProperty("Physician")]
    public virtual ICollection<PhysicianRegion> PhysicianRegions { get; } = new List<PhysicianRegion>();

    [ForeignKey("Regionid")]
    [InverseProperty("Physicians")]
    public virtual Region? Region { get; set; }

    [InverseProperty("Physician")]
    public virtual ICollection<RequestStatusLog> RequestStatusLogs { get; } = new List<RequestStatusLog>();

    [InverseProperty("Physician")]
    public virtual ICollection<Request> Requests { get; } = new List<Request>();

    [ForeignKey("Roleid")]
    [InverseProperty("Physicians")]
    public virtual Role? Role { get; set; }

    [InverseProperty("Physician")]
    public virtual ICollection<Shift> Shifts { get; } = new List<Shift>();

    [InverseProperty("Physician")]
    public virtual ICollection<Smslog> Smslogs { get; } = new List<Smslog>();
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace hellodoc.DbEntity.DataModels;

[Table("EmailLog")]
public partial class EmailLog
{
    [Key]
    [Column("emaillogid")]
    public int Emaillogid { get; set; }

    [Column("emailtemplate", TypeName = "character varying")]
    public string Emailtemplate { get; set; } = null!;

    [Column("subjectname")]
    [StringLength(200)]
    public string Subjectname { get; set; } = null!;

    [Column("emailid")]
    [StringLength(200)]
    public string Emailid { get; set; } = null!;

    [Column("confirmationnumber")]
    [StringLength(200)]
    public string? Confirmationnumber { get; set; }

    [Column("filepath", TypeName = "character varying")]
    public string? Filepath { get; set; }

    [Column("roleid")]
    public int? Roleid { get; set; }

    [Column("requestid")]
    public int? Requestid { get; set; }

    [Column("adminid")]
    public int? Adminid { get; set; }

    [Column("physicianid")]
    public int? Physicianid { get; set; }

    [Column("createdate", TypeName = "timestamp without time zone")]
    public DateTime Createdate { get; set; }

    [Column("sentdate", TypeName = "timestamp without time zone")]
    public DateTime? Sentdate { get; set; }

    [Column("isemailsent")]
    public int? Isemailsent { get; set; }

    [Column("senttries")]
    public int? Senttries { get; set; }

    [Column("action")]
    public int? Action { get; set; }

    [ForeignKey("Adminid")]
    [InverseProperty("EmailLogs")]
    public virtual Admin? Admin { get; set; }

    [ForeignKey("Physicianid")]
    [InverseProperty("EmailLogs")]
    public virtual Physician? Physician { get; set; }

    [ForeignKey("Requestid")]
    [InverseProperty("EmailLogs")]
    public virtual Request? Request { get; set; }

    [ForeignKey("Roleid")]
    [InverseProperty("EmailLogs")]
    public virtual AspNetRole? Role { get; set; }
}

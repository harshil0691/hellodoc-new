using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace hellodoc.DbEntity.DataModels;

[Table("RequestStatusLog")]
public partial class RequestStatusLog
{
    [Key]
    [Column("requeststatuslogid")]
    public int Requeststatuslogid { get; set; }

    [Column("requestid")]
    public int Requestid { get; set; }

    [Column("status")]
    public int Status { get; set; }

    [Column("physicianid")]
    public int? Physicianid { get; set; }

    [Column("adminid")]
    public int? Adminid { get; set; }

    [Column("transtophysicianid")]
    public int? Transtophysicianid { get; set; }

    [Column("notes")]
    [StringLength(500)]
    public string? Notes { get; set; }

    [Column("createddate", TypeName = "timestamp without time zone")]
    public DateTime Createddate { get; set; }

    [Column("ip")]
    [StringLength(20)]
    public string? Ip { get; set; }

    [Column("transtoadmin", TypeName = "bit(1)")]
    public BitArray? Transtoadmin { get; set; }

    [ForeignKey("Adminid")]
    [InverseProperty("RequestStatusLogs")]
    public virtual Admin? Admin { get; set; }

    [ForeignKey("Physicianid")]
    [InverseProperty("RequestStatusLogs")]
    public virtual Physician? Physician { get; set; }

    [ForeignKey("Requestid")]
    [InverseProperty("RequestStatusLogs")]
    public virtual Request Request { get; set; } = null!;
}

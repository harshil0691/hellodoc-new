using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace hellodoc.DbEntity.DataModels;

[Table("Invoicing")]
public partial class Invoicing
{
    [Key]
    [Column("invoicingid")]
    public int Invoicingid { get; set; }

    [Column("physicianid")]
    public int? Physicianid { get; set; }

    [Column("fromdate")]
    public DateOnly? Fromdate { get; set; }

    [Column("todate")]
    public DateOnly? Todate { get; set; }

    [Column("isapproved", TypeName = "bit(1)")]
    public BitArray? Isapproved { get; set; }

    [Column("approvedby")]
    public int? Approvedby { get; set; }

    [Column("aproveddate")]
    public DateOnly? Aproveddate { get; set; }

    [Column("isfinalized", TypeName = "bit(1)")]
    public BitArray? Isfinalized { get; set; }

    [Column("finalizedby")]
    public int? Finalizedby { get; set; }

    [Column("finalizeddate")]
    public DateOnly? Finalizeddate { get; set; }

    [Column("createdby")]
    public int? Createdby { get; set; }

    [Column("createddate")]
    public DateOnly? Createddate { get; set; }

    [Column("modifiedby")]
    public int? Modifiedby { get; set; }

    [Column("modifieddate")]
    public DateOnly? Modifieddate { get; set; }

    [Column("monthnumber")]
    public int? Monthnumber { get; set; }

    [Column("monthhalf")]
    public int? Monthhalf { get; set; }

    [Column("year")]
    public int? Year { get; set; }

    [ForeignKey("Physicianid")]
    [InverseProperty("Invoicings")]
    public virtual Physician? Physician { get; set; }

    [InverseProperty("Invoicing")]
    public virtual ICollection<Timesheet> Timesheets { get; } = new List<Timesheet>();
}

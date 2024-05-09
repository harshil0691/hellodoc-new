using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace hellodoc.DbEntity.DataModels;

[Table("Payrate")]
public partial class Payrate
{
    [Key]
    [Column("payrateid")]
    public int Payrateid { get; set; }

    [Column("physicianid")]
    public int? Physicianid { get; set; }

    [Column("nightshiftweekend")]
    public int? Nightshiftweekend { get; set; }

    [Column("shift")]
    public int? Shift { get; set; }

    [Column("housecallnightweekend")]
    public int? Housecallnightweekend { get; set; }

    [Column("phonecounsults")]
    public int? Phonecounsults { get; set; }

    [Column("phonecounsultsnightweekend")]
    public int? Phonecounsultsnightweekend { get; set; }

    [Column("batchtesting")]
    public int? Batchtesting { get; set; }

    [Column("housecall")]
    public int? Housecall { get; set; }

    [Column("modifiedby", TypeName = "character varying")]
    public string? Modifiedby { get; set; }

    [Column("modifieddate")]
    public DateOnly? Modifieddate { get; set; }

    [ForeignKey("Physicianid")]
    [InverseProperty("Payrates")]
    public virtual Physician? Physician { get; set; }
}

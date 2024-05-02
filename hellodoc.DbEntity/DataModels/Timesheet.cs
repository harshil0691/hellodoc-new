using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace hellodoc.DbEntity.DataModels;

[Table("Timesheet")]
public partial class Timesheet
{
    [Key]
    [Column("timesheetid")]
    public int Timesheetid { get; set; }

    [Column("date")]
    public DateOnly? Date { get; set; }

    [Column("oncallhours")]
    public int? Oncallhours { get; set; }

    [Column("totalhours")]
    public int? Totalhours { get; set; }

    [Column(TypeName = "bit(1)")]
    public BitArray? Isweekendorholiday { get; set; }

    [Column("housecalls")]
    public int? Housecalls { get; set; }

    [Column("phoneconsults")]
    public long? Phoneconsults { get; set; }

    [Column("item", TypeName = "character varying")]
    public string? Item { get; set; }

    [Column("amount")]
    public int? Amount { get; set; }

    [Column("billdoc")]
    public string? Billdoc { get; set; }

    [Column("invoicingid")]
    public int? Invoicingid { get; set; }

    [ForeignKey("Invoicingid")]
    [InverseProperty("Timesheets")]
    public virtual Invoicing? Invoicing { get; set; }
}

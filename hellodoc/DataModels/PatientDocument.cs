using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace hellodoc.DataModels;

[Keyless]
public partial class PatientDocument
{
    [Column("requestwisefileid")]
    public short? Requestwisefileid { get; set; }

    [Column("requestid")]
    public int? Requestid { get; set; }

    [Column("docdate", TypeName = "timestamp without time zone")]
    public DateTime? Docdate { get; set; }

    [Column("firstname")]
    [StringLength(100)]
    public string? Firstname { get; set; }

    [Column("userid")]
    public int? Userid { get; set; }

    [Column("filename")]
    [StringLength(500)]
    public string? Filename { get; set; }

    [Column("status")]
    public short? Status { get; set; }

    [Column("createddate", TypeName = "timestamp without time zone")]
    public DateTime? Createddate { get; set; }

    [Column("document1")]
    public byte[]? Document1 { get; set; }
}

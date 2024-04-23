using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace hellodoc.DataModels;

[Table("RequestClosed")]
public partial class RequestClosed
{
    [Key]
    [Column("requestclosedid")]
    public short Requestclosedid { get; set; }

    [Column("requestid")]
    public int Requestid { get; set; }

    [Column("requeststatuslogid")]
    public short Requeststatuslogid { get; set; }

    [Column("phynotes")]
    [StringLength(500)]
    public string? Phynotes { get; set; }

    [Column("clientnotes")]
    [StringLength(500)]
    public string? Clientnotes { get; set; }

    [Column("ip")]
    [StringLength(20)]
    public string? Ip { get; set; }
}

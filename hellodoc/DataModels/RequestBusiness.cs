using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace hellodoc.DataModels;

[Table("RequestBusiness")]
public partial class RequestBusiness
{
    [Key]
    [Column("requestbusinessid")]
    public short Requestbusinessid { get; set; }

    [Column("requestid")]
    public int Requestid { get; set; }

    [Column("businessid")]
    public short Businessid { get; set; }

    [Column("ip")]
    [StringLength(20)]
    public string? Ip { get; set; }

    [ForeignKey("Businessid")]
    [InverseProperty("RequestBusinesses")]
    public virtual Business Business { get; set; } = null!;

    [ForeignKey("Requestid")]
    [InverseProperty("RequestBusinesses")]
    public virtual Request Request { get; set; } = null!;
}

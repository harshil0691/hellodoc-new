﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace hellodoc.DbEntity.DataModels;

[Table("RequestConcierge")]
public partial class RequestConcierge
{
    [Key]
    [Column("id")]
    public short Id { get; set; }

    [Column("requestid")]
    public int Requestid { get; set; }

    [Column("conciergeid")]
    public short Conciergeid { get; set; }

    [Column("ip")]
    [StringLength(20)]
    public string? Ip { get; set; }

    [ForeignKey("Conciergeid")]
    [InverseProperty("RequestConcierges")]
    public virtual Concierge Concierge { get; set; } = null!;

    [ForeignKey("Requestid")]
    [InverseProperty("RequestConcierges")]
    public virtual Request Request { get; set; } = null!;
}

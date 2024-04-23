﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace hellodoc.DataModels;

[Table("PhysicianLocation")]
public partial class PhysicianLocation
{
    [Key]
    [Column("locationid")]
    public int Locationid { get; set; }

    [Column("physicianid")]
    public int Physicianid { get; set; }

    [Column("latitude")]
    public double? Latitude { get; set; }

    [Column("longitude")]
    public double? Longitude { get; set; }

    [Column("createddate", TypeName = "timestamp without time zone")]
    public DateTime? Createddate { get; set; }

    [Column("physicianname")]
    [StringLength(50)]
    public string? Physicianname { get; set; }

    [Column("address")]
    [StringLength(500)]
    public string? Address { get; set; }

    [ForeignKey("Physicianid")]
    [InverseProperty("PhysicianLocations")]
    public virtual Physician Physician { get; set; } = null!;
}

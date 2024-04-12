using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace hellodoc.DbEntity.DataModels;

[Table("Region")]
public partial class Region
{
    [Key]
    [Column("regionid")]
    public int Regionid { get; set; }

    [Column("name")]
    [StringLength(50)]
    public string Name { get; set; } = null!;

    [Column("abbreviation")]
    [StringLength(50)]
    public string? Abbreviation { get; set; }

    [InverseProperty("Region")]
    public virtual ICollection<AdminRegion> AdminRegions { get; } = new List<AdminRegion>();

    [InverseProperty("Region")]
    public virtual ICollection<Admin> Admins { get; } = new List<Admin>();

    [InverseProperty("Region")]
    public virtual ICollection<PhysicianRegion> PhysicianRegions { get; } = new List<PhysicianRegion>();

    [InverseProperty("Region")]
    public virtual ICollection<Physician> Physicians { get; } = new List<Physician>();

    [InverseProperty("Region")]
    public virtual ICollection<ShiftDetail> ShiftDetails { get; } = new List<ShiftDetail>();
}

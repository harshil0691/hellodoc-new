using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace hellodoc.DataModels;

[Table("Concierge")]
public partial class Concierge
{
    [Key]
    [Column("conciergeid")]
    public short Conciergeid { get; set; }

    [Column("conciergename")]
    [StringLength(100)]
    public string Conciergename { get; set; } = null!;

    [Column("address")]
    [StringLength(150)]
    public string? Address { get; set; }

    [Column("street")]
    [StringLength(50)]
    public string Street { get; set; } = null!;

    [Column("city")]
    [StringLength(50)]
    public string City { get; set; } = null!;

    [Column("state")]
    [StringLength(50)]
    public string State { get; set; } = null!;

    [Column("zipcode")]
    public long Zipcode { get; set; }

    [Column("createddate", TypeName = "timestamp without time zone")]
    public DateTime Createddate { get; set; }

    [Column("regionid")]
    public short Regionid { get; set; }

    [Column("roleid")]
    [StringLength(20)]
    public string? Roleid { get; set; }

    [InverseProperty("Concierge")]
    public virtual ICollection<RequestConcierge> RequestConcierges { get; } = new List<RequestConcierge>();
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace hellodoc.DataModels;

[Table("CaseTag")]
public partial class CaseTag
{
    [Key]
    [Column("casetagid")]
    public int Casetagid { get; set; }

    [Column("name")]
    [StringLength(50)]
    public string Name { get; set; } = null!;

    [InverseProperty("CasetagNavigation")]
    public virtual ICollection<Request> Requests { get; } = new List<Request>();
}

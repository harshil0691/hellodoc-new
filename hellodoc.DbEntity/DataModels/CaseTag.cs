using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace hellodoc.DbEntity.DataModels;

[Table("CaseTag")]
public partial class CaseTag
{
    [Key]
    [Column("casetagid")]
    public int Casetagid { get; set; }

    [Column("name")]
    [StringLength(50)]
    public string Name { get; set; } = null!;
}

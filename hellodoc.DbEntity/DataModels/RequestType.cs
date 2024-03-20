using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace hellodoc.DbEntity.DataModels;

[Table("RequestType")]
public partial class RequestType
{
    [Key]
    [Column("requesttypeid")]
    public short Requesttypeid { get; set; }

    [Column("name")]
    [StringLength(50)]
    public string Name { get; set; } = null!;
}

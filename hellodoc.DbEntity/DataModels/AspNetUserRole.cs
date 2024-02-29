using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace hellodoc.DbEntity.DataModels;

[Keyless]
public partial class AspNetUserRole
{
    [Column("userid")]
    [StringLength(128)]
    public string Userid { get; set; } = null!;

    [Column("roleid")]
    [StringLength(128)]
    public string Roleid { get; set; } = null!;
}

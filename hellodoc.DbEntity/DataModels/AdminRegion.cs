using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace hellodoc.DbEntity.DataModels;

[Table("AdminRegion")]
public partial class AdminRegion
{
    [Key]
    [Column("adminregionid")]
    public int Adminregionid { get; set; }

    [Column("adminid")]
    public int Adminid { get; set; }

    [Column("regionid")]
    public int Regionid { get; set; }
}

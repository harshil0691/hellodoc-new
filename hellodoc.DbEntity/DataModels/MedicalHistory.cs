using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace hellodoc.DbEntity.DataModels;

[Table("medical_history")]
public partial class MedicalHistory
{
    [Key]
    public int Id { get; set; }

    public string Date { get; set; } = null!;

    public string Status { get; set; } = null!;

    [Column("document")]
    public bool Document { get; set; }
}

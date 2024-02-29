﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace hellodoc.DbEntity.DataModels;

[Table("PhysicianNotification")]
public partial class PhysicianNotification
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("physicianid")]
    public int Physicianid { get; set; }

    [Column("isnotificationstopped", TypeName = "bit(1)")]
    public BitArray Isnotificationstopped { get; set; } = null!;

    [ForeignKey("Physicianid")]
    [InverseProperty("PhysicianNotifications")]
    public virtual Physician Physician { get; set; } = null!;
}

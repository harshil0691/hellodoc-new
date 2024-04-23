using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace hellodoc.DataModels;

[Table("NotificationMessage")]
public partial class NotificationMessage
{
    [Key]
    [Column("notificationid")]
    public int Notificationid { get; set; }

    [Column("notification", TypeName = "character varying")]
    public string? Notification { get; set; }

    [Column("isclosed")]
    public short? Isclosed { get; set; }

    [Column("physicianid")]
    public int? Physicianid { get; set; }

    [Column("adminid")]
    public int? Adminid { get; set; }

    [Column("createdby")]
    public int? Createdby { get; set; }

    [Column("createdate")]
    public TimeOnly[]? Createdate { get; set; }

    [Column("aspetuserid")]
    public int? Aspetuserid { get; set; }

    [ForeignKey("Adminid")]
    [InverseProperty("NotificationMessages")]
    public virtual Admin? Admin { get; set; }

    [ForeignKey("Aspetuserid")]
    [InverseProperty("NotificationMessages")]
    public virtual AspNetUser? Aspetuser { get; set; }

    [ForeignKey("Physicianid")]
    [InverseProperty("NotificationMessages")]
    public virtual Physician? Physician { get; set; }
}

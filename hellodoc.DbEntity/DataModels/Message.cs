using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace hellodoc.DbEntity.DataModels;

[Table("Message")]
public partial class Message
{
    [Key]
    [Column("messageid")]
    public int Messageid { get; set; }

    [Column("chattype")]
    public int? Chattype { get; set; }

    [Column("senderaspid")]
    public int? Senderaspid { get; set; }

    [Column("recieveraspid")]
    public int? Recieveraspid { get; set; }

    [Column("message")]
    public string? Message1 { get; set; }

    [Column("senttime", TypeName = "timestamp without time zone")]
    public DateTime? Senttime { get; set; }

    [Column("sentfrom", TypeName = "character varying")]
    public string? Sentfrom { get; set; }

    [Column("requestid")]
    public int? Requestid { get; set; }

    [ForeignKey("Chattype")]
    [InverseProperty("Messages")]
    public virtual ChatType? ChattypeNavigation { get; set; }

    [ForeignKey("Recieveraspid")]
    [InverseProperty("MessageRecieverasps")]
    public virtual AspNetUser? Recieverasp { get; set; }

    [ForeignKey("Requestid")]
    [InverseProperty("Messages")]
    public virtual Request? Request { get; set; }

    [ForeignKey("Senderaspid")]
    [InverseProperty("MessageSenderasps")]
    public virtual AspNetUser? Senderasp { get; set; }
}

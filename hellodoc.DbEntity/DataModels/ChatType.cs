using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace hellodoc.DbEntity.DataModels;

[Table("ChatType")]
public partial class ChatType
{
    [Key]
    [Column("chattypeid")]
    public int Chattypeid { get; set; }

    [Column("chattype", TypeName = "character varying")]
    public string? Chattype1 { get; set; }

    [InverseProperty("ChattypeNavigation")]
    public virtual ICollection<Message> Messages { get; } = new List<Message>();
}

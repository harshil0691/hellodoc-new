using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace hellodoc.DbEntity.DataModels;

[Table("Encounter")]
public partial class Encounter
{
    [Key]
    [Column("encounterid")]
    public int Encounterid { get; set; }

    [Column("firstname", TypeName = "character varying")]
    public string? Firstname { get; set; }

    [Column("lastname", TypeName = "character varying")]
    public string? Lastname { get; set; }

    [Column("location", TypeName = "character varying")]
    public string? Location { get; set; }

    [Column("dateOfBirth")]
    public DateOnly? DateOfBirth { get; set; }

    [Column("dateOfService")]
    public DateOnly? DateOfService { get; set; }

    [Column("phone")]
    public long? Phone { get; set; }

    [Column("email", TypeName = "character varying")]
    public string? Email { get; set; }

    [Column("historyOfInjury", TypeName = "character varying")]
    public string? HistoryOfInjury { get; set; }

    [Column("medicalHistory", TypeName = "character varying")]
    public string? MedicalHistory { get; set; }

    [Column("medications", TypeName = "character varying")]
    public string? Medications { get; set; }

    [Column("allergies", TypeName = "character varying")]
    public string? Allergies { get; set; }

    [Column("temperature", TypeName = "character varying")]
    public string? Temperature { get; set; }

    [Column("HR")]
    public int? Hr { get; set; }

    [Column("RR")]
    public int? Rr { get; set; }

    [Column("bloodpressure1", TypeName = "character varying")]
    public string? Bloodpressure1 { get; set; }

    [Column("bloodpressure2", TypeName = "character varying")]
    public string? Bloodpressure2 { get; set; }

    [Column(TypeName = "character varying")]
    public string? O2 { get; set; }

    [Column("pain", TypeName = "character varying")]
    public string? Pain { get; set; }

    [Column("heent", TypeName = "character varying")]
    public string? Heent { get; set; }

    [Column("cv", TypeName = "character varying")]
    public string? Cv { get; set; }

    [Column("chest", TypeName = "character varying")]
    public string? Chest { get; set; }

    [Column("abd", TypeName = "character varying")]
    public string? Abd { get; set; }

    [Column("extr", TypeName = "character varying")]
    public string? Extr { get; set; }

    [Column("skin", TypeName = "character varying")]
    public string? Skin { get; set; }

    [Column("neuro", TypeName = "character varying")]
    public string? Neuro { get; set; }

    [Column("other", TypeName = "character varying")]
    public string? Other { get; set; }

    [Column("dignosis", TypeName = "character varying")]
    public string? Dignosis { get; set; }

    [Column("treatmentplan", TypeName = "character varying")]
    public string? Treatmentplan { get; set; }

    [Column("medicationsDispensed", TypeName = "character varying")]
    public string? MedicationsDispensed { get; set; }

    [Column("procedures", TypeName = "character varying")]
    public string? Procedures { get; set; }

    [Column("folloup", TypeName = "character varying")]
    public string? Folloup { get; set; }

    [Column("isfinalized")]
    public short? Isfinalized { get; set; }

    [Column("createdby", TypeName = "character varying")]
    public string? Createdby { get; set; }

    [Column("createddate", TypeName = "timestamp without time zone")]
    public DateTime? Createddate { get; set; }

    [Column("modifiedby", TypeName = "character varying")]
    public string? Modifiedby { get; set; }

    [Column("modifieddate", TypeName = "timestamp without time zone")]
    public DateTime? Modifieddate { get; set; }

    [Column("ip", TypeName = "character varying")]
    public string? Ip { get; set; }

    [Column("finalizedby", TypeName = "character varying")]
    public string? Finalizedby { get; set; }

    [Column("finalizeddate", TypeName = "timestamp without time zone")]
    public DateTime? Finalizeddate { get; set; }

    [Column("requestid")]
    public int? Requestid { get; set; }

    [ForeignKey("Requestid")]
    [InverseProperty("Encounters")]
    public virtual Request? Request { get; set; }
}

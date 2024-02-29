using hellodoc.DbEntity.DataModels;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hellodoc.DbEntity.ViewModels
{
    public partial class AdminDashModel
    {
        
        public int Requestid { get; set; }

        public short Requesttypeid { get; set; }

        public int? Userid { get; set; }

        public string? PatientName { get; set; }

        public DateOnly? DateOfBirth { get; set; }

        public string? RequestorName { get; set; }

        public long? Phonenumber_P { get; set; }
        public long? Phonenumber_R { get; set; }

        public string? Address { get; set; }

        public string? Notes { get; set; }

        public string? Email { get; set; }

        public short Status { get; set; }

        public int? Physicianid { get; set; }

        public string? Confirmationnumber { get; set; }

        public DateTime Createddate { get; set; }

        public BitArray? Isdeleted { get; set; }

        public DateTime? Modifieddate { get; set; }

        public int Requestclientid { get; set; }

    }
}

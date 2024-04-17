using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace hellodoc.DbEntity.ViewModels
{
    public partial class NotesModel
    {
        public short Requestnotesid { get; set; }

        public short Requeststatuslogid { get; set; }

        public int Requestid { get; set; }

        public string? Physiciannotes { get; set; }
        [Required]
        public string? Adminnotes { get; set; }

        public string? Createdby { get; set; }

        public DateTime? Createddate { get; set; }

        
        public string? Modifiedby { get; set; }

        
        public DateTime? Modifieddate { get; set; }

       
        public string? Ip { get; set; }

        
        public string? Administrativenotes { get; set; }

        
        public short Status { get; set; }

        
        public short? Physicianid { get; set; }

        
        public short? Adminid { get; set; }

        
        public short? Transtophysicianid { get; set; }

       
        public string? Notes { get; set; }
        public string? TransferNotes { get; set; }
        public string? PhysicianNotes { get; set; }

        public BitArray? Transtoadmin { get; set; }
    }
}

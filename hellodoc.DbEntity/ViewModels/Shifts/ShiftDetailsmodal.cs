using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using hellodoc.DbEntity.DataModels;

namespace hellodoc.DbEntity.ViewModels.Shifts
{
    public partial class ShiftDetailsmodal
    {
        public int Shiftid { get; set; }
        [Required]
        public int Physicianid { get; set; }
        public string PhysicianName { get; set; }
        public DateOnly Startdate { get; set; }
        public bool Isrepeat { get; set; }
        public string? Weekdays { get; set; }
        public int? Repeatupto { get; set; }
        public int Shiftdetailid { get; set; }
        [Required]
        public DateTime Shiftdate { get; set; }
        [Required]
        public int? Regionid { get; set; }
        public string region { get; set; }
        [Required(ErrorMessage = "Please specify a Start time.")]
        public TimeOnly Starttime { get; set; }

        [Required(ErrorMessage = "Please specify an End time.")]
        public TimeOnly Endtime { get; set; }
        public short Status { get; set; }
        public BitArray Isdeleted { get; set; }
        public string? Eventid { get; set; }
        public List<Region> regions { get; set; }
        public List<Physician> physics { get; set; }
        public string regionname { get; set; }
        public string datename { get; set; }
        public string role { get; set; }
        public List<int> SelectedDays { get; set; }
        public string LoginType { get; set; }
    }
}

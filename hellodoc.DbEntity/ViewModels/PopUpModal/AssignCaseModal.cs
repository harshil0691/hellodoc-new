using hellodoc.DbEntity.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hellodoc.DbEntity.ViewModels.PopUpModal
{
    public partial class AssignCaseModal
    {
        public int Requestid { get; set; }
        [Required]
        public int Regionid { get; set; }
        [Required]
        public int Physicianid { get; set; }
        public string PhysicianName { get; set; }
        [Required]
        public string Discription { get; set; }
        public string Modaltype { get; set; }
        public List<Region> Regions { get; set; }
        public List<Physician> Physicians { get; set; }
    }
}

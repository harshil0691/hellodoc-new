using hellodoc.DbEntity.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hellodoc.DbEntity.ViewModels.PopUpModal
{
    public partial class OrdersModal
    {
        public int requestid { get; set; }
        public int? aspid { get; set; }
        [Required]
        public int Business { get; set; }
        [Required]
        public long BusinessContact { get; set; }
        [Required]
        public string OrderEmail { get; set; }
        [Required]
        public long Faxnumber { get; set; }

        public string Prescription { get; set; }
        [Required]
        public int SelectProfession { get; set; }
        public int NumberOfRefills { get ; set; }

        public List<HealthProfessionalType> professionName { get; set; }
        public List<HealthProfessional> healthProfessionals { get; set; }
    }
}

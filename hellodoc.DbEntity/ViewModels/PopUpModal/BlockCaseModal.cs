using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hellodoc.DbEntity.ViewModels.PopUpModal
{
    public partial class BlockCaseModal
    {
        public int Requestid { get; set; }
        public string PatientName { get; set; }
        [Required]
        public string Blocknotes { get; set; }
    }
}

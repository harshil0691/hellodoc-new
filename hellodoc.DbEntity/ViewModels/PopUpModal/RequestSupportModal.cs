using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hellodoc.DbEntity.ViewModels.PopUpModal
{
    public partial class RequestSupportModal
    {
        public int Requestid { get; set; }
        [Required]
        public string Message { get; set; }
    }
}

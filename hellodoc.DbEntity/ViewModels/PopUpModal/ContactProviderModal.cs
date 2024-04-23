using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hellodoc.DbEntity.ViewModels.PopUpModal
{
    public partial class ContactProviderModal
    {
        [Required]
        public string Message { get; set; }

        public string MessagwType { get; set; }
        public int physicianid { get; set; }
    }
}

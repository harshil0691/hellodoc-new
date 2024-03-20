using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hellodoc.DbEntity.ViewModels.PopUpModal
{
    public partial class SendLinkModal
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public long Phone { get; set; }
        
        [EmailAddress(ErrorMessage = "Please enter a valid email")]
        public string Email { get; set; }
        public int reqid { get; set; }

    }
}

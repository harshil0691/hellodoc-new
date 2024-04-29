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
        [Required(ErrorMessage = "Phone number is required")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Phone number must be 10 digits long")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Phone number must contain only numbers")]
        public long Phone { get; set; }
        [Required]
        [EmailAddress(ErrorMessage = "Please enter a valid email")]
        public string MailEmail { get; set; }
        public int reqid { get; set; }

    }
}

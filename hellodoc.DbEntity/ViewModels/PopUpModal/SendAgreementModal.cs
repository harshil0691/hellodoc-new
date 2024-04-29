using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hellodoc.DbEntity.ViewModels.PopUpModal
{
    public partial class SendAgreementModal
    {
        public string reqtype { get; set; }
        public int reqid { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Email is Invalid")]
        public string email { get; set; }
        [Required(ErrorMessage = "Phone number is required")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Phone number must be 10 digits long")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Phone number must contain only numbers")]
        public string phonenumber { get; set; }
        public string bcolor { get; set; }
        public string patientName { get; set;}
        public string CancelNotes { get; set;}
    }
}

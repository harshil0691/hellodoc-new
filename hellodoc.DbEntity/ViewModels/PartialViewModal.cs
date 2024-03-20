using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hellodoc.DbEntity.ViewModels
{
    public class PartialViewModal
    {
        public string actionType { get; set; }
        public int requestid { get; set; }
        public int requestwisefileid { get; set; }  
        public string bcolor { get; set; }
        public string btext { get; set; }
        public string  patientName { get; set; }
        public string email { get; set;}
        public long? phonenumber { get; set; }
        public int physicianid { get; set; }
    }
}

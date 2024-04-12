using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hellodoc.DbEntity.ViewModels
{
    public class PartialViewModal
    {
        public int roleid { get; set; }
        public int accounttype { get; set; }
        public string actionType { get; set; }
        public int requestid { get; set; }
        public int regionid { get; set; }
        public int requestwisefileid { get; set; }  
        public string bcolor { get; set; }
        public string btext { get; set; }
        public string  patientName { get; set; }
        public string email { get; set;}
        public long? phonenumber { get; set; }
        public int physicianid { get; set; }
        public int shiftdetailsid { get; set; }
        public string datestring { get; set; }
        public DateTime shiftdate { get; set; }
        public int venorid { get; set; }
        public int pageNumber { get; set; }
        public string search { get; set; }
        public int professionalType { get; set; }

        public string exportType { get; set; }
        AdminRecordsListModal AdminRecords;
    }
}

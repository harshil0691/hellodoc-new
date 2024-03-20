using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hellodoc.DbEntity.ViewModels.DocumentModal
{
    public partial class ShowDocModal
    {
        public short Requestwisefileid { get; set; }
        public int? Requestid { get; set; }
        public string Createddate { get; set; }
        public string? Filename { get; set; }
        public string? Name { get; set; }  
        public string? Doctype { get; set; }
    }
}

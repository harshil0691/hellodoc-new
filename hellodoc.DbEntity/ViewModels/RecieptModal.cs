using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hellodoc.DbEntity.ViewModels
{
    public class RecieptModal
    {
        public int Timesheetid { get; set; }
        public DateOnly? Date { get; set; }
        public string? Item { get; set; }
        public int? Amount { get; set; }
        public IFormFile Billdoc { get; set; }
        public int? Invoicingid { get; set; }

    }
}

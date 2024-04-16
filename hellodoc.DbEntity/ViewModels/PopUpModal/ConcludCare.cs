using hellodoc.DbEntity.DataModels;
using hellodoc.DbEntity.ViewModels.DocumentModal;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hellodoc.DbEntity.ViewModels.PopUpModal
{
    public class ConcludCare
    {
        public int Requestid { get; set; }

        public string PatientName { get; set; } = null!;

        public IFormFile Doc { get; set; }
        public string providerNotes { get; set; }
        public int isfinalized { get; set; }

        public string PhysicianNotes { get; set; }

        public List<RequestTableModel> requestTable { get; set; }

        public List<ShowDocModal> patientDocuments { get; set; }

        public List<RequestWiseFile> RequestWiseFiles { get; set; }

        public User users { get; set; }
    }
}

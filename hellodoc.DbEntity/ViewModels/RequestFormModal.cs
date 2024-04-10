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

namespace hellodoc.DbEntity.ViewModels
{
    public partial class RequestFormModal
    {
        public string RequestType { get; set; }
        public int Requestid { get; set; }

        public string Firstname { get; set; } = null!;
        public string Lastname { get; set; } = null!;
        public DateOnly? DateOfBirth { get; set; }
        public string Symptoms { get; set; } = null!;
        public string Email { get; set; } = null!;
        public long Phonenumber { get; set; }
        public string Street { get; set; } = null!;
        public string City { get; set; } = null!;
        public string State { get; set; } = null!;
        public long Zipcode { get; set; }
        public string? Roomno { get; set; }
        public string Password { get; set; }
        public string Conform_password { get; set; }
        public IFormFile Doc { get; set; }

        public string? Confirmationnumber { get; set; }
        public string requesttypeid { get; set; }
        public string bgcolor { get; set; }
        public string btext { get; set; }
        public int activeid { get; set; }

        public string AdminNotes { get; set; }
        public string PhysicianNotes { get; set; }

        public List<RequestTableModel> requestTable { get; set; }

        public List<ShowDocModal> patientDocuments { get; set; }

        public List<RequestWiseFile> RequestWiseFiles { get; set; }

        public User users { get; set; }

        public string B_Firstname { get; set; } = null!;

        public string B_Lastname { get; set; } = null!;

        public string B_Email { get; set; } = null!;

        public long B_Phonenumber { get; set; }

        public string B_BusinessName { get; set; }

        public string B_CaseNo { get; set; }

        
        public string C_Firstname { get; set; } = null!;

        public string C_Lastname { get; set; } = null!;

        public string C_Email { get; set; } = null!;

        public long C_Phonenumber { get; set; }

        public string C_PropertyName { get; set; }

        public string C_Street { get; set; } = null!;

        public string C_City { get; set; } = null!;

        public string C_State { get; set; } = null!;

        public long C_Zipcode { get; set; }


        public string F_Firstname { get; set; } = null!;

        public string F_Lastname { get; set; } = null!;

        public string F_Email { get; set; } = null!;

        public long F_Phonenumber { get; set; }

        public string F_RelationType { get; set; }

    }
}

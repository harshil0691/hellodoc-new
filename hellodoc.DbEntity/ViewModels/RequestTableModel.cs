using hellodoc.DbEntity.DataModels;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hellodoc.DbEntity.ViewModels;
    public class RequestTableModel
{

            [Column("requestid")]
            public int Requestid { get; set; }

            [Column("status")]
            public short Status { get; set; }

            [Column("createddate", TypeName = "timestamp without time zone")]
            public DateTime Createddate { get; set; }

            [Column("documents")]
            public string Documents { get; set; }

}


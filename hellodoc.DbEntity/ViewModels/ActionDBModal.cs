using hellodoc.DbEntity.ViewModels.PopUpModal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hellodoc.DbEntity.ViewModels
{
    public class ActionDBModal
    {
        public string actionType { get; set; }
        public int aspid { get; set; }
        public int requestid { get; set; }
        public AssignCaseModal assignCase;
        public string Modaltype { get; set; }
    }
}

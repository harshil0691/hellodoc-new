using hellodoc.DbEntity.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hellodoc.DbEntity.ViewModels.AdminAccess
{
    public partial class CreateRoleModal
    {
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public int accountType { get; set; }
        public List<Menu> menus { get; set; }
    }
}

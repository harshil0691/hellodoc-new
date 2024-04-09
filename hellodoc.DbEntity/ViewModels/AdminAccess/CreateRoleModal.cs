using hellodoc.DbEntity.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hellodoc.DbEntity.ViewModels.AdminAccess
{
    public partial class CreateRoleModal
    {
        public int RoleId { get; set; }

        [Required(ErrorMessage ="Rolename is required")]
        public string RoleName { get; set; }

        public int accountType { get; set; }
        public List<Menu> menus { get; set; }
        public List<int> selectedAccess { get; set; } 
    }
}

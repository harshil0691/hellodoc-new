using hellodoc.DbEntity.DataContext;
using hellodoc.DbEntity.DataModels;
using hellodoc.DbEntity.ViewModels;
using hellodoc.DbEntity.ViewModels.AdminAccess;
using hellodoc.Repositories.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hellodoc.Repositories.Repository
{
    public  class AdminAccess : IAdminAccess
    {
        private readonly ApplicationDbContext _context;

        public AdminAccess(ApplicationDbContext context)
        {
            _context = context;
        }
        
        public AdminAccessModal AccountAccessData(int pagenumber, string accessType)
        {
            var pageSize = 5;
            var pageNumber = 1;
            var list = new List<AccessTableModal>();
            if (pagenumber > 0)
            {
                pageNumber = pagenumber;
            }

            if (accessType == "userAccess")
            {
                var aspnetuser = _context.AspNetUsers;
                list = aspnetuser.Select(u => new AccessTableModal
                {
                    aspid = u.Id,
                    accountPOC = u.Username,
                    accountType1 = _context.AspNetRoles.FirstOrDefault(r => r.Id == u.AspNetUserRole.Role.Accounttype).Name,
                    phoneNumber = u.Phonenumber??0,
                }).ToList();
            }
            else
            {
                var role = _context.Roles.Where(u => u.Isdeleted != 1);
                list = role.Select(a => new AccessTableModal
                {
                    accessName = a.Name,
                    accountType = a.Accounttype,
                    roleid = a.Roleid,
                }).ToList();
            }


            AdminAccessModal adminAccess = new AdminAccessModal();
            adminAccess.accessTables = list.ToList();

            adminAccess.accessTables = list.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            adminAccess.pageNumber = pageNumber;
            adminAccess.pageSize = pageSize;
            adminAccess.totalEntries = list.Count();
            if (list.Skip((pageNumber) * pageSize).Take(pageSize).Count() > 0)
            {
                adminAccess.morePages = true;
            }
            adminAccess.entries = ((pageNumber - 1) * pageSize + 1) + "-" + (((pageNumber - 1) * pageSize) + adminAccess.accessTables.Count());

            return adminAccess;


        }

        public List<AccessTableModal> accessTables()
        {
            var role = _context.Roles.Where(u=> u.Isdeleted != 1);
            var list = role.Select(a => new AccessTableModal
            {
                accessName = a.Name,
                accountType = a.Accounttype,
                roleid = a.Roleid,
            });

            return list.ToList();
        }

        public CreateRoleModal CreateRole(int accounttype,int roleid)
        {
            CreateRoleModal createRoleModal = new CreateRoleModal();

            if (roleid != 0) 
            {
                var roledetails = _context.Roles.FirstOrDefault(u => u.Roleid == roleid);
                createRoleModal.RoleId = roleid;
                createRoleModal.RoleName = roledetails.Name;
                createRoleModal.accountType = roledetails.Accounttype;
                createRoleModal.menus = _context.Menus.Where(u => u.Accounttype == roledetails.Accounttype).ToList();
                createRoleModal.selectedAccess = _context.RoleMenus.Where(r => r.Roleid == roleid).Select(r=>r.Menuid).ToList();
            }
            else
            {
                if (accounttype == 0)
                {
                    var menu = _context.Menus;
                    createRoleModal.menus = menu.ToList();
                }
                else
                {
                    var menu = _context.Menus.Where(u => u.Accounttype == accounttype);
                    createRoleModal.menus = menu.ToList();
                }
            }

            return createRoleModal;
        }

        public async Task NewRole(List<int> menulist, int aspid, string name, short accounttype)
        {
            var aspuser = _context.AspNetUsers.FirstOrDefault(u=>u.Id == aspid);
            Role role = new Role
            {
                Name = name,
                Accounttype = accounttype,
                Createdby = aspid.ToString(),
                Createddate = DateTime.Now,
            };

            _context.Roles.Add(role);

            foreach (var item in menulist)
            {
                RoleMenu roleMenu = new RoleMenu();
                roleMenu.Menuid = item;
                roleMenu.Roleid = role.Roleid;
                roleMenu.Role = role;
                _context.RoleMenus.Add(roleMenu);
               
            }
            _context.SaveChanges();

        }

        public async Task EditRole(List<int> menulist, short accounttype, int roleid, int aspid, string name)
        {
            var aspuser = _context.AspNetUsers.FirstOrDefault(u => u.Id == aspid);
            var role = _context.Roles.FirstOrDefault(u=> u.Roleid ==  roleid);

            role.Name = name;
            role.Accounttype = accounttype;
            role.Modifiedby = aspid.ToString();
            role.Modifieddate = DateTime.Now;

            var rolemenu = _context.RoleMenus.Where(u=> u.Roleid==roleid);
            var dbmenulist = _context.RoleMenus.Where(u => u.Roleid == roleid).Select(u=> u.Menuid).ToList();

            foreach (var item in rolemenu)
            {
                if (!menulist.Contains(item.Menuid))
                {
                    _context.RoleMenus.Remove(item);
                }
            }

            foreach (var item in menulist)
            {
                if (!dbmenulist.Contains(item))
                {
                    RoleMenu roleMenu = new RoleMenu();
                    roleMenu.Menuid = item;
                    roleMenu.Roleid = role.Roleid;
                    _context.RoleMenus.Add(roleMenu);
                }
            }
            _context.SaveChanges();
        }

        public async Task DeleteRole(int roleid, int aspid)
        {
            var aspuser = _context.AspNetUsers.FirstOrDefault(u => u.Id == aspid);
            var role = _context.Roles.FirstOrDefault(u => u.Roleid == roleid);
            role.Isdeleted = 1;
            role.Modifieddate = DateTime.Now;
            role.Modifiedby = aspid.ToString();

            _context.SaveChanges();
        }

        public string CreateAdmin(AdminProfileModal adminProfile)
        {
            if (adminProfile == null)
            {
                return "Admin Not Create Internal Error";
            }
            else
            {
                AspNetUser aspNetUser = new AspNetUser
                {
                    Username = adminProfile.username,
                    Passwordhash = adminProfile.password,
                    Email = adminProfile.Email,
                    Phonenumber = long.Parse(adminProfile.Phone),
                    Createddate = DateTime.Now,
                };
                Admin admin = new Admin
                {
                    Firstname = adminProfile.Firstname,
                    Lastname = adminProfile.Lastname,
                    Email = adminProfile.Email,
                    Mobile =  long.Parse(adminProfile.Phone),
                    Address1 = adminProfile.Address1,
                    Address2 = adminProfile.Address2,
                    City = adminProfile.City,
                    Zip = adminProfile.Zipcode,
                    Altphone = long.Parse(adminProfile.MailingNumber),
                    Status = 1,
                    //Createddate = DateTime.Now.ToString("yyyy-MM-dd"),
                    Createdby = adminProfile.aspid.ToString(),

                    Aspnetuser = aspNetUser,
                };
                _context.AspNetUsers.Add(aspNetUser);
                _context.Admins.Add(admin);
                _context.SaveChanges();
                foreach(var i in adminProfile.selectedRegion)
                {
                    AdminRegion adminRegion = new AdminRegion()
                    {
                        Regionid = i,
                        Adminid = admin.Adminid
                    };
                    _context.AdminRegions.Add(adminRegion);
                }

                AspNetUserRole aspNetUserRole = new AspNetUserRole()
                {
                    User = aspNetUser,
                    Roleid = adminProfile.selectrole,
                };

                _context.AspNetUserRoles.Add(aspNetUserRole);
                _context.SaveChanges();
            }
            return "New Admin Are Created";
        }

        public AdminProfileModal GetForCreateAdmin()
        {
            AdminProfileModal adminProfile = new AdminProfileModal()
            {
                regions = _context.Regions.ToList(),
                roles = _context.Roles.ToList(),
            };
            return adminProfile;
        }

    }
}

using hellodoc.DbEntity.DataContext;
using hellodoc.DbEntity.DataModels;
using hellodoc.Repositories.Repository.Interface;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using hellodoc.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using hellodoc.Repositories.Services.Interface;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection.Metadata.Ecma335;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using hellodoc.Repositories.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Abstractions;
using NUnit.Framework.Constraints;

namespace hellodoc.Repositories.Repository
{
    public partial class AuthManager: IAuthManager
    {
        private readonly ApplicationDbContext _context;

        public AuthManager(ApplicationDbContext context)
        {
            _context = context;
        }

        public AspNetUser Login(string email, string password)
        {
            var aspnetuser = _context.AspNetUsers.Include(u => u.AspNetUserRole.Role).FirstOrDefault(x => x.Email == email && x.Passwordhash == password);

            return aspnetuser;
        }

        public int GetPhysician(int aspid)
        {
            try
            {
                var physician = _context.Physicians.FirstOrDefault(p => p.Aspnetuserid == aspid && p.Role.Accounttype == 4).Physicianid == null ? 0 : _context.Physicians.FirstOrDefault(p => p.Aspnetuserid == aspid).Physicianid;

                return physician;
            }
            catch
            {
                return 0;
            }
            
        }

        public void authaction(HttpContext httpContext, string role)
        {
            var filterContext = new AuthorizationFilterContext(new ActionContext(httpContext, new RouteData(), new ActionDescriptor()), new List<IFilterMetadata>());
            var auth = AuthorizationHelper.IsAuthorized(filterContext, role);

            if (!auth)
            {
                RedirectToAccessDenied(httpContext);
            }
        }
        private static void RedirectToAccessDenied(HttpContext httpContext)
        {
            httpContext.Response.Redirect("/Login/AccessDenied");
        }
    }

    public class AuthorizationHelper
    {
        private static readonly ApplicationDbContext _context = new ApplicationDbContext();
        public static bool IsAuthorized(AuthorizationFilterContext filterContext, string requiredRole)
        {
            var jwtservice = filterContext.HttpContext.RequestServices.GetService<IJwtServices>();

            if (jwtservice == null)
            {
                RedirectToLogin(filterContext);
                return false;
            }

            var request = filterContext.HttpContext.Request;
            var token = request.Cookies["jwt"];

            if (token == null || !jwtservice.ValidateToken(token, out JwtSecurityToken jwtSecurityToken))
            {
                RedirectToLogin(filterContext);
                return false;
            }

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            var roleClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Role)?.Value;

            if (roleClaim == null || string.IsNullOrWhiteSpace(requiredRole))
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Login", action = "AccessDenied" }));
                return false;
            }

            // Assuming _context is disposed properly elsewhere
            var rolelist = _context.RoleMenus.Where(r => r.Roleid.ToString() == roleClaim).Select(m => m.Menu.Name);

            if (!rolelist.Contains(requiredRole))
            {
                RedirectToAccessDenied(filterContext);
                return false;
            }

            return true;
        }

        private static void RedirectToLogin(AuthorizationFilterContext filterContext)
        {
            filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Login", action = "login" }));
        }

        private static IActionResult RedirectToAccessDenied(AuthorizationFilterContext filterContext)
        {
            return new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Login", action = "AccessDenied" }));
        }
    }


    public class CustomAuthorize : Attribute, IAuthorizationFilter
    {
        private readonly ApplicationDbContext _context = new ApplicationDbContext();
        private readonly string _role;

        public CustomAuthorize(string role)
        {
            _role = role;
        }

        public void OnAuthorization(AuthorizationFilterContext filterContext)
        {
            var jwtservice = filterContext.HttpContext.RequestServices.GetService<IJwtServices>();

            if(jwtservice == null)
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Login", action = "login"})) ;
                return;
            }

            var request = filterContext.HttpContext.Request;
            var token = request.Cookies["jwt"];

            if(token == null || !jwtservice.ValidateToken(token, out JwtSecurityToken jwtSecurityToken))
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Login", action = "login" }));
                return;
            }
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            var roleClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Role).Value;

            var rolelist = _context.RoleMenus.Where(r => r.Roleid.ToString() == roleClaim).Select(m => m.Menu.Name);

            if (roleClaim == null || string.IsNullOrWhiteSpace(_role) || !rolelist.Contains(_role))
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Login", action = "AccessDenied" }));
                return;
            }


            //var user = SessionUtils.GetLoggedInUser(filterContext.HttpContext.Session);
            //if (user == null)
            //{
            //    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Login", action  = "login" }));
            //    return;
            //}

            //if(!string.IsNullOrEmpty(user.Name))
            //{

            //    if(!(user.Role == _role))
            //    {
            //        filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "AdminDash", action = "admin_dash" }));
            //    }
            //}

        }
    }

    public class CustomUserAuthorize : Attribute, IAuthorizationFilter
    {
        private readonly ApplicationDbContext _context = new ApplicationDbContext();
        private readonly string[] _role;

        public CustomUserAuthorize(params string[] role)
        {
            _role = role;
        }

        public void OnAuthorization(AuthorizationFilterContext filterContext)
        {
            var jwtservice = filterContext.HttpContext.RequestServices.GetService<IJwtServices>();

            if (jwtservice == null)
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Login", action = "login", loginType = _role }));
                return;
            }

            var request = filterContext.HttpContext.Request;
            var token = request.Cookies["jwt"];

            if (token == null || !jwtservice.ValidateToken(token, out JwtSecurityToken jwtSecurityToken))
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Login", action = "login", loginType = _role }));
                return;
            }
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            var roleClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.UserData).Value;

            if (roleClaim == null ||  !_role.Contains(roleClaim))
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Login", action = "login" , loginType = _role}));
                return;
            }




            //var user = SessionUtils.GetLoggedInUser(filterContext.HttpContext.Session);
            //if (user == null)
            //{
            //    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Login", action  = "login" }));
            //    return;
            //}

            //if(!string.IsNullOrEmpty(user.Name))
            //{

            //    if(!(user.Role == _role))
            //    {
            //        filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "AdminDash", action = "admin_dash" }));
            //    }
            //}

        }
    }
}

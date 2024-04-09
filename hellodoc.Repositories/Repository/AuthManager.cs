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

        
    }

    public class CustomAuthorize : Attribute, IAuthorizationFilter
    {
        private readonly string _role;
        public CustomAuthorize(string role ="")
        {
             _role = role;
        }

        public void OnAuthorization(AuthorizationFilterContext filterContext)
        {
            var jwtservice = filterContext.HttpContext.RequestServices.GetService<IJwtServices>();

            if(jwtservice == null)
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Login", action = "login" }));
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

            var roleClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Role);

            if(roleClaim == null || string.IsNullOrWhiteSpace(_role) || roleClaim.Value != _role)
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Login", action = "login" }));
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

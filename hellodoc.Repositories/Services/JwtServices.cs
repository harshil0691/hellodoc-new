using hellodoc.DbEntity.DataContext;
using hellodoc.DbEntity.DataModels;
using hellodoc.Repositories.Services.Interface;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace hellodoc.Repositories.Services
{
    public class JwtServices : IJwtServices
    {
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;

        public JwtServices(IConfiguration configuration,ApplicationDbContext applicationDb)
        {
            _configuration = configuration;
            _context = applicationDb;
        }

        public string GenarateJwtToken(AspNetUser aspNetUser)
        {
            var role = _context.AspNetRoles.FirstOrDefault(u=> u.Id == aspNetUser.AspNetUserRole.Role.Accounttype).Name;

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, aspNetUser.Email),
                new Claim(ClaimTypes.Role, aspNetUser.AspNetUserRole.Role.Roleid.ToString()),
                new Claim(ClaimTypes.UserData, role),
                new Claim("Aspid", aspNetUser.Id.ToString()),
                new Claim("username",aspNetUser.Username)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes( _configuration ["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddMinutes(60);

            var token = new JwtSecurityToken(
                Convert.ToString(_configuration ["Jwt:Issuer"]),
                Convert.ToString(_configuration["Jwt:Audience"]),
                claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        public bool ValidateToken(string token, out JwtSecurityToken jwtSecurityToken)
        {
            jwtSecurityToken = null;
            if (token == null)

                return false;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero

                }, out SecurityToken validatedToken);

                // Corrected access to the validatedToken
                var jwtToken = (JwtSecurityToken)validatedToken;
                
                if(jwtToken != null) return true;

                return false;
            }
            catch
            {
                return false;
            }
        }
}
}

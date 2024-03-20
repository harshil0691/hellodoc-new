using hellodoc.DbEntity.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;

namespace hellodoc.Repositories.Services.Interface
{
    public interface IJwtServices
    {
        string GenarateJwtToken(AspNetUser aspNetUser);

        bool ValidateToken(string token, out JwtSecurityToken jwtSecurityToken);
    }
}

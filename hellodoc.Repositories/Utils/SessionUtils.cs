

using hellodoc.DbEntity.DataModels;
using hellodoc.DbEntity.ViewModels;
using Microsoft.AspNetCore.Http;

namespace hellodoc.Utils
{
    public class SessionUtils
    {
        public static UserInfo  GetLoggedInUser(ISession session)
        {
            
            UserInfo user = null;

            if (!string.IsNullOrEmpty(session.GetString("userid")))
            {
                user = new UserInfo();
                user.Id = Convert.ToInt16(session.GetString("userid"));
                user.Name = session.GetString("username");
                user.Role = session.GetString("role");
            }

            return user;

        }

        public static void SetLoggedInUser(ISession session,AspNetUser aspNetUser)
        {

            if (aspNetUser != null)
            {
                session.SetString("userid", aspNetUser.Id.ToString());
                session.SetString("username", aspNetUser.Username);
                session.SetString("role", aspNetUser.AspNetUserRole.Role.Name);
            }

        }
    }
}


using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;

namespace Mbp.Core.User
{
    public static class ICurrentUserExtensions
    {
        public static void ReloveUser(this ICurrentUser currentUser, TokenValidatedContext token)
        {
            var claims = token.Principal.Claims;

            // 自定义解析，和Identityserver4中的写入对应起来
            currentUser.UserName = claims.First(x => x.Type == ClaimTypes.Name)?.Value;
            currentUser.LoginName = claims.First(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            currentUser.UserId= claims.First(x => x.Type == "UserPrimaryKey")?.Value;
            
            currentUser.AccessToken = (token.SecurityToken as JwtSecurityToken).RawData;
        }

        public static void ReloveUser(this ICurrentUser currentUser, string userId, string userName, string loginName, string token)
        {
            currentUser.UserId = userId;
            currentUser.UserName = userName;
            currentUser.LoginName = loginName;
            currentUser.AccessToken = token;
        }
    }
}

using JWTTEST.Helpers;
using JWTTEST.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace JWTTEST.service
{
    public class Authservice : IAuthService
    {
        private readonly UserManager<ApplictionUser> _userManager;
        private readonly JWT _jwt;
        public Authservice(UserManager<ApplictionUser> userManager,IOptions<JWT>jwt)
        {
            this._userManager = userManager;
            this._jwt = jwt.Value;
        }

        public async Task<AuthModel> GetTokenAsynce(TokenReguestModel model)
        {
            var authappliction = new AuthModel();

            var user = await _userManager.FindByEmailAsync(model.Email);
            if(user is null || !await _userManager.CheckPasswordAsync(user,model.Password))
            {
                authappliction.Massege = "Email And Password not Corect";
                return authappliction;
            }
            var jwtToken = await CreatJwtToken(user);
            var rolelist = await _userManager.GetRolesAsync(user);
            authappliction.IsAuthenticated = true;
            authappliction.Token = new JwtSecurityTokenHandler().WriteToken(jwtToken);
            authappliction.UserName = user.UserName;
            authappliction.Email = user.Email;
            authappliction.ExpiresOn = jwtToken.ValidTo;          
            authappliction.Roles = rolelist.ToList();
            return authappliction;
        }

        public async Task<AuthModel> RegisterAsynce(RegstirModel model)
        {
            if (await _userManager.FindByEmailAsync(model.Email) is not null)
                return new AuthModel { Massege = "Email is Alredy Regstred" };
            if (await _userManager.FindByNameAsync(model.Username) is not null)
                return new AuthModel { Massege = "User Name is Alredy Regstred" };
            var user = new ApplictionUser
            {
                Email = model.Email,
                FirstName = model.FirstName,
                LastName=model.LastName,
                UserName=model.Username
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                var errors = string.Empty;
                foreach(var err in result.Errors)
                {
                    errors += $"{err.Description},";
                }
                return new AuthModel { Massege = errors };
            }
            await _userManager.AddToRoleAsync(user, "User");
            var jwtToken = await CreatJwtToken(user);
            return new AuthModel
            {
                Email = user.Email,
                ExpiresOn = jwtToken.ValidTo,
                IsAuthenticated = true,
                Roles = new List<string> { "user" },
                Token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
                UserName = user.UserName
            };
        }
        private async Task<JwtSecurityToken> CreatJwtToken(ApplictionUser user)
        {
            var userClims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            var roleCalime = new List<Claim>();
            foreach (var role in roles)
            {
                roleCalime.Add(new Claim("roles", role));
            }
            var Claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub,user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email,user.Email),
                new Claim("uid",user.Id)
            }
            .Union(userClims)
            .Union(roleCalime);
            var symtricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var sigingCredentials = new SigningCredentials(symtricSecurityKey, SecurityAlgorithms.HmacSha256);
            var JwtSecurityToken = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience:_jwt.Audience,
                claims:Claims,
                expires:DateTime.Now.AddDays(_jwt.DurationInDays),
                signingCredentials: sigingCredentials);
            return JwtSecurityToken;

        }
    }
}

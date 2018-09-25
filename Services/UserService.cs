using test.Helpers;
using System;
using test.Entity;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens;
using System.Security.Claims;
namespace test.Services
{
    
    public interface IUserService
    {
         bool Auth(string username, string password);
         bool Auth(User user);
        IEnumerable<User> GetAll();
    }
    public class UserService:IUserService
    {
        private List<User> _users = new List<User>
        { 
            new User {Name = "test", Password = "test" } 
        };
        private readonly AppSettings    _appSetting;
        public UserService(IOptions<AppSettings> appSettings)
        {
            _appSetting = appSettings.Value;
        }
        public bool Auth(string username, string password)
        {
            var user = _users.FirstOrDefault(p => p.Name == username && password == p.Password);
            if(user == null)
            {
                return false;
            }else
            {
                // authentication successful so generate jwt token
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_appSetting.Secret);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[] 
                    {
                        new Claim(ClaimTypes.Name, username)
                    }),
                    Expires = DateTime.UtcNow.AddDays(7),
                    Audience="test",
                    Issuer="test",
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                user.Token = tokenHandler.WriteToken(token);

                // remove password before returning
                user.Password = null;
                return true;
            }                    

        }
         public bool Auth(User user)
         {
            var u = _users.FirstOrDefault(p => p.Name == user.Name && user.Password == p.Password);
            if(u == null)
            {
                return false;
            }else
            {
                // authentication successful so generate jwt token
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_appSetting.Secret);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.Name, user.Name),
                        new Claim("hehe","")
                    }),
                    Expires = DateTime.UtcNow.AddDays(7),
                    Audience = "test",
                    Issuer = "test",
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                user.Token = tokenHandler.WriteToken(token);

                // remove password before returning
                user.Password = null;
                return true;
            }    
         }
        public IEnumerable<User> GetAll()
        {
            return null;
        }
    }
}
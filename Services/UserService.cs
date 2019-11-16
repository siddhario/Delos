using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;

namespace Delos.Services
{
    public interface IUserService
    {
        korisnik Authenticate(string username, string password);
        IEnumerable<korisnik> GetAll();
    }

    public class UserService : IUserService
    {
        // users hardcoded for simplicity, store in a db with hashed passwords in production applications
        //private List<User> _users = new List<User>
        //{
        //    new User { Id = 1, FirstName = "Test", LastName = "User", Username = "test", Password = "test" }
        //};

        private readonly AppSettings _appSettings;

        public UserService(IOptions<AppSettings> appSettings, IServiceProvider serviceProvider)
        {
            _appSettings = appSettings.Value;
            _provider = serviceProvider;
        }

        private readonly IServiceProvider _provider;
        public korisnik Authenticate(string username, string password)
        {
            using (IServiceScope scope = _provider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<DelosDbContext>();
                //var user = _users.SingleOrDefault(x => x.Username == username && x.Password == password);
                var user = context.korisnik.FirstOrDefault(u => u.korisnicko_ime == username && u.lozinka.ToLower() == Helper.CreateMD5(password).ToLower());

                // return null if user not found
                if (user == null)
                    return null;

                // authentication successful so generate jwt token
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                    new Claim(ClaimTypes.Name, user.korisnicko_ime)
                    }),
                    Expires = DateTime.UtcNow.AddDays(7),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                user.token = tokenHandler.WriteToken(token);

                return user.WithoutPassword();
            }
        }

        public IEnumerable<korisnik> GetAll()
        {
            using (IServiceScope scope = _provider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<DelosDbContext>();
                return context.korisnik.ToList().WithoutPasswords();
            }
        }
    }
}
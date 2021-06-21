using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ParkyAPI.Data;
using ParkyAPI.Models;
using ParkyAPI.Repository.IRepository;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace ParkyAPI.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly AppSettings _appSettings;

        public UserRepository(ApplicationDbContext db, IOptions<AppSettings> appsettings)
        {
            _db = db;
            _appSettings = appsettings.Value;
        }


        /// <summary>
        /// Authenticate usr
        /// </summary>
        /// <param name="username">user name</param>
        /// <param name="password">password user</param>
        /// <returns>user authentified witj token</returns>
        public User Authenticate(string username, string password)
        {
            var user = _db.Users.SingleOrDefault(x => x.Username == username && x.Password == password);

            //user not found
            if (user == null)
            {
                return null;
            }

            //if user was found generate JWT Token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[] {
                    //get user login
                    new Claim(ClaimTypes.Name, user.Id.ToString()),
                    //Get Role
                    new Claim(ClaimTypes.Role,user.Role)
                }),

                Expires = DateTime.UtcNow.AddDays(7),

                SigningCredentials = new SigningCredentials
                                (new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            //Set user token
            user.Token = tokenHandler.WriteToken(token);
            user.Password = "";
            return user;
        }
        
        /// <summary>
        /// chek user is unique
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public bool IsUniqueUser(string username)
        {
            var user = _db.Users.SingleOrDefault(x => x.Username == username);

            // return null if user not found
            if (user == null)
                return true;

            return false;
        }

        /// <summary>
        /// add new user
        /// </summary>
        /// <param name="username">user name</param>
        /// <param name="password">passsword</param>
        /// <returns></returns>
        public User Register(string username, string password)
        {
            User userObj = new User()
            {
                Username = username,
                Password = password,
                //SSet role
                Role="Admin"
            };

            _db.Users.Add(userObj);

            _db.SaveChanges();
            
            //Clean password
            userObj.Password = "";
            return userObj;
        }
    }
}

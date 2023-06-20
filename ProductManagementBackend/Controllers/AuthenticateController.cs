using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ProductManagementBackend.Models;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace ProductManagementBackend.Controllers
{
    [Route("api")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        #region Property  
        /// <summary>  
        /// Property Declaration  
        /// </summary>  
        /// <param name="data"></param>  
        /// <returns></returns>  
        private IConfiguration _config;

        #endregion

        #region Contructor Injector  
        /// <summary>  
        /// Constructor Injection to access all methods or simply DI(Dependency Injection)  
        /// </summary>  
        public AuthenticateController(IConfiguration config)
        {
            _config = config;
        }
        #endregion


        private string GenerateJSONWebToken(User userInfo)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
              _config["Jwt:Issuer"],
              null,
              expires: DateTime.Now.AddMinutes(120),
              signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private async Task<User> AuthenticateUser(LoginUser login)
        {
            User user = null;

            //Validate the User Credentials      
            //Demo Purpose, I have Passed HardCoded User Information      
            if (login.Email == "test@gradspace.org")
            {
                user = new User { Name = "Test", Email = "test@gradspace.org", Password = "qwer1234" };
            }
            return user;
        }

        [AllowAnonymous]
        [HttpPost(nameof(Login))]
        public async Task<IActionResult> Login([FromBody] LoginUser data)
        {
            IActionResult response = Unauthorized();
            var user = await AuthenticateUser(data);
            if (user != null)
            {
                var tokenString = GenerateJSONWebToken(user);
                response = Ok(new { Token = tokenString, Message = "Success", User = new { Email = user.Email} });
            }
            return response;
        }

        [HttpGet(nameof(Get))]
        public async Task<IEnumerable<string>> Get()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");

            return new string[] { accessToken };
        }
    }
}

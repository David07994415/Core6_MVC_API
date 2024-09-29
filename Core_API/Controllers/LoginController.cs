using Core_Common2.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Core_API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly JwtHelper _jwtHelper;

        public LoginController(JwtHelper jwtHelper) 
        {
            _jwtHelper = jwtHelper;
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel LoginData)
        {
            if (LoginData == null) 
            {
                return Unauthorized();
            }

            if (!ModelState.IsValid)
            {
                return Unauthorized();
            }

            if (LoginData.UserName == "test_UserName" && LoginData.Password == "1234")
            {
                string tokenString = _jwtHelper.GenerateToken(LoginData.UserName);
                return Ok(tokenString);
            }


            return Unauthorized();
        }
    }
}

using Core_Common2.ViewModels;
using Core_MVC.GetApiLayer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace Core_MVC.Controllers
{
    public class LoginController : Controller
    {

        private readonly ApiList _apiList;

        // 注入 API Service 實體
        public LoginController(ApiList apiList)
        {
            _apiList = apiList;
        }


        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            if (ModelState.IsValid)
            {
                // 呼叫 AP
                var data = await _apiList.PostLoginApi(loginViewModel);
                if (data != null)    // 驗證成功
				{
					// 取得 Token ，並於 Session 中加入
					HttpContext.Session.SetString("JwtToken", data);

					// 解析 JWT 以獲取負載
					var handler = new JwtSecurityTokenHandler();
					var jwtToken = handler.ReadJwtToken(data);

					// 獲取 userName，並將其存放到 Session 中
					var userName = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
					if (!string.IsNullOrEmpty(userName))
					{
						HttpContext.Session.SetString("UserName", userName);
					}




					// 重新導向
					return RedirectToAction("Index", "ConnectApi");
                }
            }

            // 驗證失敗
            return View("Index", loginViewModel);
        }
    }
}

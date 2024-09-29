using Core_Common2.ViewModels;
using Core_MVC.GetApiLayer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

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
                if (data != null) 
                {
                    // 驗證成功
                    // 重新導向
                    return RedirectToAction("Index", "ConnectApi");
                }
            }

            // 驗證失敗
            return View("Index", loginViewModel);
        }
    }
}

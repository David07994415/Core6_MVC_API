using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Core_MVC.Filter
{
    public class AuthFilter : Attribute, IAsyncAuthorizationFilter
    {
        private readonly JwtHelper _jwtHelper;

        public AuthFilter(JwtHelper jwtHelper) 
        {
            _jwtHelper = jwtHelper;
        }
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var jwtToken = context.HttpContext.Session.GetString("JwtToken");

            if (string.IsNullOrEmpty(jwtToken))
            {
                // JWT 不存在，設置未授權結果
                context.Result = new RedirectToActionResult("Index", "Home", null);
            }


            bool isAuthenticated = _jwtHelper.ValidateToken(jwtToken);
            if (!isAuthenticated)
            {
                // JWT 不存在，設置未授權結果
                context.Result = new RedirectToActionResult("Index", "Home", null);
            }


           


        }
    }
}

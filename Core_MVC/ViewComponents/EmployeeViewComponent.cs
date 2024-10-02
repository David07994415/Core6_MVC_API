using Core_MVC.GetApiLayer;
using Microsoft.AspNetCore.Mvc;

namespace Core_MVC.ViewComponents
{
    public class EmployeeViewComponent : ViewComponent
    {

        private readonly ApiList _apiList;

        // 進行 DI 注入 
        public EmployeeViewComponent(ApiList apiList)
        {
            _apiList = apiList;
        }

        public async Task<IViewComponentResult> InvokeAsync(string TestParam = null)
        {
            var data = await _apiList.GetApiConnectionData();
            // 預設資料夾：/Views/Shared/Components/{檢視元件名稱}/{檢視名稱}

            if (TestParam != null)
            {
                ViewBag.TestPara = TestParam;
            }
            return View(data);
        }

    }
}

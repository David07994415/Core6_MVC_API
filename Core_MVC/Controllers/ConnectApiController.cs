using Core_Common2.ViewModels;
using Core_MVC.Filter;
using Core_MVC.GetApiLayer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Core_MVC.Controllers
{
    [ServiceFilter(typeof(AuthFilter))]
    public class ConnectApiController : Controller
    {
        private readonly ApiList _apiList;

        // 注入 API Service 實體
        public ConnectApiController(ApiList apiList) 
        {
            _apiList = apiList;
        }


        public async Task<IActionResult> Index()
        {
            var data = await  _apiList.GetApiConnectionData();

            var i = 1;
            return View();
        }


        public async Task<IActionResult> DownloadFile()
        {
            var model = await _apiList.GetFileStreamData();

            if (model == null)
            {
                return NotFound(); // 如果流為 null，返回 404
            }

            if (model.FileStream == null)
            {
                return NotFound(); // 如果流為 null，返回 404
            }



            return File(model.FileStream, model.FileContent, model.FileName);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MVC_ConnectionPost_ViewModel inputs)
        {
            if (ModelState.IsValid)
            {
                var data = await _apiList.PostApiConnectionData(inputs);
                // 呼叫 API
            }

            return View("Index",inputs);
        }

    }
}
